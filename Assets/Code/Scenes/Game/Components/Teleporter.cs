using UnityEngine;


public class Teleporter : MonoBehaviour {
    GameControllerGame gameController;
    float lastActivation = -40.0f;

    void Start () {
        gameController = GameObject.FindGameObjectWithTag("GameController")
                                   .GetComponent<GameControllerGame>();
    }

    void OnTriggerEnter(Collider other) {
        if (Time.time - lastActivation > 40f) {
            gameController.RunFinish(GameControllerGame.Finisher.Goal);

            lastActivation = Time.time;
        }
    }
}
