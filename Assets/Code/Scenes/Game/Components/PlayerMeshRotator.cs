using UnityEngine;


public class PlayerMeshRotator : MonoBehaviour {
    GameObject playerObject;
    Quaternion lastRot;

    [SerializeField]
    float slerpAmount = 5.0f;

    // Use this for initialization
    void Start () {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        //lastRot = playerObject.transform.rotation;
        lastRot = playerObject.transform.localRotation;
    }
    
    // Update is called once per frame
    void Update () {
        //Quaternion rotateDifference = Quaternion.Inverse(lastRot) * playerObject.transform.rotation;
        Quaternion rotateDifference = Quaternion.Inverse(lastRot) * playerObject.transform.localRotation;
        //Debug.Log(playerObject.transform.localRotation);
        //Debug.Log(rotateDifference);
        Quaternion defaultRot = Quaternion.Euler(new Vector3(0f, 180f, 0f));
        Vector3 quaHelper = rotateDifference.eulerAngles;
        quaHelper.y = 180f;

        rotateDifference = Quaternion.Euler(quaHelper);

        Vector3 angleHelper = rotateDifference.eulerAngles;

        if (angleHelper.x > 180f)
            angleHelper.x -= (angleHelper.x - 180f) * 2f;
        if (angleHelper.y > 180f)
            angleHelper.y -= (angleHelper.y - 180f) * 2f;
        if (angleHelper.z > 180f)
            angleHelper.z -= (angleHelper.z - 180f) * 2f;

        //lastRot = playerObject.transform.rotation;
        lastRot = playerObject.transform.localRotation;
        transform.localRotation = Quaternion.Slerp(
            defaultRot,
            rotateDifference,
            slerpAmount * Time.smoothDeltaTime * angleHelper.magnitude / 180f);

        //Debug.Log("1: " + angleHelper.magnitude + "2: " + slerpAmount * Time.smoothDeltaTime * angleHelper.magnitude / 180f);
    }
}
