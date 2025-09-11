using UnityEngine;

public class PixieDustAnimationHandler : MonoBehaviour
{
    [Header("Levitation Animation")]
    [SerializeField] private string levitatingAnimationName = "Levitating";
    [SerializeField] private float animationBlendTime = 0.2f;

    private Animator animator;
    private PixieDustEffectController effectController;
    private bool wasLevitating = false;
    private string previousAnimation;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    void Update()
    {
        CheckForPixieDustEffect();
        HandleLevitationAnimation();
    }

    private void CheckForPixieDustEffect()
    {
        if (effectController == null)
        {
            effectController = GetComponentInChildren<PixieDustEffectController>();
        }
    }

    private void HandleLevitationAnimation()
    {
        if (effectController == null || animator == null) return;

        bool isCurrentlyLevitating = effectController.IsLevitating;

        // If we just started levitating
        if (isCurrentlyLevitating && !wasLevitating)
        {
            // Store the current animation to return to later if needed
            AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);

            // Play levitation animation
            animator.CrossFade(levitatingAnimationName, animationBlendTime);

            wasLevitating = true;
        }
        // If we just stopped levitating
        else if (!isCurrentlyLevitating && wasLevitating)
        {
            wasLevitating = false;
            // Let the normal animation system take over
        }
    }

    // Optional: Method to check if we should override normal animations
    public bool ShouldOverrideAnimation()
    {
        return effectController != null && effectController.IsLevitating;
    }
}