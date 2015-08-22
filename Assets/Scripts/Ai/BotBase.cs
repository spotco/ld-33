using UnityEngine;

public class BotBase : MonoBehaviour {
	private FiniteStateMachine<BotBase> FSM;
	private Steering _steering;
	
	[SerializeField]
	private float _maxVelocity = 1.0f;
	[SerializeField]
	private float _mass = 10.0f;
	[SerializeField]
	private float _friction = 0.05f;
	
	public float MaxVelocity {
		get {
			return _maxVelocity;
		}
	}
	public float Mass {
		get {
			return _mass;
		}
	}
	public float Friction {
		get {
			return _friction;
		}
	}
	
	public Steering Steering {
		get { return _steering; }
	}
	
	public void ChangeState(FSMState<BotBase> e) {
		FSM.ChangeState(e);		
	}
	
	public void Awake() {
		FSM = new FiniteStateMachine<BotBase>();
		_steering = new Steering();
		
		FSM.Configure(this, State_TendGoal.Instance);
	}
 
	public void Update() {
		FSM.Update();
		Steering.Update(this);
	}
}


