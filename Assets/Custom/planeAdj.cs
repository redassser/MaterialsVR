using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planeAdj : MonoBehaviour
{
    public int[] planeType = new int[3];
    public float[] h = new float[3];
    public float min = 0;
    public bool locked;
    // Start is called before the first frame update
    public void rePlane(Vector3[] points)
    {
        Vector3 initpos = transform.position;
        Quaternion initrot = transform.rotation;
        Vector3 initscale = transform.localScale;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        Plane plane = new Plane();
        plane.Set3Points(points[0], points[1], points[2]);
        plane.normal.Set(plane.normal.x, plane.normal.y, plane.normal.z);
        transform.GetChild(2).up = plane.normal;
        transform.GetChild(2).localPosition = points[0];

        MeshFilter planeMesh = transform.GetChild(2).GetComponent<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[1];

        combine[0].mesh = planeMesh.sharedMesh;
        combine[0].transform = planeMesh.transform.localToWorldMatrix;

        transform.GetChild(2).gameObject.SetActive(false);

        Mesh mesh = new Mesh();
        mesh.CombineMeshes(combine);
        transform.GetComponent<MeshFilter>().sharedMesh = mesh;
        transform.gameObject.SetActive(true);

        transform.position = initpos;
        transform.rotation = initrot;
        transform.localScale = initscale;

        h[0] = plane.normal.x;
        h[1] = plane.normal.z;
        h[2] = plane.normal.y;
        min = 0;

        int negnum = 0, nonzero = 0;

        for (int i=0;i<3;i++) { // GET non zero min
            if (h[i] < 0) negnum++;
            if (min == 0) min = Mathf.Abs(h[i]);
            if (h[i] == 0) continue;
            else nonzero++;
            if (Mathf.Abs(min) > Mathf.Abs(h[i])) min = Mathf.Abs(h[i]);
        }
        for (int i = 0; i < 3; i++) { // ASSIGN PLANE TYPE
            if (h[i] == 0.0) planeType[i] = 0;
            else planeType[i] = (int)(h[i] / min);
        }
    }
}
