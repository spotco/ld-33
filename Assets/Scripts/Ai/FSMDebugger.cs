using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FSMDebugger : MonoBehaviour {
	[SerializeField]
	private BotBase _bot;
	
	[SerializeField]
	private Text _botText;
	
	private void Update() {
		if (_bot != null) {
			var fsm = _bot.DBG_FSM;
			_botText.text = fsm.CurrentState.Name;
		}
	}
}
