using System.Collections;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;


public class ScoreCommsController : MonoBehaviour {
    UIControllerGame uiController;
    const string hashSalt = "crushedassumptions";
    const string sendScoreURL = "http://tigsune.eu/db/addScore.php?";
    const string fetchScoresURL = "http://tigsune.eu/db/getScores.php";
    const string fetchFourScoresURL = "https://tigsune.eu/db/get4Scores.php?";
    float runTime = 500.0f;

    internal float RunTime {
        set { runTime = value; }
    }

    void Start () {
        uiController = GetComponent<UIControllerGame>();
    }


    internal void ScoreSend(string name) {
        StartCoroutine(PostScore(name));
    }

    internal void ScoresFetch() {
        StartCoroutine(WaitForScores());
    }

    internal void FourScoresFetch() {
        StartCoroutine(WaitForFourScores());
    }
    
    IEnumerator WaitForScores() {
        WWW getTimes = new WWW(fetchScoresURL);
        yield return getTimes;

        if (getTimes.error != null) {
            Debug.Log("Error: " + getTimes.error);
        }
        else {
            uiController.UpdateScoreboard(getTimes.text);
        }
    }

    IEnumerator WaitForFourScores() {
        string fullURL = fetchFourScoresURL + "time=" + runTime;
        WWW getTimes = new WWW(fullURL);
        yield return getTimes;

        if (getTimes.error != null) {
            Debug.Log("Error: " + getTimes.error);
            uiController.FiveTimesFailure(getTimes.error);
        }
        else {
            uiController.UpdateFiveTimes(getTimes.text);
        }
    }

    IEnumerator PostScore(string name) {
        string md5 = MD5Hash(name + runTime + hashSalt);
        string fullURL = sendScoreURL + 
                         "name=" + WWW.EscapeURL(name) + 
                         "&time=" + runTime + 
                         "&hash=" + md5;
        Debug.Log(fullURL);

        WWW postTime = new WWW(fullURL);
        yield return postTime;

        if (postTime.error != null) {
            Debug.Log("Error: " + postTime.error);
            uiController.SubmitFailure();
            yield break;
        }

        uiController.SubmitSuccess();
    }

    string MD5Hash(string toBeHashed) {
        MD5 md5 = MD5.Create();
        byte[] toBeHashedBytes = Encoding.ASCII.GetBytes(toBeHashed);
        byte[] hash = md5.ComputeHash(toBeHashedBytes);

        StringBuilder hashSb = new StringBuilder();

        for (int i = 0; i < hash.Length; i++) {
            hashSb.Append(hash[i].ToString("x2"));
        }

        return hashSb.ToString();
    }
}
