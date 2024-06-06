using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManagerv5 : MonoBehaviour
{
    public GameObject rightCont;
    public LineRenderer line;

    private LayerMask layermaskUI;
    private LayerMask layermaskUIinteract;
    private LayerMask layermaskDraw;

    public MainMenuContentManager mm;

    private List<LineRenderer> drawnlines = new List<LineRenderer>();
    private LineRenderer drawline;
    private bool startedDraw = false;

    private void Start() {
        layermaskUI = LayerMask.GetMask("UI");
        layermaskUIinteract = LayerMask.GetMask("UIinteract");
        layermaskDraw = LayerMask.GetMask("Draw");
    }
    private void Update() {
        RaycastHit hit;

        if (drawnlines.Count != 0 && OVRInput.GetDown(OVRInput.RawButton.B)) {
            drawnlines[drawnlines.Count - 1].GetComponentInParent<MeshFilter>().mesh = null;
            drawnlines[drawnlines.Count - 1].loop = false;
            drawnlines.RemoveAt(drawnlines.Count - 1);
        }

        if (Physics.Raycast(rightCont.transform.position, rightCont.transform.forward, out hit, 0.2f, layermaskDraw)) {
            line.SetPositions(new Vector3[] { rightCont.transform.position, rightCont.transform.position + rightCont.transform.forward * 0.2f });
            line.startColor = Color.yellow;
            line.endColor = Color.yellow;
            if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger)) {
                drawline = hit.collider.GetComponentInParent<LineRenderer>();
                if (drawnlines.Contains(drawline)) {
                    line.startColor = Color.red;
                    line.endColor = Color.red;
                    drawline = null;
                    return;
                }
                if (drawline.positionCount == 0) {
                    startedDraw = true;
                    drawline.positionCount = 2;
                    drawline.SetPosition(0, hit.collider.transform.localPosition);
                    drawline.SetPosition(1, hit.collider.transform.localPosition);
                } else {
                    for (int i=0;i<drawline.positionCount;i++) {
                        if (drawline.GetPosition(i) == hit.collider.transform.localPosition) {
                            if (drawline.positionCount == 4) {
                                Vector3[] points = new Vector3[3];
                                startedDraw = false;
                                drawline.positionCount--;
                                drawline.SetPosition(drawline.positionCount - 1, hit.collider.transform.localPosition);
                                drawline.loop = true;
                                drawnlines.Add(drawline);
                                drawline.GetPositions(points);
                                drawline.transform.GetComponentInParent<planeAdj>().rePlane(points);
                                drawline.positionCount = 0;
                                drawline = null;
                            }
                            return;
                        }
                    }
                    drawline.positionCount++;
                    drawline.SetPosition(drawline.positionCount - 1, drawline.transform.InverseTransformPoint(rightCont.transform.position + rightCont.transform.forward * 0.2f));
                    drawline.SetPosition(drawline.positionCount - 2, hit.collider.transform.localPosition);
                }
            } else if (startedDraw) {
                startedDraw = false;
                drawline.positionCount = 0;
            }
        } else if (startedDraw) {
            line.SetPositions(new Vector3[] { rightCont.transform.position, rightCont.transform.position + rightCont.transform.forward * 0.2f });
            if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger)) {
                drawline.SetPosition(drawline.positionCount - 1, drawline.transform.InverseTransformPoint(rightCont.transform.position + rightCont.transform.forward * 0.2f));
            } else {
                startedDraw = false;
                drawline.positionCount = 0;
            }
        }//The checker for UI holders
        else if(Physics.Raycast(rightCont.transform.position, rightCont.transform.forward, out hit, 1.5f, layermaskUI)) {
            line.SetPositions(new Vector3[] { rightCont.transform.position, hit.point });
            line.startColor = Color.green;
            line.endColor = Color.green;
            hit.collider.transform.FindChildRecursive("Cursor").position = hit.point;
            if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger)) {
                RaycastHit interact;
              //The checker for UI buttons in holders
                if (Physics.Raycast(rightCont.transform.position, rightCont.transform.forward, out interact, 1.5f, layermaskUIinteract)) {
                    if (mm.PopupOnly && interact.collider.gameObject.tag != "popup") return;
                    UnityEngine.EventSystems.ExecuteEvents.Execute(interact.collider.gameObject, new UnityEngine.EventSystems.BaseEventData(UnityEngine.EventSystems.EventSystem.current), UnityEngine.EventSystems.ExecuteEvents.submitHandler);
                }
            }
        } else {
            line.SetPositions(new Vector3[] { rightCont.transform.position, rightCont.transform.position + rightCont.transform.forward * 0.2f });
            line.startColor = Color.white;
            line.endColor = Color.white;
        }
    }
}
