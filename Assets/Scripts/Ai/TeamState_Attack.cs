using UnityEngine;

public class TeamState_Attack : FSMState<TeamBase> {
	
	static readonly TeamState_Attack instance = new TeamState_Attack();
	public static TeamState_Attack Instance {
		get {
			return instance;
		}
	}
	static TeamState_Attack() { }
	
	private TeamState_Attack() { }
	
	public override void Enter (TeamBase bot) {
		
	}
	
	public override void Execute (TeamBase bot) {
		// TODO: if we lose the ball, transition to defense
	}
	
	public override void Exit(TeamBase bot) {
	}
}