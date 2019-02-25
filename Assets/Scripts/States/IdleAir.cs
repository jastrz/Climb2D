using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public override PlayerState HandleInput()
    {
        return base.HandleInput();
    }

    public override void OnStateExit()
    {
        base.OnStateExit();
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
            // StateMachine.PlayerStates.Rotating.rotatingHand.BodyJointSetEnabled(false);
            // StateMachine.PlayerStates.Rotating.nonRotatingHand.SetAsKinematic();
            // StateMachine.PlayerStates.Rotating.nonRotatingHand.BodyJointSetEnabled(false);
        }
        else if(previousState == StateMachine.PlayerStates.IdleGround || previousState == null)
        {
            this.ActivateBothHands();
        }
        else if(previousState == StateMachine.PlayerStates.JumpReady)
        {
            this.ActivateBothHands();
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
        //foreach(GameObject hand in TargetController.Hands.Values)
        //{
        //    Hand handController = hand.GetComponent<Hand>();
        //    handController.SetAsKinematic();
        //    handController.BodyJointSetEnabled(false);
        //}\
        TargetController.LeftHand.GetComponent<Hand>().SetAsKinematic();
        TargetController.LeftHand.GetComponent<Hand>().BodyJointSetEnabled(false);
        handsLocked = true;
    }

    private void UpdateLockedHandsPositions()
    {
        foreach(GameObject hand in TargetController.Hands.Values)
        {
            hand.transform.localPosition += TargetController.Body.GetComponent<Body>().localPositionDelta;
        }
        // StateMachine.PlayerStates.Rotating.rotatingHand.transform.localPosition +=
        //         TargetController.Body.GetComponent<Body>().localPositionDelta;

        //     StateMachine.PlayerStates.Rotating.nonRotatingHand.transform.localPosition +=
        //         TargetController.Body.GetComponent<Body>().localPositionDelta;
    }

}
