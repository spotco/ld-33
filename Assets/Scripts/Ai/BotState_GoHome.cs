using UnityEngine;

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
		if (Vector3.Distance(bot.HomePosition, bot.transform.localPosition) <= AiConstants.ArriveDistance) {
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