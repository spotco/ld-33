using UnityEngine;

public class BotBase : MonoBehaviour {
	private FiniteStateMachine<BotBase> FSM;
	private Steering _steering;
	
	public Steering Steering {
		get { return _steering; }
	}
	
	public void ChangeState(FSMState<BotBase> e) {
		FSM.ChangeState(e);		
	}
	
	public void Awake() {
		FSM = new FiniteStateMachine<BotBase>();
		_steering = GetComponent<Steering>();
		
		FSM.Configure(this, State_TendGoal.Instance);
	}
 
	public void Update() {
		FSM.Update();
	}
}


