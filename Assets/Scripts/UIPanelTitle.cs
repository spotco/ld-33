using UnityEngine;
using System.Collections;

public class UIPanelTitle : Uzu.UiPanel {

	public override void OnInitialize() {}

	[SerializeField] private CameraFade _camera_fade;
	[SerializeField] private SpriteRenderer _spotco_logo;
	[SerializeField] private SpriteRenderer _mnm_logo;
	[SerializeField] private SpriteRenderer _mnm_base;
	[SerializeField] private SpriteRenderer _cover;
	[SerializeField] private SpriteRenderer _click_anywhere_to_start;

	private Uzu.AudioHandle _intro_bgm_handle;

	public override void OnEnter(Uzu.PanelEnterContext context) {
		Main.GameCamera.gameObject.SetActive(false);
		Main.Instance._tvCamera.gameObject.SetActive(false);
		Main.Instance._titleCamera.gameObject.SetActive(true);
		gameObject.SetActive(true);
		_intro_bgm_handle = Main.AudioController.PlayBgm(AudioClipIds.BGM_MENU_INTRO,false);

	}
	
	public override void OnExit(Uzu.PanelExitContext context) {
		Main.Instance._titleCamera.gameObject.SetActive(false);
		gameObject.SetActive(false);
	}

	void Update() {
		if (_intro_bgm_handle._handle_audio_source != null && !_intro_bgm_handle._handle_audio_source.isPlaying) {
			_intro_bgm_handle._handle_audio_source = null;
			Main.AudioController.PlayBgm(AudioClipIds.BGM_MENU_LOOP);
		}
	}
}
