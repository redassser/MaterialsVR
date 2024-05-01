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
    public LineRenderer drawline;
    public List<LineRenderer> drawnlines = new List<LineRenderer>();
    LayerMask layermaskUI;
    LayerMask layermaskDraw;
    LayerMask layermaskUIinteract = 1 << 3;
    bool startedDraw = false;
    bool pressedb = false;
    // Start is called before the first frame update
    void Start()
    {
        line.enabled = true;
        layermaskUI = LayerMask.GetMask("UI");
        layermaskDraw = LayerMask.GetMask("Draw");
        layermaskUIinteract = LayerMask.GetMask("UIinteract");
    }

    // Update is called once per frame
    void Update()
    {
        if (drawnlines.Count != 0 && OVRInput.GetDown(OVRInput.RawButton.B)) {
            drawnlines[drawnlines.Count - 1].GetComponentInParent<MeshFilter>().mesh = null;
            drawnlines[drawnlines.Count - 1].loop = false;
            drawnlines.RemoveAt(drawnlines.Count - 1);
        } else if (Physics.Raycast(rightCont.transform.position, rightCont.transform.forward, out hit, 0.2f, layermaskDraw)) {
            drawline = hit.collider.GetComponentInParent<LineRenderer>();
            line.SetPositions(new Vector3[] { rightCont.transform.position, rightCont.transform.position + rightCont.transform.forward * 0.2f });
            if (drawnlines.Contains(drawline)) {
                line.startColor = Color.red;
                line.endColor = Color.red;
                drawline = null;
                return;
            }
            line.startColor = Color.yellow;
            line.endColor = Color.yellow;
            if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger)) {
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
            } else {
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
        } else if (Physics.Raycast(rightCont.transform.position, rightCont.transform.forward, out hit, 1.5f, layermaskUI)) {
            if (OVRInput.Get(OVRInput.RawAxis2D.RThumbstick).magnitude > 0.1)
                menuScrollRect.verticalNormalizedPosition += OVRInput.Get(OVRInput.RawAxis2D.RThumbstick).y * Time.deltaTime;

            line.SetPositions(new Vector3[] { rightCont.transform.position, hit.point });
            line.startColor = Color.green;
            line.endColor = Color.green;
            hit.collider.transform.FindChildRecursive("Cursor").position = hit.point;
            if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger)) {
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
