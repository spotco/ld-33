using UnityEngine;

public static class BotConstants {
	public const float ArriveDistance = 10.0f;
	public const float KeeperFetchDistance = 400.0f;
	public const float DefenderChaseDistance = 500.0f;
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
			bot.GetGoalpostPositions(bot.Team, out topPost, out bottomPost);
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
		
		if (bot.GetDistanceFromGoal(bot.Team) >= BotConstants.KeeperFetchDistance) {
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
		// Throw the ball.
		if (bot.GetBallOwner() == bot) {
			BotBase throwTarget = bot.GetClosestTeammate();
			Vector3 throwDir = throwTarget.transform.position - bot.transform.position;
			float throwDist = throwDir.magnitude;
			throwDir /= throwDist;
			bot.ThrowBall(throwDir, throwDist);
		}
		
		bot.ChangeState(BotState_GoHome.Instance);
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
		if (bot.FieldPosition == FieldPosition.Defender) {
			if (bot.GetBallTeamOwner() == bot.Team) {
				// If a teammate already has it, do nothing.
			} else {
				if (bot.GetBallDistance() <= BotConstants.DefenderChaseDistance) {
					bot.ChangeState(BotState_ChaseBall.Instance);
					return;
				}
			}
		}
	}
	
	public override void Exit(BotBase bot) {
		
	}
}

/**
 * 
 */
public class BotState_Wait : FSMState<BotBase> {
	static readonly BotState_Wait instance = new BotState_Wait();
	public static BotState_Wait Instance {
		get {
			return instance;
		}
	}
	static BotState_Wait() { }
	
	private BotState_Wait() { }
	
	public override void Enter (BotBase bot) {
		bot.Steering.AllOff();
	}
	
	public override void Execute (BotBase bot) {
		if (bot.IsStunned()) {
			// Waiting..
			return;
		}
		
		// Unstunned.
		bot.RevertState();
	}
	
	public override void Exit(BotBase bot) {
	}
}

/**
 * 
 */
public class BotState_ChaseBall : FSMState<BotBase> {
	static readonly BotState_ChaseBall instance = new BotState_ChaseBall();
	public static BotState_ChaseBall Instance {
		get {
			return instance;
		}
	}
	static BotState_ChaseBall() { }
	
	private BotState_ChaseBall() { }
	
	public override void Enter (BotBase bot) {
		bot.Steering.PursuitOn();
	}
	
	public override void Execute (BotBase bot) {
		if (bot.FieldPosition == FieldPosition.Defender) {
			// Too far - go home.
			if (bot.GetBallDistance() > BotConstants.DefenderChaseDistance) {
				bot.ChangeState(BotState_GoHome.Instance);
				return;
			}
			
			// We got the ball!
			if (bot.GetBallOwner() == bot) {
				bot.ChangeState(BotState_Dribble.Instance);
				return;
			}
			
			// Someone else from our team got it - go home.
			if (bot.GetBallTeamOwner() == bot.Team) {
				bot.ChangeState(BotState_GoHome.Instance);
				return;
			}
		}
				
		bot.Steering.CurrentTarget = bot.GetBallPosition();
		bot.Steering.CurrentEvaderVelocity = bot.GetBallVelocity();
	}
	
	public override void Exit(BotBase bot) {
		bot.Steering.PursuitOff();
	}
}

/**
 * 
 */
public class BotState_Dribble : FSMState<BotBase> {
	static readonly BotState_Dribble instance = new BotState_Dribble();
	public static BotState_Dribble Instance {
		get {
			return instance;
		}
	}
	static BotState_Dribble() { }
	
	private BotState_Dribble() { }
	
	public override void Enter (BotBase bot) {
		bot.DribbleTime = Random.Range(0.5f, 1.0f);
		
		bot.Steering.SeekOn();
		bot.Steering.CurrentTarget = bot.GetGoalpostPosition(bot.OtherTeam);
	}
	
	public override void Execute (BotBase bot) {
		if (bot.FieldPosition == FieldPosition.Defender) {
			// Lost the ball.
			if (bot.GetBallOwner() != bot) {
				bot.ChangeState(BotState_ChaseBall.Instance);
				return;
			}
			
			bot.DribbleTime -= Time.deltaTime;
			if (bot.DribbleTime <= 0.0f) {
				BotBase throwTarget = bot.GetClosestTeammate();
				Vector3 throwDir = throwTarget.transform.position - bot.transform.position;
				float throwDist = throwDir.magnitude;
				throwDir /= throwDist;
				bot.ThrowBall(throwDir, throwDist);
				bot.ChangeState(BotState_GoHome.Instance);
				
				throwTarget.Msg_ReceivePass();
			}
		}
	}
	
	public override void Exit(BotBase bot) {
		bot.Steering.SeekOff();
	}
}

/**
 * 
 */
public class BotState_ReceivePass : FSMState<BotBase> {
	static readonly BotState_ReceivePass instance = new BotState_ReceivePass();
	public static BotState_ReceivePass Instance {
		get {
			return instance;
		}
	}
	static BotState_ReceivePass() { }
	
	private BotState_ReceivePass() { }
	
	public override void Enter (BotBase bot) {
		bot.Steering.PursuitOn();
	}

	public override void Execute (BotBase bot) {
		if (bot.FieldPosition == FieldPosition.Defender) {
			if (bot.GetBallOwner() == bot) {
				bot.ChangeState(BotState_Dribble.Instance);
				return;
			}
			
			if (!bot.IsBallLoose()) {
				bot.ChangeState(BotState_GoHome.Instance);
				return;
			}
		}
			
		bot.Steering.CurrentTarget = bot.GetBallPosition();
		bot.Steering.CurrentEvaderVelocity = bot.GetBallVelocity();
	}
	
	public override void Exit(BotBase bot) {
		bot.Steering.PursuitOff();
	}
}