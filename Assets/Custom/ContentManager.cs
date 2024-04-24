using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentManager : MonoBehaviour
{
    public TextAsset levelSetFile;
    public TMPro.TextMeshProUGUI title;
    public GameObject[] prefabs; //0 cube, 1 SC, 2 BCC, 3 FCC
    public Material[] correctIncorrect;
    public GameObject resetButton;
    public GameObject exampleObject;
    public Transform pedestal;

    public int currentCorrect;
    public string currentType;
    private List<OptionPanelController> currentPanels = new List<OptionPanelController>();
    private int currentLevel = 0;
    private LevelSet levelset;

    public OptionPanelController panelPrefab;

    // Correct / Incorrect addressing
    private void CorrectAnswer(bool isCorrect) {
        if(isCorrect) {
            Color green = new Color(0.1921119f, 0.7547169f, 0.1245995f);
            correctIncorrect[0].SetColor("_EmissionColor", green);
        } else {
            Color red = new Color(0.8490566f, 0.1161445f, 0.1161445f);
            correctIncorrect[1].SetColor("_EmissionColor", red);
        }
        Invoke("resetColors", 3);
    }
    private void resetColors() {
        Color green = new Color(0.02175654f, 0.1226415f, 0.01214845f);
        Color red = new Color(0.09433959f, 0.01112495f, 0.01112495f);
        correctIncorrect[0].SetColor("_EmissionColor", green);
        correctIncorrect[1].SetColor("_EmissionColor", red);
    }

    // Model Setting
    private GameObject SpawnModel(string model) {
        int type;
        switch(model) {
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
        exampleObject.transform.parent = pedestal;
        exampleObject.transform.localPosition = new Vector3(0, 1.29f, 0f);
        exampleObject.transform.localRotation = Quaternion.identity;
        exampleObject.transform.localScale = new Vector3(0.66f, 0.573913f, 0.66f);
    }

    // MCMM ( Multiple Choice Multiple Model )
    // Multiple choice menu where each has a small model that can be inspected, with a single text question
    private void setMCMM(Level level) {
        currentType = level.type;
        currentCorrect = level.correctIndex;

        title.text = level.title;

        for (int ind = 0; ind < level.options.Length; ind++) {
            OptionPanelController temppanel = Instantiate(panelPrefab, transform, false);
            temppanel.optionButton.onClick.AddListener(delegate { answerSelect(temppanel.index); });
            temppanel.color = Random.ColorHSV();
            temppanel.index = ind;
            temppanel.ButtonText = level.options[ind].Button;
            temppanel.OptionText = level.options[ind].Text;
            temppanel.ModelOption = level.options[ind].Model;
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

        foreach (OptionPanelController op in currentPanels) {
            Debug.Log(op.name);
            if (op != null) {
                op.resetExample();
                Destroy(op.gameObject);
            }
        }
        currentPanels.Clear();
    }

    // MCSM ( Multiple Choice Single Model )
    // Multiple choice menu where there is only one large model that can be inspected, associated with a text question
    private void setMCSM(Level level) {
        currentType = level.type;
        currentCorrect = level.correctIndex;

        title.text = level.title;
        resetButton.SetActive(true);
        exampleObject = SpawnModel(level.Model);
        Invoke("resetExample", 0.5f);

        for (int ind = 0; ind < level.options.Length; ind++) {
            OptionPanelController temppanel = Instantiate(panelPrefab, transform, false);
            temppanel.optionButton.onClick.AddListener(delegate { answerSelect(temppanel.index); });
            temppanel.color = Random.ColorHSV();
            temppanel.index = ind;
            temppanel.ButtonText = level.options[ind].Button;
            temppanel.OptionText = level.options[ind].Text;
            temppanel.ModelOption = level.options[ind].Model;
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

        foreach (OptionPanelController op in currentPanels) {
            Debug.Log(op.name);
            if (op != null) {
                op.resetExample();
                Destroy(op.gameObject);
            }
        }
        Destroy(exampleObject);
        resetButton.SetActive(false);
        currentPanels.Clear();
    }

    // MCAM ( Multiple Choice All Model )
    // Multiple choice menu where each has a small model that can be inspected, with a single text question that also has an associated model
    private void setMCAM(Level level) {
        currentType = level.type;
        currentCorrect = level.correctIndex;

        title.text = level.title;
        resetButton.SetActive(true);
        exampleObject = SpawnModel(level.Model);
        Invoke("resetExample", 0.5f);

        for (int ind = 0; ind < level.options.Length; ind++) {
            OptionPanelController temppanel = Instantiate(panelPrefab, transform, false);
            temppanel.color = Random.ColorHSV();
            temppanel.optionButton.onClick.AddListener(delegate { answerSelect(temppanel.index); });
            temppanel.index = ind;
            temppanel.ButtonText = level.options[ind].Button;
            temppanel.OptionText = level.options[ind].Text;
            temppanel.ModelOption = level.options[ind].Model;
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

        foreach (OptionPanelController op in currentPanels) {
            Debug.Log(op.name);
            if (op != null) {
                op.resetExample();
                Destroy(op.gameObject);
            }
        }
        Destroy(exampleObject);
        resetButton.SetActive(false);
        currentPanels.Clear();
    }

    // PM ( Plane Move )
    // Single Model with another associated plane that can be dragged to intersect the model at the behest of the text question
    private void setPM(Level level) {
        currentType = level.type;
        currentCorrect = level.correctIndex;

        title.text = level.title;
        resetButton.SetActive(true);
        exampleObject = SpawnModel(level.Model);
        Invoke("resetExample", 0.5f);

    }
    private void checkPM() {
        if (currentCorrect != 0) {
            CorrectAnswer(true);
        } else {
            CorrectAnswer(false);
        }

        foreach (OptionPanelController op in currentPanels) {
            Debug.Log(op.name);
            if (op != null) {
                op.resetExample();
                Destroy(op.gameObject);
            }
        }
        Destroy(exampleObject);
        resetButton.SetActive(false);
        currentPanels.Clear();
    }

    // PD ( Plane Draw )
    // Single Model with line drawing that can be used to make planes to intersect the model at the behest of the text question
    private void setPD(Level level) {
        currentType = level.type;
        currentCorrect = level.correctIndex;

        title.text = level.title;
        resetButton.SetActive(true);
        exampleObject = SpawnModel(level.Model);
        Invoke("resetExample", 0.5f);

    }
    private void checkPD() {
        if (currentCorrect != 0) {
            CorrectAnswer(true);
        } else {
            CorrectAnswer(false);
        }

        foreach (OptionPanelController op in currentPanels) {
            if (op != null) {
                op.resetExample();
                Destroy(op.gameObject);
            }
        }
        Destroy(exampleObject);
        resetButton.SetActive(false);
        currentPanels.Clear();
    }

    // Setting Level
    // Selecting Answer
    public void setLevel() {
        if (currentLevel == levelset.Levels.Length) {
            title.text = "No more levels";
            return;
        }

        Level level = levelset.Levels[currentLevel];
        currentType = level.type;

        switch (level.type) {
            case "MCMM":
                setMCMM(level);
                break;
            case "MCSM":
                setMCSM(level);
                break;
            case "MCAM":
                setMCAM(level);
                break;
            case "PM":
                setPM(level);
                break;
            case "PD":
                setPD(level);
                break;
        }
    }
    public void answerSelect(int ind) {

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
        currentLevel++;
        Invoke("setLevel", 3);
    }

    // Start is called before the first frame update
    void Start()
    {
        levelSetFile = Resources.Load("levelSet") as TextAsset;
        levelset = LevelSet.createFromJson(levelSetFile.ToString());
        setLevel();
    }

    [System.Serializable]
    public class Option
    {
        public string Button;
        public string Text;
        public string Model;
    }
    [System.Serializable]
    public class Level
    {
        public string type;
        public string title;
        public string Model;
        public Option[] options;
        public int correctIndex;
        public int[] correntPlane;
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
