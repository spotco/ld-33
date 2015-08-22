using UnityEngine;

public class TeamState_Defend : FSMState<TeamBase> {
	
	static readonly TeamState_Defend instance = new TeamState_Defend();
	public static TeamState_Defend Instance {
		get {
			return instance;
		}
	}
	static TeamState_Defend() { }
	
	private TeamState_Defend() { }
	
	public override void Enter (TeamBase bot) {
		// TODO: send players to correct spots
	}
	
	public override void Execute (TeamBase bot) {
	}
	
	public override void Exit(TeamBase bot) {
	}
}