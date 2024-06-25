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
        qcorrect.Clear();
        return;
    }
    private void clearLevel() {
        unloadLevel();
        for(int i=0;i< MainMenuContent.transform.GetChild(1).childCount; i++) {
            if (i == 0) continue;
            Destroy(MainMenuContent.transform.GetChild(1).GetChild(i).gameObject);
        }
        currentQI = -1;
        qpanels.Clear();
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
        for (int i = 0; i < levelset.Questions.Length; i++) {
            GameObject temppanel = Instantiate(questionHolder, transform, false);
            temppanel.transform.localPosition = new Vector3(0.85f + 0.65f*i, 0f, 0f);

            GameObject exampleObject = null;
            if (levelset.Questions[i].Model != "None") {
                exampleObject = SpawnModel(levelset.Questions[i].Model);
                StartCoroutine(resetExample(exampleObject, temppanel.transform));
                qExamples.Add(exampleObject);
            }

            int ind = 0;

            if (levelset.Questions[i].type == "PD") {
                temppanel.transform.GetChild(0).GetComponentInChildren<TMPro.TextMeshProUGUI>().text = levelset.Questions[i].title;
                GameObject tempop = Instantiate(questionOptionPanel, temppanel.transform.GetChild(0).GetChild(0).GetChild(0), false);
                int pi = i; int opi = ind;
                tempop.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(delegate { submitAnswer(j, pi, opi, exampleObject.GetComponent<planeAdj>()); });
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
    /*
    private void loadLevel(TextAsset text) {

        
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
            int[] init = new int[level.Questions.Length];
            for(int l=0;l<level.Questions.Length;l++) {
                init[l] = -1;
            }
            qcorrect.Add(init);
        }
        oneTry = levelset.oneTry;
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
    private void loadQuestions(TextAsset text) {
        levelset = LevelSet.createFromJson(text.ToString());
        if () return;
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

            GameObject exampleObject = null;
            if(level.Questions[i].Model != "None") {
                exampleObject = SpawnModel(level.Questions[i].Model);
                StartCoroutine(resetExample(exampleObject, temppanel.transform));
                qExamples.Add(exampleObject);
            }

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
                    if(qcorrect[qi][i] == -2 && ind == level.Questions[i].correctIndex) {
                        tempop.GetComponent<UnityEngine.UI.Image>().color = new Color(0, 1, 0);
                    } else if (qcorrect[qi][i] != -1 && qcorrect[qi][i] == ind) {
                        tempop.GetComponent<UnityEngine.UI.Image>().color = new Color(1, 0, 0);
                    }
                    tempop.GetComponent<RectTransform>().anchoredPosition = new Vector2(10 + 250 * (ind % 4), -5 - 95 * (ind / 4));
                    tempop.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = level.Questions[i].options[ind].Text;
                    int pi = i; int opi = ind;
                    tempop.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(delegate { submitAnswer(qi, pi, opi); });

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
        return;
    }

    public void submitAnswer(int qi, int pi, int opi, planeAdj h = null) { // Question index, part index, option index. for example Question 1, part B, option 3
        if (oneTry && qcorrect[qi][pi] != -1) return;
        int correcti = levelset.Levels[qi].Questions[pi].correctIndex;
        if (h != null) {
            int[] ints = h.planeType;
            int[] currentCorrectPlane = levelset.Levels[qi].Questions[pi].correctPlane;
            UnityEngine.UI.Image button = qHolders[pi].transform.GetChild(0).GetChild(0).GetChild(1).GetChild(opi).GetComponent<UnityEngine.UI.Image>();
            if ((currentCorrectPlane[0] == ints[0] && currentCorrectPlane[1] == ints[1] && currentCorrectPlane[2] == ints[2]) ||
            (currentCorrectPlane[0] == -ints[0] && currentCorrectPlane[1] == -ints[1] && currentCorrectPlane[2] == -ints[2])) { // check negative as well
                qpanels[qi][pi + 1].GetComponent<UnityEngine.UI.Image>().color = new Color(0, 1, 0);
                button.color = new Color(0, 1, 0);
                qcorrect[qi][pi] = -2;
            } else {
                qpanels[qi][pi + 1].GetComponent<UnityEngine.UI.Image>().color = new Color(1, 0, 0);
                button.color = new Color(1, 0, 0);
                qcorrect[qi][pi] = opi;
            }
        } else {
            foreach(Transform f in qHolders[pi].transform.GetChild(0).GetChild(0).GetChild(1)) {
                f.GetComponent<UnityEngine.UI.Image>().color = new Color(1, 1, 1);
            }
            UnityEngine.UI.Image button = qHolders[pi].transform.GetChild(0).GetChild(0).GetChild(1).GetChild(opi).GetComponent<UnityEngine.UI.Image>();
            if (opi == correcti) {
                qpanels[qi][pi + 1].GetComponent<UnityEngine.UI.Image>().color = new Color(0, 1, 0);
                button.color = new Color(0, 1, 0);
                qcorrect[qi][pi] = -2;
            } else {
                qpanels[qi][pi + 1].GetComponent<UnityEngine.UI.Image>().color = new Color(1, 0, 0);
                button.color = new Color(1, 0, 0);
                qcorrect[qi][pi] = opi;
            }
        }
    }*/

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
        public Question[] Questions;
    }

}
