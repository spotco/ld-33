using UnityEngine;

public class State_TendGoal : FSMState<BotBase> {
	
	static readonly State_TendGoal instance = new State_TendGoal();
	public static State_TendGoal Instance {
		get {
			return instance;
		}
	}
	static State_TendGoal() { }
	
	private State_TendGoal() { }
	
	public override void Enter (BotBase bot) {
		// bot.Steering.ArriveOn();
		
		Vector3 anchorPos = GameObject.Find("Goal").transform.localPosition;
		bot.Steering.InterposeOn(anchorPos, 350.0f);
		// bot.Steering.SeekOn();
		
		// bot.Steering.EnableArrive(true);
		// bot.Steering.SetTarget(Vector3.zero);
			
		// bot.Steering.EnableInterpose(true);
		// bot.Steering.SetTarget(...);
		
		// bot.Steering.InterposeOn(Prm.GoalKeeperTendingDistance);
		// bot.Steering.SetTarget(bot->GetRearInterposeTarget());
	}
	
	public override void Execute (BotBase bot) {
		bot.Steering.SetTarget(GameObject.Find("Mover").transform.localPosition);
		// bot->Steering()->SetTarget(bot->GetRearInterposeTarget());
		// 
		// if (bot->BallWithinPlayerRange())
		// {
		// bot->Ball()->Trap();
		// bot->Pitch()->SetGoalKeeperHasBall(true);
		// bot->ChangeState(bot, PutBallBackInPlay::Instance());
		// return;
		// }
		// //if ball is within a predefined distance, the bot moves out from
		// //position to try to intercept it.
		// if (bot->BallWithinRangeForIntercept())
		// {
		// bot->ChangeState(bot, InterceptBall::Instance());
		// }
	}
	
	public override void Exit(BotBase bot) {
		// bot->Steering()->InterposeOff();
	}
}