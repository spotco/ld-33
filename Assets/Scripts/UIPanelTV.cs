using UnityEngine;
using System.Collections;

public class UIPanelTV : Uzu.UiPanel {

	[SerializeField] private GameObject _MNM_logo;
	[SerializeField] private GameObject _zoomexit_target;
	[SerializeField] private GameObject _zoomexit_start;
	[SerializeField] private CameraFade _camera_fade;

	public override void OnInitialize() {
	}

	public override void OnEnter(Uzu.PanelEnterContext context) {
		gameObject.SetActive(true);
		Main.GameCamera.gameObject.SetActive(false);
		Main.Instance._tvCamera.gameObject.SetActive(true);
		_MNM_logo.SetActive(true);
		_current_mode = UIPanelTVMode.Idle;

		Main.Instance._tvCamera.transform.position = _zoomexit_start.transform.position;
		_camera_fade.set_alpha(0);
		_camera_fade.set_target_alpha(0);

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
	
	private void Update() {
		if (_current_mode == UIPanelTVMode.Idle) {
			if (Input.GetMouseButton(0)) {
				_current_mode = UIPanelTVMode.ZoomExit;
				_anim_t = 0;
				_camera_fade.set_target_alpha(1);
			}

		} else if (_current_mode == UIPanelTVMode.ZoomExit) {
			_anim_t += Util.dt_scale * 0.05f;
			Main.Instance._tvCamera.transform.position = Vector3.Lerp(_zoomexit_start.transform.position,_zoomexit_target.transform.position,_anim_t);
			if (_anim_t >= 1) {
				Main.PanelManager.ChangeCurrentPanel(PanelIds.Game);
			}
		}
	}
}