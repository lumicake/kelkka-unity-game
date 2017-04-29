using UnityEngine;


public class PlayerInput : MonoBehaviour {
    PlayerMover playerMover;
    GameControllerGame gameController;
    [SerializeField]
    Transform respawn;


    Animator playerAnimator;

    [SerializeField]
    float turnSpeed = 5f;

    void Start () {
        playerMover = gameObject.GetComponent<PlayerMover>();
        playerAnimator = gameObject.GetComponent<Animator>();
        gameController = GameObject.FindGameObjectWithTag("GameController")
                                   .GetComponent<GameControllerGame>();
    }
    
    void Update () {
        if (gameController.IsPaused) {
            if (Input.GetButtonDown("Pause")) {
                gameController.GameResume();
            }
        }
        else {
            playerAnimator.SetBool("accelerate", false);
            playerAnimator.SetBool("brake", false);
            playerAnimator.SetBool("flight", false);
            playerAnimator.SetBool("boost", false);
            playerMover.BoostMode = false;

            Vector3 turnAmount = Vector3.zero;
            turnAmount.x = Input.GetAxis("Horizontal");
            turnAmount.y = Input.GetAxis("Vertical");
            turnAmount.z = Input.GetAxis("Roll") * 2f;

            if (playerMover.IsGrounded) {
                handleGroundControl();
                playerAnimator.SetFloat("turn", turnAmount.x);
            }
            else {
                handleAirControl();
                playerAnimator.SetBool("jumpcharge", false);
            }

            if (Input.GetButtonDown("Reset")) {
                transform.rotation = respawn.transform.rotation;
                transform.position = respawn.transform.position;
                gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                gameController.RunFinish(GameControllerGame.Finisher.Player);
            }
            else if (Input.GetButtonDown("Pause")) {
                gameController.GamePause(GameControllerGame.Pauser.Player);
            }

            if (Input.GetButtonDown("SlowMo")) {
                if (Time.timeScale == 0.05f)
                    Time.timeScale = 1.0f;
                else
                    Time.timeScale = 0.05f;
            }

            playerMover.TurnAmount = Vector3.MoveTowards(playerMover.TurnAmount, turnAmount, Time.deltaTime * turnSpeed);
        }
    }

    private void handleAirControl() {
        playerAnimator.SetFloat("turn", 0.0f);
        playerAnimator.SetBool("accelerate", false);
        playerAnimator.SetBool("flight", true);
    }

    private void handleGroundControl() {
        playerAnimator.SetBool("flight", false);

        if (Input.GetAxis("Brake") != 0.0f) {
            playerAnimator.SetBool("brake", true);
        }
        else if (Input.GetAxis("Accelerate") != 0.0f) {
            if (playerMover.Speed > 130f) {
                playerAnimator.SetBool("boost", true);
                playerMover.BoostMode = true;
            }
            playerAnimator.SetBool("accelerate", true);
            playerMover.IsBraking = false;
        }
        else {
            playerAnimator.SetBool("brake", false);
            playerMover.IsBraking = false;
        }

        if (Input.GetButton("Jump")) {
            playerAnimator.SetBool("jumpcharge", true);
            playerMover.JumpChargeAdd();
        }
        else if (Input.GetButtonUp("Jump")) {
            playerAnimator.SetBool("jumpcharge", false);
            playerMover.Jump();
        }

    }

    public void SpeedPulse() {
        playerMover.PushForwards();
    }

    public void BeginBraking() {
        playerMover.IsBraking = true;
    }
}
