using UnityEngine;

/// <summary>
/// Controls day-night cycle animation speed via keyboard input.
/// </summary>
public class DayNightController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator _animator; // Reference to the Animator component

    [Header("Settings")]
    [SerializeField, Range(0.1f, 10f)] private float _minSpeed = 0.1f; // Minimum animation speed
    [SerializeField, Range(1f, 20f)] private float _maxSpeed = 5f; // Maximum animation speed
    [SerializeField] private float _speedChangeStep = 0.5f; // Speed change per second

    private float _animationSpeed; // Current animation speed

    private void Start()
    {
        // Validate Animator reference
        if (_animator == null)
        {
            Debug.LogError("Animator is not assigned in DayNightController!");
            enabled = false;
            return;
        }

        InitializeAnimationSpeed();
    }

    private void Update()
    {
        HandleSpeedInput();
        UpdateAnimatorSpeed();
    }

    /// <summary>
    /// Initializes animation speed with the default value from Animator
    /// </summary>
    private void InitializeAnimationSpeed()
    {
        _animationSpeed = _animator.speed;
    }

    /// <summary>
    /// Handles keyboard input for speed adjustment
    /// </summary>
    private void HandleSpeedInput()
    {
        // Speed increase
        if (Input.GetKey(KeyCode.UpArrow))
        {
            _animationSpeed += _speedChangeStep * Time.deltaTime;
        }
        // Speed decrease
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            _animationSpeed -= _speedChangeStep * Time.deltaTime;
        }

        ClampAnimationSpeed();
    }

    /// <summary>
    /// Updates Animator component with current speed value
    /// </summary>
    private void UpdateAnimatorSpeed()
    {
        _animator.speed = _animationSpeed;
    }

    /// <summary>
    /// Keeps animation speed within defined boundaries
    /// </summary>
    private void ClampAnimationSpeed()
    {
        _animationSpeed = Mathf.Clamp(
            _animationSpeed,
            _minSpeed,
            _maxSpeed
        );
    }
}