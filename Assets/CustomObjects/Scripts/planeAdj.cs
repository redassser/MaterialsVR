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
        locked = true;

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

    private void CreatePlane(Vector3 normal, Vector3 center, float interplanarDistance = 0.5f) {
        foreach(GameObject plane in planes) {
            Destroy(plane);
        }
        planes.Clear();

        GameObject new1Plane = Instantiate(planeObject, transform);
        new1Plane.transform.up = normal;
        new1Plane.transform.localPosition = center;
        planes.Add(new1Plane);

        GameObject new2Plane = Instantiate(planeObject, transform);
        new2Plane.transform.up = normal;
        new2Plane.transform.localPosition = center;
        new2Plane.transform.localPosition += normal * interplanarDistance;
        planes.Add(new2Plane);

        GameObject new3Plane = Instantiate(planeObject, transform);
        new3Plane.transform.up = normal;
        new3Plane.transform.localPosition = center;
        new3Plane.transform.localPosition -= normal * interplanarDistance;
        planes.Add(new3Plane);
    }
}
