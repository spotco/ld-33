using UnityEngine;

/**
 * Responsible for controlling members of a team, and dictating overall
 * team strategy (defense, offense, etc).
 */
public class TeamBase : MonoBehaviour {
	private FiniteStateMachine<TeamBase> _FSM;
	
	private BotBase _keeper;
	private BotBase _defense0;
	private BotBase _defense1;
	
	public FiniteStateMachine<TeamBase> DBG_FSM {
		get { return _FSM; }
	}
	
	public Team Team {
		get; set;
	}
	
	public void SetPlayers(BotBase keeper, BotBase d0, BotBase d1) {
		_keeper = keeper;
		_defense0 = d0;
		_defense1 = d1;
		
		_keeper.FieldPosition = FieldPosition.Keeper;
		_defense0.FieldPosition = FieldPosition.Defense;
		_defense1.FieldPosition = FieldPosition.Defense;
		
		_keeper.Team = this;
		_defense0.Team = this;
		_defense1.Team = this;
	}
	
	public void StartKickoff() {
		ChangeState(TeamState_Kickoff.Instance);
	}
	
	public bool AreAllPlayersHome() {
		return _keeper.IsAtHomePosition() &&
			_defense0.IsAtHomePosition() &&
			_defense1.IsAtHomePosition();
	}
	
	public void SendPlayersHome() {
		// TODO: depending on team
		_keeper.GoToRegion(16);
		_defense0.GoToRegion(12);
		_defense1.GoToRegion(14);
	}
	
	public void ChangeState(FSMState<TeamBase> s) {
		_FSM.ChangeState(s);
	}
		
	public void Awake() {
		_FSM = new FiniteStateMachine<TeamBase>();
		_FSM.Configure(this, TeamState_Wait.Instance);
	}
 
	public void Update() {
		_FSM.Update();
	}
}


