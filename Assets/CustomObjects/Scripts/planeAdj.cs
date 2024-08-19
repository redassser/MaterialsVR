using UnityEngine;
using System.Collections.Generic;

public class planeAdj : MonoBehaviour
{
    public GameObject planeObject;

    public int[] planeType = new int[3];
    private float[] planeNormal = new float[3];
    public bool locked;
    public List<GameObject> planes = new List<GameObject>();

    /// <summary>
    /// Sets the plane to the given miller indeces
    /// </summary>
    /// <param name="given">The miller indeces</param>
    public void Set(int[] given) {
        Vector3 center;
        Vector3 normal;
        Vector3 initpos = transform.position;
        Quaternion initrot = transform.rotation;
        Vector3 initscale = transform.localScale;

        planeType = given;
        Lock(true);

        normal = new Vector3(given[0], given[2], given[1]);
        center = new Vector3(-0.5f, -0.5f, -0.5f); 
        for (int i = 0; i < 3; i++) {
            if(normal[i] != 0) {
                center[i] += (1f / normal[i]);
                break;
            }
        }

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        CreatePlane(normal, center);
        
        CombineInstance[] combine = new CombineInstance[planes.Count];

        for (int i=0;i<planes.Count;i++) {
            MeshFilter planeMesh = planes[i].GetComponent<MeshFilter>();
            combine[i].mesh = planeMesh.sharedMesh;
            combine[i].transform = planeMesh.transform.localToWorldMatrix;
            planes[i].SetActive(false);
        }

        Mesh mesh = new Mesh();
        mesh.CombineMeshes(combine);
        transform.GetComponent<MeshFilter>().sharedMesh = mesh;
        transform.gameObject.SetActive(true);

        transform.position = initpos;
        transform.rotation = initrot;
        transform.localScale = initscale;
    }

    /// <summary>
    /// Changes the plane to lie on the given points on the cube
    /// </summary>
    /// <param name="points">Points on the cube</param>
    public void RePlane(Vector3[] points)
    {
        Plane testPlane = new Plane();
        CombineInstance[] combine = new CombineInstance[1];
        float min = 0;
        int negnum = 0, nonzero = 0;
        Vector3 initpos = transform.position;
        Quaternion initrot = transform.rotation;
        Vector3 initscale = transform.localScale;

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        testPlane.Set3Points(points[0], points[1], points[2]);
        testPlane.normal.Set(testPlane.normal.x, testPlane.normal.y, testPlane.normal.z);

        CreatePlane(testPlane.normal, points[0]);

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

        transform.position = initpos;
        transform.rotation = initrot;
        transform.localScale = initscale;

        planeNormal[0] = testPlane.normal.x;
        planeNormal[1] = testPlane.normal.z;
        planeNormal[2] = testPlane.normal.y;

        for (int i=0;i<3;i++) { // GET non zero min
            if (planeNormal[i] < 0) negnum++;
            if (min == 0) min = Mathf.Abs(planeNormal[i]);
            if (planeNormal[i] == 0) continue;
            else nonzero++;
            if (Mathf.Abs(min) > Mathf.Abs(planeNormal[i])) min = Mathf.Abs(planeNormal[i]);
        }
        for (int i = 0; i < 3; i++) { // ASSIGN PLANE TYPE
            if (planeNormal[i] == 0.0) planeType[i] = 0;
            else planeType[i] = (int)(planeNormal[i] / min);
        }
    }

    /// <summary>
    /// Creates a set of 3 parallel planes after deleteing any previous planes
    /// </summary>
    /// <param name="normal">Normal vector of the planes</param>
    /// <param name="center">Center point of middle plane</param>
    private void CreatePlane(Vector3 normal, Vector3 center) {
        float interplanarDistance = 1 / Mathf.Sqrt(Mathf.Pow(planeType[0],2) + Mathf.Pow(planeType[1], 2) + Mathf.Pow(planeType[2], 2));
        int iteration = 0;
        bool h = true;

        foreach(GameObject plane in planes) {
            Destroy(plane);
        }
        planes.Clear();

        while(h) {
            Vector3 newCenter = center + normal.normalized * interplanarDistance * iteration;
            for(int i=0;i<3;i++) {
                if(newCenter[i] > 0.5) {
                    iteration = -1; newCenter = center + normal.normalized * interplanarDistance * iteration;
                    break;
                } else if (newCenter[i] < -0.5) {
                    h = false;
                    break;
                }
            }
            if (!h) break;

            GameObject newPlane = Instantiate(planeObject, transform);
            newPlane.transform.up = normal;
            newPlane.transform.localPosition = newCenter;
            planes.Add(newPlane);

            if (iteration >= 0) iteration++;
            else iteration--;
        }
    }

    public void Lock(bool isLock) {
        this.locked = isLock;
        for(int i=0;i<transform.Find("Atoms").childCount;i++) {
            transform.Find("Atoms").GetChild(i).gameObject.SetActive(!isLock);
        }
    }
}
