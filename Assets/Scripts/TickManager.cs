using System;
using UnityEngine;

public class TickManager : MonoBehaviour
{
    // Singleton instance for easy global access (optional)
    public static TickManager Instance;

    // How many ticks per second
    [SerializeField] private float ticksPerSecond = 10f;

    // Event called every tick
    public event Action OnTick;

    private float _tickTimer;
    private float _tickInterval;
    private int _tickCount;

    private void Awake()
    {
        // Simple Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _tickInterval = 1f / ticksPerSecond;
    }

    private void Update()
    {
        _tickTimer += Time.deltaTime;

        if (_tickTimer >= _tickInterval)
        {
            _tickTimer -= _tickInterval;
            _tickCount++;
            OnTick?.Invoke();
        }
    }

    // Optional utility function to get current tick count
    public int GetTickCount()
    {
        return _tickCount;
    }

    // Optional method to change tick speed dynamically
    public void SetTickRate(float newTicksPerSecond)
    {
        ticksPerSecond = Mathf.Max(0.1f, newTicksPerSecond);
        _tickInterval = 1f / ticksPerSecond;
    }
}
