using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentManager : MonoBehaviour
{
    public TMPro.TextMeshProUGUI title;
    public GameObject[] prefabs; //0 cube, 1 SC, 2 BCC, 3 FCC
    public UnityEngine.UI.Image bg;
    public GameObject resetButton;
    public GameObject exampleObject;
    public Transform holder;

    public int[] currentCorrectPlane = new int[3];
    public int currentCorrect;
    public string currentType;
    private List<OptionPanelController> currentPanels = new List<OptionPanelController>();
    public int numCorrect = 0;
    public int numTotal = 0;
    private int currentLevel = 0;
    public bool checkedf = false;
    private LevelSet levelset;
    private TextAsset[] FSitems;

    public OptionPanelController panelPrefab;
    public GameObject gradepanel;
    private List<GameObject> panels = new List<GameObject>();
    public GameObject panelLevel;

    public bool isMain = false;
    public GameObject holderinstant;
    public ContentManager main;
    public List<ContentManager> nonmain = new List<ContentManager>();
    public int an = 0;

    public void selfDestruct() {
        Destroy(holder.gameObject);
    }

    // Correct / Incorrect addressing
    private void CorrectAnswer(bool isCorrect) {
        main.numTotal++;
        main.an++;
        if(isCorrect) {
            Color green = new Color(0.1921119f, 0.7547169f, 0.1245995f, 0.39f);
            main.numCorrect++;
            bg.color = green;
        } else {
            Color red = new Color(0.8490566f, 0.1161445f, 0.1161445f, 0.39f);
            bg.color = red;
        }
        Invoke("resetColors", 3);
    }
    private void resetColors() {
        Color normal = new Color(1f, 1f, 1f, 0.39f);
        bg.color = normal;

        foreach (OptionPanelController op in currentPanels) {
            Debug.Log(op.name);
            if (op != null) {
                op.resetExample();
                Destroy(op.gameObject);
            }
        }
        currentPanels.Clear();
    }

    // Model Setting
    private GameObject SpawnModel(string model) {
        int type;
        switch(model) {
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
    public void resetExample() {
        exampleObject.transform.parent = holder;
        exampleObject.transform.localPosition = new Vector3(-1.1f,0,0);
        exampleObject.transform.localRotation = Quaternion.Euler(new Vector3(0f,0f,0f));
        exampleObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }

    // MCMM ( Multiple Choice Multiple Model )
    // Multiple choice menu where each has a small model that can be inspected, with a single text question
    private void setMCMM(Question q) {
        currentCorrect = q.correctIndex;

        title.text = q.title;

        for (int ind = 0; ind < q.options.Length; ind++) {
            OptionPanelController temppanel = Instantiate(panelPrefab, transform, false);
            temppanel.optionButton.onClick.AddListener(delegate { answerSelect(temppanel.index); });
            temppanel.color = Random.ColorHSV();
            temppanel.index = ind;
            temppanel.ButtonText = q.options[ind].Button;
            temppanel.OptionText = q.options[ind].Text;
            temppanel.ModelOption = q.options[ind].Model;
            Vector3 temp = temppanel.GetComponent<RectTransform>().anchoredPosition;
            temp.y = 750 - 220 * ind;
            temppanel.GetComponent<RectTransform>().anchoredPosition = temp;
            currentPanels.Add(temppanel);
        }
    }
    private void checkMCMM(int ind) {
        if (ind == currentCorrect) {
            CorrectAnswer(true);
        } else {
            CorrectAnswer(false);
        }
    }

    // MCSM ( Multiple Choice Single Model )
    // Multiple choice menu where there is only one large model that can be inspected, associated with a text question
    private void setMCSM(Question q) {
        currentCorrect = q.correctIndex;

        title.text = q.title;
        resetButton.SetActive(true);
        exampleObject = SpawnModel(q.Model);
        Invoke("resetExample", 0.125f);

        for (int ind = 0; ind < q.options.Length; ind++) {
            OptionPanelController temppanel = Instantiate(panelPrefab, transform, false);
            temppanel.optionButton.onClick.AddListener(delegate { answerSelect(temppanel.index); });
            temppanel.color = Random.ColorHSV();
            temppanel.index = ind;
            temppanel.ButtonText = q.options[ind].Button;
            temppanel.OptionText = q.options[ind].Text;
            temppanel.ModelOption = q.options[ind].Model;
            Vector3 temp = temppanel.GetComponent<RectTransform>().anchoredPosition;
            temp.y = 750 - 220 * ind;
            temppanel.GetComponent<RectTransform>().anchoredPosition = temp;
            currentPanels.Add(temppanel);
        }
    }
    private void checkMCSM(int ind) {
        if (ind == currentCorrect) {
            CorrectAnswer(true);
        } else {
            CorrectAnswer(false);
        }
        resetButton.SetActive(false);
    }

    // MCAM ( Multiple Choice All Model )
    // Multiple choice menu where each has a small model that can be inspected, with a single text question that also has an associated model
    private void setMCAM(Question q) {
        currentCorrect = q.correctIndex;

        title.text = q.title;
        resetButton.SetActive(true);
        exampleObject = SpawnModel(q.Model);
        Invoke("resetExample", 0.125f);

        for (int ind = 0; ind < q.options.Length; ind++) {
            OptionPanelController temppanel = Instantiate(panelPrefab, transform, false);
            temppanel.color = Random.ColorHSV();
            temppanel.optionButton.onClick.AddListener(delegate { answerSelect(temppanel.index); });
            temppanel.index = ind;
            temppanel.ButtonText = q.options[ind].Button;
            temppanel.OptionText = q.options[ind].Text;
            temppanel.ModelOption = q.options[ind].Model;
            Vector3 temp = temppanel.GetComponent<RectTransform>().anchoredPosition;
            temp.y = 750 - 220 * ind;
            temppanel.GetComponent<RectTransform>().anchoredPosition = temp;
            currentPanels.Add(temppanel);
        }
    }
    private void checkMCAM(int ind) {
        if (ind == currentCorrect) {
            CorrectAnswer(true);
        } else {
            CorrectAnswer(false);
        }
        resetButton.SetActive(false);
    }

    // PM ( Plane Move )
    // Single Model with another associated plane that can be dragged to intersect the model at the behest of the text question
    private void setPM(Question q) {
        currentCorrect = q.correctIndex;

        title.text = q.title;
        resetButton.SetActive(true);
        exampleObject = SpawnModel(q.Model);
        Invoke("resetExample", 0.125f);

    }
    private void checkPM() {
        if (currentCorrect != 0) {
            CorrectAnswer(true);
        } else {
            CorrectAnswer(false);
        }
        resetButton.SetActive(false);
    }

    // PD ( Plane Draw )
    // Single Model with line drawing that can be used to make planes to intersect the model at the behest of the text question
    private void setPD(Question q) {
        currentCorrectPlane = q.correctPlane;

        title.text = q.title;
        resetButton.SetActive(true);
        exampleObject = SpawnModel(q.Model);
        OptionPanelController temppanel = Instantiate(panelPrefab, transform, false);
        temppanel.color = Color.green;
        temppanel.optionButton.onClick.AddListener(delegate { answerSelect(0); });
        temppanel.ButtonText = "A";
        temppanel.OptionText = "Draw the ["+q.correctPlane[0]+", "+q.correctPlane[1]+", "+ q.correctPlane[2]+"] plane";
        temppanel.ModelOption = "None";
        Vector3 temp = temppanel.GetComponent<RectTransform>().anchoredPosition;
        temp.y = 750 - 220;
        temppanel.GetComponent<RectTransform>().anchoredPosition = temp;
        currentPanels.Add(temppanel);
        Invoke("resetExample", 0.125f);

    }
    private void checkPD() {
        int[] ints;
        ints = exampleObject.GetComponentInChildren<planeAdj>().planeType;
        if ((currentCorrectPlane[0] == ints[0] && currentCorrectPlane[1] == ints[1] && currentCorrectPlane[2] == ints[2]) ||
            (currentCorrectPlane[0] ==-ints[0] && currentCorrectPlane[1] ==-ints[1] && currentCorrectPlane[2] ==-ints[2])) { // check negative as well
            CorrectAnswer(true);
        } else {
            CorrectAnswer(false);
        }
        resetButton.SetActive(false);
    }

    // Setting Level
    // Selecting Answer
    public void setLevel() {
        Debug.Log(currentLevel +" "+ levelset.Levels.Length);
        if (currentLevel == levelset.Levels.Length) {
            endLevel();
            return;
        }

        Level level = levelset.Levels[currentLevel];

        for (int i=0;i<level.Questions.Length;i++) {
            ContentManager cm;
            if (i == 0) {
                cm = this;
            } else {
                cm = Instantiate(holderinstant).GetComponentInChildren<ContentManager>();
                nonmain.Add(cm);
            }
            cm.main = this;
            Question tq = level.Questions[i];
            cm.currentType = tq.type;
            main.an = 0;
            switch (tq.type) {
                case "MCMM":
                    cm.setMCMM(tq);
                    break;
                case "MCSM":
                    cm.setMCSM(tq);
                    break;
                case "MCAM":
                    cm.setMCAM(tq);
                    break;
                case "PM":
                    cm.setPM(tq);
                    break;
                case "PD":
                    cm.setPD(tq);
                    break;
            }
        }
        checkedf = false;
    }
    public void answerSelect(int ind) {
        if (checkedf) return;
        checkedf = true;
        switch(currentType) {
            case "MCMM":
                checkMCMM(ind);
                break;
            case "MCSM":
                checkMCSM(ind);
                break;
            case "MCAM":
                checkMCAM(ind);
                break;
            case "PM":
                checkPM();
                break;
            case "PD":
                checkPD();
                break;
        }
        if(exampleObject) {
            Destroy(exampleObject);
        }
        if (main.an == main.levelset.Levels[currentLevel].Questions.Length) {
            main.currentLevel++;
            main.Invoke("setLevel", 3);
        } 
    }
    public void endLevel() {
        gradepanel.SetActive(true);
        title.text = "No more levels";
        gradepanel.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Grade: " + numCorrect+" / "+numTotal;
        numCorrect = 0; numTotal = 0; currentLevel = 0; an = 0;
        foreach (ContentManager cm in nonmain) {
            cm.selfDestruct();
        }
        nonmain.Clear();
    }
    public void LevelSelect() {
        gradepanel.SetActive(false);
        FSitems = Resources.LoadAll<TextAsset>("Levels");
        int ind = 0;
        title.text = "Select Level";
        foreach(TextAsset i in FSitems) {
            GameObject temppanel = Instantiate(panelLevel, transform, false);
            temppanel.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(delegate { selectLevel(i); });
            temppanel.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = i.name;
            Vector3 temp = temppanel.GetComponent<RectTransform>().anchoredPosition;
            temp.y = 750 - 100 * ind; ind++;
            temppanel.GetComponent<RectTransform>().anchoredPosition = temp;
            panels.Add(temppanel);
        }
    }
    public void selectLevel(TextAsset file) {
        foreach (GameObject op in panels) {
            op.SetActive(false);
        }
        currentPanels.Clear();
        levelset = LevelSet.createFromJson(file.ToString());
        setLevel();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!isMain) return;
        gradepanel.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(delegate { LevelSelect(); });
        LevelSelect();
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
