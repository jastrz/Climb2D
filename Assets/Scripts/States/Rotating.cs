using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotating : PlayerState
{
    public Hand rotatingHand;
    public Hand nonRotatingHand;

    private Trajectory trajectory;
    private Vector3 forceToAdd;
    private readonly float rotationJumpForceConstant = 15.0f;

    public override string GetName()
    {
        return "Rotating";
    }

    public override void OnStateEnter()
    {
        trajectory = Target.GetComponent<Trajectory>();
        rotatingHand = GetHand(false);
        nonRotatingHand = GetHand(true);

        if(StateMachine.previousState == StateMachine.PlayerStates.DoubleGrab)
        {
            rotatingHand.StartRotating(nonRotatingHand.transform, true);            
        }
        else
        {
            rotatingHand.StartRotating(nonRotatingHand.transform, false);
        }

        base.OnStateEnter();
    }

    public override void OnStateExit()
    {
        trajectory.DisableTrajectory();
        base.OnStateExit();
    }

    public override PlayerState Update()
    {
        forceToAdd = (rotatingHand.transform.position - TargetController.Body.transform.position).normalized * rotationJumpForceConstant;
        var arrowForce = (rotatingHand.transform.position - nonRotatingHand.transform.position).normalized * rotationJumpForceConstant;
        trajectory.DrawTrajectory(rotatingHand.transform.position, arrowForce);

        return base.Update();
    }

    public override PlayerState HandleInput()
    {
        if (TargetController.HandleHandGrabInputs() == 0)
        {
            Jump(forceToAdd);
            return StateMachine.PlayerStates.IdleAir;
        }

        if (Input.GetKey(KeyCode.Q) && Input.GetKey(KeyCode.W))
        {
            if (BothHandsGrabbing())
            {
                if(BothHandsGrabbingSameObject())
                {
                    return StateMachine.PlayerStates.JumpReady;
                }
                else
                {
                    return StateMachine.PlayerStates.DoubleGrab;
                }
            }
            else
            {
                rotatingHand.Attach(nonRotatingHand.GrabbedObject);
                return StateMachine.PlayerStates.JumpReady;
            }
        }

        return base.HandleInput();
    }

    private void Jump(Vector3 forceToAdd)
    {
        TargetController.Body.GetComponent<Body>().Jump(forceToAdd);
    }

    private Hand GetRotatingHand()
    {
        foreach(GameObject hand in TargetController.Hands.Values)
        {
            var handCtrl = hand.GetComponent<Hand>();
            if (handCtrl.isGrabbing == false)
            {
                return handCtrl;
            }
        }
        return null;
    }

    private bool BothHandsGrabbing()
    {
        foreach (GameObject hand in TargetController.Hands.Values)
        {
            var handCtrl = hand.GetComponent<Hand>();
            if (handCtrl.isGrabbing == false)
            {
                return false;
            }
        }
        return true;
    }

    private Hand GetNonRotatingHand()
    {
        foreach (GameObject hand in TargetController.Hands.Values)
        {
            var handCtrl = hand.GetComponent<Hand>();
            if (handCtrl.isGrabbing == true)
            {
                return handCtrl;
            }
        }
        return null;
    }

    private Hand GetHand(bool grabbing)
    {
        foreach (GameObject hand in TargetController.Hands.Values)
        {
            var handCtrl = hand.GetComponent<Hand>();
            if (handCtrl.isGrabbing == grabbing)
            {
                return handCtrl;
            }
        }
        return null;
    }

    private bool BothHandsGrabbingSameObject()
    {
        return TargetController.LeftHand.GetComponent<Hand>().GrabbedObject == TargetController.RightHand.GetComponent<Hand>().GrabbedObject;
    }
}
