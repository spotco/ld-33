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

/**
 * 
 */
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
		
	}
	
	public override void Execute (TeamBase bot) {
		// TODO: if we have the ball, transition to attack
	}
	
	public override void Exit(TeamBase bot) {
	}
}

/**
 * 
 */
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

/**
 * 
 */
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