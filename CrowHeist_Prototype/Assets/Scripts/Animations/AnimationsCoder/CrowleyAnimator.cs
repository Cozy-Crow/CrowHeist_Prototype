using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SHG.AnimatorCoder;
using KinematicCharacterController.Examples;

public class CrowleyAnimator : AnimatorCoder
{
    [SerializeField] private Controller2Point5D controller;
    private AnimationData IDLE = new AnimationData("Idle");
    private AnimationData IDLEHOLD = new AnimationData("IdleHold");
    private AnimationData IDLECHARGE = new AnimationData("IdleCharge");
    private AnimationData WALK = new AnimationData("Walk");
    private AnimationData WALKHOLD = new AnimationData("WalkHold");
    private AnimationData WALKCHARGE = new AnimationData("WalkCharge");
    
    private AnimationData JUMP = new AnimationData("Jump", true, new AnimationData());
    private AnimationData AIRHOLD = new AnimationData("AirHold");
    private AnimationData AIRCHARGE = new AnimationData("AirCharge");

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
        
        if (controller.IsThrowing)
        {
            Play(THROW);
            controller.IsThrowing = false;
            return;
        }

        if (controller.ChargeThrowing)
        {
            if (!controller.IsGrounded) { Play(AIRCHARGE); return; }
            if (!controller.IsMoving && controller.IsGrounded) { Play(IDLECHARGE); return;}
            if (controller.IsMoving) { Play(WALKCHARGE); return;}
        }

        if (controller.IsHoldingItem)
        {
            if (!controller.IsGrounded) { Play(AIRHOLD); return; }
            if (!controller.IsMoving && controller.IsGrounded) { Play(IDLEHOLD); return;}
            if (controller.IsMoving) { Play(WALKHOLD); return;}
        }

        if (controller.IsMoving && !controller.IsGrounded)
        {
            Play(JUMP);
            return;
        }

        if (!controller.IsMoving && controller.IsGrounded)
        {
            Play(IDLE);
        }
        else if (controller.IsMoving)
        {
            Play(WALK);
        }
    }

}
