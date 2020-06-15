public class DoubleGrab : PlayerState {

    public override string GetName()
    {
        return "Double Grab";
    }

    public override PlayerState Update()
    {
        if(TargetController.HandleHandGrabInputs() < 2)
        {
            return StateMachine.PlayerStates.Rotating;
        }

        return base.Update();
    }
}
