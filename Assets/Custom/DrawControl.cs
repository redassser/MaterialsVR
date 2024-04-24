using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawControl : MonoBehaviour
{
    public GameObject rightCont;
    public GameObject leftCont;
    public RaycastHit hit;
    public LineRenderer line;

    LayerMask layermask3D;

    public bool isDrawing = false;
    void Start()
    {
        layermask3D = LayerMask.GetMask("UI3D");
    }

    // Update is called once per frame
    void Update()
    {
        OVRInput.Update();
        if (Physics.Raycast(rightCont.transform.position, rightCont.transform.forward, out hit, 1.5f, layermask3D)) {
            line.SetPositions(new Vector3[] { rightCont.transform.position, hit.point });
            line.startColor = Color.red;
            line.endColor = Color.red;
            if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger)) {
            }
        } else {
            line.SetPositions(new Vector3[] { rightCont.transform.position, rightCont.transform.position + rightCont.transform.forward * 0.2f });
            line.startColor = Color.white;
            line.endColor = Color.white;
        }
    }
  
}
