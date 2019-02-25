using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpReady : PlayerState {

    private float jumpForce = 15.0f;
    public override string GetName()
    {
        return "JumpReady";
    }

    public override void OnStateEnter()
    {
        this.EnableHandsJumpReadyState();
        base.OnStateEnter();
    }

    public override PlayerState Update()
    {
        //if (TargetController.HandleHandGrabInputs() == 1)
        //{
        //    return StateMachine.PlayerStates.Handling;
        //}

        return base.Update();
    }

    public override PlayerState HandleInput()
    {
        //if (Input.GetKeyUp(KeyCode.Q) && Input.GetKeyUp(KeyCode.W))
        //{     
        //    TargetController.Body.GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        //    return StateMachine.PlayerStates.IdleAir;
        //}

        //if (Input.GetKeyUp(KeyCode.Q) || Input.GetKeyUp(KeyCode.W))
        //{
        //    return StateMachine.PlayerStates.Handling;
        //}


        return HandleDelayedInput(.2f);
        

        //return base.HandleInput();
    }

    public void EnableHandsJumpReadyState()
    {
        foreach (GameObject hand in TargetController.Hands.Values)
        {
            Hand handController = hand.GetComponent<Hand>();
            handController.SetAsActive(Target.transform);
        }
    }

    private PlayerState HandleDelayedInput(float delayTime)
    {
        float time = 0.0f;
        PlayerState stateToReturn = this;

        while(time <= delayTime)
        {
            int numGrabbingHands = TargetController.HandleHandGrabInputs();

            if (numGrabbingHands == 0)
            {
                stateToReturn = StateMachine.PlayerStates.IdleAir;
            }

            if (numGrabbingHands == 1)
            {
                stateToReturn = StateMachine.PlayerStates.Handling;
            }

            time += Time.deltaTime;
        }

        if(stateToReturn == StateMachine.PlayerStates.IdleAir)
        {
            JumpUp();
        }

        return stateToReturn;
    }

    private void JumpUp()
    {
        TargetController.Body.GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
}
