using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuControl : MonoBehaviour
{
    public UnityEngine.UI.ScrollRect menuScrollRect;
    public GameObject rightCont;
    public GameObject leftCont;
    public RaycastHit hit;
    public RaycastHit interact;
    public LineRenderer line;
    public GameObject Canvas;
    public Transform selection;
    public Vector3 pointOnCanvas;
    LayerMask layermaskUI;
    LayerMask layermask3D;
    LayerMask layermaskUIinteract = 1 << 3;
    // Start is called before the first frame update
    void Start()
    {
        line.enabled = true;
        layermaskUI = LayerMask.GetMask("UI");
        layermask3D = LayerMask.GetMask("UI3D");
        layermaskUIinteract = LayerMask.GetMask("UIinteract");
    }

    // Update is called once per frame
    void Update()
    {
        OVRInput.Update();
        /*
        if (Physics.Raycast(rightCont.transform.position, rightCont.transform.forward, out hit, 1.5f, layermask3D)) {
            Debug.Log(hit.collider.transform.InverseTransformPoint(hit.point).sqrMagnitude);
            if(hit.textureCoord.sqrMagnitude < 0.2) {
                line.SetPositions(new Vector3[] { rightCont.transform.position, hit.point });
                line.startColor = Color.red;
                line.endColor = Color.red;
                if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger)) {
                }
            }
            
        } else */if (Physics.Raycast(rightCont.transform.position, rightCont.transform.forward, out hit, 1.5f, layermaskUI)) {
            if (OVRInput.Get(OVRInput.RawAxis2D.RThumbstick).magnitude > 0.1)
                menuScrollRect.verticalNormalizedPosition += OVRInput.Get(OVRInput.RawAxis2D.RThumbstick).y * Time.deltaTime;
            
            line.SetPositions(new Vector3[] { rightCont.transform.position, hit.point });
            line.startColor = Color.green;
            line.endColor = Color.green;
            hit.collider.transform.FindChildRecursive("Cursor").position = hit.point;
            if(OVRInput.Get(OVRInput.RawButton.RIndexTrigger)) {
                if (Physics.Raycast(rightCont.transform.position, rightCont.transform.forward, out interact, 1.5f, layermaskUIinteract)) {
                    UnityEngine.EventSystems.ExecuteEvents.Execute(interact.collider.gameObject, new UnityEngine.EventSystems.BaseEventData(UnityEngine.EventSystems.EventSystem.current), UnityEngine.EventSystems.ExecuteEvents.submitHandler);
                    //interact.collider.gameObject.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
                }
            }
        } else {
            line.SetPositions(new Vector3[] { rightCont.transform.position, rightCont.transform.position + rightCont.transform.forward * 0.2f });
            line.startColor = Color.white;
            line.endColor = Color.white;
        }
    }
}
