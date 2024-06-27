using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private PlayerControls _playerControls;
    private CarController _carController;
    private ResetCar _resetCar;
    private RacingController _racingController;

    public float accel;
    public float handBrake;
    public float turn;

    public bool isPaused = false;
    public bool isRaceStart = false;

    public static Action<bool> OnPause;
    public static Action<bool> OnRaceStart;

    private void Awake()
    {
        _playerControls = new PlayerControls();
        _playerControls.Player.Enable();
        _playerControls.Player.Reset.performed += _ => _resetCar.Respawn();
        _playerControls.Player.Pause.performed += Pause_performed;
        _playerControls.Player.RaceStart.performed += Race_Start;

        _carController = GetComponent<CarController>();
        _resetCar = GetComponent<ResetCar>();
        _racingController = GetComponent<RacingController>();
    }

    private void Race_Start(InputAction.CallbackContext context)
    {
        isRaceStart = !isRaceStart;
        OnRaceStart?.Invoke(isRaceStart);
    }

    private void Pause_performed(InputAction.CallbackContext context)
    {
        isPaused = !isPaused;
        OnPause?.Invoke(isPaused);
    }

    private void Update()
    {
        accel = _playerControls.Player.Accelerate.ReadValue<float>();
        turn = _playerControls.Player.Turn.ReadValue<float>();
        handBrake = _playerControls.Player.HandBrake.ReadValue<float>();
    }

    private void FixedUpdate()
    {
        _carController.Move(turn, accel, accel, handBrake);
        _racingController.pressRace = isRaceStart;
    }
}
