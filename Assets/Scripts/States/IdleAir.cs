﻿using UnityEngine;

public class IdleAir : PlayerState {

    private bool handsLocked = false;

    public override string GetName()
    {
        return "IdleAir";
    }

    public override void OnStateEnter()
    {
        this.EnableHandsAirState(StateMachine.previousState);
        base.OnStateEnter();
    }

    public override PlayerState Update()
    {
        TargetController.HandleHandGrabInputs();
        
        if(isAnyHandGrabbing())
        {
            return StateMachine.PlayerStates.Handling;
        }

        if(handsLocked)
        {
            this.UpdateLockedHandsPositions();
        }

        return base.Update();
    }

    private bool isAnyHandGrabbing()
    {
        foreach(GameObject hand in TargetController.Hands.Values)
        {
            if (hand.GetComponent<Hand>().isGrabbing == true)
                return true;
        }
        return false;
    }

    public void EnableHandsAirState(PlayerState previousState)
    {
        if(previousState == StateMachine.PlayerStates.Rotating)
        {
            this.LockHands();
        }
        else if(previousState == StateMachine.PlayerStates.IdleGround || previousState == null)
        {
            this.ActivateBothHands();
        }
        else if(previousState == StateMachine.PlayerStates.JumpReady)
        {
            this.LockHands();
        }
        else
        {
            Debug.LogError("Wrong state transition");
        }
    }

    private void ActivateBothHands()
    {
        foreach (GameObject hand in TargetController.Hands.Values)
        {
            Hand handController = hand.GetComponent<Hand>();
            handController.SetAsActive(Target.transform);
            hand.GetComponent<Rigidbody2D>().velocity = TargetController.Body.GetComponent<Rigidbody2D>().velocity;
        }
        handsLocked = false;
    }

    private void LockHands()
    {
        foreach(GameObject hand in TargetController.Hands.Values)
        {
            Hand handController = hand.GetComponent<Hand>();
            handController.SetAsKinematic();
            handController.BodyJointSetEnabled(false);
        }
        handsLocked = true;
    }

    private void UpdateLockedHandsPositions()
    {
        foreach(GameObject hand in TargetController.Hands.Values)
        {
            hand.transform.localPosition += TargetController.Body.GetComponent<Body>().localPositionDelta;
        }
    }

}
