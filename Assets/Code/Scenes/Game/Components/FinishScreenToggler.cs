using UnityEngine;


public class FinishScreenToggler : MonoBehaviour {
    UIControllerGame uiController;

    void Start () {
        uiController = GameObject.FindGameObjectWithTag("GameController")
                                 .GetComponent<UIControllerGame>();
        gameObject.SetActive(false);
    }
    
    void Update () {
        
    }

    internal void Activate(float finishTime, int finishScore) {
        uiController.TimeSendPromptShow(finishTime, finishScore);

    }

    internal void Deactivate() {
        uiController.TimeSendPromptHide();
        uiController.ScoreboardHide();
        gameObject.SetActive(false);
    }
}
