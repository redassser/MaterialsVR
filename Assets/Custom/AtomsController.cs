using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtomsController : MonoBehaviour
{
    public float maxScale;
    public float minScale;

    private void Start() {
        Vector3 initpos = transform.position;
        transform.position = new Vector3(0, 0, 0);
        MeshFilter[] atomMeshes = transform.GetChild(0).GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[atomMeshes.Length];

        for (int i=0;i<atomMeshes.Length;i++) {
            combine[i].mesh = atomMeshes[i].sharedMesh;
            combine[i].transform = atomMeshes[i].transform.localToWorldMatrix * transform.worldToLocalMatrix;
            atomMeshes[i].gameObject.SetActive(false);
        }

        Mesh mesh = new Mesh();
        mesh.CombineMeshes(combine);
        transform.GetComponent<MeshFilter>().sharedMesh = mesh;
        transform.gameObject.SetActive(true);
        transform.position = initpos;
    }
    public void setBig() {
        foreach(Transform atom in transform.GetChild(0)) {
            atom.localScale = new Vector3(maxScale, maxScale, maxScale);
        }
        transform.GetChild(1).gameObject.SetActive(false);
    }
    public void setSmall() {
        foreach (Transform atom in transform.GetChild(0)) {
            atom.localScale = new Vector3(minScale, minScale, minScale);
        }
        transform.GetChild(1).gameObject.SetActive(true);
    }
    public void setColor(Color color) {
        GetComponent<Renderer>().material.SetColor("_Color", color);
    }
}
