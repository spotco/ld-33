using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UiPanelGame : Uzu.UiPanel {

	[SerializeField] private MultiText _home_score;
	[SerializeField] private MultiText _away_score;
	[SerializeField] private MultiText _time_text;
	[SerializeField] private Image _pause_icon;

	public override void OnInitialize() {
	}

	private float _tar_pause_icon_alpha = 0;
	private void set_pause_icon_alpha(float val) {
		Color c = _pause_icon.color;
		c.a = val;
		_pause_icon.color = c;
	}

	public override void OnEnter(Uzu.PanelEnterContext context) {
		gameObject.SetActive(true);
		_home_score.set_string("0");
		_home_score.set_string("0");
		_time_text.set_string("0:00:00");
		set_pause_icon_alpha(0);
		_tar_pause_icon_alpha = 0;
		Main.LevelController.StartLevel();
	}
	
	public override void OnExit(Uzu.PanelExitContext context) {
		gameObject.SetActive(false);
	}
	
	private void Update() {
		if (Main.LevelController.m_currentMode == LevelController.LevelControllerMode.Timeout) {
			_tar_pause_icon_alpha = 0.75f;
		} else {
			_tar_pause_icon_alpha = 0.0f;
		}
		set_pause_icon_alpha(Util.drpt(_pause_icon.color.a,_tar_pause_icon_alpha,1/5.0f));

		_home_score.set_string(Main.LevelController._player_team_score+"");
		_away_score.set_string(Main.LevelController._enemy_team_score+"");
		_time_text.set_string(Main.LevelController.get_time_remaining_formatted());

		if (Input.GetKeyDown(KeyCode.R)) {
			Main.LevelController.StartLevel();
		}
	}
}
