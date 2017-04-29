using UnityEngine;


public class PlayerMover : MonoBehaviour {
    Rigidbody myRB;
    ParticleSystem[] particleSystems;
    GameControllerGame gameController;

    float normalForceStrength = 25.0f;
    float gravityForceStrength = 25.0f;
    float boostModeBoost = 1.4f;
    int pushCounter = 0;
    bool isBraking = false;
    float jumpCharge = 0f;
    bool isGrounded = false;
    bool wasGrounded = false;
    bool boostMode = false;
    float lastCollisionTime;
    Vector3 lastVel = Vector3.zero;
    Vector3 turnAmount = Vector3.zero;
    Vector3 playerUpAtLanding = Vector3.zero;
    Vector3 surfaceNormalAtLandingPoint = Vector3.zero;

    internal bool IsGrounded {
        get { return isGrounded; }
    }

    internal float Speed {
        get { return myRB.velocity.magnitude; }
    }

    internal bool BoostMode {
        get { return boostMode; }
        set { boostMode = value; }
    }

    internal bool IsBraking {
        get { return isBraking; }
        set { isBraking = value; }
    }

    internal Vector3 TurnAmount {
        get {
            return turnAmount;
        }
        set { turnAmount = value; }
    }

    // All of the variables past here are used for movement
    [SerializeField]
    float accelerateStrength = 10.0f;
    [SerializeField]
    float stickToGroundForceStrength = 5.0f;
    [SerializeField]
    float turnFactor = 2.0f;
    [SerializeField]
    float frictionStrength = 1.5f;
    [SerializeField]
    float frictionDamper = 15.0f;
    [SerializeField]
    float frictionOffset = 2.0f;
    [SerializeField]
    float forceMultiplier = 5.0f;
    [SerializeField]
    float turnStrength = 200.0f;
    [SerializeField]
    float powerSteering = 100.0f;
    [SerializeField]
    float airControlStrength = 200.0f;
    [SerializeField]
    float steeringDamper = 1.5f;
    [SerializeField]
    float airGravityScale = 0.25f;
    [SerializeField]
    float brakingFriction = 0.5f;
    [SerializeField]
    float checkDistance = 0.2f;
    [SerializeField]
    float jumpChargeIncrement = 7.5f;
    [SerializeField]
    float jumpStrength = 2.0f;
    [SerializeField]
    float deathSpeedChange = 40f;
    [SerializeField]
    float dragAmount = 0001f;
    [SerializeField]
    float hoverHeight = 0.25f;
    [SerializeField]
    Vector3 raycastPointOffset = new Vector3(0f, 0f, 0f);
    [SerializeField]
    AudioSource windSource;
    [SerializeField]
    AudioSource sledSource;


    void Start() {
        myRB = gameObject.GetComponent<Rigidbody>();
        particleSystems = gameObject.GetComponentsInChildren<ParticleSystem>();
        gameController = GameObject.FindGameObjectWithTag("GameController")
                                   .GetComponent<GameControllerGame>();
        windSource = GameObject.FindGameObjectWithTag("MainCamera")
                               .GetComponent<AudioSource>();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.G)) {
            transform.gameObject.SetActive(false);
            gameController.RagdollTime(transform.position, transform.rotation, lastVel);
        }

        RaycastHit hit;
        if (GroundCheck(out hit)) {
            Vector3 averageNormal = CalculateAverageSurfaceNormal(hit);
            RotatePlayerToSurface(averageNormal);
        }
    }

    void FixedUpdate() {
        lastVel = myRB.velocity;

        //Debug.Log("deltaPos: " + Vector3.Distance(lastPos, transform.position) + ", isGrounded: " + isGrounded);

        DoMovement();
    }


    void OnCollisionEnter(Collision collision) {
        if (Time.time - lastCollisionTime > 1.0f && !isGrounded) {
            //Debug.Log("test");
            RaycastHit landRayHit;

            if (Physics.Raycast(transform.position,
                Vector3.down,
                out landRayHit,
                5.0f,
                LayerMask.GetMask("Ground")) && !isGrounded) {

                surfaceNormalAtLandingPoint = landRayHit.normal;
                playerUpAtLanding = transform.up;
            }

            lastCollisionTime = Time.time;
        }
        else {
            if (lastVel.magnitude - myRB.velocity.magnitude > deathSpeedChange) {
                transform.gameObject.SetActive(false);
                gameController.RagdollTime(transform.position, transform.rotation, lastVel);
            }
        }
    }

    void RotatePlayerToSurface(Vector3 averageNormal) {
        Vector3 playerForward = Vector3.Cross(transform.right, averageNormal);
        Quaternion groundRot = Quaternion.LookRotation(playerForward, averageNormal);

        //Quaternion forwardHelper = Quaternion.groundCheckHit.
        transform.rotation = Quaternion.Slerp(transform.rotation, groundRot, .1f);
    }

    void PlayerHoverAboveGround(RaycastHit hit, Vector3 averageNormal) {
        Vector3 posOnGround = hit.point + averageNormal * hoverHeight;
        transform.position = posOnGround;
        //Debug.Log("hD: " + Vector3.Distance(hit.point, posOnGround) +
        //" aD: " + Vector3.Distance(hit.point, transform.position));
    }

    Vector3 CalculateAverageSurfaceNormal(RaycastHit hit) {
        Ray frontRight = new Ray(
            transform.position -
            transform.forward * raycastPointOffset.z -
            transform.right * raycastPointOffset.x +
            transform.up * raycastPointOffset.y,
            Vector3.down);
        Ray frontLeft = new Ray(
            transform.position -
            transform.forward * raycastPointOffset.z +
            transform.right * raycastPointOffset.x +
            transform.up * raycastPointOffset.y,
            Vector3.down);
        Ray rearRight = new Ray(
            transform.position +
            transform.forward * raycastPointOffset.z -
            transform.right * raycastPointOffset.x +
            transform.up * raycastPointOffset.y,
            Vector3.down);
        Ray rearLeft = new Ray(
            transform.position +
            transform.forward * raycastPointOffset.z +
            transform.right * raycastPointOffset.x +
            transform.up * raycastPointOffset.y,
            Vector3.down);

        Debug.DrawRay(frontRight.origin, frontRight.direction * checkDistance * 3f, Color.red);
        Debug.DrawRay(frontLeft.origin, frontLeft.direction * checkDistance * 3f, Color.green);
        Debug.DrawRay(rearRight.origin, rearRight.direction * checkDistance * 3f, Color.cyan);
        Debug.DrawRay(rearLeft.origin, rearLeft.direction * checkDistance * 3f, Color.yellow);

        RaycastHit fR;
        RaycastHit fL;
        RaycastHit rR;
        RaycastHit rL;

        if (!Physics.Raycast(frontRight, out fR, Mathf.Infinity, LayerMask.GetMask("Ground"))) {
            fR.normal = Vector3.zero;
            Debug.Log("fR no contact");
        }
        if (!Physics.Raycast(frontRight, out fL, Mathf.Infinity, LayerMask.GetMask("Ground"))) {
            fL.normal = Vector3.zero;
            Debug.Log("fL no contact");
        }
        if (!Physics.Raycast(frontRight, out rR, Mathf.Infinity, LayerMask.GetMask("Ground"))) {
            rR.normal = Vector3.zero;
            Debug.Log("rR no contact");
        }
        if (!Physics.Raycast(frontRight, out rL, Mathf.Infinity, LayerMask.GetMask("Ground"))) {
            rL.normal = Vector3.zero;
            Debug.Log("rL no contact");
        }

        Vector3 averageNormal = fR.normal + fL.normal + rR.normal + rL.normal + hit.normal;
        averageNormal /= 5;
        Debug.DrawRay(transform.position, averageNormal * 5f, Color.magenta);
        return averageNormal;
    }

    // New (normal based) movement is handled here
    void DoMovement() {
        // Add all of the various forces that affect the player's velocity to MoveVector.
        Vector3 sumForces = Vector3.zero;
        Vector3 normalForce = Vector3.zero;
        Vector3 gravityForce = Vector3.down * gravityForceStrength;
        myRB.angularVelocity *= 0.95f;

        if (pushCounter > 0) {
            Vector3 pushForce = transform.forward * accelerateStrength / 10.0f;
            myRB.AddForce(pushForce, ForceMode.Impulse);
            pushCounter--;
        }

        // Reduce the player's velocity based on how well they land. If the landing surface's normal matches
        // the player's up direction, no velocity is lost. The check is done as soon as the first part of the
        // player's collider touches the ground. The velocity loss is applied once the player is considered
        // to be grounded.
        if (!wasGrounded && isGrounded) {
            if (surfaceNormalAtLandingPoint != Vector3.zero) {
                float velLoss = 0.5f * Vector3.Dot(playerUpAtLanding, surfaceNormalAtLandingPoint) + 0.5f;
                myRB.velocity *= velLoss;

                playerUpAtLanding = Vector3.zero;
                surfaceNormalAtLandingPoint = Vector3.zero;
            }
        }

        float gravityMultiplier = 1.0f;
        wasGrounded = isGrounded;

        Vector3 posOnGround = transform.position;
        if (isGrounded) {
            // Handle all ground specific things here.

            // If the sled is on the ground, make sure it stays there.
            //sumForces = -transform.up * stickToGroundForceStrength;

            //Vector3 playerForward = Vector3.Cross(transform.right, groundCheckHit.normal);
            //Quaternion groundRot = Quaternion.LookRotation(playerForward, groundCheckHit.normal);

            // Friction is always in the direction opposite to movement, so -thisRigid.velocity.
            sumForces += -myRB.velocity * CalculateFriction() * frictionStrength;

            normalForce = CalculatePlayerSteering() * normalForceStrength;
        }
        else {
            // If the sled is not on the ground, it is in the air. Handle air movement here. 
            isBraking = false;
            transform.Rotate(
                turnAmount.y * Time.deltaTime * airControlStrength,
                turnAmount.x * Time.deltaTime * airControlStrength,
                turnAmount.z * Time.deltaTime * airControlStrength * 2f);

            gravityMultiplier = airGravityScale;

            foreach (ParticleSystem ps in particleSystems) {
                if (ps.isPlaying && ps.name.Contains("Friction"))
                    ps.Stop();
            }
        }

        // gravityMultiplier is used to dampen the gravity if the player is airborne.
        Vector3 gravityAndNormalForceSum = forceMultiplier * (normalForce + gravityForce * gravityMultiplier);

        RaycastHit hit;
        if (isGrounded && Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Ground"))) {
            Vector3 averageNormal = CalculateAverageSurfaceNormal(hit);
            gravityAndNormalForceSum = Vector3.ProjectOnPlane(gravityAndNormalForceSum, averageNormal);
            PlayerHoverAboveGround(hit, averageNormal);
        }

        Debug.DrawRay(transform.position, gravityAndNormalForceSum, Color.magenta);
        sumForces += gravityAndNormalForceSum;

        sumForces += -myRB.velocity * (dragAmount / (boostMode ? boostModeBoost : 1f));

        myRB.AddForce(sumForces * Time.deltaTime, ForceMode.Impulse);

        Debug.DrawRay(transform.position, myRB.velocity, Color.white);

        if (isBraking) {
            foreach (ParticleSystem particles in particleSystems) {
                if (particles.isStopped && particles.name.Contains("Brake")) {
                    particles.Play();
                }
                ParticleSystem.EmissionModule em = particles.emission;
                float emRate = Mathf.Min(myRB.velocity.magnitude / 150f, 1f);
                emRate = Mathf.Pow(5, Mathf.Pow(emRate, 2)) - 1f;
                //Debug.Log("setting emrate: " + emRate);
                em.rateOverTime = emRate * 500f;
            }
        }
        else {
            foreach (ParticleSystem particles in particleSystems) {
                if (particles.isPlaying && particles.name.Contains("Brake"))
                    particles.Stop();
            }
        }

        windSource.volume = Mathf.Min(myRB.velocity.magnitude / 300f, 0.5f);
        windSource.pitch = Mathf.Min(((myRB.velocity.magnitude / 300f) + .25f)*3f, 2.5f);

        if (isGrounded) {
            sledSource.UnPause();
            sledSource.volume = Mathf.Min(myRB.velocity.magnitude / 300f, 0.5f) + .25f;
            sledSource.pitch = Mathf.Min(((myRB.velocity.magnitude / 300f) + .25f) * 1.5f, 2f);
        }
        else {
            sledSource.Pause();
        }
    }

    Vector3 CalculatePlayerSteering() {
        RaycastHit groundHit;
        if (Physics.Raycast(transform.position, Vector3.down, out groundHit, Mathf.Infinity, LayerMask.GetMask("Ground"))) {
            // Calculate how well the player is able to affect the sled's facing. The faster
            // the sled is going, the worse the controls are. This can be plotted with:
            // 2 ^ -(steeringDamper * x / powerSteering), where x is the player's speed.
            float playerTurnability = Mathf.Pow(2.0f, -(steeringDamper * myRB.velocity.magnitude / powerSteering));

            float playerAngleToVel = CalculatePlayerAngleToVelocity() * AngleDir(transform.forward, myRB.velocity);

            // Rotate the sled according to the player's input.
            float rotationAmount = playerTurnability * turnStrength * Time.deltaTime * (turnAmount.x - turnAmount.z);
            transform.Rotate(0.0f, rotationAmount, 0.0f);

            // Rotate the normal using the sled's velocity as the axis. This is done because
            // in order to move the sled down along a slope, its movement is calculated as the
            // combined vector of gravity (always down) and the normal of the surface currently
            // under the player.
            bool isPlayerSlowAndNotTurning = myRB.velocity.magnitude < 3.0f && turnAmount.x < 0.05f;
            float calcHelper = playerAngleToVel * (isPlayerSlowAndNotTurning ? 0.0f : turnFactor);

            Vector3 rotatedNormal = Quaternion.AngleAxis(calcHelper, myRB.velocity) * groundHit.normal;
            return rotatedNormal;
        }

        return Vector3.zero;
    }

    bool GroundCheck(out RaycastHit rayCastResult) {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, out rayCastResult, checkDistance, LayerMask.GetMask("Ground"));
        //Debug.Log(rayCastResult.distance);
        return isGrounded;
    }

    // Calculate the angle between the player's velocity and where the sled is facing
    // Ignore the Y-axis.
    float CalculatePlayerAngleToVelocity() {
        Vector3 tempVector1 = new Vector3(transform.forward.x, 0.0f, transform.forward.z);
        Vector3 tempVector2 = new Vector3(myRB.velocity.x, 0.0f, myRB.velocity.z);

        // Here we make sure the result is always between 0 and 90 degrees because we
        // want the player to be able to move backwards too without the friction affecting
        // the player's velocity.
        float playerAngleToVelocity = Vector3.Angle(tempVector1, tempVector2);
        if (playerAngleToVelocity > 90.0f) {
            playerAngleToVelocity -= (playerAngleToVelocity - 90.0f) * 2.0f;
            playerAngleToVelocity *= -1;
        }

        return playerAngleToVelocity;
    }

    float AngleDir(Vector3 fwd, Vector3 targetDir) {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, Vector3.up);

        return Mathf.Sign(dir);
    }

    float CalculateFriction() {
        float velocityToFacingAngle = Mathf.Abs(CalculatePlayerAngleToVelocity());

        // Divide the angle by 22.5f so the value is always [0, 4] because those values are what
        // the friction equation has been tuned to work with.
        velocityToFacingAngle = velocityToFacingAngle / 15f;

        // This is where we simulate "friction", i.e. how much to slow the player down if they are not
        // facing the velocity. The equation is: 
        // f(x) = ((2 ^ (2 * x) - 1) / frictionDamper + frictionOffset) / (17 + frictionOffset) 
        // With its default values of frictionDamper = 15 and frictionOffset = 0, f(0) = 0 and f(4) = 17
        // The equation can be plotted on the desmos calculator with: 
        // \frac{\left(\frac{\left(2^{2x}-1\right)}{b}+c\right)}{c\ +\ 17}
        float frictionForce = (((Mathf.Pow(2.0f, 2.0f * velocityToFacingAngle) - 1.0f) / frictionDamper) + frictionOffset);
        frictionForce /= 17.0f + frictionOffset;

        string breakingDir = AngleDir(transform.forward, myRB.velocity) > 0.0f ? "Right" : "Left";
        if (velocityToFacingAngle > 1) {
            foreach (ParticleSystem particleSystem in particleSystems) {
                if (particleSystem.name.Contains("Friction" + breakingDir)) {
                    if (particleSystem.isStopped)
                        particleSystem.Play();

                    ParticleSystem.EmissionModule em = particleSystem.emission;
                    float emRate = velocityToFacingAngle / 6f;
                    emRate = Mathf.Pow(5f, Mathf.Pow(emRate, 2f)) - 1f;
                    //Debug.Log(emRate);
                    em.rateOverTime = emRate * 500f;
                }
            }
        }
        else {
            foreach (ParticleSystem ps in particleSystems) {
                if (ps.isPlaying && ps.name.Contains("Friction"))
                    ps.Stop();
            }
        }

        breakingDir = AngleDir(transform.forward, myRB.velocity) > 0.0f ? "Left" : "Right";
        float otherVelThing = velocityToFacingAngle;
        if (otherVelThing > 3f)
            otherVelThing -= (otherVelThing - 3f) * 2f;
        if (velocityToFacingAngle > 0) {
            foreach (ParticleSystem particleSystem in particleSystems) {
                if (particleSystem.name.Contains("Turn" + breakingDir)) {
                    if (particleSystem.isStopped)
                        particleSystem.Play();

                    ParticleSystem.EmissionModule em = particleSystem.emission;
                    float emRate = otherVelThing / 6f;
                    emRate = Mathf.Pow(5f, Mathf.Pow(emRate, 2f)) - 1f;
                    Debug.Log(emRate);
                    em.rateOverTime = emRate * 100f;
                }
            }
        }
        else {
            foreach (ParticleSystem ps in particleSystems) {
                if (ps.isPlaying && ps.name.Contains("Turn"))
                    ps.Stop();
            }
        }

        frictionForce += isBraking ? brakingFriction : 0.0f;

        return (frictionForce / (boostMode ? boostModeBoost : 1f));
    }

    internal void Jump() {
        myRB.velocity += transform.up * jumpCharge * jumpStrength;
        jumpCharge = 0f;
    }

    internal void PushForwards() {
        myRB.AddForce(transform.forward * (accelerateStrength / 10.0f), ForceMode.Impulse);
        pushCounter = 9;
    }

    internal void JumpChargeAdd() {
        // Increment jumpCharge by jumpChargeIncrement steps. It always counts up to 10.0f
        // and stops there. Use the jumpStrength variable instead to change how high the
        // player can jump.
        jumpCharge += Time.deltaTime * (jumpCharge < 10.0f ? jumpChargeIncrement : 0.0f);
    }
}
