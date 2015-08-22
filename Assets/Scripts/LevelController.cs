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
	private List<GenericFootballer> m_allFootballers = new List<GenericFootballer>();
	public GenericFootballer m_playerControlledFootballer;
	private LevelControllerMode m_currentMode;

	public void StartLevel() {
		m_pathRenderer = this.GetComponent<PathRenderer>();

		m_allFootballers.Add(this.CreateFootballer(new Vector3(0,0)));
		m_currentMode = LevelControllerMode.GamePlay;
	}

	private GenericFootballer CreateFootballer(Vector3 pos) {
		GameObject neu_obj = Util.proto_clone(proto_genericFootballer);
		GenericFootballer rtv = neu_obj.GetComponent<GenericFootballer>();
		rtv.transform.position = pos;
		rtv.sim_initialize();
		return rtv;
	}

	private bool IsClickAndPoint(out Vector2 point) {
		if (Input.GetMouseButtonDown(0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Plane gamePlane = new Plane(new Vector3(0,0,-1),new Vector3(0,0,0));
			float rayout;
			bool did_hit = gamePlane.Raycast(ray,out rayout);
			if (did_hit) {
				Vector3 plane_intersect = Util.vec_add(ray.origin,Util.vec_scale(ray.direction,rayout));
				point = new Vector2(plane_intersect.x,plane_intersect.y);
				return true;
			}
		}
		point = Vector2.zero;
		return false;
	}

	void Update () {
		if (Main.PanelManager.CurrentPanelId != PanelIds.Game) return;

		float dt_scale = (1/60.0f)/(Time.unscaledDeltaTime);
		Util.dt_scale = dt_scale;

		if (m_currentMode == LevelControllerMode.GamePlay) {
			m_pathRenderer._path_renderer_root.SetActive(false);

			for (int i = 0; i < this.m_allFootballers.Count; i++) {
				GenericFootballer itr = this.m_allFootballers[i];	
				itr.sim_update();
			}

			if (Input.GetKey(KeyCode.Space)) {
				for (int i = 0; i < this.m_allFootballers.Count; i++) {
					GenericFootballer itr = this.m_allFootballers[i];
					itr.timeout_start();
				}
				m_currentMode = LevelControllerMode.Timeout;
			}

		} else if (m_currentMode == LevelControllerMode.Timeout) {
			for (int i = 0; i < this.m_allFootballers.Count; i++) {
				GenericFootballer itr = this.m_allFootballers[i];
				itr.timeout_update();
			}
			m_pathRenderer._path_renderer_root.SetActive(true);

			Vector2 click_pt;
			if (this.IsClickAndPoint(out click_pt)) {
				for (int i = 0; i < this.m_allFootballers.Count; i++) {
					GenericFootballer itr = this.m_allFootballers[i];	
					itr.CommandMoveTo(click_pt);
				}
			}

			if (!Input.GetKey(KeyCode.Space)) {
				m_currentMode = LevelControllerMode.GamePlay;
				for (int i = 0; i < this.m_allFootballers.Count; i++) {
					GenericFootballer itr = this.m_allFootballers[i];
					itr.timeout_end();
				}
			}
		}

	}

}