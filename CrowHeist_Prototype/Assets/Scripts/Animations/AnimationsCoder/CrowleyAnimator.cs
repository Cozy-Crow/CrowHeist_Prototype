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
    private AnimationData JUMP = new AnimationData("Jump", true, new AnimationData());
    private AnimationData THROWREADY = new AnimationData("ThrowReady");
    private AnimationData CHARGETHROW = new AnimationData("ChargeThrow");
    private AnimationData THROW = new AnimationData("Throw", true, new AnimationData());

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

            if (controller.ChargeThrowing)
            {
                Play(CHARGETHROW);
                return;
            }

            if (controller.IsThrowing)
            {
                Play(THROW);
                controller.IsThrowing = false;
                return;
            }

            if (controller.Velocity.y > 0.1f || !controller.IsGrounded)
            {
                Play(JUMP);
                return;
            }

            if (controller.Velocity.magnitude < 0.1f && controller.IsGrounded)
            {
                Play(IDLE);
            }
            else if (controller.Velocity.magnitude > 0.1f)
            {
                Play(WALK);
            }
    }

}
