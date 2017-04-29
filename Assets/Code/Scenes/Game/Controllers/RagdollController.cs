using UnityEngine;


public class RagdollController : MonoBehaviour {
    Rigidbody[] rigidbodies;
    Animator myAnimator;
    Rigidbody myRB;
    GameObject pulkka;
    Vector3 ragdollSpot;
    GameControllerGame gameController;

    void Start () {
        myAnimator = gameObject.GetComponent<Animator>();
        rigidbodies = gameObject.GetComponentsInChildren<Rigidbody>();
        pulkka = GameObject.FindGameObjectWithTag("Pulkka");
        ragdollSpot = GameObject.FindGameObjectWithTag("RagdollSpot")
                                .transform.position;
        gameController = GameObject.FindGameObjectWithTag("GameController")
                                   .GetComponent<GameControllerGame>();
    }

    void Update() {
        if (Input.GetButtonDown("Reset") && !myAnimator.enabled) {
            myRB.tag = "Untagged";
            transform.position = ragdollSpot;
            transform.rotation = Quaternion.Euler(Vector3.zero);
            myAnimator.enabled = true;

            foreach (Rigidbody rb in rigidbodies) {
                rb.isKinematic = true;
                rb.interpolation = RigidbodyInterpolation.None;
            }

            Rigidbody pRb = pulkka.GetComponent<Rigidbody>();
            pulkka.transform.position = new Vector3(ragdollSpot.x,
                                                    ragdollSpot.y - 0.4f,
                                                    ragdollSpot.z);
            pulkka.transform.rotation = Quaternion.Euler(Vector3.zero);
            pRb.isKinematic = true;
            pRb.useGravity = false;

            gameController.RagdollTimeOver();
        }
    }

    public void ActivateMe(Vector3 pos, Quaternion rot, Vector3 vel) {
        transform.position = new Vector3(pos.x, pos.y + 3f, pos.z);
        Vector3 holder = rot.eulerAngles;
        rot = Quaternion.Euler(holder.x, holder.y + 180f, holder.z);
        transform.rotation = rot;
        myAnimator.enabled = false;

        foreach (Rigidbody rb in rigidbodies) {
            rb.isKinematic = false;
            //rb.AddForce((vel * 15f) / rigidbodies.Length, ForceMode.Impulse);
            rb.velocity = (vel * 10f) / rigidbodies.Length;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.interpolation = RigidbodyInterpolation.Interpolate;

            if (rb.name == "Bone")
                myRB = rb;
        }

        myRB.gameObject.tag = "Player";
        //thisRigid.AddForce(vel * 15, ForceMode.Impulse);
        pulkka.transform.position = new Vector3(pos.x, pos.y + .4f, pos.z);
        Rigidbody pRb = pulkka.GetComponent<Rigidbody>();
        pRb.useGravity = true;
        pRb.isKinematic = false;
        pRb.AddForce(vel, ForceMode.Impulse);
    }
}
