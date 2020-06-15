using UnityEngine;

public class StateMachine : MonoBehaviour 
{
    public string currentStateName;
    public static PlayerStates PlayerStates = new PlayerStates();

    private GameObject Target;
    private PlayerState nextState;

    private static PlayerState _previousState;
    public static PlayerState previousState
    {
        get { return _previousState; }
        private set
        {
            _previousState = value;
        }
    }

    private PlayerState _currentState;
    public PlayerState currentState
    {
        get { return _currentState; }
        private set
        {
            _currentState = value;
            currentStateName = _currentState.GetName();
        }
    }

    private void Start ()
    {
        this.Target = transform.gameObject;
        PlayerState.Target = this.transform.gameObject;
        this.currentState = PlayerStates.IdleAir;
        this.currentState.OnStateEnter();
    }
	
	private void FixedUpdate ()
    {
        nextState = currentState.Update();
        if(nextState != currentState)
        {
            this.SwitchStates(nextState);
        }
	}

    public void SwitchStates(PlayerState state)
    {
        if(state != currentState)
        {
            currentState.OnStateExit();
            previousState = currentState;
            currentState = state;
            currentState.OnStateEnter();
        }
    }
}

public class PlayerStates
{
    public IdleGround IdleGround = new IdleGround();
    public IdleAir IdleAir = new IdleAir();
    public Handling Handling = new Handling();
    public Dead Dead = new Dead();
    public Rotating Rotating = new Rotating();
    public JumpReady JumpReady = new JumpReady();
    public DoubleGrab DoubleGrab = new DoubleGrab();
}