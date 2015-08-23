using UnityEngine;

/**
 * Responsible for controlling the AI of a single bot.
 */
[RequireComponent (typeof (Steering))]
public class BotBase : MonoBehaviour {
	private FiniteStateMachine<BotBase> _FSM;
	private Steering _steering;
	
	public FiniteStateMachine<BotBase> DBG_FSM {
		get { return _FSM; }
	}
	
	public TeamBase Team {
		get; set;
	}
	
	public FieldPosition FieldPosition {
		get; set;
	}
	
	public Steering Steering {
		get { return _steering; }
	}
	
	public void ChangeState(FSMState<BotBase> e) {
		_FSM.ChangeState(e);		
	}
	
	public Vector3 HomePosition {
		get; set;
	}
	
	public void GoToRegion(int region) {
		HomePosition = Main.FieldController.GetRegionPosition(region);
		ChangeState(BotState_GoHome.Instance);
	}
	
	public bool IsAtHomePosition() {
		return Vector3.Distance(HomePosition, this.transform.localPosition) <= AiConstants.ArriveDistance;
	}
	
	public void GetGoalpostPositions(out Vector3 top, out Vector3 bottom) {
		Main.FieldController.GetRightGoalLinePositions(out top, out bottom);
	}
	
	public Vector3 GetBallPosition() {
		// TODO: 
		// return Main.LevelController.m_looseBalls[0].transform.position;
		return Main.LevelController.m_playerTeamFootballers[0].transform.position;
	}
	
	public void Awake() {
		_FSM = new FiniteStateMachine<BotBase>();
		_steering = GetComponent<Steering>();
		_FSM.Configure(this, BotState_Idle.Instance);
	}
 
	public void Update() {
		_FSM.Update();
	}
}

public enum FieldPosition {
	Keeper,
	Defense,
}