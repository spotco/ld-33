using UnityEngine;

/**
 * Responsible for controlling the AI of a single bot.
 */
[RequireComponent (typeof (Steering))]
public class BotBase : MonoBehaviour {
	private FiniteStateMachine<BotBase> FSM;
	private Steering _steering;
	private TeamBase _team;

	// TODO:
	public Transform topPost;
	public Transform bottomPost;
	
	public TeamBase Team {
		get {
			return _team;
		}
	}
	
	public Steering Steering {
		get { return _steering; }
	}
	
	public void ChangeState(FSMState<BotBase> e) {
		FSM.ChangeState(e);		
	}
	
	public void GetGoalpostPositions(out Vector3 top, out Vector3 bottom) {
		top = topPost.position;
		bottom = bottomPost.position;
	}
	
	public Vector3 GetBallPosition() {
		return GameObject.Find("Mover").transform.position;
	}
	
	public void Awake() {
		FSM = new FiniteStateMachine<BotBase>();
		_steering = GetComponent<Steering>();
		_team = Uzu.Util.FindInParents<TeamBase>(this.gameObject);
		
		// FSM.Configure(this, BotState_TendGoal.Instance);
	}
 
	public void Update() {
		FSM.Update();
	}
}


