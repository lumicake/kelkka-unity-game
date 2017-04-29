using UnityEngine;
using UnityEngine.UI;


public class UIControllerGame : MonoBehaviour {
    ScoreCommsController scManager;
    GameControllerGame gameManager;

    [SerializeField]
    Text gateText;
    Text splitText;

    [SerializeField]
    GameObject scorePanel;
    [SerializeField]
    GameObject infoPanel;
    [SerializeField]
    GameObject namePromptPanel;


    [SerializeField]
    Text speedText;
    [SerializeField]
    Text timeText;

    [SerializeField]
    Text finishGates;
    [SerializeField]
    Text finishTime;
    [SerializeField]
    Text finishRank;
    [SerializeField]
    Text finish5Names;
    Text finish5Times;

    [SerializeField]
    Button submitButton;

    bool canSubmit = false;
    string playerName = "";

    internal string PlayerName {
        get { return playerName; }
        set { playerName = value; }
    }

    void Start () {
        namePromptPanel.GetComponentInChildren<InputField>()
                       .onEndEdit
                       .AddListener(
            delegate {
            if (Input.GetButton("Submit"))
                OnNamePromptSubmit();
            });

        splitText = gateText.transform.GetChild(0).GetComponent<Text>();
        finish5Times = finish5Names.transform.GetChild(0).GetComponent<Text>();

        scorePanel.SetActive(false);
        infoPanel.SetActive(false);
        namePromptPanel.SetActive(false);

        scManager = gameObject.GetComponent<ScoreCommsController>();
        gameManager = gameObject.GetComponent<GameControllerGame>();
    }

    internal void FiveTimesFailure(string error) {
        finishRank.text = "ERROR";
        finish5Names.text = "Something went wrong =(" +
                            "\nHere's the error message:" +
                            "\n" + error;
        finish5Times.text = "";
    }

    void Update() {
        if (gameManager.IsRunning) {
            speedText.text = gameManager.PlayerSpeed.ToString();
            timeText.text = gameManager.RunTime.ToString();
        }
    }

    public void OnInfoSubmitClick() {
        Text buttonText = submitButton.transform.GetChild(0).GetComponent<Text>();
        if (canSubmit) {
            SendScore();
            buttonText.text = "Working!";
            canSubmit = false;
        }
        else {
            buttonText.text = "Nu!";
        }
    }

    public void OnInfoCloseClick() {
        gameManager.GameReset();
    }

    public void OnNamePromptSubmit() {
        UpdateName();
        NamePromptHide();
    }

    internal void SubmitSuccess() {
        submitButton.transform.GetChild(0).GetComponent<Text>()
                                          .text = "Success!";
    }

    internal void SubmitFailure() {
        submitButton.transform.GetChild(0).GetComponent<Text>()
                                          .text = "Failure! :(";
    }

    internal void GateAndSplitTextSet(int score, float split) {
        gateText.text = score + "\n" + gateText.text;
        splitText.text = split + "\n" + splitText.text;
    }

    internal void GateAndSplitTextReset() {
        gateText.text = "";
        splitText.text = "";
    }


    // FIXME: Use SerializeField to set these colors instead
    internal void TimeSendPromptShow(float runTime, int gates) {
        if (gates < 34) {
            submitButton.GetComponent<Image>().color = new Color(255f / 255f, 146f / 255f, 146f / 255f);
            submitButton.transform.GetChild(0).GetComponent<Text>().text = "Not enough gates!";
            canSubmit = false;
        }
        else {
            submitButton.GetComponent<Image>().color = new Color(146f / 255f, 210f / 255f, 255f / 255f);
            canSubmit = true;
        }
        infoPanel.SetActive(true);
        finishGates.text = gates.ToString();
        finishTime.text = runTime.ToString();
        scManager.RunTime = runTime;
        scManager.FourScoresFetch();
    }

    internal void TimeSendPromptHide() {
        infoPanel.SetActive(false);
    }

    internal void ScoreboardShow() {
        scorePanel.gameObject.SetActive(true);
        scManager.ScoresFetch();
    }

    internal void ScoreboardHide() {
        scorePanel.gameObject.SetActive(false);
    }

    internal void NamePromptShow() {
        namePromptPanel.SetActive(true);
        namePromptPanel.GetComponentInChildren<InputField>().Select();
        namePromptPanel.GetComponentInChildren<InputField>().ActivateInputField();
    }

    void NamePromptHide() {
        namePromptPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    internal void UpdateName() {
        string name = namePromptPanel.GetComponentInChildren<InputField>().text;
        playerName = name == "" ? "happy_sledder" : name;
    }


    internal void UpdateScoreboard(string scoreList) {
        Text namesText = GameObject.FindGameObjectWithTag("Names").GetComponent<Text>();
        Text timesText = GameObject.FindGameObjectWithTag("Times").GetComponent<Text>();
        namesText.text = "";
        timesText.text = "";

        string[][] namesAndTimes = ParseTextToArray(scoreList);

        for (int i = 0; i < namesAndTimes.Length; i++) {
            namesText.text += (i+1) + GetOrdinal(i + 1) + " " + namesAndTimes[i][0] + "\n";
            timesText.text += namesAndTimes[i][1] + "\n";
        }
    }

    internal void UpdateFiveTimes(string text) {
        string[][] namesAndTimes = ParseTextToArray(text);
        finish5Names.text = "";
        finish5Times.text = "";

        for (int i = 0; i < namesAndTimes.Length; i++) {
            string name = namesAndTimes[i][0];
            string time = namesAndTimes[i][1];
            
            if (name == "your name") {
                name = "<color=lime>" + playerName + "</color>";
                time = "<color=lime>" + time + "</color>";
            }

            finish5Names.text += name + "\n";
            finish5Times.text += time + "\n";
        }

        string rank = text.Substring(text.LastIndexOf('/')+1);
        finishRank.text = rank;
    }

    void SendScore() {
        scManager.ScoreSend(playerName);
    }

    string[][] ParseTextToArray(string textList) {
        string[][] array = new string[CountCharInString(textList, '/')][];

        for (int i = 0; i < array.Length; i++) {
            int index = textList.IndexOf('/');
            string nameAndTime = textList.Substring(0, index);
            textList = textList.Remove(0, index + 1);
            int indexOfComma = nameAndTime.IndexOf(',');
            array[i] = new string[] { nameAndTime.Substring(0, indexOfComma),
                                      nameAndTime.Substring(indexOfComma+1) };
        }

        return array;
    }

    string GetOrdinal(int number) {
        switch (number % 10) {
            case 1:
                return "st";
            case 2:
                return "nd";
            case 3:
                return "rd";
            default:
                return "th";
        }
    }

    int CountCharInString(string counteeString, char countThis) {
        int cuts = 0;

        while (counteeString.IndexOf(countThis) != -1) {
            int index = counteeString.IndexOf(countThis);
            counteeString = counteeString.Remove(index, 1);
            cuts++;
        }

        return cuts;
    }
}
