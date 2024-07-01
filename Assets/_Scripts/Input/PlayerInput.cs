using System;
using System.Collections;
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

    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject nicknamePanel; // Reference to the nickname panel in the scene

    [SerializeField] private GameObject[] carModels;

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

        string nickname = PlayerPrefs.GetString("Nickname", "");

        if (string.IsNullOrEmpty(nickname))
        {
            // Enable the nickname panel if nickname does not exist
            if (nicknamePanel != null)
            {
                nicknamePanel.SetActive(true);
            }
        }
        else
        {
            Debug.Log($"API: {PlayerPrefs.GetInt("Car")}");
            //StartCoroutine(ChangeCar());
        }
    }

    IEnumerator ChangeCar()
    {
        yield return new WaitUntil(() => Manager.Instance != null);
        Manager.Instance.InitCar(PlayerPrefs.GetInt("Car"));
    }

    public void ChangeCar(int index)
    {
        foreach(var c in carModels)
        {
            c.SetActive(false);
        }

        carModels[index].SetActive(true);
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

    public enum PlayerInputButtons
    {
        None,
        Brake,
    }
}
