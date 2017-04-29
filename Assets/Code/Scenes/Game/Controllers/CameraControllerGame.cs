using UnityEngine;


[ExecuteInEditMode]
public class CameraControllerGame : MonoBehaviour {
    bool isRagdoll = false;
    Quaternion cameraNewRotation = new Quaternion();
    Vector3 cameraNewPosition = Vector3.zero;

    [SerializeField]
    private float cameraDistance = 6.5f;
    [SerializeField]
    private float cameraHeight = 1.8f;
    [SerializeField]
    private float cameraTargetOffset = 5.0f;
    [SerializeField]
    private float lerpDamper = 5.0f;
    [SerializeField]
    private float cameraMinFov = 50.0f;
    [SerializeField]
    private float cameraAddFov = 60.0f;
    [SerializeField]
    private float playerMaxSpeed = 180.0f;
    [SerializeField]
    private float cameraTargetOffsetAdd = 5.0f;
    [SerializeField]
    private float cameraDistanceAdd = -2.0f;
    [SerializeField]
    private float cameraHeightAdd = 5.0f;
    [SerializeField]
    private float fovCap = 125.0f;
    [SerializeField]
    private float cameraMoveSpeed = 5f;
    [SerializeField]
    private float camRotSpeed = 1f;

    Transform lookAtTransform;
    Rigidbody lookAtRb;
    Camera playerCamera;

    void Start () {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        lookAtTransform = playerObject.transform;
        lookAtRb = playerObject.GetComponent<Rigidbody>();
        playerCamera = gameObject.GetComponent<Camera>();

        cameraNewRotation = lookAtTransform.rotation;
        cameraNewPosition = lookAtTransform.position;
    }

    void LateUpdate () {
        CameraTrackPlayer();
    }

    internal void ResetCam() {
        isRagdoll = false;
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        lookAtTransform = playerObject.transform;
        lookAtRb = playerObject.GetComponent<Rigidbody>();

        transform.position = lookAtTransform.position;
        transform.position -= lookAtTransform.forward * cameraDistance;
        transform.position += Vector3.up * cameraHeight;
        transform.rotation = lookAtTransform.rotation;

        cameraNewRotation = lookAtTransform.rotation;
        cameraNewPosition = lookAtTransform.position;
    }

    internal void RagdollCam() {
        isRagdoll = true;
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        lookAtTransform = playerObject.transform;
        lookAtRb = playerObject.GetComponent<Rigidbody>();
    }

    void CameraTrackPlayer() {
        float playerVelDivByMax = lookAtRb.velocity.magnitude / playerMaxSpeed;

        playerCamera.fieldOfView = CalculateFieldOfView(playerVelDivByMax);

        Vector3 cameraTarget = CalculateCameraTarget(playerVelDivByMax);

        // The position of the camera should be behind the player's position by cameraDistance
        // amount of units, and above it by cameraHeight amount of units.
        cameraNewPosition = CalculateCameraPosition(playerVelDivByMax);
        cameraNewRotation = Quaternion.LookRotation(cameraTarget - cameraNewPosition);

        // Scale the amount we interpolate by to the player's velocity.
        //Debug.Log(lerpAmount);

        // Finally, set the position and rotation.
        //transform.position = Vector3.Lerp(transform.position, 
        //                                  cameraNewPosition,
        //                                  lerpAmount / lerpDamper * Time.smoothDeltaTime);
        //Vector3 newPos = Vector3.Lerp(transform.position, cameraNewPosition, .95f);
        //newPos.z = cameraNewPosition.z;
        float _speed = Time.deltaTime * cameraMoveSpeed * lookAtRb.velocity.magnitude;
        transform.position = Vector3.MoveTowards(transform.position, 
                                                 cameraNewPosition, 
                                                 _speed);

        transform.rotation = Quaternion.RotateTowards(transform.rotation,
                                                      cameraNewRotation,
                                                      Time.smoothDeltaTime * camRotSpeed);
        //transform.rotation = cameraNewRotation;
    }

    Vector3 CalculateCameraTarget(float playerVelDivByMax) {
        // The camera should be pointed at a point in space that is above the player's
        // position by cameraTargetOffset units.
        Vector3 target = lookAtTransform.position;
        target += lookAtTransform.up * CalculateCameraTargetOffset(playerVelDivByMax);
        return target;
    }

    Vector3 CalculateCameraPosition(float playerVelDivByMax) {
        Vector3 pos = lookAtTransform.position;
        pos -= lookAtTransform.forward * CalculateCameraDistance(playerVelDivByMax);
        pos += Vector3.up * CalculateCameraHeight(playerVelDivByMax);
        //pos += transform.position;
        //pos /= 2f;
        return pos;
    }

    float CalculateCameraTargetOffset(float playerVelDivByMax) {
        float offset = 0f;

        if (!isRagdoll)
            offset = cameraTargetOffset + cameraTargetOffsetAdd * playerVelDivByMax;

        return offset;
    }

    float CalculateCameraDistance(float playerVelDivByMax) {
        return cameraDistance + cameraDistanceAdd * playerVelDivByMax;
    }

    float CalculateCameraHeight(float playerVelDivByMax) {
        return cameraHeight + cameraHeightAdd * playerVelDivByMax;
    }

    float CalculateFieldOfView(float playerVelDivByMax) {
        float fov = cameraMinFov + playerVelDivByMax * cameraAddFov;

        // Cap the field of view at fovCap.
        fov = Mathf.Min(fov, fovCap);
        return fov;
    }
}
