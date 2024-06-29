using Fusion;
using UnityEngine;

internal enum CarDriveType
{
    FrontWheelDrive,
    RearWheelDrive,
    AllWheelDrive
}

public enum SpeedType
{
    MPH,
    KPH
}

[RequireComponent(typeof(Rigidbody), typeof(NetworkTransform))]
public class CarController : NetworkBehaviour
{
    [Header("Car Presets")]
    [SerializeField] private CarDriveType _carDriveType = CarDriveType.AllWheelDrive;
    public SpeedType _speedType = SpeedType.MPH;

    [Header("Car Components"), Space(7)]
    [SerializeField] private GameObject[] _wheelMeshes = new GameObject[4];
    public WheelCollider[] wheelColliders = new WheelCollider[4];

    [Header("Car Settings"), Space(7)]
    [Tooltip("The amount of offset to apply to the rigidbody center of mass.")]
    [SerializeField] private Vector3 _centerOfMassOffset;
    [Tooltip("How far the wheels can turn."), Range(20f, 35f)]
    [SerializeField] private float _maximumSteerAngle;
    [Tooltip("How much torque to add to the drive wheels when moving forward.")]
    [SerializeField] private float _fullTorqueOverAllWheels;
    [Tooltip("How much torque to add to the drive wheels in reverse.")]
    [SerializeField] private float _reverseTorque;
    [Tooltip("How much force should be used for the handbrake.")]
    [SerializeField] private float _maxHandbrakeTorque;
    [Tooltip("Will limit how fast the car can go.")]
    [SerializeField] private float _topSpeed = 200.0f;
    [Tooltip("The limit of the rev range.")]
    [SerializeField] private float _revRangeBoundary = 1f;
    [Tooltip("How much slip until wheel effects start playing."), Range(0.1f, 1f)]
    [SerializeField] private float _slipLimit;
    [Tooltip("How much force will be used to apply the brakes")]
    [SerializeField] private float _brakeTorque;
    [Tooltip("How quickly digital input reaches the max value.")]
    [SerializeField] private float _smoothInputSpeed = 0.2f;
    private static int NumberOfGears = 5;

    [Header("Steering Helpers"), Space(7)]
    [Tooltip("How much force will be applied to the wheels to prevent flipping. (A good value is around the spring value of the wheel collider.")]
    [SerializeField] private float _antiRollVal = 3500.0f;
    [Tooltip("How much down force to add to the car.")]
    [SerializeField] private float _downForce = 100.0f;
    [Tooltip("0 is pure physics, 1 the car will grip in the direction it's facing.")]
    [SerializeField, Range(0, 1)] private float _steerHelper;
    [Tooltip("0 is no traction control, 1 will try and prevent any slipping")]
    [SerializeField, Range(0, 1)] private float _tractionControl;

    [Header("Knockback Settings")]
    [SerializeField] private LayerMask barrierLayer;
    [SerializeField] private float knockbackForce = 1000f;

    private Quaternion[] _wheelMeshLocalRotations;
    private float _steerAngle;
    private int _gearNum;
    private float _gearFactor;
    private float _oldRotation;
    private float _currentTorque;
    private Rigidbody _rigidbody;
    private Vector2 _currentInputVector;
    private Vector2 _smoothInputVelocity;
    private int _emissionPropertyId;
    private float _currentMaxSteerAngle;

    [Networked]
    public Vector3 NetworkedPosition { get; set; }

    [Networked]
    public Quaternion NetworkedRotation { get; set; }

    public bool Skidding { get; private set; }
    public float BrakeInput { get; private set; }
    public float CurrentSteerAngle { get { return _steerAngle; } }
    public float CurrentSpeed { get { return _speedType == SpeedType.MPH ? _rigidbody.velocity.magnitude * 2.23693629f : _rigidbody.velocity.magnitude * 3.6f; } }
    public float MaxSpeed { get { return _topSpeed; } }
    public float Revs { get; private set; }
    public float AccelInput { get; private set; }

    private void Awake()
    {
        _wheelMeshLocalRotations = new Quaternion[4];
        for (int i = 0; i < 4; i++)
        {
            _wheelMeshLocalRotations[i] = _wheelMeshes[i].transform.localRotation;
        }

        _maxHandbrakeTorque = float.MaxValue;
        _rigidbody = GetComponent<Rigidbody>();
        _currentTorque = _fullTorqueOverAllWheels - (_tractionControl * _fullTorqueOverAllWheels);
        _rigidbody.centerOfMass += _centerOfMassOffset;
        _emissionPropertyId = Shader.PropertyToID("_EmissionColor");

        // Ensure the NetworkTransform component is present
        if (GetComponent<NetworkTransform>() == null)
        {
            gameObject.AddComponent<NetworkTransform>();
        }
    }

    private void Start()
    {
        if (NetworkRunnerHandler.Instance.loadingScreenManager != null)
        {
            NetworkRunnerHandler.Instance.loadingScreenManager.HideLoadingScreen();
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasInputAuthority)
        {
            CheckBarrierCollision();
            // Handle movement based on inputs
        }

        if (Object.HasStateAuthority)
        {
            NetworkedPosition = transform.position;
            NetworkedRotation = transform.rotation;
        }
        else
        { 
            transform.position = Vector3.Lerp(transform.position, NetworkedPosition, 0.2f);
            transform.rotation = Quaternion.Slerp(transform.rotation, NetworkedRotation, 0.2f);
        }

    }

    private void CheckBarrierCollision()
    {
        RaycastHit hit;

        // Raycast to check collision with barriers on BarrierLayer
        if (Physics.Raycast(transform.position, transform.forward, out hit, 2f, barrierLayer))
        {
            // Calculate knockback direction (opposite of current forward direction)
            Vector3 knockbackDirection = -transform.forward;

            // Apply knockback force to rigidbody
            _rigidbody.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
        }
    }

    private void GearChanging()
    {
        float f = Mathf.Abs(CurrentSpeed / MaxSpeed);
        float upGearLimit = (1 / (float)NumberOfGears) * (_gearNum + 1);
        float downGearLimit = (1 / (float)NumberOfGears) * _gearNum;

        if (_gearNum > 0 && f < downGearLimit)
            _gearNum--;

        if (f > upGearLimit && (_gearNum < (NumberOfGears - 1)))
            _gearNum++;
    }

    private static float CurveFactor(float factor)
    {
        return 1 - (1 - factor) * (1 - factor);
    }

    private static float UnclampedLerp(float from, float to, float value)
    {
        return (1.0f - value) * from + value * to;
    }

    private void CalculateGearFactor()
    {
        float f = (1 / (float)NumberOfGears);

        float targetGearFactor = Mathf.InverseLerp(f * _gearNum, f * (_gearNum + 1), Mathf.Abs(CurrentSpeed / MaxSpeed));
        _gearFactor = Mathf.Lerp(_gearFactor, targetGearFactor, Time.deltaTime * 5.0f);
    }

    private void CalculateRevs()
    {
        CalculateGearFactor();
        float gearNumFactor = _gearNum / (float)NumberOfGears;
        float revsRangeMin = UnclampedLerp(0f, _revRangeBoundary, CurveFactor(gearNumFactor));
        float revsRangeMax = UnclampedLerp(_revRangeBoundary, 1f, gearNumFactor);
        Revs = UnclampedLerp(revsRangeMin, revsRangeMax, _gearFactor);
    }

    public void Move(float steering, float accel, float footBrake, float handBrake)
    {
        if (!Object.HasInputAuthority) return;

        Vector2 input = new Vector2(steering, accel);
        _currentInputVector = Vector2.SmoothDamp(_currentInputVector, input, ref _smoothInputVelocity, _smoothInputSpeed);
        accel = _currentInputVector.y;
        steering = _currentInputVector.x;

        for (int i = 0; i < 4; i++)
        {
            wheelColliders[i].GetWorldPose(out Vector3 position, out Quaternion quat);
            _wheelMeshes[i].transform.SetPositionAndRotation(position, quat);
        }

        steering = Mathf.Clamp(steering, -1, 1);
        AccelInput = accel = Mathf.Clamp(accel, 0, 1);
        BrakeInput = footBrake = -1 * Mathf.Clamp(footBrake, -1, 0);
        handBrake = Mathf.Clamp(handBrake, 0, 1);

        _steerAngle = steering * _currentMaxSteerAngle;
        wheelColliders[0].steerAngle = _steerAngle;
        wheelColliders[1].steerAngle = _steerAngle;

        SteerHelper();
        ApplyDrive(accel, footBrake);
        CapSpeed();

        if (handBrake > 0f)
        {
            var hbTorque = handBrake * _maxHandbrakeTorque;
            wheelColliders[2].brakeTorque = hbTorque;
            wheelColliders[3].brakeTorque = hbTorque;
        }

        CalculateRevs();
        GearChanging();

        AddDownForce();
        TractionControl();
    }

    private void CapSpeed()
    {
        float speed = _rigidbody.velocity.magnitude;
        switch (_speedType)
        {
            case SpeedType.MPH:
                speed *= 2.23693629f;
                if (speed > _topSpeed)
                    _rigidbody.velocity = (_topSpeed / 2.23693629f) * _rigidbody.velocity.normalized;
                break;

            case SpeedType.KPH:
                speed *= 3.6f;
                if (speed > _topSpeed)
                    _rigidbody.velocity = (_topSpeed / 3.6f) * _rigidbody.velocity.normalized;
                break;
        }
    }

    private void ApplyDrive(float accel, float footBrake)
    {
        float thrustTorque;
        switch (_carDriveType)
        {
            case CarDriveType.AllWheelDrive:
                thrustTorque = accel * (_currentTorque / 4f);
                for (int i = 0; i < 4; i++)
                {
                    wheelColliders[i].motorTorque = thrustTorque;
                }
                break;

            case CarDriveType.FrontWheelDrive:
                thrustTorque = accel * (_currentTorque / 2f);
                wheelColliders[0].motorTorque = wheelColliders[1].motorTorque = thrustTorque;
                break;

            case CarDriveType.RearWheelDrive:
                thrustTorque = accel * (_currentTorque / 2f);
                wheelColliders[2].motorTorque = wheelColliders[3].motorTorque = thrustTorque;
                break;
        }

        for (int i = 0; i < 4; i++)
        {
            if (CurrentSpeed > 5 && Vector3.Angle(transform.forward, _rigidbody.velocity) < 50f)
            {
                wheelColliders[i].brakeTorque = _brakeTorque * footBrake;
            }
            else if (footBrake > 0)
            {
                wheelColliders[i].brakeTorque = 0f;
                wheelColliders[i].motorTorque = -_reverseTorque * footBrake;
            }
        }
    }

    private void SteerHelper()
    {
        for (int i = 0; i < 4; i++)
        {
            WheelHit wheelhit;
            wheelColliders[i].GetGroundHit(out wheelhit);
            if (wheelhit.normal == Vector3.zero)
                return;
        }

        if (Mathf.Abs(_oldRotation - transform.eulerAngles.y) < 10f)
        {
            var turnadjust = (transform.eulerAngles.y - _oldRotation) * _steerHelper;
            Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
            _rigidbody.velocity = velRotation * _rigidbody.velocity;
        }
        _oldRotation = transform.eulerAngles.y;
    }

    private void AddDownForce()
    {
        wheelColliders[0].attachedRigidbody.AddForce(-transform.up * _downForce *
                                                     wheelColliders[0].attachedRigidbody.velocity.magnitude);
    }

    private void TractionControl()
    {
        WheelHit wheelHit;
        switch (_carDriveType)
        {
            case CarDriveType.AllWheelDrive:
                for (int i = 0; i < 4; i++)
                {
                    wheelColliders[i].GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);
                }
                break;

            case CarDriveType.RearWheelDrive:
                wheelColliders[2].GetGroundHit(out wheelHit);
                AdjustTorque(wheelHit.forwardSlip);

                wheelColliders[3].GetGroundHit(out wheelHit);
                AdjustTorque(wheelHit.forwardSlip);
                break;

            case CarDriveType.FrontWheelDrive:
                wheelColliders[0].GetGroundHit(out wheelHit);
                AdjustTorque(wheelHit.forwardSlip);

                wheelColliders[1].GetGroundHit(out wheelHit);
                AdjustTorque(wheelHit.forwardSlip);
                break;
        }
    }

    private void AdjustTorque(float forwardSlip)
    {
        if (forwardSlip >= _slipLimit && _currentTorque >= 0)
        {
            _currentTorque -= 10 * _tractionControl;
        }
        else
        {
            _currentTorque += 10 * _tractionControl;
            if (_currentTorque > _fullTorqueOverAllWheels)
            {
                _currentTorque = _fullTorqueOverAllWheels;
            }
        }
    }
}
