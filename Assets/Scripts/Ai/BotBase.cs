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
		return Vector3.Distance(HomePosition, this.transform.localPosition) <= BotConstants.ArriveDistance;
	}
	
	public float GetDistanceFromGoal() {
		Vector3 top, bottom;
		GetGoalpostPositions(out top, out bottom);
		Vector3 center = (top + bottom) * 0.5f;
		return Vector3.Distance(center, this.transform.position);
	}
	
	public void GetGoalpostPositions(out Vector3 top, out Vector3 bottom) {
		Main.FieldController.GetRightGoalLinePositions(out top, out bottom);
	}
	
	public Vector3 GetBallPosition() {
		return Main.LevelController.currentBallPosition();
	}
	
	public Vector3 GetBallVelocity() {
		return Main.LevelController.currentLooseBallVelocity();
	}
	
	public bool IsBallLoose() {
		return GetBallOwner() == null;
	}
	
	public BotBase GetBallOwner() {
		GenericFootballer footballer = Main.LevelController.nullableCurrentFootballerWithBall();
		if (footballer != null) {
			return footballer.GetComponent<BotBase>();
		}
		return null;
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