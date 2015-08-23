using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class LevelController : MonoBehaviour {

	[SerializeField] private GameObject proto_team;
	[SerializeField] private GameObject proto_genericFootballer;
	[SerializeField] private GameObject proto_looseBall;
	[SerializeField] private GameObject proto_mouseTarget;

	enum LevelControllerMode {
		GamePlay,
		Timeout
	}

	[SerializeField] public BoxCollider2D m_gameBounds;
	[SerializeField] public BoxCollider2D m_playerGoalBounds;
	[SerializeField] public BoxCollider2D m_enemyGoalBounds;

	[System.NonSerialized] public PathRenderer m_pathRenderer;
	public List<GenericFootballer> m_playerTeamFootballers = new List<GenericFootballer>();
	public List<GenericFootballer> m_enemyTeamFootballers = new List<GenericFootballer>();


	public List<GenericFootballer> m_playerTeamFootballersWithBall = new List<GenericFootballer>();
	public List<GenericFootballer> m_enemyTeamFootballersWithBall = new List<GenericFootballer>();

	//Nullable
	public GenericFootballer m_timeoutSelectedFootballer;
	public List<LooseBall> m_looseBalls = new List<LooseBall>();

	private LevelControllerMode m_currentMode;
	
	private TeamBase m_playerTeam;
	private TeamBase m_enemyTeam;
	
	private GameObject m_mouseTargetIcon;
	private float m_mouseTargetIconTheta;
	public void StartLevel() {
		m_pathRenderer = this.GetComponent<PathRenderer>();
		
		m_playerTeam = this.CreateTeam(Team.PlayerTeam);
		m_enemyTeam = this.CreateTeam(Team.EnemyTeam);
		
		{
			this.CreateFootballer(m_playerTeam, new Vector3(1800,-250));
			this.CreateFootballer(m_playerTeam, new Vector3(-300,-300));
			this.CreateFootballer(m_playerTeam, new Vector3(-300,0));
			this.CreateFootballer(m_playerTeam, new Vector3(0,-300));
			this.CreateFootballer(m_playerTeam, new Vector3(-600,-300));
		}

		/*
		{
			BotBase keeper = this.CreateFootballer(m_enemyTeam, new Vector3(300,0));
			BotBase d0 = this.CreateFootballer(m_enemyTeam, new Vector3(200,0));
			BotBase d1 = this.CreateFootballer(m_enemyTeam, new Vector3(100,0));
			m_enemyTeam.SetPlayers(keeper, d0, d1);
			m_enemyTeam.StartKickoff();
		}
		*/
		
		m_playerTeamFootballersWithBall.Add(m_playerTeamFootballers[0]);
		m_currentMode = LevelControllerMode.GamePlay;
		m_mouseTargetIcon = Util.proto_clone(proto_mouseTarget);
	}

	public void Update () {
		if (Main.PanelManager.CurrentPanelId != PanelIds.Game) return;
		float mouse_target_anim_speed = 0.3f;

		float dt_scale = (1/60.0f)/(Time.unscaledDeltaTime);
		Util.dt_scale = dt_scale;

		if (m_currentMode == LevelControllerMode.GamePlay) {
			if (m_playerTeamFootballersWithBall.Count > 0) {
				Main.GameCamera.SetTargetPos(m_playerTeamFootballersWithBall[0].transform.position);
				if (Input.GetMouseButton(0)) {
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

			for (int i = 0; i < this.m_enemyTeamFootballers.Count; i++) {
				GenericFootballer itr = this.m_enemyTeamFootballers[i];	
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

			for (int i = m_looseBalls.Count-1; i >= 0; i--) {
				LooseBall itr = this.m_looseBalls[i];
				if (m_enemyGoalBounds.OverlapPoint(itr.transform.position)) {

				}
				if (m_playerGoalBounds.OverlapPoint(itr.transform.position)) {

				}
			}


		} else if (m_currentMode == LevelControllerMode.Timeout) {

			Vector3 mouse_to_center_delta = Util.vec_sub(Input.mousePosition,new Vector2(Screen.width/2,Screen.height/2));
			float mmouse_move_rad = (Screen.width+Screen.height)/2.0f * 0.25f;
			if (mouse_to_center_delta.magnitude > mmouse_move_rad) {
				Vector3 n_mouse_to_center_delta = mouse_to_center_delta.normalized;
				Vector3 tar_delta = Util.vec_scale(n_mouse_to_center_delta,(mouse_to_center_delta.magnitude-mmouse_move_rad)*0.1f);
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
				m_mouseTargetIcon.SetActive(true);
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
				} else if (m_timeoutSelectedFootballer != null && !this.footballer_has_ball(m_timeoutSelectedFootballer)) {
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

	public void CreateLooseBall(Vector2 start, Vector2 vel) {
		GameObject neu_obj = Util.proto_clone(proto_looseBall);
		LooseBall rtv = neu_obj.GetComponent<LooseBall>();
		rtv.sim_initialize(start,vel);
		m_looseBalls.Add(rtv);
	}
	
	public void PickupLooseBall(LooseBall looseball, GenericFootballer tar) {
		m_looseBalls.Remove(looseball);
		if (this.get_footballer_team(tar) == Team.PlayerTeam) {
			m_playerTeamFootballersWithBall.Add(tar);
			tar._current_mode = GenericFootballer.GenericFootballerMode.Idle;
			tar.set_wait_delay(15);
		} else {
			this.m_enemyTeamFootballersWithBall.Add(tar);
			tar._current_mode = GenericFootballer.GenericFootballerMode.Idle;
			tar.set_wait_delay(15);
		}
		Destroy(looseball.gameObject);
	}
	
	private TeamBase CreateTeam(Team team) {
		GameObject neu_obj = Util.proto_clone(proto_team);
		TeamBase rtv = neu_obj.GetComponent<TeamBase>();
		rtv.Team = team;
		return rtv;
	}
	
	private BotBase CreateFootballer(TeamBase team, Vector3 pos) {
		GameObject neu_obj = Util.proto_clone(proto_genericFootballer);
		GenericFootballer rtv = neu_obj.GetComponent<GenericFootballer>();
		rtv.transform.position = pos;
		rtv.sim_initialize();
		if (team.Team == Team.PlayerTeam) {
			m_playerTeamFootballers.Add(rtv);
		} else {
			m_enemyTeamFootballers.Add(rtv);
		}
		return neu_obj.GetComponent<BotBase>();
	}
	
	public Vector3 GetMousePoint() {
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
			if (this.footballer_has_ball(list[i])) continue;
			if (list[i].ContainsPoint(pt)) return list[i];
		}
		return null;
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

	public Team get_footballer_team(GenericFootballer tar) {
		if (m_playerTeamFootballers.Contains(tar)) return Team.PlayerTeam;
		if (m_enemyTeamFootballers.Contains(tar)) return Team.EnemyTeam;
		return Team.None;
	}

	public bool footballer_has_ball(GenericFootballer tar) {
		if (get_footballer_team(tar) == Team.PlayerTeam) {
			return m_playerTeamFootballersWithBall.Contains(tar);
		} else {
			return m_enemyTeamFootballersWithBall.Contains(tar);
		}
	}

	public GenericFootballer nullableCurrentFootballerWithBall() {
		if (m_playerTeamFootballersWithBall.Count > 0) return m_playerTeamFootballersWithBall[0];
		if (m_enemyTeamFootballersWithBall.Count > 0) return m_enemyTeamFootballersWithBall[0];
		return null;
	}

	public Vector3 currentBallPosition() {
		GenericFootballer ball_holder = nullableCurrentFootballerWithBall();
		if (ball_holder != null) {
			return ball_holder.transform.position;
		} else {
			if (m_looseBalls.Count > 0) return m_looseBalls[0].transform.position;
			return Vector3.zero;
		}
	}
}

public enum Team {
	PlayerTeam,
	EnemyTeam,
	None
}