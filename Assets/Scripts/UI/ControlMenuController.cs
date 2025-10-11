using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlMenuController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _stopButton;
    [SerializeField] private TMP_Dropdown _startPatternDropdown;
    [SerializeField] private Slider _speedSlider;

    [Header("Systems")]
    [SerializeField] private Simulation _simulation;
    [SerializeField] private TickManager _tickManager;
    [SerializeField] private GridManager _gridManager;

    private void Awake()
    {
        if (_playButton == null || _stopButton == null || _startPatternDropdown == null || _speedSlider == null)
        {
            Debug.LogError("One or more UI elements are not assigned in the inspector.");
            return;
        }

        if (_simulation == null)
        {
            Debug.LogError("Simulation reference is missing in ControlMenuController script.");
            return;
        }

        if (_tickManager == null)
        {
            Debug.LogError("TickManager reference is missing in ControlMenuController script.");
            return;
        }

        if (_gridManager == null)
        {
            Debug.LogError("GridManager reference is missing in ControlMenuController script.");
            return;
        }

        _startPatternDropdown.ClearOptions();
        _startPatternDropdown.AddOptions(Enum.GetNames(typeof(Grid.StartPattern)).ToList());

        _playButton.onClick.AddListener(_simulation.StartSimulation);
        _stopButton.onClick.AddListener(_simulation.StopSimulation);

        _startPatternDropdown.onValueChanged.AddListener(OnStartPatternChanged);
        _speedSlider.onValueChanged.AddListener(OnSpeedChanged);
    }

    private void OnSpeedChanged(float speed)
    {
        Debug.Log($"Speed changed to: {speed * 100}");
        _tickManager.SetTickRate(speed * 100);
    }

    private void OnStartPatternChanged(int index)
    {
        _gridManager.ChangeStartPattern((Grid.StartPattern)index);
    }
}
