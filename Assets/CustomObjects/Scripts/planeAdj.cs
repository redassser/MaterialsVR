using UnityEngine;
using System.Collections.Generic;

public class planeAdj : MonoBehaviour
{
    public GameObject planeObject;

    public Vector3 planeType = new Vector3();

    private Vector3 originalCenter;
    private bool locked;
    private List<GameObject> planes = new List<GameObject>();

    /// <summary>
    /// Creates a set of parallel planes in the unit from the given miller indeces
    /// </summary>
    /// <param name="given">The miller indeces</param>
    public void Set(Vector3 given) {
        Vector3 normal = new Vector3(given[0], given[2], given[1]);
        Vector3 center = new Vector3(-0.5f, -0.5f, -0.5f);

        planeType = given;
        Lock(true);

        //Get one of the intercepts that isn't infinity
        for (int i = 0; i < 3; i++) {
            if(normal[i] != 0) {
                center[i] += (1f / normal[i]);
                break;
            }
        }

        CreatePlane(normal, center);
    }

    /// <summary>
    /// Changes the plane to lie on the given 3 points on the cube
    /// </summary>
    /// <param name="points">3 Points on the cube</param>
    public void RePlane(Vector3[] points)
    {
        Plane testPlane = new Plane();
        float min = 0;
        int negnum = 0, nonzero = 0;
        Vector3 initpos = transform.position;
        Quaternion initrot = transform.rotation;
        Vector3 initscale = transform.localScale;

        testPlane.Set3Points(points[0], points[1], points[2]);

        for (int i = 0; i < 3; i++) { // GET non zero min
            if (testPlane.normal[i] < 0) negnum++;
            if (min == 0) min = Mathf.Abs(testPlane.normal[i]);
            if (testPlane.normal[i] == 0) continue;
            else nonzero++;
            if (Mathf.Abs(min) > Mathf.Abs(testPlane.normal[i])) min = Mathf.Abs(testPlane.normal[i]);
        }
        for (int i = 0; i < 3; i++) { // ASSIGN PLANE TYPE
            if (testPlane.normal[i] == 0.0) planeType[i] = 0;
            else planeType[i] = (int)(testPlane.normal[i] / min);
        }

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        CreatePlane(testPlane.normal, points[0]);

        transform.position = initpos;
        transform.rotation = initrot;
        transform.localScale = initscale;
    }

    /// <summary>
    /// Set unit cell's drawing to locked or unlocked
    /// </summary>
    /// <param name="setLock">true for locked, false for unlocked</param>
    public void Lock(bool setLock) {
        this.locked = setLock;
        for(int i=0;i<transform.Find("Atoms").childCount;i++) {
            transform.Find("Atoms").GetChild(i).gameObject.SetActive(!setLock);
        }
    }

    /// <returns>Whether the drawing on this cell is locked or unlocked</returns>
    public bool isLocked() {
        return this.locked;
    }

    /// <param name="normal">normal of the plane</param>
    /// <param name="pointOnPlane">point anywhere on the plane</param>
    /// <returns>Returns the point on the plane that is closest to the point 0 0 0</returns>
    private Vector3 ClosestPointToOrigin(Vector3 normal, Vector3 pointOnPlane) {
        float total = normal.x * normal.x + normal.y * normal.y + normal.z * normal.z;
        float dotProd = normal.x * pointOnPlane.x + normal.y * pointOnPlane.y + normal.z * pointOnPlane.z;
        Vector3 point = -normal * dotProd / total;
        return point;
    }

    /// <summary>
    /// Creates a set of parallel planes after deleteing any previous planes, interplanar distance calculated from planeType
    /// </summary>
    /// <param name="normal">Normal vector of the planes</param>
    /// <param name="center">Center point of middle plane</param>
    private void CreatePlane(Vector3 normal, Vector3 center, float interplanarDistance = 0, int interval = 1) {
        bool over = false;

        // First time being run
        if (interplanarDistance == 0) {
            foreach (GameObject plane in planes) {
                Destroy(plane);
            }
            planes.Clear();
            interplanarDistance = 1 / Mathf.Sqrt(Mathf.Pow(planeType[0], 2) + Mathf.Pow(planeType[1], 2) + Mathf.Pow(planeType[2], 2));
            center = ClosestPointToOrigin(normal, center);
            originalCenter = center;
        }

        GameObject newPlane = Instantiate(planeObject, transform);
        newPlane.transform.up = normal;
        newPlane.transform.localPosition = center;
        planes.Add(newPlane);

        center += normal.normalized * interplanarDistance * interval;

        for (int i = 0; i < 3; i++) { //over if center is outside the cube
            if (Mathf.Abs(center[i]) > 0.5) {
                over = true;
            } 
        }
        
        if(over && interval > 0) {
            CreatePlane(normal, originalCenter - normal.normalized * interplanarDistance, interplanarDistance, -1);

        } else if(over && interval < 0) {
            CombineInstance[] combine = new CombineInstance[planes.Count];

            for (int i = 0; i < planes.Count; i++) {
                MeshFilter planeMesh = planes[i].GetComponent<MeshFilter>();
                combine[i].mesh = planeMesh.sharedMesh;
                combine[i].transform = planeMesh.transform.localToWorldMatrix;
                planes[i].SetActive(false);
            }

            Mesh mesh = new Mesh();
            mesh.CombineMeshes(combine);
            transform.GetComponent<MeshFilter>().sharedMesh = mesh;
            transform.gameObject.SetActive(true);

        } else {
            CreatePlane(normal, center, interplanarDistance, interval);
        }
    }
}
