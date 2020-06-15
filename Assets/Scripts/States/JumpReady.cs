using System.Collections;
using UnityEngine;

public class JumpReady : PlayerState 
{
    private delegate PlayerState TimeWindowResponseDelegate();
    private TimeWindowResponseDelegate TimeWindowResponse = null;

    private Coroutine TimeWindowForInput = null;
    private float jumpForce = 1000.0f;
    private float timeWindowPeriod = 0.1f;

    public override string GetName()
    {
        return "JumpReady";
    }

    public override void OnStateEnter()
    {
        this.TimeWindowForInput = null;
        this.TimeWindowResponse = null;
        this.EnableHandsJumpReadyState();
        base.OnStateEnter();
    }

    public override PlayerState Update()
    {
        if(TimeWindowResponse != null)
        {
            return TimeWindowResponse.Invoke();
        }

        return base.Update();
    }

    public override PlayerState HandleInput()
    {
        if(TargetController.HandleHandGrabInputs() < 2 && TimeWindowForInput == null)
        {
            TimeWindowForInput = TargetController.StartCoroutine(WaitForInput(timeWindowPeriod));
        }

        return this;
    }

    public void EnableHandsJumpReadyState()
    {
        foreach (GameObject hand in TargetController.Hands.Values)
        {
            Hand handController = hand.GetComponent<Hand>();
            handController.SetAsActive(Target.transform);
        }
    }

    private void JumpUp()
    {
        var bodyRB = TargetController.Body.GetComponent<Rigidbody2D>();
        bodyRB.velocity = Vector2.zero;
        bodyRB.AddForce(Vector2.up * jumpForce, ForceMode2D.Force);
    }

    private IEnumerator WaitForInput(float time)
    {
        yield return new WaitForSeconds(time);
        if (TargetController.HandleHandGrabInputs() == 0)
        {
            yield return new WaitForFixedUpdate();
            this.JumpUp();
            this.TimeWindowResponse = () => { return StateMachine.PlayerStates.IdleAir; };
            yield break;
        }
        else if(TargetController.HandleHandGrabInputs() == 1)
        {
            this.TimeWindowResponse = () => { return StateMachine.PlayerStates.Rotating; };
            yield break;
        }
        else
        {
            TimeWindowResponse = () => { return this; };
        }
    }
}
