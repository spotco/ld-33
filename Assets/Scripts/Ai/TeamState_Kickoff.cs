using UnityEngine;

public class TeamState_Kickoff : FSMState<TeamBase> {
	
	static readonly TeamState_Kickoff instance = new TeamState_Kickoff();
	public static TeamState_Kickoff Instance {
		get {
			return instance;
		}
	}
	static TeamState_Kickoff() { }
	
	private TeamState_Kickoff() { }
	
	public override void Enter (TeamBase team) {
		team.SendPlayersHome();
	}
	
	public override void Execute (TeamBase team) {
		if (team.AreAllPlayersHome()) {
			team.ChangeState(TeamState_Defend.Instance);
		}
	}
	
	public override void Exit(TeamBase team) {
	}
}