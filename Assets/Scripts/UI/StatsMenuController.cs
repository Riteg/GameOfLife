using TMPro;
using UnityEngine;

public class StatsMenuController : MonoBehaviour
{
    public static StatsMenuController Instance { get; private set; }

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI _gridCreateText;
    [SerializeField] private TextMeshProUGUI _gridUpdateText;
    [SerializeField] private TextMeshProUGUI _simCalcText;

    //[SerializeField] private TextMeshProUGUI _aliveCellsText;
    //[SerializeField] private TextMeshProUGUI _deadCellsText;
    //[SerializeField] private TextMeshProUGUI _gridSizeText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    public void UpdateGridCreateTime(float time)
    {
        _gridCreateText.text = $"Grid Create Time: {time} ms";
    }

    public void UpdateGridUpdateTime(float time)
    {
        _gridUpdateText.text = $"Grid Update Time: {time} t";
    }

    public void UpdateSimulationCalcTime(float time)
    {
        _simCalcText.text = $"Sim Calc Time: {time} ms";
    }
}
