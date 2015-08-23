using UnityEngine;

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
		Vector3 targetPos;
		Vector3 topPost, bottomPost;
		bot.GetGoalpostPositions(out topPost, out bottomPost);
		Uzu.Math.ClosestPtPointSegment(
			topPost, bottomPost,
			bot.GetBallPosition(),
			out targetPos
		);
		bot.Steering.CurrentTarget = targetPos;
		
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
	}
	
	public override void Exit(BotBase bot) {
		bot.Steering.ArriveOff();
	}
}