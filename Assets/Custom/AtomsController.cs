using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtomsController : MonoBehaviour
{
    public float maxScale;
    public float minScale;
    public float makeUnit = 1;

    private void Start() {
        Vector3 initpos = transform.position;
        transform.position = new Vector3(0, 0, 0);
        MeshFilter[] atomMeshes = transform.GetChild(0).GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[atomMeshes.Length];

        for (int i=0;i<atomMeshes.Length;i++) {
            combine[i].mesh = atomMeshes[i].sharedMesh;
            combine[i].transform = atomMeshes[i].transform.localToWorldMatrix * transform.worldToLocalMatrix;
        }
        transform.GetChild(0).gameObject.SetActive(false);

        Mesh mesh = new Mesh();
        mesh.CombineMeshes(combine);
        transform.GetComponent<MeshFilter>().sharedMesh = mesh;
        transform.gameObject.SetActive(true);
        transform.position = initpos;

        Invoke("setUnit", 3);
    }
    public void setUnit() {
        GetComponent<Renderer>().material.SetFloat("_Unitize", makeUnit);
        GetComponent<Renderer>().enabled = true;
        transform.GetChild(0).gameObject.SetActive(false);
        if(makeUnit > 0) transform.GetChild(1).gameObject.SetActive(false);
    }
    public void setDraw() {
        GetComponent<Renderer>().material.SetFloat("_Unitize", 0);
        GetComponent<Renderer>().enabled = false;
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);
        foreach (Transform atom in transform.GetChild(0)) {
            atom.localScale = new Vector3(minScale, minScale, minScale);
        }
    }
    public void setColor(Color color) {
        GetComponent<Renderer>().material.SetColor("_Color", color);
    }
}
