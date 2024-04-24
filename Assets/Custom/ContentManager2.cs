using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentManager2 : MonoBehaviour
{
    public TextAsset levelSetFile;
    public TMPro.TextMeshProUGUI title;
    private string titleModelOption = null;
    public GameObject selectedPrefab;
    public GameObject platePrefab;

    public GameObject[] correctIncorrect;
    public Transform cubes;

    public List<GameObject> options = new List<GameObject>();
    public Detector Detector;
    public int currentCorrect;
    private int currentLevel = 0;
    private LevelSet levelset;

    public void removeLevels() {
        Detector.clearList();
        foreach (GameObject op in options) {
            if (op != null) {
                Destroy(op);
            }
        }
        options.Clear();
    }

    public void answerSelect() {
        GameObject answer = Detector.getAnswer();
        if (answer == null) return;
        if (options.IndexOf(answer) == currentCorrect) {
            correctIncorrect[0].SetActive(true);
        } else {
            correctIncorrect[1].SetActive(true);
        }
        removeLevels();
        currentLevel++;
        Invoke("setLevel", 3);
    }

    public void setLevel() {
        correctIncorrect[0].SetActive(false);
        correctIncorrect[1].SetActive(false);

        if (currentLevel == levelset.Levels.Length) {
            title.text = "No more levels";
            return;
        }

        title.text = levelset.Levels[currentLevel].title;
        titleModelOption = levelset.Levels[currentLevel].Model;

        if (titleModelOption == "None") {
            for (int ind = 0; ind < levelset.Levels[currentLevel].options.Length; ind++) {
                GameObject cube = Instantiate(selectedPrefab, cubes, false);
                currentCorrect = levelset.Levels[currentLevel].correct;
                float x = (0.3f + 0.6f * ind % 5);
                float z = (0.3f + 0.6f * Mathf.Floor(ind / 5));
                cube.transform.localPosition = new Vector3(x, 0.25f, z);
                cube.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                options.Add(cube);
            }
        } else {
            for (int ind = 0; ind < levelset.Levels[currentLevel].options.Length; ind++) {
                GameObject plate = Instantiate(platePrefab, cubes, false);
                plate.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = levelset.Levels[currentLevel].options[ind].Text;
                currentCorrect = levelset.Levels[currentLevel].correct;
                float x = (0.3f + 0.6f * ind % 5);
                float z = (0.3f + 0.6f * Mathf.Floor(ind / 5));
                plate.transform.localPosition = new Vector3(x, 0.25f, z);
                options.Add(plate);
            }
        } 
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
        public string title;
        public string Model;
        public Option[] options;
        public int correct;
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
