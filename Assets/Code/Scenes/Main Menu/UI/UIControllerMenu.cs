using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class UIControllerMenu : MonoBehaviour {
    AsyncOperation loadScene;
    [SerializeField]
    GameObject scorePanel;
    [SerializeField]
    GameObject mainMenuPanel;
    [SerializeField]
    GameObject optionsPanel;
    ScoreCommsController scManager;
    const string fetchScoresURL = "http://tigsune.eu/db/getScores.php";

    float idleTimer = 0f;

    void Start() {
        scorePanel.SetActive(false);
        optionsPanel.SetActive(false);
    }

    void Update() {
        if (loadScene != null)
            Debug.Log(loadScene.progress);

        idleTimer += Time.deltaTime;
        if (Input.GetAxis("Mouse") != 0f)
            idleTimer = 0f;

        if (idleTimer > 5f) {
            mainMenuPanel.SetActive(false);
        }
        else {
            mainMenuPanel.SetActive(true);
        }
    }

    void ScoresFetch() {
        StartCoroutine(WaitForScores());
    }

    void UpdateScoreboard(string scoreList) {
        Text namesText = GameObject.FindGameObjectWithTag("Names").GetComponent<Text>();
        Text timesText = GameObject.FindGameObjectWithTag("Times").GetComponent<Text>();
        namesText.text = "";
        timesText.text = "";

        int counter = 1;
        do {
            int index = scoreList.IndexOf("/");
            string nameAndTime = scoreList.Substring(0, index);
            scoreList = scoreList.Remove(0, index + 1);
            int indexOfComma = nameAndTime.IndexOf(',');
            namesText.text += counter + ". " + nameAndTime.Substring(0, indexOfComma) + "\n";
            timesText.text += nameAndTime.Substring(indexOfComma + 1) + "\n";

            counter++;
        } while (scoreList.IndexOf("/") != -1 && counter <= 10);
    }

    IEnumerator WaitForScores() {
        WWW getTimes = new WWW(fetchScoresURL);
        yield return getTimes;

        if (getTimes.error != null) {
            Debug.Log("eRRoR: " + getTimes.error);
        }
        else {
            UpdateScoreboard(getTimes.text);
        }
    }

    internal void ScoreboardShow() {
        scorePanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        ScoresFetch();
    }

    public void OnStartClick() {
        loadScene = SceneManager.LoadSceneAsync("Kelkkapeli");
    }

    public void OnExitClick() {
        Application.Quit();
    }

    public void OnHighscoresClick() {
        ScoreboardShow();
    }

    public void OnOptionsClick() {
        optionsPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    public void OnBackButtonClick() {
        mainMenuPanel.SetActive(true);
        scorePanel.SetActive(false);
        optionsPanel.SetActive(false);
    }
}
