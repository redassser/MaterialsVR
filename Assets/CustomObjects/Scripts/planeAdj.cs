using UnityEngine;

public class planeAdj : MonoBehaviour
{
    public int[] planeType = new int[3];
    private float[] planeNormal = new float[3];
    public bool locked;

    /// <summary>
    /// Sets the plane to the given miller indeces
    /// </summary>
    /// <param name="given">The miller indeces</param>
    public void Set(int[] given) {
        MeshFilter planeMesh = transform.Find("Plane").GetComponent<MeshFilter>();
        Vector3 center = new Vector3();
        Vector3 normal = new Vector3();
        Vector3 initpos = transform.position;
        Quaternion initrot = transform.rotation;
        Vector3 initscale = transform.localScale;

        planeType = given;
        Lock(true);

        if (CompareArrays(given, new int[3] { 1, 0, 0 })) {
            center = new Vector3(0.5f, 0f, 0f);
            normal = new Vector3(1, 0, 0);
        } else if (CompareArrays(given, new int[3] { 2, 1, 1 })) {
            center = new Vector3(0f, 0f, -1f);
            normal = new Vector3(2, 1, 1);
        } else if (CompareArrays(given, new int[3] { 0, 0, 3 })) {
            center = new Vector3(0f, -0.167f, 0f);
            normal = new Vector3(0, 1, 0);
        }

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        transform.Find("Plane").up = normal;
        transform.Find("Plane").localPosition = center;

        CombineInstance[] combine = new CombineInstance[1];

        combine[0].mesh = planeMesh.sharedMesh;
        combine[0].transform = planeMesh.transform.localToWorldMatrix;

        transform.Find("Plane").gameObject.SetActive(false);

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
        Plane plane = new Plane();
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
       
        plane.Set3Points(points[0], points[1], points[2]);
        plane.normal.Set(plane.normal.x, plane.normal.y, plane.normal.z);
        transform.Find("Plane").up = plane.normal;
        transform.Find("Plane").localPosition = points[0];

        combine[0].mesh = planeMesh.sharedMesh;
        combine[0].transform = planeMesh.transform.localToWorldMatrix;

        transform.Find("Plane").gameObject.SetActive(false);

        Mesh mesh = new Mesh();
        mesh.CombineMeshes(combine);
        transform.GetComponent<MeshFilter>().sharedMesh = mesh;
        transform.gameObject.SetActive(true);

        transform.position = initpos;
        transform.rotation = initrot;
        transform.localScale = initscale;

        planeNormal[0] = plane.normal.x;
        planeNormal[1] = plane.normal.z;
        planeNormal[2] = plane.normal.y;

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
    /// Compares the values in two arrays
    /// </summary>
    /// <param name="a1">The first array to compare</param>
    /// <param name="a2">The second array to compare</param>
    /// <returns>True is all values are equal, false if not or different sizes</returns>
    private bool CompareArrays(int[] a1, int[] a2) {
        bool areEqual = true;

        if (a1.Length != a2.Length) return false;

        for (int i = 0; i < a1.Length; i++) {
            if (!a1[i].Equals(a2[i])) areEqual = false;
        }
        return areEqual;
    }

    public void Lock(bool isLock) {
        this.locked = isLock;
        for(int i=0;i<transform.Find("Atoms").childCount;i++) {
            transform.Find("Atoms").GetChild(i).gameObject.SetActive(!isLock);
        }
    }
}
