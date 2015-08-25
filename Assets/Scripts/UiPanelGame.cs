using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UiPanelGame : Uzu.UiPanel {

	public static UiPanelGame inst;

	[SerializeField] private Sprite _popup_sprite_gameon, _popup_sprite_goal, _popup_sprite_timeup;
	[SerializeField] private Image _popup_message;

	[SerializeField] private MultiText _home_score;
	[SerializeField] private MultiText _away_score;
	[SerializeField] private MultiText _time_text;
	[SerializeField] private MultiText _quarter_text;
	[SerializeField] private Image _pause_icon;
	[SerializeField] public CameraFade _fadein;

	[SerializeField] public ChatManager _chats;

	public override void OnInitialize() {
		inst = this;
	}

	private float _tar_pause_icon_alpha = 0;
	private void set_pause_icon_alpha(float val) {
		Color c = _pause_icon.color;
		c.a = val;
		_pause_icon.color = c;
	}

	public override void OnEnter(Uzu.PanelEnterContext context) {
		Main.AudioController.PlayBgm(AudioClipIds.GameBgm);
		gameObject.SetActive(true);
		_home_score.set_string("0");
		_home_score.set_string("0");
		_time_text.set_string("0:00:00");
		set_pause_icon_alpha(0);
		_tar_pause_icon_alpha = 0;
		_quarter_text.set_string(Main.LevelController._quarter_display);
		Main.LevelController.StartLevel();
		_fadein.set_alpha(1.0f);
		_fadein.set_target_alpha(0.0f);
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
		_quarter_text.set_string(Main.LevelController._quarter_display);
		
		#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.R)) {
			Main.LevelController.StartLevel(LevelController.StartMode.Sequence);
		}
		if (Input.GetKeyDown(KeyCode.T)) {
			Main.LevelController.StartLevel(LevelController.StartMode.Immediate);
		}
		#endif

		if (_popup_t > 0) {
			_popup_t-=0.015f*Util.dt_scale;
			float t = 1-_popup_t;
			popup_set_alpha(Util.bezier_val_for_t(new Vector2(0,0),new Vector2(0,2), new Vector2(0.5f,1), new Vector2(1,0),t).y);
			_popup_message.transform.localScale = Util.valv(Util.bezier_val_for_t(new Vector2(0,2),new Vector2(0.25f,0.25f), new Vector2(1,1), new Vector2(1,1.25f),t).y);
		} else {
			popup_set_alpha(0);
		}
	}

	public bool can_take_message() { return _chats._messages.Count == 0 && _chats._ct <= 0Ω; }

	private void popup_set_alpha(float val) {
		Color c = _popup_message.color;
		c.a = val;
		_popup_message.color = c;
	}

	private float _popup_t;
	public void show_popup_message(int i) {
		_popup_t = 1;
		if (i == 0) {
			_popup_message.sprite = _popup_sprite_gameon;
		} else if (i == 1) {
			_popup_message.sprite = _popup_sprite_goal;
		} else {
			_popup_message.sprite = _popup_sprite_timeup;
		}
	}
}
