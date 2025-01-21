using UnityEngine;

public class DayNightController : MonoBehaviour
{
    public Animator animator; // Link to Animator
    public float animationSpeed = 0.5f; // Animation speed

    void Start()
    {
        // Setting the initial speed
        animator.speed = animationSpeed;
    }

    void Update()
    {
        // changing the speed in real time
        if (Input.GetKeyDown(KeyCode.UpArrow)) // speedup
        {
            animationSpeed += 0.1f;
            animator.speed = animationSpeed;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow)) // slowdown
        {
            animationSpeed -= 0.1f;
            animator.speed = Mathf.Max(animationSpeed, 0.1f); // A minimum of 0.1 so that the animation does not stop
        }
    }
}
