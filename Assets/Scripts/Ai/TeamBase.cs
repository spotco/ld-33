using UnityEngine;
using System.Collections.Generic;

/**
 * Responsible for controlling members of a team, and dictating overall
 * team strategy (defense, offense, etc).
 */
public class TeamBase : MonoBehaviour {
	private FiniteStateMachine<TeamBase> _FSM;
	
	private BotBase _keeper;
	private BotBase _defense0;
	private BotBase _defense1;
	private BotBase _forward0;
	private BotBase _forward1;
	
	private List<BotBase> _teamMembers = new List<BotBase>();
	
	public FiniteStateMachine<TeamBase> DBG_FSM {
		get { return _FSM; }
	}
	
	public Team Team {
		get; set;
	}
	public Team OtherTeam {
		get {
			return Team == Team.PlayerTeam ? Team.EnemyTeam : Team.PlayerTeam;
		}
	}
	
	public List<BotBase> TeamMembers {
		get { return _teamMembers; }
	}
	
	public List<BotBase> GetTeamExcept(BotBase exceptBot) {
		List<BotBase> members = new List<BotBase>();
		for (int i = 0; i < _teamMembers.Count; i++) {
			if (exceptBot != _teamMembers[i]) {
				members.Add(_teamMembers[i]);
			}
		}
		return members;
	}
	
	public Team GetBallTeamOwner() {
		BotBase owner = GetBallOwner();
		if (owner != null) {
			return owner.Team;
		}
		return Team.None;
	}
	
	public BotBase GetBallOwner() {
		GenericFootballer footballer = Main.LevelController.nullableCurrentFootballerWithBall();
		if (footballer != null) {
			return footballer.GetComponent<BotBase>();
		}
		return null;
	}
	
	public void SetPlayers(BotBase keeper, BotBase d0, BotBase d1, BotBase f0, BotBase f1) {
		_keeper = keeper;
		_defense0 = d0;
		_defense1 = d1;
		_forward0 = f0;
		_forward1 = f1;
		
		_keeper.FieldPosition = FieldPosition.Keeper;
		_defense0.FieldPosition = FieldPosition.Defender;
		_defense1.FieldPosition = FieldPosition.Defender;
		_forward0.FieldPosition = FieldPosition.Attacker;
		_forward1.FieldPosition = FieldPosition.Attacker;
		
		_teamMembers.Clear();
		_teamMembers.Add(_keeper);
		_teamMembers.Add(_defense0);
		_teamMembers.Add(_defense1);
		_teamMembers.Add(_forward0);
		_teamMembers.Add(_forward1);
		
		foreach (BotBase bot in _teamMembers) {
			bot.TeamBase = this;
		}
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
		_forward0.GoToRegion(9);
		_forward1.GoToRegion(11);
	}
	
	public void ChangeState(FSMState<TeamBase> s) {
		_FSM.ChangeState(s);
	}
		
	public void Awake() {
		_FSM = new FiniteStateMachine<TeamBase>();
		_FSM.Configure(this, TeamState_Wait.Instance);
	}
 
	public void Update() {
		if (Main.IsPaused(PauseFlags.TimeOut | PauseFlags.Ai)) {
			return;
		}
		
		_FSM.Update();
	}
}
