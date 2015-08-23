using UnityEngine;

public static class BotConstants {
	public const float ArriveDistance = 10.0f;
	public const float KeeperFetchDistance = 400.0f;
}

/**
 * 
 */
public class BotState_TendGoal : FSMState<BotBase> {
	
	static readonly BotState_TendGoal instance = new BotState_TendGoal();
	public static BotState_TendGoal Instance {
		get {
			return instance;
		}
	}
	static BotState_TendGoal() { }
	
	private BotState_TendGoal() { }
	
	public override void Enter (BotBase bot) {
		bot.Steering.ArriveOn();
	}
	
	public override void Execute (BotBase bot) {
		{
			Vector3 targetPos;
			Vector3 topPost, bottomPost;
			bot.GetGoalpostPositions(out topPost, out bottomPost);
			Uzu.Math.ClosestPtPointSegment(
				topPost, bottomPost,
				bot.GetBallPosition(),
				out targetPos
			);
			bot.Steering.CurrentTarget = targetPos;
		}
		
		BotBase ballOwner = bot.GetBallOwner();
		
		// We have the ball - throw it back into play.
		if (ballOwner == bot) {
			bot.ChangeState(BotState_PutBallBackInPlay.Instance);
			return;
		}
		
		// Ball is loose and close to us - go get it.
		float ballDistance = Vector3.Distance(bot.GetBallPosition(), bot.transform.position);
		if (ballOwner == null && ballDistance <= BotConstants.KeeperFetchDistance) {
			bot.ChangeState(BotState_Intercept.Instance);
			return;
		}
	}
	
	public override void Exit(BotBase bot) {
		bot.Steering.ArriveOff();
	}
}

/**
 * 
 */
public class BotState_Intercept : FSMState<BotBase> {
	
	static readonly BotState_Intercept instance = new BotState_Intercept();
	public static BotState_Intercept Instance {
		get {
			return instance;
		}
	}
	static BotState_Intercept() { }
	
	private BotState_Intercept() { }
	
	public override void Enter (BotBase bot) {
		bot.Steering.PursuitOn();
	}
	
	public override void Execute (BotBase bot) {
		if (!bot.IsBallLoose()) {
			bot.ChangeState(BotState_GoHome.Instance);
			return;
		}
		
		if (bot.GetDistanceFromGoal() >= BotConstants.KeeperFetchDistance) {
			Debug.Log("Too far from goal - go home.");
			bot.ChangeState(BotState_GoHome.Instance);
			return;
		}
		
		bot.Steering.CurrentTarget = bot.GetBallPosition();
		bot.Steering.CurrentEvaderVelocity = bot.GetBallVelocity();
	}
	
	public override void Exit(BotBase bot) {
		bot.Steering.PursuitOff();
	}
}

public class BotState_PutBallBackInPlay : FSMState<BotBase> {
	static readonly BotState_PutBallBackInPlay instance = new BotState_PutBallBackInPlay();
	public static BotState_PutBallBackInPlay Instance {
		get {
			return instance;
		}
	}
	static BotState_PutBallBackInPlay() { }
	
	private BotState_PutBallBackInPlay() { }
	
	public override void Enter (BotBase bot) {
		
	}
	
	public override void Execute (BotBase bot) {
		
	}
	
	public override void Exit(BotBase bot) {
		
	}
}

/**
 * 
 */
public class BotState_GoHome : FSMState<BotBase> {
	
	static readonly BotState_GoHome instance = new BotState_GoHome();
	public static BotState_GoHome Instance {
		get {
			return instance;
		}
	}
	static BotState_GoHome() { }
	
	private BotState_GoHome() { }
	
	public override void Enter (BotBase bot) {
		bot.Steering.ArriveOn();
		bot.Steering.CurrentTarget = bot.HomePosition;
	}
	
	public override void Execute (BotBase bot) {
		if (Vector3.Distance(bot.HomePosition, bot.transform.localPosition) <= BotConstants.ArriveDistance) {
			if (bot.FieldPosition == FieldPosition.Keeper) {
				bot.ChangeState(BotState_TendGoal.Instance);
			} else {
				bot.ChangeState(BotState_Idle.Instance);
			}
		}
	}
	
	public override void Exit(BotBase bot) {
		bot.Steering.ArriveOff();
	}
}

/**
 * 
 */
public class BotState_Idle : FSMState<BotBase> {
	static readonly BotState_Idle instance = new BotState_Idle();
	public static BotState_Idle Instance {
		get {
			return instance;
		}
	}
	static BotState_Idle() { }
	
	private BotState_Idle() { }
	
	public override void Enter (BotBase bot) {
		
	}
	
	public override void Execute (BotBase bot) {
		
	}
	
	public override void Exit(BotBase bot) {
		
	}
}