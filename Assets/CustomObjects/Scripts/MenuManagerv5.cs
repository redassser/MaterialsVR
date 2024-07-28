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

    private List<LineRenderer> drawnLines = new List<LineRenderer>();
    private LineRenderer workingLine;
    private bool startedDraw = false;

    private void Start() {
        layermaskUI = LayerMask.GetMask("UI");
        layermaskUIinteract = LayerMask.GetMask("UIinteract");
        layermaskDraw = LayerMask.GetMask("Draw");
    }

    private void Update() {
        InputHandler();
    }

    private void InputHandler() {
        RaycastHit shortHitDraw;
        bool shortHitDrawSuccess = Physics.Raycast(rightCont.transform.position, rightCont.transform.forward, out shortHitDraw, 0.2f, layermaskDraw);

        RaycastHit longHitUi;
        bool longHitUiSuccess = Physics.Raycast(rightCont.transform.position, rightCont.transform.forward, out longHitUi, 1.5f, layermaskUI);

        RaycastHit longHitUiInteract;
        bool longHitUiInteractSuccess = Physics.Raycast(rightCont.transform.position, rightCont.transform.forward, out longHitUiInteract, 1.5f, layermaskUIinteract);

        bool rightPressedB = OVRInput.GetDown(OVRInput.RawButton.B);
        bool rightPressedR = OVRInput.Get(OVRInput.RawButton.RIndexTrigger);

        // Pressing B removes last drawn line
        if (rightPressedB && drawnLines.Count > 0) {
            removeLastLine();
        }
        if (shortHitDrawSuccess) {
            setControllerLine(Color.yellow, new Vector3[] { rightCont.transform.position, rightCont.transform.position + rightCont.transform.forward * 0.2f });

            workingLine = shortHitDraw.collider.GetComponentInParent<LineRenderer>();

            if (rightPressedR) {
                if (shortHitDraw.collider.transform.parent.GetComponentInParent<planeAdj>().locked) {
                    setControllerLine(Color.red, new Vector3[] { });
                    workingLine = null;
                } 
                else if (workingLine.positionCount == 0) {
                    startedDraw = true;
                    workingLine.positionCount = 2;
                    workingLine.SetPosition(0, shortHitDraw.collider.transform.localPosition);
                    workingLine.SetPosition(1, shortHitDraw.collider.transform.localPosition);
                } else {
                    for (int i = 0; i < workingLine.positionCount; i++) {
                        if (workingLine.GetPosition(i) == shortHitDraw.collider.transform.localPosition) {
                            if (workingLine.positionCount == 4) {
                                Vector3[] points = new Vector3[3];
                                startedDraw = false;
                                workingLine.positionCount--;
                                workingLine.SetPosition(workingLine.positionCount - 1, shortHitDraw.collider.transform.localPosition);
                                workingLine.loop = true;
                                drawnLines.Add(workingLine);
                                workingLine.GetPositions(points);
                                workingLine.transform.GetComponentInParent<planeAdj>().RePlane(points);
                                workingLine.transform.GetComponentInParent<planeAdj>().locked = true;
                                workingLine.positionCount = 0;
                                workingLine = null;
                            }
                            return;
                        }
                    }
                    workingLine.positionCount++;
                    workingLine.SetPosition(workingLine.positionCount - 1, workingLine.transform.InverseTransformPoint(rightCont.transform.position + rightCont.transform.forward * 0.2f));
                    workingLine.SetPosition(workingLine.positionCount - 2, shortHitDraw.collider.transform.localPosition);
                }
            } else if (startedDraw) {
                startedDraw = false;
                workingLine.positionCount = 0;
            }
        } else if (startedDraw) {
            line.SetPositions(new Vector3[] { rightCont.transform.position, rightCont.transform.position + rightCont.transform.forward * 0.2f });
            if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger)) {
                workingLine.SetPosition(workingLine.positionCount - 1, workingLine.transform.InverseTransformPoint(rightCont.transform.position + rightCont.transform.forward * 0.2f));
            } else {
                startedDraw = false;
                workingLine.positionCount = 0;
            }
        }
        else if (longHitUiSuccess) {
            setControllerLine(Color.green, new Vector3[] { rightCont.transform.position, longHitUi.point });
            longHitUi.collider.transform.FindChildRecursive("Cursor").position = longHitUi.point;

            if (rightPressedR && longHitUiInteractSuccess) {
                if (mm.PopupOnly && longHitUiInteract.collider.gameObject.tag != "popup") return;
                UnityEngine.EventSystems.ExecuteEvents.Execute(longHitUiInteract.collider.gameObject, new UnityEngine.EventSystems.BaseEventData(UnityEngine.EventSystems.EventSystem.current), UnityEngine.EventSystems.ExecuteEvents.submitHandler);
            }
        } else {
            setControllerLine(Color.white, new Vector3[] { rightCont.transform.position, rightCont.transform.position + rightCont.transform.forward * 0.2f });
        }
    }

    private void removeLastLine() {
        drawnLines[drawnLines.Count - 1].GetComponentInParent<MeshFilter>().mesh = null;
        drawnLines[drawnLines.Count - 1].loop = false;
        drawnLines.RemoveAt(drawnLines.Count - 1);
    } 
    
    private void setControllerLine(Color color, Vector3[] points) {
        line.startColor = color;
        line.endColor = color;
        if (points.Length != 0) {
            line.SetPositions(points);
        }
    }
}
