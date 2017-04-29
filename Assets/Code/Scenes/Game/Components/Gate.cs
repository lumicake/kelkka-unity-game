using UnityEngine;


public class Gate : MonoBehaviour {
    float lastActivation = 0.0f;
    bool isActive = true;
    GameControllerGame gameController;

    void Start () {
        gameController = GameObject.FindGameObjectWithTag("GameController")
                                   .GetComponent<GameControllerGame>();
    }

    internal void ResetMe() {
        lastActivation = Time.time;
        isActive = true;
        //Debug.Log(isActive);
    }

    private void OnTriggerEnter(Collider other) {
        if (Time.time - lastActivation > 1.0f && isActive) {
            lastActivation = Time.time;
            gameController.GateIncrement();
            isActive = false;
        }
    }
}
