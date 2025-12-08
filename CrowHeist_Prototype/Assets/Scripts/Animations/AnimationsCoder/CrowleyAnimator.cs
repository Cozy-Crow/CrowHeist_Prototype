using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SHG.AnimatorCoder;
using KinematicCharacterController.Examples;

public class CrowleyAnimator : AnimatorCoder
{
    [SerializeField] private Controller2Point5D controller;
    private AnimationData WALK = new AnimationData("Walk");
    private AnimationData IDLE = new AnimationData("Idle");
    private AnimationData JUMP = new AnimationData("Jump", true, new AnimationData(), 0.2f);
    private AnimationData THROW = new AnimationData("Throw", true, new AnimationData(), 0.5f);

    void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        controller = GetComponentInParent<Controller2Point5D>();
    }

    void Update()
    {
        DefaultAnimation(0);
    }

    public override void DefaultAnimation(int layer)
    {
            animator.SetFloat("MoveX", controller.FaceDirection.x);
            animator.SetFloat("MoveZ", controller.FaceDirection.z);

            if (controller.IsThrowing)
            {
                Play(THROW);
                return;
            }

            if (controller.Velocity.magnitude < 0.1f && controller.IsGrounded)
            {
                Play(IDLE);
                return;
            }

            if (controller.Velocity.y > 0.1f || !controller.IsGrounded)
            {
                Play(JUMP);
            }
            else if (controller.Velocity.magnitude > 0.1f)
            {
                Play(WALK);
            }
    }

}
