using UnityEngine;
using System.Collections;

public class UIPanelTV : Uzu.UiPanel {

	[SerializeField] private GameObject _MNM_logo;
	[SerializeField] private GameObject _zoomexit_target;
	[SerializeField] private GameObject _zoomexit_start;
	[SerializeField] private CameraFade _camera_fade;

	[SerializeField] private BobDirection _skele_talk_left,_skele_talk_right;

	[SerializeField] private ChatManager _chats;

	public override void OnInitialize() {
	}

	public override void OnEnter(Uzu.PanelEnterContext context) {
		Main.AudioController.PlayEffect("crowd");
		Main.AudioController.PlayBgm(AudioClipIds.BGM_MENU_LOOP);
		gameObject.SetActive(true);
		Main.GameCamera.gameObject.SetActive(false);
		Main.Instance._tvCamera.gameObject.SetActive(true);
		_MNM_logo.SetActive(true);
		_current_mode = UIPanelTVMode.Idle;

		Main.Instance._tvCamera.transform.position = _zoomexit_start.transform.position;
		_camera_fade.set_alpha(0);
		_camera_fade.set_target_alpha(0);

		_chats.clear_messages();
		if (Main._current_repeat_reason == RepeatReason.None) {
			if (Main._current_level == GameLevel.Level1) {
				_chats.push_message("Welcome back to Monday Night Monsters!",2);
				_chats.push_message("In tonight's game, the away team...",2);
				_chats.push_message("the 0-6 Reds...",1);
				_chats.push_message("are facing off against the undefeated Blues!",2);
				_chats.push_message("(That's you, by the way...)",1);
				_chats.push_message("For all the first time viewers, let's talk controls!",2);
				_chats.push_message("(When you have the ball, click and hold to pass.)",1);
				_chats.push_message("(Hold space any time to enter time out.)",1);
				_chats.push_message("(Then, click and drag to tell your teammates what to do.)",1);
				_chats.push_message("(The goal is to get the ball in the opposing red goal.)",1);
				_chats.push_message("Kickoff's just about to begin.",2);
				_chats.push_message("Can the Blues score in the first quarter?",2);
				_chats.push_message("(You've got 5 minutes on the clock.)",1);
				_chats.push_message("Kickoff's just about to begin, let's watch.",2);

			} else if (Main._current_level == GameLevel.Level2) {
				_chats.push_message("It's a close fought game, and we're nearing halftime.",2);
				_chats.push_message("The score's tied, with three minutes on the clock.",2);
				_chats.push_message("Can the Blues end the half on a strong note?",2);
				_chats.push_message("(You've got three minutes to score a goal.)",1);
				_chats.push_message("The players are lining back up on the field.",2);
				_chats.push_message("Who knows what could happen next?",2);

			} else if (Main._current_level == GameLevel.Level3) {
				_chats.push_message("Who would have guessed? It's down to the final two minutes...",2);
				_chats.push_message("And both teams are in a dead heat.",2);
				_chats.push_message("Can the hometown favorites, the Blues, pull through?",2);
				_chats.push_message("(Time to pull out all the stops.)",1);
				_chats.push_message("The players are heading out from the sidelines.",2);
				_chats.push_message("Everyone's at the edge of their seats!",2);

			} else {
				_chats.push_message("And as expected, the hometown heroes...",2);
				_chats.push_message("Pull through and win in dramatic fashion!",2);
				_chats.push_message("Tune in next week for more Monday Night Monsters!",2);
				_chats.push_message("And coming up next...",2);
				_chats.push_message("The hit Emmy award-winning daytime drama, Ludum Dare!",2);
				_chats.push_message("(Thanks for playing!)",1);
			}

		} else {
			if (Main._current_repeat_reason == RepeatReason.ScoredOn) {
				_chats.push_message("What a shock! The Reds broke away and scored last minute!",2);
				_chats.push_message("I can't believe it! Is this the beginning of the end for the Blues?",2);
				_chats.push_message("(As a wise man once said...)",1);
				_chats.push_message("(To score a touchdown, you've gotta move the ball to the endzone.)",1);
				_chats.push_message("(Let's try that one again.)",1);
				_chats.push_message("Let's watch an instant replay to see what just happened.",2);

			} else {
				_chats.push_message("In a absolutely SHOCKING turn of events...",2);
				_chats.push_message("The Reds defense held and allowed ZERO points!",2);
				_chats.push_message("Unbelieveable! The Red offence was first in the league in yards.",2);
				_chats.push_message("(As a wise man once said...)",1);
				_chats.push_message("(If a team doesn't put points on the board, I don't see how they can win.)",1);
				_chats.push_message("(Let's try that one again.)",1);
				_chats.push_message("Let's watch an instant replay to see what just happened.",2);
			}
		}
	}
	
	public override void OnExit(Uzu.PanelExitContext context) {
		gameObject.SetActive(false);
		Main.GameCamera.gameObject.SetActive(true);
		Main.Instance._tvCamera.gameObject.SetActive(false);
		_MNM_logo.SetActive(false);
	}

	enum UIPanelTVMode {
		Idle,
		ZoomExit
	}
	private UIPanelTVMode _current_mode;
	private float _anim_t;
	
	private float _time_until_next_talk_sound = 0;
	private void Update() {
		if (_current_mode == UIPanelTVMode.Idle) {
			if (Input.GetKey(KeyCode.Space) || (_chats._messages.Count == 0 && _chats._text_scroll.finished() && _chats._ct <= 0)) {
				_current_mode = UIPanelTVMode.ZoomExit;
				_anim_t = 0;
				_camera_fade.set_target_alpha(1);
			}
			_skele_talk_left.set_enabled(false);
			_skele_talk_right.set_enabled(false);
			if (!_chats._text_scroll.finished()) {
				if (_chats._current_id == 1) {
					_skele_talk_left.set_enabled(true);
				} else if (_chats._current_id == 2) {
					_skele_talk_right.set_enabled(true);
				}
				if (_time_until_next_talk_sound <= 0) {
					_time_until_next_talk_sound = 0.25f;
					if (_chats._current_id == 1) {
						Main.AudioController.PlayEffect("speak_0_"+((int)Util.rand_range(0,6)));
					} else {
						Main.AudioController.PlayEffect("speak_1_"+((int)Util.rand_range(0,6)));
					}
				} else {
					_time_until_next_talk_sound -= Time.deltaTime;
				}
				
			}

		} else if (_current_mode == UIPanelTVMode.ZoomExit) {
			_anim_t += Util.dt_scale * 0.05f;
			Main.Instance._tvCamera.transform.position = Vector3.Lerp(_zoomexit_start.transform.position,_zoomexit_target.transform.position,_anim_t);
			if (_anim_t >= 1) {
				if (Main._current_level == GameLevel.Level1) {
					Main.PanelManager.ChangeCurrentPanel(PanelIds.Game);
					Main.LevelController.CurrentDifficulty = Difficulty.Easy;
					Main.LevelController.StartLevel(LevelController.StartMode.Sequence);

				} else if (Main._current_level == GameLevel.Level2) {
					Main.PanelManager.ChangeCurrentPanel(PanelIds.Game);
					Main.LevelController.CurrentDifficulty = Difficulty.Normal;
					Main.LevelController.StartLevel(LevelController.StartMode.Sequence);

				} else if (Main._current_level == GameLevel.Level3) {
					Main.PanelManager.ChangeCurrentPanel(PanelIds.Game);
					Main.LevelController.CurrentDifficulty = Difficulty.Hard;
					Main.LevelController.StartLevel(LevelController.StartMode.Sequence);

				} else {
					Main._current_level = GameLevel.Level1;
					Main.PanelManager.ChangeCurrentPanel(PanelIds.Title);
				}

			}
		}
	}
}