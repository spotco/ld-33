using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FSMDebugger : MonoBehaviour {
	[SerializeField]
	private BotBase _bot;
	[SerializeField]
	private TeamBase _team;
	
	[SerializeField]
	private Text _botText;
	[SerializeField]
	private Text _teamText;
	
	public BotBase Bot {
		get { return _bot; }
		set { _bot = value; }
	}
	
	public TeamBase Team {
		get { return _team; }
		set { _team = value; }
	}
	
	private void Update() {
		if (_bot != null) {
			var fsm = _bot.DBG_FSM;
			_botText.text = fsm.CurrentState.Name;
		} else {
			_botText.text = "-";
		}
		
		if (_team != null) {
			var fsm = _team.DBG_FSM;
			_teamText.text = fsm.CurrentState.Name;
		} else {
			_teamText.text = "-";
		}
	}
}
