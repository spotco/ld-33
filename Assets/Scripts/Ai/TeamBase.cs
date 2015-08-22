using UnityEngine;

/**
 * Responsible for controlling members of a team, and dictating overall
 * team strategy (defense, offense, etc).
 */
public class TeamBase : MonoBehaviour {
	private FiniteStateMachine<TeamBase> FSM;
	
	[SerializeField]
	private BotBase _keeper;
	[SerializeField]
	private BotBase _fielder;
	
	public void SendPlayersHome() {
		
	}
	
	public void ChangeState(FSMState<TeamBase> s) {
		FSM.ChangeState(s);
	}
		
	public void Awake() {
		FSM = new FiniteStateMachine<TeamBase>();
	}
 
	public void Update() {
		FSM.Update();
	}
}


