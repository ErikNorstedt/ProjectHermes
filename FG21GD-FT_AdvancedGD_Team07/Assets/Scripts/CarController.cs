using System;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    AnimationCurve turnInputCurve = AnimationCurve.Linear(-1.0f, -1.0f, 1.0f, 1.0f);
    public WheelCollider[] TurnWheel { get { return turnWheel; } }

    bool isGrounded = false;
    int lastGroundCheck = 0;
    public bool IsGrounded
    {
        get
        {
            /*
            if (lastGroundCheck == Time.frameCount)
                return isGrounded;

            lastGroundCheck = Time.frameCount;
            isGrounded = true;

            foreach (WheelCollider wheel in wheels)
            {
                if (!wheel.gameObject.activeSelf || !wheel.isGrounded)
                    isGrounded = false;
            }
            */

            return isGrounded;
        }
    }
    public void SetGrounded(int ID) { if(ID == 0) isGrounded = true; }
    public void SetIsNotGrounded(int ID) { if (ID == 0) isGrounded = false; }

    [Title("Motor")]
    [SerializeField] AnimationCurve motorTorque = new AnimationCurve(new Keyframe(0, 200), new Keyframe(50, 300), new Keyframe(200, 0));
    [Range(2, 16), SerializeField] float forwardForce = 12.0f;
    [Range(2, 16), SerializeField] float reverseForce = 6.0f;
    [Range(0f, 1f), SerializeField] float accelerationMultiplier = 0.0f;

    [Range(0f, 100f), SerializeField] float velocityClamp = 40.0f;
    [Range(0f, 10f), SerializeField] float slowdownFactor = 2.0f;

    public float DiffGearing { get { return forwardForce; } set { forwardForce = value; } }

    [Title("Steering")]
    [SerializeField] float brakeForce = 1500.0f;
    public float BrakeForce { get { return brakeForce; } set { brakeForce = value; } }

    [SerializeField, Range(0f, 50.0f)] float steerAngle = 30.0f;
    public float SteerAngle { get { return steerAngle; } set { steerAngle = Mathf.Clamp(value, 0.0f, 50.0f); } }

    [SerializeField, Range(0.001f, 1.0f)] float steerSpeed = 0.2f;
    public float SteerSpeed { get { return steerSpeed; } set { steerSpeed = Mathf.Clamp(value, 0.001f, 1.0f); } }

    public int jumpCount = 2;
    private int numjumps;

    [SerializeField, Range(0f, 30f)] float jumpVel = 8f;
    public float JumpVel { get { return jumpVel; } set { jumpVel = Mathf.Clamp(value, 1.0f, 1.5f); } }

    [SerializeField, Range(0.0f, 2f)] float driftIntensity = 1f;
    public float DriftIntensity { get { return driftIntensity; } set { driftIntensity = Mathf.Clamp(value, 0.0f, 2.0f); } }

    public Transform spawnPosition;
    public Transform centerOfMass;

    [SerializeField, Range(0.5f, 10f)] float downforce = 1.0f;
    public float Downforce { get { return downforce; } set { downforce = Mathf.Clamp(value, 0, 5); } }

    float steering;
    public float Steering { get { return steering; } set { steering = Mathf.Clamp(value, -1f, 1f); } }

    float throttle;
    public float Throttle { get { return throttle; } set { throttle = Mathf.Clamp(value, -1f, 1f); } }

    bool handbrake;
    public bool Handbrake { get { return handbrake; } set { handbrake = value; } }

    [HideInInspector] public bool allowDrift = true;
    bool drift;
    public bool Drift { get { return drift; } set { drift = value; } }

    float speed = 0.0f;
    public float Speed { get { return speed; } }

    private bool glide;
    [SerializeField, Range(1f,1.2f)]private float glideForce = 1f;
    [SerializeField, Range(1f, 20f)] private float glideRotation = 5f;
    [SerializeField, Range(1f, 90f)] private float airBorneZRotationAmount = 20f;

    [FoldoutGroup("Events")] public UnityEvent onAccelerate, onBreak, onJump, onDoubleJump;

    [Header("Wheels")]
    [SerializeField] WheelCollider[] driveWheel;
    public WheelCollider[] DriveWheel { get { return driveWheel; } }
    [SerializeField] WheelCollider[] turnWheel;
    [SerializeField] Transform[] meshWheel;

    Rigidbody _rb;
    WheelCollider[] wheels;

    float gearing;

    public bool isToppled;
    public void SetIsTobbled() => isToppled = true;
    public void SetIsNotTobbled() => isToppled = false;

    private float groundedTimer = 0f;

    void OnEnable()
    {
        RaceManager.onStartRace += SetResetSpawnPos;
        RaceManager.onEndRace += ResetToSpawn;
    }
    private void OnDisable()
    {
        RaceManager.onStartRace -= SetResetSpawnPos;
        RaceManager.onEndRace -= ResetToSpawn;
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody>();

        if (_rb != null && centerOfMass != null)
            _rb.centerOfMass = centerOfMass.localPosition;

        wheels = GetComponentsInChildren<WheelCollider>();

        foreach (WheelCollider wheel in wheels)
            wheel.motorTorque = 0.0001f;

        foreach (WheelCollider wheel in wheels)
            wheel.ConfigureVehicleSubsteps(10f, 12, 8);
    }

    private void Update()
    {
        HandleInput();
        HandleJump();
    }

    void FixedUpdate()
    {
        if (IsGrounded)
        {
            HandleSteering();
            HandleDrifting();
            _rb.AddForce(-transform.up * speed * downforce);
            numjumps = 0;
        }
        else
        {
            _rb.angularVelocity = Vector3.zero;
            HandleGliding();
            HandleFlyingRotation();
        }

        _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, velocityClamp);

        HandleSlowdown();
        HandleWheelAnimation();
    }

    private void HandleSlowdown()
    {
        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) < 0.1f && Mathf.Abs(Input.GetAxisRaw("Vertical")) < 0.1f)
            _rb.drag = slowdownFactor;
        else
            _rb.drag = 0f;
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.R))
            ResetToSpawn(spawnPosition, true);

        speed = transform.InverseTransformDirection(_rb.velocity).z * 3.6f;

        throttle = GetInput("Vertical") - GetInput("Cancel");

        if (throttle < 0.5f)
            onAccelerate?.Invoke();

        handbrake = Input.GetKey(KeyCode.LeftControl);

        if (handbrake)
            onBreak?.Invoke();

        glide = _rb.velocity.y < -0.1f && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && !IsGrounded;

        steering = turnInputCurve.Evaluate(GetInput("Horizontal")) * steerAngle;
        drift = GetInput("Submit") > 0 && _rb.velocity.sqrMagnitude > 100;
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (IsGrounded)
            {
                numjumps++;
                isGrounded = false;
                groundedTimer = 0f;

                _rb.angularVelocity = Vector3.zero;

                if(isToppled)
                    _rb.velocity = new Vector3(_rb.velocity.x, jumpVel, _rb.velocity.z);
                else
                    _rb.velocity += transform.up * jumpVel;

                onJump?.Invoke();
            }
            else
            {
                groundedTimer += Time.deltaTime;

                if (numjumps < jumpCount - 1)
                {
                    numjumps++;
                    isGrounded = false;
                    _rb.angularVelocity = Vector3.zero;
                    _rb.velocity += transform.up * jumpVel;

                    onDoubleJump?.Invoke();
                }
            }
        }
    }

    private void HandleSteering()
    {
        foreach (WheelCollider wheel in turnWheel)
            wheel.steerAngle = Mathf.Lerp(wheel.steerAngle, steering, steerSpeed);

        foreach (WheelCollider wheel in wheels)
            wheel.brakeTorque = 0;

        if (handbrake)
        {
            foreach (WheelCollider wheel in wheels)
            {
                wheel.motorTorque = 0.0001f;
                wheel.brakeTorque = brakeForce;
            }

            Vector3 vel = transform.InverseTransformDirection(_rb.velocity);
            vel.x *= 0.95f;
            vel.z *= 0.95f;
            _rb.velocity = transform.TransformDirection(vel);
        }
        else if (Mathf.Abs(speed) < 4 || Mathf.Sign(speed) == Mathf.Sign(throttle))
        {
            
            if (Input.GetAxisRaw("Vertical") > 0)
                gearing = forwardForce;
            else
                gearing = reverseForce;
            
            _rb.velocity += transform.forward * Input.GetAxisRaw("Vertical") * accelerationMultiplier;

            foreach (WheelCollider wheel in driveWheel)
                wheel.motorTorque = throttle * motorTorque.Evaluate(speed) * gearing / driveWheel.Length;
        }
        else
        {
            foreach (WheelCollider wheel in wheels)
                wheel.brakeTorque = Mathf.Abs(throttle) * brakeForce;
        }
    }

    private void HandleDrifting()
    {
        if (drift && allowDrift)
        {
            Vector3 driftForce = -transform.right;
            driftForce.y = 0.0f;
            driftForce.Normalize();

            if (steering != 0)
                driftForce *= _rb.mass * speed / 7f * throttle * steering / steerAngle;
            Vector3 driftTorque = transform.up * 0.1f * steering / steerAngle;


            _rb.AddForce(driftForce * driftIntensity, ForceMode.Force);
            _rb.AddTorque(driftTorque * driftIntensity, ForceMode.VelocityChange);
        }
    }

    private void HandleGliding()
    {
        if (glide)
        {
            Vector3 vel = transform.InverseTransformDirection(_rb.velocity);
            vel.y *= 1 / glideForce;
            _rb.velocity = transform.TransformDirection(vel);
            _rb.velocity += transform.forward * Mathf.Clamp(Input.GetAxisRaw("Vertical"), 0f, 1f) * Time.deltaTime * 50f;
        }
    }

    private void HandleFlyingRotation()
    {
        if(groundedTimer < 0.5f)
        {
            float yRot = _rb.rotation.eulerAngles.y + Input.GetAxisRaw("Horizontal") * glideRotation;
            float zRot = -Input.GetAxisRaw("Horizontal") * airBorneZRotationAmount;

            _rb.rotation = Quaternion.Slerp(_rb.rotation, Quaternion.Euler(0f, yRot, zRot), Time.deltaTime * 5f);
        }
    }

    private void HandleWheelAnimation()
    {
        for (int i = 0; i < driveWheel.Length; i++)
        {
            Vector3 pos;
            Quaternion rot;
            driveWheel[i].GetWorldPose(out pos, out rot);

            meshWheel[i].position = pos;
            meshWheel[i].rotation = Quaternion.Slerp(meshWheel[i].rotation, rot, Time.deltaTime * 10f);
        }
    }

    public void SetResetSpawnPos(Transform pos) => spawnPosition = pos;

    public void ResetToSpawn(Transform pos, bool resetPos)
    {
        if(resetPos)
        {
            if(pos == null && spawnPosition != null)
            {
                transform.position = spawnPosition.position;
                transform.rotation = spawnPosition.rotation;
            }
            else
            {
                transform.position = pos.position;
                transform.rotation = pos.rotation;
            }
        }

        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
    }

    public void toogleHandbrake(bool h) => handbrake = h;

    private float GetInput(string input) => Input.GetAxisRaw(input);
}