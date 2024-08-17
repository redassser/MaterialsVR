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
        MeshFilter planeMesh;
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
        planeMesh = planes[0].GetComponent<MeshFilter>();

        CombineInstance[] combine = new CombineInstance[1];

        combine[0].mesh = planeMesh.sharedMesh;
        combine[0].transform = planeMesh.transform.localToWorldMatrix;

        foreach (GameObject plane in planes) {
            plane.SetActive(false);
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
        MeshFilter planeMesh = transform.Find("Plane").GetComponent<MeshFilter>();
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

        combine[0].mesh = planeMesh.sharedMesh;
        combine[0].transform = planeMesh.transform.localToWorldMatrix;

        foreach(GameObject plane in planes) {
            plane.SetActive(false);
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

    private void CreatePlane(Vector3 normal, Vector3 center) {
        foreach(GameObject plane in planes) {
            Destroy(plane);
        }
        planes.Clear();

        GameObject newPlane = Instantiate(planeObject, transform);
        newPlane.transform.up = normal;
        newPlane.transform.localPosition = center;
        planes.Add(newPlane);
    }
}
