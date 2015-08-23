using UnityEngine;

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