using UnityEngine;

/**
 * Responsible for controlling members of a team, and dictating overall
 * team strategy (defense, offense, etc).
 */
public class TeamBase : MonoBehaviour {
	private FiniteStateMachine<TeamBase> FSM;
	
	private BotBase _keeper;
	
	public Team Team {
		get; set;
	}
	
	public void SetPlayers(BotBase keeper) {
		_keeper = keeper;
		_keeper.Team = this;
	}
	
	public void StartKickoff() {
		ChangeState(TeamState_Kickoff.Instance);
	}
	
	public bool AreAllPlayersHome() {
		return _keeper.IsAtHomePosition();
	}
	
	public void SendPlayersHome() {
		_keeper.GoToRegion(16);
	}
	
	public void ChangeState(FSMState<TeamBase> s) {
		FSM.ChangeState(s);
	}
		
	public void Awake() {
		FSM = new FiniteStateMachine<TeamBase>();
		FSM.Configure(this, TeamState_Wait.Instance);
	}
 
	public void Update() {
		FSM.Update();
	}
}


