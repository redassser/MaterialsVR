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
    //

    // Level Select
    public GameObject levelPanel;
    private List<GameObject> panels = new List<GameObject>();
    public LevelSet levelset;
    private bool levelsLoaded = false;

    // Question set
    public GameObject questionPanel;
    public GameObject partPanel;
    private List<GameObject[]> qpanels = new List<GameObject[]>();
    private List<int[]> qcorrect = new List<int[]>(); // 0 is unanswered, 1 is correct, 2 is incorrect

    // Question
    public GameObject questionHolder;
    public GameObject questionOptionPanel;
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
    public void returnToLevelSelectFromPopup() {
        ClosePopup();
        returnToLevelSelect();
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
                if (n == 1) c += 1;
                else if (n == 2) i += 1;
                else u += 1;
            }
        }
        PopupContent.transform.GetChild(3).GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Correct: "+c+"\nIncorrect: "+i+"\nUnanswered: "+u;
    }
    public void returnToLevelSelect() {
        closeMenus();
        clearLevel();
        MainMenuContent.transform.GetChild(0).gameObject.SetActive(true);
        TopBarContent.transform.GetChild(0).gameObject.SetActive(false);
        TopBarContent.transform.GetChild(1).gameObject.SetActive(false);
        if (levelsLoaded) return;
        TextAsset[] FSitems = Resources.LoadAll<TextAsset>("Levels");
        foreach (TextAsset i in FSitems) {
            GameObject temppanel = Instantiate(levelPanel, MainMenuContent.transform.GetChild(0), false);
            temppanel.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(delegate { loadLevel(i); });
            temppanel.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = i.name;
            panels.Add(temppanel);
        }
        levelsLoaded = true;
        return;
    }
    private void clearLevel() {
        for(int i=0;i< MainMenuContent.transform.GetChild(1).childCount; i++) {
            if (i == 0) continue;
            Destroy(MainMenuContent.transform.GetChild(1).GetChild(i).gameObject);
        }
        unloadQuestion();
        currentQI = -1;
        qpanels.Clear();
        qcorrect.Clear();
    }

    private void loadLevel(TextAsset text) {
        closeMenus();
        clearLevel();
        MainMenuContent.transform.GetChild(1).gameObject.SetActive(true);
        TopBarContent.transform.GetChild(0).gameObject.SetActive(true);
        TopBarContent.transform.GetChild(1).gameObject.SetActive(true);
        levelset = LevelSet.createFromJson(text.ToString());
        for(int i=0;i<levelset.Levels.Length;i++) {
            Level level = levelset.Levels[i];
            GameObject temppanel = Instantiate(questionPanel, MainMenuContent.transform.GetChild(1), false);
            int ind = i;
            temppanel.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(delegate { loadQuestion(ind); });
            temppanel.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Q"+(i+1);
            GameObject[] panes = new GameObject[level.Questions.Length+1];
            panes[0] = temppanel;
            for (int j=0;j<level.Questions.Length;j++) {
                Question question = level.Questions[j];
                GameObject temp1panel = Instantiate(partPanel, MainMenuContent.transform.GetChild(1), false);
                temp1panel.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "P" + (j + 1);
                panes[j+1] = temp1panel;
            }
            qpanels.Add(panes);
            qcorrect.Add(new int[level.Questions.Length]);
        }
        return;
    }
    private void unloadQuestion() {
        foreach(GameObject qHolder in qHolders) {
            Destroy(qHolder);
        }
        foreach (GameObject qExample in qExamples) {
            if(qExample) Destroy(qExample);
        }
        qHolders.Clear();
        qExamples.Clear();
    }
    private void loadQuestion(int qi) {
        if (qi == currentQI) return;
        currentQI = qi;
        unloadQuestion();
        for(int i=0;i<qpanels.Count;i++) {
            qpanels[i][0].transform.GetChild(1).GetComponent<UnityEngine.UI.Image>().color = new Color(0.5581516f, 1f, 0.4470588f);
        }
        qpanels[qi][0].transform.GetChild(1).GetComponent<UnityEngine.UI.Image>().color = Color.grey;
        Level level = levelset.Levels[qi];
        for (int i = 0; i < level.Questions.Length; i++) {
            GameObject temppanel = Instantiate(questionHolder, transform, false);
            temppanel.transform.localPosition = new Vector3(1.2f,-0.55f+0.55f*i,0f);
            temppanel.transform.GetChild(0).GetChild(0).GetComponentInChildren<TMPro.TextMeshProUGUI>().text = level.Questions[i].title;

            GameObject exampleObject = SpawnModel(level.Questions[i].Model);
            StartCoroutine(resetExample(exampleObject, temppanel.transform));
            qExamples.Add(exampleObject);

            int ind = 0;

            if (level.Questions[i].type == "PD") {
                GameObject tempop = Instantiate(questionOptionPanel, temppanel.transform.GetChild(0).GetChild(0).GetChild(1), false);
                tempop.GetComponent<RectTransform>().anchoredPosition = new Vector2(10, -5);
                tempop.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Check";
                int pi = i; int opi = 0;
                tempop.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(delegate { submitAnswer(qi, pi, opi, exampleObject.GetComponent<planeAdj>()); });
            }

            if (level.Questions[i].options != null) {
                foreach (Option op in level.Questions[i].options) {
                    GameObject tempop = Instantiate(questionOptionPanel, temppanel.transform.GetChild(0).GetChild(0).GetChild(1), false);
                    tempop.GetComponent<RectTransform>().anchoredPosition = new Vector2(10 + 250 * (ind % 4), -5 - 95 * (ind / 4));
                    tempop.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = level.Questions[i].options[ind].Text;
                    int pi = i; int opi = ind;
                    tempop.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(delegate { submitAnswer(qi, pi, opi); });
                    ind++;
                }
            }
            qHolders.Add(temppanel);
        }
        return;
    }

    public void submitAnswer(int qi, int pi, int opi, planeAdj h = null) { // Question index, part index, option index. for example Question 1, part B, option 3
        int correcti = levelset.Levels[qi].Questions[pi].correctIndex;
        if (h != null) {
            int[] ints = h.planeType;
            int[] currentCorrectPlane = levelset.Levels[qi].Questions[pi].correctPlane;
            if ((currentCorrectPlane[0] == ints[0] && currentCorrectPlane[1] == ints[1] && currentCorrectPlane[2] == ints[2]) ||
            (currentCorrectPlane[0] == -ints[0] && currentCorrectPlane[1] == -ints[1] && currentCorrectPlane[2] == -ints[2])) { // check negative as well
                qpanels[qi][pi + 1].GetComponent<UnityEngine.UI.Image>().color = new Color(0, 1, 0);
                qcorrect[qi][pi] = 1;
            } else {
                qpanels[qi][pi + 1].GetComponent<UnityEngine.UI.Image>().color = new Color(1, 0, 0);
                qcorrect[qi][pi] = 2;
            }
        } else {
            if (opi == correcti) {
                qpanels[qi][pi + 1].GetComponent<UnityEngine.UI.Image>().color = new Color(0, 1, 0);
                qcorrect[qi][pi] = 1;
            } else {
                qpanels[qi][pi + 1].GetComponent<UnityEngine.UI.Image>().color = new Color(1, 0, 0);
                qcorrect[qi][pi] = 2;
            }
        }
    }

    IEnumerator resetExample(GameObject exampleObject, Transform holder) {
        yield return new WaitForSeconds(0.025f);
        exampleObject.transform.parent = holder;
        exampleObject.transform.localPosition = new Vector3(-0.5f, 0, 0);
        exampleObject.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        exampleObject.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
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
        returnToLevelSelect();
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
    public class Level
    {
        public Question[] Questions;
    }
    [System.Serializable]
    public class LevelSet
    {
        public static LevelSet createFromJson(string jsonString) {
            return JsonUtility.FromJson<LevelSet>(jsonString);
        }
        public Level[] Levels;
    }

}
