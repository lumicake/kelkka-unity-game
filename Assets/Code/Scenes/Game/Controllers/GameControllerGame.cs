using UnityEngine;


public class GameControllerGame : MonoBehaviour {
    GameObject[] gates;
    UIControllerGame uiController;
    GameObject playerObject;
    PlayerMover playerMover;
    [SerializeField]
    RagdollController rdController;
    [SerializeField]
    Transform teleDestination;
    [SerializeField]
    FinishScreenToggler finishScreenToggler;
    [SerializeField]
    CameraControllerGame playerCam;

    int currentScore = 0;
    float sessionHighScore = 500f;
    float runStartTime = 0f;
    float runFinishTime = 500f;
    bool isRunning = false;
    bool isPaused = false;

    internal bool IsRunning {
        get { return isRunning; }
    }

    internal bool IsPaused {
        get { return isPaused; }
    }

    internal float RunTime {
        get { return Mathf.Round((Time.time - runStartTime) * 100f) / 100f; }
    }

    internal float PlayerSpeed {
        get { return Mathf.Round(playerMover.Speed); }
    }

    internal enum Finisher {
        Goal,
        Player
    }

    internal enum Pauser {
        Goal,
        Player
    }

    void Start () {
        gates = GameObject.FindGameObjectsWithTag("Gate");
        uiController = gameObject.GetComponent<UIControllerGame>();
        playerObject = GameObject.FindGameObjectWithTag("Player");
        playerMover = playerObject.GetComponent<PlayerMover>();
        Terrain myTerrain = GameObject.FindGameObjectWithTag("Terrain")
                                      .GetComponent<Terrain>();
        myTerrain.detailObjectDistance = 350f;

        isRunning = true;
        GameReset();
    }

    void Update() {
    }

    internal void GateIncrement() {
        currentScore++;
        float splitTime = Mathf.Round((Time.time - runStartTime) * 100f);
        splitTime /= 100f;
        uiController.GateAndSplitTextSet(currentScore, splitTime);
    }

    internal void RunFinish(Finisher whoFinished) {
        if (whoFinished == Finisher.Goal) {
            runFinishTime = Mathf.Round((Time.time - runStartTime) * 100f);
            runFinishTime /= 100f;

            uiController.GateAndSplitTextSet(currentScore + 1, runFinishTime);

            isRunning = false;
            playerObject.GetComponent<Rigidbody>().isKinematic = true;
            playerCam.gameObject.SetActive(false);
            finishScreenToggler.gameObject.SetActive(true);
            finishScreenToggler.Activate(runFinishTime, currentScore + 1);
        }

        GameReset();
    }

    internal void GamePause(Pauser whoPaused) {
        Time.timeScale = 0f;
        isPaused = true;

        if (whoPaused == Pauser.Player) {
            uiController.ScoreboardShow();
        }
        else {
            if (runFinishTime < sessionHighScore || sessionHighScore == 500f) {
                sessionHighScore = runFinishTime;

                uiController.TimeSendPromptShow(runFinishTime, currentScore);
            }

            runFinishTime = 500f;
        }
    }

    internal void GameResume() {
        Time.timeScale = 1f;
        isPaused = false;
        uiController.ScoreboardHide();
    }

    internal void GameReset() {
        playerCam.ResetCam();

        playerObject.transform.position = teleDestination.transform.position;
        playerObject.transform.rotation = teleDestination.transform.rotation;
        Rigidbody rb = playerObject.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = false;

        foreach (GameObject gate in gates) {
            gate.GetComponent<Gate>().ResetMe();
        }

        currentScore = 0;
        runStartTime = Time.time;
        uiController.GateAndSplitTextReset();

        if (!isRunning) {
            playerCam.gameObject.SetActive(true);
            finishScreenToggler.Deactivate();
            isRunning = true;
        }

        if (uiController.PlayerName == "") {
            Time.timeScale = 0f;
            uiController.NamePromptShow();
        }
    }

    internal void RagdollTime(Vector3 pos, Quaternion rot, Vector3 vel) {
        rdController.ActivateMe(pos, rot, vel);
        playerCam.RagdollCam();
    }

    internal void RagdollTimeOver() {
        playerObject.SetActive(true);
        RunFinish(Finisher.Player);
    }
}
