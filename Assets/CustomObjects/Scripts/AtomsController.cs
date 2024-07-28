using UnityEngine;

public class AtomsController : MonoBehaviour
{
    private void Start() {
        MeshCombine();
    }

    /// <summary>
    /// Combines the Atoms meshes in the unit and sets the shared mesh to it. 
    /// </summary>
    private void MeshCombine() {
        Vector3 initpos = transform.position;
        MeshFilter[] atomMeshes = transform.Find("Atoms").GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[atomMeshes.Length];

        transform.position = new Vector3(0, 0, 0);

        for (int i = 0; i < atomMeshes.Length; i++) {
            combine[i].mesh = atomMeshes[i].sharedMesh;
            combine[i].transform = atomMeshes[i].transform.localToWorldMatrix * transform.worldToLocalMatrix;
        }

        Mesh mesh = new Mesh();
        mesh.CombineMeshes(combine);
        GetComponent<MeshFilter>().sharedMesh = mesh;

        gameObject.SetActive(true);
        transform.position = initpos;

        Invoke(nameof(SetUnit), 3);
    }

    /// <summary>
    /// Activates and Enables the CubeMat Renderer and Disables the physical atoms and sticks, only allowing the 
    /// combined mesh to appear.
    /// </summary>
    private void SetUnit() {
        GetComponent<Renderer>().material.SetFloat("_Unitize", 1);
        GetComponent<Renderer>().enabled = true;
        transform.Find("Atoms").gameObject.SetActive(false);
        transform.Find("Sticks").gameObject.SetActive(false);
    }
}
