using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionPanelController : MonoBehaviour
{
    public string ButtonText;
    public string OptionText;
    public string ModelOption;
    public GameObject resetButton;
    public GameObject[] prefabs; //0 cube, 1 SC, 2 BCC, 3 FCC
    private GameObject exampleObject;
    public Color color;

    public int index;
    public UnityEngine.UI.Button optionButton;

    private GameObject SpawnModel(string model) {
        int type;
        switch (model) {
            case "FCC":
                type = 3;
                break;
            case "BCC":
                type = 2;
                break;
            case "SC":
                type = 1;
                break;
            default:
                type = 0;
                break;
        }
        return Instantiate(prefabs[type]);
    }
    public void resetExample() {
        if (exampleObject) {
            exampleObject.transform.parent = transform;
            exampleObject.transform.localPosition = new Vector3(396, 0, 0);
            exampleObject.transform.localRotation = Quaternion.identity;
            exampleObject.transform.localScale = new Vector3(200, 200, 200);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.FindChildRecursive("ButtonText").GetComponent<TMPro.TextMeshProUGUI>().text = ButtonText;
        transform.Find("OptionText").GetComponent<TMPro.TextMeshProUGUI>().text = OptionText;
        if(ModelOption != "None") {
            exampleObject = SpawnModel(ModelOption);
            exampleObject.transform.parent = transform;
            resetButton.SetActive(true);
            optionButton.GetComponent<UnityEngine.UI.Image>().color = color;
            exampleObject.GetComponent<AtomsController>().setColor(color);
        }
        Invoke("resetExample", 0.5f);
    }
    void OnPostRender() {
        resetExample();
    }
}
