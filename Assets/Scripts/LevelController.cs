using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class LevelController : MonoBehaviour {

	enum LevelControllerMode {
		GamePlay,
		Timeout
	}

	[SerializeField] private GameObject proto_genericFootballer;

	public PathRenderer m_pathRenderer;
	private List<GenericFootballer> m_playerTeamFootballers = new List<GenericFootballer>();

	public GenericFootballer m_playerControlledFootballer;
	public GenericFootballer m_timeoutSelectedFootballer;
	private LevelControllerMode m_currentMode;

	public void StartLevel() {
		m_pathRenderer = this.GetComponent<PathRenderer>();

		m_playerTeamFootballers.Add(this.CreateFootballer(new Vector3(0,0)));
		m_playerTeamFootballers.Add(this.CreateFootballer(new Vector3(300,300)));
		m_playerTeamFootballers.Add(this.CreateFootballer(new Vector3(300,0)));
		m_playerTeamFootballers.Add(this.CreateFootballer(new Vector3(0,300)));
		m_playerTeamFootballers.Add(this.CreateFootballer(new Vector3(600,300)));

		m_playerControlledFootballer = m_playerTeamFootballers[0];

		m_currentMode = LevelControllerMode.GamePlay;
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

	private static GenericFootballer IsPointTouchFootballer(Vector3 pt, List<GenericFootballer> list) {
		for (int i = 0; i < list.Count; i++) {
			if (list[i].ContainsPoint(pt)) return list[i];
		}
		return null;
	}

	void Update () {
		if (Main.PanelManager.CurrentPanelId != PanelIds.Game) return;

		float dt_scale = (1/60.0f)/(Time.unscaledDeltaTime);
		Util.dt_scale = dt_scale;

		if (m_currentMode == LevelControllerMode.GamePlay) {
			Main.GameCamera.SetTarget(m_playerControlledFootballer.transform);
			m_pathRenderer._path_renderer_root.SetActive(false);

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
			if (m_timeoutSelectedFootballer != null) {
				Main.GameCamera.SetTarget(m_timeoutSelectedFootballer.transform);
			} else {
				Main.GameCamera.SetTarget(m_playerControlledFootballer.transform);
			}

			for (int i = 0; i < this.m_playerTeamFootballers.Count; i++) {
				GenericFootballer itr = this.m_playerTeamFootballers[i];
				itr.timeout_update();
			}
			m_pathRenderer._path_renderer_root.SetActive(true);

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