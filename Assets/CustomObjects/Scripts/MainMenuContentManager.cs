using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuContentManager : MonoBehaviour
{
    public GameObject MainMenuContent;
    public GameObject TopBarContent;
    public GameObject PopupContent;

    public GameObject[] prefabs;

    // Popup stuff
    public bool PopupOnly = false;

    // Level Select
    public GameObject levelPanel;
    private List<GameObject> panels = new List<GameObject>();
    public LevelSet levelset;
    private bool oneTry;
    private bool levelsLoaded = false;

    // Question set
    public GameObject questionPanel;
    public GameObject partPanel;
    private List<GameObject> qpanels = new List<GameObject>();
    private List<int[]> qcorrect = new List<int[]>(); // -1 is unaswered, -2 is correct, anything else is option answered incorrect

    // Question
    public GameObject questionHolder;
    public GameObject questionOptionPanel;
    public GameObject LabelHolder;
    private List<GameObject> qHolders = new List<GameObject>();
    private List<GameObject> qExamples = new List<GameObject>();
    private int currentQI = -1;

    // Popup functions
    public void popup(int popuptype) {
        PopupOnly = true;
        PopupContent.transform.GetChild(0).gameObject.SetActive(true);
        foreach(GameObject qh in qHolders) {
            qh.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
        }
        switch(popuptype) {
            case 2:
                PopupContent.transform.GetChild(3).gameObject.SetActive(true);
                return;
            case 1:
                PopupContent.transform.GetChild(2).gameObject.SetActive(true);
                return;
            case 0:
                PopupContent.transform.GetChild(1).gameObject.SetActive(true);
                return;
            default:
                return;
        }
    }
    public void ClosePopup() {
        PopupOnly = false;
        foreach (GameObject qh in qHolders) {
            qh.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
        }
        for (int i = 0;i < PopupContent.transform.childCount; i++) {
            PopupContent.transform.GetChild(i).gameObject.SetActive(false);
        }
        return;
    }
    public void returnToCategorySelectFromPopup() {
        ClosePopup();
        returnToCategorySelect();
        return;
    }
    public void submitLevelFromPopup() {
        ClosePopup();
        submitLevel();
        return;
    }

    public void closeMenus() {
        for (int i = 0; i < MainMenuContent.transform.childCount; i++) {
            MainMenuContent.transform.GetChild(i).gameObject.SetActive(false);
        }
        return;
    }

    // Menu
    public void submitLevel() {
        closeMenus();
        popup(2);
        int c = 0, i= 0, u = 0;
        foreach (int[] k in qcorrect) {
            foreach (int n in k) {
                if (n == -2) c += 1;
                else if (n == -1) u += 1;
                else i += 1;
            }
        }
        PopupContent.transform.GetChild(3).GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Correct: "+c+"\nIncorrect: "+i+"\nUnanswered: "+u;
    }
    
    public void toLevelSelect(string category) {
        closeMenus();
        clearLevel();
        MainMenuContent.transform.GetChild(0).gameObject.SetActive(true);
        TopBarContent.transform.GetChild(0).gameObject.SetActive(true);
        TopBarContent.transform.GetChild(1).gameObject.SetActive(false);
        if (levelsLoaded) return;
        TextAsset[] FSitems = Resources.LoadAll<TextAsset>(category);
        int ind = 0;
        foreach (TextAsset i in FSitems) {
            GameObject temppanel = Instantiate(levelPanel, MainMenuContent.transform.GetChild(0), false);
            int h = ind;
            temppanel.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { loadLevel(i, h); });
            temppanel.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = i.name;
            panels.Add(temppanel);
            
            int[] nums = new int[LevelSet.createFromJson(i.ToString()).Questions.Length];
            for (int f=0;f<nums.Length;f++) { nums[f] = -1; }
            qcorrect.Add(nums);
            ind++;
        }
        levelsLoaded = true;
        return;
    }
    public void returnToCategorySelect() {
        closeMenus();
        clearLevel();
        MainMenuContent.transform.GetChild(2).gameObject.SetActive(true);
        TopBarContent.transform.GetChild(0).gameObject.SetActive(false);
        TopBarContent.transform.GetChild(1).gameObject.SetActive(false);
        foreach(GameObject panel in panels) {
            Destroy(panel);
        }
        panels.Clear();
        qcorrect.Clear();
        return;
    }
    private void clearLevel() {
        unloadLevel();
        for(int i=0;i< MainMenuContent.transform.GetChild(0).childCount; i++) {
            if (i == 0) continue;
            Destroy(MainMenuContent.transform.GetChild(0).GetChild(i).gameObject);
        }
        currentQI = -1;
        qpanels.Clear();
        levelsLoaded = false;
    }
    private void unloadLevel() {
        foreach (GameObject qHolder in qHolders) {
            Destroy(qHolder);
        }
        foreach (GameObject qExample in qExamples) {
            if (qExample) Destroy(qExample);
        }
        qHolders.Clear();
        qExamples.Clear();
    }
    private void loadLevel(TextAsset text, int j) {
        levelset = LevelSet.createFromJson(text.ToString());
        if (currentQI == j) return;
        currentQI = j;
        unloadLevel();
        GameObject templabel = null;
        if (levelset.label != null) {
            templabel = Instantiate(LabelHolder, transform, false);
            templabel.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = levelset.label;
            templabel.transform.localPosition = new Vector3(1.491f, 0.5080001f, 0f);
        }
        for (int i = 0; i < levelset.Questions.Length; i++) {
            GameObject temppanel = Instantiate(questionHolder, transform, false);
            temppanel.transform.localPosition = new Vector3(0.85f + 0.65f*i, 0f, 0f);

            GameObject exampleObject = null;
            if (levelset.Questions[i].Model != "None") {
                exampleObject = SpawnModel(levelset.Questions[i].Model);
                if (levelset.Questions[i].type == "P") {
                    exampleObject.GetComponent<planeAdj>().Set(levelset.Questions[i].correctPlane);
                }
                StartCoroutine(resetExample(exampleObject, temppanel.transform));
                qExamples.Add(exampleObject);
            }

            int ind = 0;

            if (levelset.Questions[i].type == "PD") {
                temppanel.transform.GetChild(0).GetComponentInChildren<TMPro.TextMeshProUGUI>().text = levelset.Questions[i].title;
                GameObject tempop = Instantiate(questionOptionPanel, temppanel.transform.GetChild(0).GetChild(0).GetChild(0), false);
                int pi = i; int opi = ind;
                tempop.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(delegate { submitAnswer(j, pi, opi, exampleObject.GetComponent<planeAdj>()); });
                if (qcorrect[j][i] == -2) {
                    tempop.GetComponent<UnityEngine.UI.Image>().color = new Color(0, 1, 0);
                } else if (qcorrect[j][i] != -1) {
                    tempop.GetComponent<UnityEngine.UI.Image>().color = new Color(1, 0, 0);
                }
            }

            if (levelset.Questions[i].options != null) {
                foreach (Option op in levelset.Questions[i].options) {
                    GameObject tempop = Instantiate(questionOptionPanel, temppanel.transform.GetChild(0).GetChild(0).GetChild(0), false);
                    if (qcorrect[j][i] == -2 && ind == levelset.Questions[i].correctIndex) {
                        tempop.GetComponent<UnityEngine.UI.Image>().color = new Color(0, 1, 0);
                    } else if (qcorrect[j][i] != -1 && qcorrect[j][i] == ind) {
                        tempop.GetComponent<UnityEngine.UI.Image>().color = new Color(1, 0, 0);
                    }
                    tempop.GetComponent<RectTransform>().anchoredPosition = new Vector2(-150 + 250 * (ind % 2), -10 - 95 * (ind / 2));
                    tempop.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = levelset.Questions[i].options[ind].Text;
                    int pi = i; int opi = ind;
                    tempop.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(delegate { submitAnswer(j, pi, opi); });

                    if (op.Model != "None" && op.Model != null) {
                        GameObject optionObject = SpawnModel(op.Model);
                        StartCoroutine(resetExample(optionObject, tempop.transform, 1));
                        qExamples.Add(optionObject);
                    }

                    ind++;
                }
            }
            qHolders.Add(temppanel);
        }
        if (templabel != null) {
            qHolders.Add(templabel);
        }
        return;
    }

    public void submitAnswer(int qi, int pi, int opi, planeAdj h = null) { // Question index, part index, option index. for example Question 1, part B, option 3
        if (oneTry && qcorrect[qi][pi] != -1) return;
        int correcti = levelset.Questions[pi].correctIndex;
        if (h != null) {
            int[] ints = h.planeType;
            int[] currentCorrectPlane = levelset.Questions[pi].correctPlane;
            UnityEngine.UI.Image button = qHolders[pi].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(opi).GetComponent<UnityEngine.UI.Image>();
            if ((currentCorrectPlane[0] == ints[0] && currentCorrectPlane[1] == ints[1] && currentCorrectPlane[2] == ints[2]) ||
            (currentCorrectPlane[0] == -ints[0] && currentCorrectPlane[1] == -ints[1] && currentCorrectPlane[2] == -ints[2])) { // check negative as well
                button.color = new Color(0, 1, 0);
                qcorrect[qi][pi] = -2;
            } else {
                button.color = new Color(1, 0, 0);
                qcorrect[qi][pi] = opi;
            }
        } else {
            foreach (Transform f in qHolders[pi].transform.GetChild(0).GetChild(0).GetChild(0)) {
                f.GetComponent<UnityEngine.UI.Image>().color = new Color(1, 1, 1);
            }
            UnityEngine.UI.Image button = qHolders[pi].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(opi).GetComponent<UnityEngine.UI.Image>();
            if (opi == correcti) {
                button.color = new Color(0, 1, 0);
                qcorrect[qi][pi] = -2;
            } else {
                button.color = new Color(1, 0, 0);
                qcorrect[qi][pi] = opi;
            }
        }
    }

    IEnumerator resetExample(GameObject exampleObject, Transform holder, int op = 0) {
        yield return new WaitForSeconds(0.025f);
        exampleObject.transform.parent = holder;
        if (op == 0) {
            exampleObject.transform.localPosition = new Vector3(0f, 0.135f, 0f);
            exampleObject.transform.localRotation = Quaternion.Euler(new Vector3(0f, 135f, 0f));
            exampleObject.transform.localScale = new Vector3(0.16f, 0.16f, 0.16f);
        } else {
            exampleObject.transform.localPosition = new Vector3(120f, -45f, 0);
            exampleObject.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
            exampleObject.transform.localScale = new Vector3(72f, 72f, 72f);
        }
        
        yield return null;
    }
    private GameObject SpawnModel(string model) {
        int type;
        switch (model) {
            case "D":
                type = 4;
                break;
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

    void Start() {
        returnToCategorySelect();
    }

    [System.Serializable]
    public class Option
    {
        public string Button;
        public string Text;
        public string Model;
    }
    [System.Serializable]
    public class Question
    {
        public string type;
        public string title;
        public string Model;
        public Option[] options;
        public int correctIndex;
        public int[] correctPlane;
    }
    [System.Serializable]
    public class LevelSet
    {
        public static LevelSet createFromJson(string jsonString) {
            return JsonUtility.FromJson<LevelSet>(jsonString);
        }
        public bool oneTry;
        public string name;
        public string label;
        public string instr;
        public Question[] Questions;
    }

}
