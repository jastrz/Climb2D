using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleGround : PlayerState {

    private float jumpForce = 15.0f;
    private float walkSpeed = 3.0f;

    public override string GetName()
    {
        return "IdleGround";
    }

    public override PlayerState HandleInput()
    {
        this.HandleMovementInput();

        if(this.HandleJump())
        {
            return StateMachine.PlayerStates.IdleAir;
        }

        return base.HandleInput();
    }

    public override PlayerState Update()
    {
        return base.Update();
    }


    public override void OnStateEnter()
    {
        this.EnableHandsGroundState();
        base.OnStateEnter();
    }

    public override void OnStateExit()
    {
        base.OnStateExit();
    }

    private void EnableHandsGroundState()
    {
        TargetController.Body.transform.rotation = Quaternion.identity;
        foreach (GameObject hand in TargetController.Hands.Values)
        {
            Hand handController = hand.GetComponent<Hand>();
            handController.SetAsIdle(TargetController.Body.transform);
        }
    }

    private void HandleMovementInput()
    {
        float horizontalMovement = Input.GetAxis("Horizontal");
        Rigidbody2D rigidbody = TargetController.Body.GetComponent<Rigidbody2D>();

        if (Mathf.Abs(horizontalMovement) > 0)
        {
            rigidbody.velocity = new Vector3(horizontalMovement * walkSpeed, 0, 0);
        }
        else
        {
            rigidbody.velocity = Vector2.zero;
        }
    }

    private bool HandleJump()
    {
        if(Input.GetKey(KeyCode.Q) && Input.GetKey(KeyCode.W))
        {
            var ForceToAdd = Vector2.up * jumpForce;
            TargetController.Body.GetComponent<Rigidbody2D>().AddForce(ForceToAdd, ForceMode2D.Impulse);
            return true;
        }
        return false;
    }
}
