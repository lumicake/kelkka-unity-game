using UnityEngine;


public class Snowman : MonoBehaviour {
    Rigidbody[] rigidbodies;
    CapsuleCollider myCollider;

    [SerializeField]
    float forceMultiplier = 50f;
    [SerializeField]
    float collisionOffset = 2f;
    [SerializeField]
    bool useGravity = true;

    void Start () {
        rigidbodies = gameObject.GetComponentsInChildren<Rigidbody>();
        myCollider = gameObject.GetComponent<CapsuleCollider>();
    }

    void OnTriggerEnter(Collider other) {
        if (other.name == "Pulkkailija") {
            Destroy(gameObject, 5f);

            Rigidbody pRb = other.GetComponent<Rigidbody>();
            foreach (Rigidbody rb in rigidbodies) {
                SphereCollider thisSphere = rb.GetComponent<SphereCollider>();

                // We use the colliders to help calculate collission points because the
                // transforms for each of the snowmen's components contain weird data,
                // possibly as a result of something wrong in the file import from Blender?
                Vector3 rbPos = rb.transform.position + 0.05f * thisSphere.center;
                Vector3 forceVector = rbPos - (pRb.transform.position + pRb.transform.up * collisionOffset);

                rb.AddForce(forceMultiplier * (forceVector.normalized * pRb.velocity.magnitude) / rigidbodies.Length);
                rb.isKinematic = false;

                if (useGravity)
                    rb.useGravity = true;
                else
                    rb.useGravity = false;
            }
        }
    }
}
