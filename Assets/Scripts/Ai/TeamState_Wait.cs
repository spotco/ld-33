using UnityEngine;

public class TeamState_Wait : FSMState<TeamBase> {
	
	static readonly TeamState_Wait instance = new TeamState_Wait();
	public static TeamState_Wait Instance {
		get {
			return instance;
		}
	}
	static TeamState_Wait() { }
	
	private TeamState_Wait() { }
	
	public override void Enter (TeamBase team) {
	}
	
	public override void Execute (TeamBase team) {
	}
	
	public override void Exit(TeamBase team) {
	}
}