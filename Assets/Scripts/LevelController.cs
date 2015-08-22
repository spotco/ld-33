using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class LevelController : MonoBehaviour {

	enum LevelControllerMode {
		GamePlay,
		Timeout
	}

	[SerializeField] private GameObject proto_genericFootballer;
	[SerializeField] private GameObject proto_looseBall;
	[SerializeField] private GameObject proto_mouseTarget;

	[SerializeField] public BoxCollider2D m_gameBounds;
	[SerializeField] public BoxCollider2D m_playerGoalBounds;
	[SerializeField] public BoxCollider2D m_enemyGoalBounds;

	[System.NonSerialized] public PathRenderer m_pathRenderer;
	[System.NonSerialized] public List<GenericFootballer> m_playerTeamFootballers = new List<GenericFootballer>();

	[System.NonSerialized] public GenericFootballer m_playerControlledFootballer;
	[System.NonSerialized] public GenericFootballer m_timeoutSelectedFootballer;
	[System.NonSerialized] public List<LooseBall> m_looseBalls = new List<LooseBall>();

	private GameObject m_mouseTargetIcon;
	private float m_mouseTargetIconTheta;

	private LevelControllerMode m_currentMode;

	public void StartLevel() {
		m_pathRenderer = this.GetComponent<PathRenderer>();

		m_playerTeamFootballers.Add(this.CreateFootballer(new Vector3(0,0)));
		m_playerTeamFootballers.Add(this.CreateFootballer(new Vector3(-300,-300)));
		m_playerTeamFootballers.Add(this.CreateFootballer(new Vector3(-300,0)));
		m_playerTeamFootballers.Add(this.CreateFootballer(new Vector3(0,-300)));
		m_playerTeamFootballers.Add(this.CreateFootballer(new Vector3(-600,-300)));

		m_playerControlledFootballer = m_playerTeamFootballers[0];

		m_currentMode = LevelControllerMode.GamePlay;

		m_mouseTargetIcon = Util.proto_clone(proto_mouseTarget);
	}

	public void CreateLooseBall(Vector2 start, Vector2 vel) {
		GameObject neu_obj = Util.proto_clone(proto_looseBall);
		LooseBall rtv = neu_obj.GetComponent<LooseBall>();
		rtv.sim_initialize(start,vel);
		m_looseBalls.Add(rtv);
	}

	public void PickupLooseBall(LooseBall looseball, GenericFootballer tar) {
		m_looseBalls.Remove(looseball);
		if (m_playerTeamFootballers.Contains(tar)) {
			m_playerControlledFootballer = tar;
			tar._current_mode = GenericFootballer.GenericFootballerMode.Idle;
			tar._waitdelay = 15;
		}
		Destroy(looseball.gameObject);
	}

	private GenericFootballer CreateFootballer(Vector3 pos) {
		GameObject neu_obj = Util.proto_clone(proto_genericFootballer);
		GenericFootballer rtv = neu_obj.GetComponent<GenericFootballer>();
		rtv.transform.position = pos;
		rtv.sim_initialize();
		return rtv;
	}

	public Vector3 GetMousePoint() {
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Plane gamePlane = new Plane(new Vector3(0,0,-1),new Vector3(0,0,0));
		float rayout;
		gamePlane.Raycast(ray,out rayout);
		return Util.vec_add(ray.origin,Util.vec_scale(ray.direction,rayout));
	}
	
	private bool IsClickAndPoint(out Vector2 point) {
		if (Input.GetMouseButtonDown(0)) {
			point = GetMousePoint();
			return true;
		}
		point = Vector2.zero;
		return false;
	}

	private GenericFootballer IsPointTouchFootballer(Vector3 pt, List<GenericFootballer> list) {
		for (int i = 0; i < list.Count; i++) {
			if (list[i] == m_playerControlledFootballer) continue;
			if (list[i].ContainsPoint(pt)) return list[i];
		}
		return null;
	}

	void Update () {
		if (Main.PanelManager.CurrentPanelId != PanelIds.Game) return;
		float mouse_target_anim_speed = 0.3f;

		float dt_scale = (1/60.0f)/(Time.unscaledDeltaTime);
		Util.dt_scale = dt_scale;

		if (m_currentMode == LevelControllerMode.GamePlay) {
			if (m_playerControlledFootballer != null) {
				Main.GameCamera.SetTargetPos(m_playerControlledFootballer.transform.position);
				if (Input.GetKey(KeyCode.Z)) {
					Main.GameCamera.SetTargetZoom(600);
				} else {
					Main.GameCamera.SetTargetZoom(400);
				}
				Main.GameCamera.SetManualOffset(new Vector3(200,0,0));

			} else {
				Main.GameCamera.SetTargetPos(this.GetMousePoint());
				Main.GameCamera.SetTargetZoom(600);
				Main.GameCamera.SetManualOffset(new Vector3(0,0,0));
			}


			mouse_target_anim_speed = 2.0f;
			m_mouseTargetIcon.SetActive(true);
			Vector3 mouse_pt = this.GetMousePoint();
			m_mouseTargetIcon.transform.position = mouse_pt;
			m_mouseTargetIcon.transform.localScale = Util.valv(50.0f);

			for (int i = m_looseBalls.Count-1; i >= 0; i--) {
				LooseBall itr = this.m_looseBalls[i];	
				itr.sim_update();
			}

			for (int i = 0; i < this.m_playerTeamFootballers.Count; i++) {
				GenericFootballer itr = this.m_playerTeamFootballers[i];	
				itr.sim_update();
			}

			if (Input.GetKey(KeyCode.Space)) {
				for (int i = 0; i < this.m_playerTeamFootballers.Count; i++) {
					GenericFootballer itr = this.m_playerTeamFootballers[i];
					itr.timeout_start();
				}
				m_currentMode = LevelControllerMode.Timeout;
				m_timeoutSelectedFootballer = null;

			}


		} else if (m_currentMode == LevelControllerMode.Timeout) {
			Vector3 mouse_to_center_delta = Util.vec_sub(this.GetMousePoint(),Main.GameCamera.GetCurrentPosition());
			if (mouse_to_center_delta.magnitude > 600) {
				Vector3 n_mouse_to_center_delta = mouse_to_center_delta.normalized;
				Vector3 tar_delta = Util.vec_scale(n_mouse_to_center_delta,(mouse_to_center_delta.magnitude-600)*0.2f);
				Main.GameCamera.SetTargetPos(Util.vec_add(Main.GameCamera.GetCurrentPosition(),tar_delta));
			} else {
				Main.GameCamera.SetTargetPositionToCurrent();
			}

			Main.GameCamera.SetManualOffset(new Vector3(0,0,0));
			Main.GameCamera.SetTargetZoom(800);
			Vector3 mouse_pt = this.GetMousePoint();
			GenericFootballer select_tar = this.IsPointTouchFootballer(mouse_pt,m_playerTeamFootballers);
			if (select_tar != null) {
				m_mouseTargetIcon.SetActive(false);
				select_tar.SetSelectedForAFrame();
			} else {
				m_mouseTargetIcon.SetActive(m_timeoutSelectedFootballer!=null);
			}
			m_mouseTargetIcon.transform.position = mouse_pt;
			m_mouseTargetIcon.transform.localScale = Util.valv(75.0f);


			for (int i = 0; i < this.m_playerTeamFootballers.Count; i++) {
				GenericFootballer itr = this.m_playerTeamFootballers[i];
				itr.timeout_update();
			}

			keyboard_switch_timeout_selected_footballer();

			Vector2 click_pt;
			if (this.IsClickAndPoint(out click_pt)) {
				GenericFootballer clicked_footballer = IsPointTouchFootballer(click_pt,m_playerTeamFootballers);
				if (clicked_footballer != null) {
					m_timeoutSelectedFootballer = clicked_footballer;
				} else if (m_timeoutSelectedFootballer != null && m_timeoutSelectedFootballer != m_playerControlledFootballer) {
					m_timeoutSelectedFootballer.CommandMoveTo(click_pt);
				}
			}

			if (!Input.GetKey(KeyCode.Space)) {
				m_currentMode = LevelControllerMode.GamePlay;
				for (int i = 0; i < this.m_playerTeamFootballers.Count; i++) {
					GenericFootballer itr = this.m_playerTeamFootballers[i];
					itr.timeout_end();
				}
			}
		}

		m_mouseTargetIconTheta += mouse_target_anim_speed * Util.dt_scale;
		Util.transform_set_euler_world(m_mouseTargetIcon.transform,new Vector3(0,0,m_mouseTargetIconTheta));

	}

	private void keyboard_switch_timeout_selected_footballer() {
		int tar = -1;
		if (Input.GetKey(KeyCode.Alpha1)) tar = 0;
		if (Input.GetKey(KeyCode.Alpha2)) tar = 1;
		if (Input.GetKey(KeyCode.Alpha3)) tar = 2;
		if (Input.GetKey(KeyCode.Alpha4)) tar = 3;
		if (Input.GetKey(KeyCode.Alpha5)) tar = 4;
		if (tar != -1) {
			m_timeoutSelectedFootballer = m_playerTeamFootballers[tar];
		}
	}

}