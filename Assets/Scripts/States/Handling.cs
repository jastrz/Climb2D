using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handling : PlayerState
{
    public override string GetName()
    {
        return "Handling";
    }

    public override void OnStateEnter()
    {
        this.DisableHandSpringCollisions();
        base.OnStateEnter();
    }

    public override void OnStateExit()
    {
        base.OnStateExit();
    }

    public override PlayerState HandleInput()
    {
        var numHandsGrabbing = TargetController.HandleHandGrabInputs();

        if (numHandsGrabbing == 1)
        {
            return StateMachine.PlayerStates.Rotating;
        }
        if (numHandsGrabbing == 2)
        {
            return StateMachine.PlayerStates.JumpReady;
        }
        return base.HandleInput();
    }

    private void DisableHandSpringCollisions()
    {
        foreach (GameObject hand in TargetController.Hands.Values)
        {
            Hand handController = hand.GetComponent<Hand>();
            handController.BodyJointSetEnabled(true);
            handController.DisableJointCollisions();
        }
    }
}


