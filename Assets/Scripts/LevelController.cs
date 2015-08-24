using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class LevelController : MonoBehaviour {

	[SerializeField] private GameObject proto_team;
	[SerializeField] private GameObject proto_genericFootballer;
	[SerializeField] private GameObject proto_looseBall;
	[SerializeField] private GameObject proto_mouseTarget;
	[SerializeField] private GameObject proto_bloodParticle;
	[SerializeField] private GameObject proto_referee;

	public enum LevelControllerMode {
		Opening,
		GamePlay,
		Timeout
	}

	[SerializeField] public BoxCollider2D m_gameBounds;
	[SerializeField] public BoxCollider2D m_ballBounds;
	[SerializeField] public BoxCollider2D m_minGameBounds;
	[SerializeField] public AnimatedGoalPost m_playerGoal;
	[SerializeField] public AnimatedGoalPost m_enemyGoal;

	[System.NonSerialized] public PathRenderer m_pathRenderer;
	public List<GenericFootballer> m_playerTeamFootballers = new List<GenericFootballer>();
	public List<GenericFootballer> m_enemyTeamFootballers = new List<GenericFootballer>();


	public List<GenericFootballer> m_playerTeamFootballersWithBall = new List<GenericFootballer>();
	public List<GenericFootballer> m_enemyTeamFootballersWithBall = new List<GenericFootballer>();

	//Nullable
	public GenericFootballer m_timeoutSelectedFootballer;
	public List<LooseBall> m_looseBalls = new List<LooseBall>();
	
	public List<int> m_matchOpeningAnimIds = new List<int>();

	public SPParticleSystem m_particles;

	public LevelControllerMode m_currentMode;
	
	private TeamBase m_playerTeam;
	private TeamBase m_enemyTeam;

	private Referee m_topReferee, m_bottomReferee;
	
	private GameObject m_mouseTargetIcon;
	private float m_mouseTargetIconTheta;
	
	private Difficulty _currentDifficulty;
	public Difficulty CurrentDifficulty {
		get { return _currentDifficulty; }
		set {
			_currentDifficulty = value;
		}
	}
		
	public void StartLevel() {
		ResetLevel();
		
		Debug.Log("Start level: " + CurrentDifficulty);
		
		m_pathRenderer = this.GetComponent<PathRenderer>();
		
		m_playerTeam = this.CreateTeam(Team.PlayerTeam);
		m_enemyTeam = this.CreateTeam(Team.EnemyTeam);


		this.set_time_remaining_seconds(30);
		_player_team_score = 3;
		_enemy_team_score = 2;
		_quarter_display = "1ST";
		
		if (CurrentDifficulty == Difficulty.Easy) {
			{
				int[] regions = { 7, 6, 8 };
				FootballerResourceKey[] keys = { FootballerResourceKey.Player1, FootballerResourceKey.Player1, FootballerResourceKey.Player1 };
				FieldPosition[] fps = { FieldPosition.Keeper, FieldPosition.Defender, FieldPosition.Defender };
				SpawnTeam(7, m_playerTeam, regions, keys, fps);
			}
			{
				int[] regions = { 16, 12, 14 };
				FootballerResourceKey[] keys = { FootballerResourceKey.EnemyGoalie, FootballerResourceKey.Enemy3, FootballerResourceKey.Enemy3 };
				FieldPosition[] fps = { FieldPosition.Keeper, FieldPosition.Defender, FieldPosition.Defender };
				SpawnTeam(10, m_enemyTeam, regions, keys, fps);
			}
		} else if (CurrentDifficulty == Difficulty.Normal) {
			{
				int[] regions = { 7, 3, 4, 5 };
				FootballerResourceKey[] keys = { FootballerResourceKey.Player1, FootballerResourceKey.Player1, FootballerResourceKey.Player1, FootballerResourceKey.Player1 };
				FieldPosition[] fps = { FieldPosition.Keeper, FieldPosition.Defender, FieldPosition.Defender, FieldPosition.Attacker };
				SpawnTeam(7, m_playerTeam, regions, keys, fps);
			}
			{
				int[] regions = { 16, 12, 14, 13 };
				FootballerResourceKey[] keys = { FootballerResourceKey.EnemyGoalie, FootballerResourceKey.Enemy3, FootballerResourceKey.Enemy3, FootballerResourceKey.Enemy2 };
				FieldPosition[] fps = { FieldPosition.Keeper, FieldPosition.Defender, FieldPosition.Defender, FieldPosition.Attacker };
				SpawnTeam(10, m_enemyTeam, regions, keys, fps);
			}
		} else {
			{
				int[] regions = { 7, 3, 5, 6, 8 };
				FootballerResourceKey[] keys = { FootballerResourceKey.Player1, FootballerResourceKey.Player1, FootballerResourceKey.Player1, FootballerResourceKey.Player1, FootballerResourceKey.Player1 };
				FieldPosition[] fps = { FieldPosition.Keeper, FieldPosition.Defender, FieldPosition.Defender, FieldPosition.Attacker, FieldPosition.Attacker };
				SpawnTeam(7, m_playerTeam, regions, keys, fps);
			}
			{
				int[] regions = { 16, 12, 14, 9, 11 };
				FootballerResourceKey[] keys = { FootballerResourceKey.EnemyGoalie, FootballerResourceKey.Enemy3, FootballerResourceKey.Enemy3, FootballerResourceKey.Enemy2, FootballerResourceKey.Enemy2 };
				FieldPosition[] fps = { FieldPosition.Keeper, FieldPosition.Defender, FieldPosition.Defender, FieldPosition.Attacker, FieldPosition.Attacker };
				SpawnTeam(10, m_enemyTeam, regions, keys, fps);
			}
		}
		
		// Debugging.
		if (Main.FSMDebugger != null) {
			Main.FSMDebugger.Team = m_enemyTeam;
		}
		
		m_playerTeamFootballersWithBall.Add(m_playerTeamFootballers[0]);
		if (m_mouseTargetIcon == null) {
			m_mouseTargetIcon = Util.proto_clone(proto_mouseTarget);
		}
		if (m_particles == null) {
			m_particles = SPParticleSystem.cons_anchor(Main.Instance._particleRoot.transform);
		}
		
		if (m_topReferee == null) {
			m_topReferee = Util.proto_clone(proto_referee).GetComponent<Referee>();
			m_bottomReferee = Util.proto_clone(proto_referee).GetComponent<Referee>();
		}
		m_topReferee.sim_initialize(Referee.RefereeMode.Top);
		m_bottomReferee.sim_initialize(Referee.RefereeMode.Bottom);
		
		DoMatchOpening();
	}
	
	private void DoMatchOpening() {
		m_currentMode = LevelControllerMode.Opening;
		
		// hide cursor
		m_mouseTargetIcon.SetActive(false);
		
		List<BotBase> allBots = new List<BotBase>(
			m_playerTeam.TeamMembers.Count + m_enemyTeam.TeamMembers.Count);
		allBots.AddRange(m_playerTeam.TeamMembers);
		allBots.AddRange(m_enemyTeam.TeamMembers);
		
		for (int i = 0; i < allBots.Count; i++) {
			BotBase bot = allBots[i];
			GenericFootballer footballer = bot.GetComponent<GenericFootballer>();
			footballer.force_play_animation(FootballerAnimResource.ANIM_RUN);
			footballer.force_facing_direction(bot.HomePosition.x >= bot.transform.position.x ? true : false);
			
			float d = Vector3.Distance(bot.transform.position, bot.HomePosition);
			float r = Util.rand_range(200.0f, 220.0f);
			float t = d / r;
			
			LTDescr animDesc = LeanTween.move(
				bot.gameObject,
				bot.HomePosition,
				t)
				.setEase(LeanTweenType.linear);
			int animId = animDesc.id;
			animDesc.setOnComplete(() => {
				footballer.force_play_animation(FootballerAnimResource.ANIM_IDLE);
				footballer.force_facing_direction(bot.Team == Team.PlayerTeam ? true : false);
				
				m_matchOpeningAnimIds.Remove(animId);
			});
			
			m_matchOpeningAnimIds.Add(animId);
		}
	}
	
	private void ResetLevel() {
		if (m_pathRenderer != null) {
			m_pathRenderer.clear_paths();
		}
		
		m_matchOpeningAnimIds.Clear();
		
		if (m_playerTeam != null) {
			TeamBase team = m_playerTeam.GetComponent<TeamBase>();
			foreach (BotBase member in team.TeamMembers) {
				GameObject.Destroy(member.gameObject);
			}
			GameObject.Destroy(m_playerTeam.gameObject);
		}
		
		if (m_enemyTeam != null) {
			TeamBase team = m_enemyTeam.GetComponent<TeamBase>();
			foreach (BotBase member in team.TeamMembers) {
				GameObject.Destroy(member.gameObject);
			}
			GameObject.Destroy(m_enemyTeam.gameObject);
		}
		
		if (m_looseBalls != null) {
			for (int i = 0; i < m_looseBalls.Count; i++) {
				GameObject.Destroy(m_looseBalls[i].gameObject);
			}
			m_looseBalls.Clear();
		}
		
		m_playerTeamFootballers.Clear();
		m_enemyTeamFootballers.Clear();
		
		m_playerTeamFootballersWithBall.Clear();
		m_enemyTeamFootballersWithBall.Clear();
		
		// Memory cleanup.
		{
			System.GC.Collect();
			Resources.UnloadUnusedAssets();
		}
	}
	
	private void SpawnTeam(int centerRegion, TeamBase team,
		int[] regions, FootballerResourceKey[] resources, FieldPosition[] fieldPositions) {
		const float startOffset = 100.0f;
		Vector3 centerPos = Main.FieldController.GetRegionPosition(centerRegion);
		float deltaAngle = Mathf.Deg2Rad * (360.0f / regions.Length);
		List<BotBase> bots = new List<BotBase>(regions.Length);
		for (int i = 0; i < regions.Length; i++) {
			BotBase bot = this.CreateFootballer(team,
				centerPos + Uzu.Math.RadiansToDirectionVector(deltaAngle * i) * startOffset,
				SpriteResourceDB.get_footballer_anim_resource(resources[i]));
			bot.HomePosition = Main.FieldController.GetRegionPosition(regions[i]);
			bot.FieldPosition = fieldPositions[i];
			bots.Add(bot);
		}
		team.SetPlayers(bots);
	}

	public void Update () {
		if (Main.PanelManager.CurrentPanelId != PanelIds.Game) return;

		float mouse_target_anim_speed = 0.3f;
		float dt_scale = (1/60.0f)/(Time.deltaTime);
		Util.dt_scale = dt_scale;

		if (m_currentMode == LevelControllerMode.GamePlay) {
			_time_remaining = Math.Max(0,_time_remaining-TimeSpan.FromSeconds(Time.deltaTime).Ticks);
			m_particles.i_update(this);
			if (m_playerTeamFootballersWithBall.Count > 0) {
				Main.GameCamera.SetTargetPos(m_playerTeamFootballersWithBall[0].transform.position);
				if (Input.GetMouseButton(0)) {
					Main.GameCamera.SetTargetZoom(600);
				} else {
					Main.GameCamera.SetTargetZoom(500);
				}
				if (Input.GetMouseButton(0)) {
					Main.GameCamera.SetManualOffset(new Vector3(0,0,0));
				} else {
					Main.GameCamera.SetManualOffset(new Vector3(150,0,0));
				}


			} else {
				Main.GameCamera.SetTargetPos(this.GetLastMousePointInBallBounds());
				Main.GameCamera.SetTargetZoom(600);
				Main.GameCamera.SetManualOffset(new Vector3(0,0,0));
			}


			mouse_target_anim_speed = 2.0f;
			m_mouseTargetIcon.SetActive(true);
			Vector3 mouse_pt = GetLastMousePointInBallBounds();
			m_mouseTargetIcon.transform.position = mouse_pt;
			m_mouseTargetIcon.transform.localScale = Util.valv(50.0f);

			for (int i = m_looseBalls.Count-1; i >= 0; i--) {
				LooseBall itr = this.m_looseBalls[i];	
				itr.sim_update();
			}

			for (int i = this.m_playerTeamFootballers.Count-1; i >= 0; i--) {
				GenericFootballer itr = this.m_playerTeamFootballers[i];	
				itr.sim_update();
				if (m_enemyGoal.box_collider().OverlapPoint(itr.transform.position) || m_playerGoal.box_collider().OverlapPoint(itr.transform.position)) {
					if (this.footballer_has_ball(itr)) {
						this.m_playerTeamFootballersWithBall.Remove(itr);
						this.CreateLooseBall(itr.transform.position,Vector2.zero);
					}
					this.blood_anim_at(itr.transform.position);
					this.m_playerTeamFootballers.RemoveAt(i);
					Destroy(itr.gameObject);
					m_enemyGoal.play_eat_anim(40);
					m_playerGoal.play_eat_anim(40);
				}
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
				Main.Pause(PauseFlags.TimeOut);
			}

			for (int i = m_looseBalls.Count-1; i >= 0; i--) {
				LooseBall itr = this.m_looseBalls[i];
				if (m_enemyGoal.box_collider().OverlapPoint(itr.transform.position)) {
					this.blood_anim_at(itr.transform.position);
					m_looseBalls.Remove(itr);
					Destroy(itr.gameObject);
					m_enemyGoal.play_eat_anim(40);
					this.enemy_goal_score();

				}
				if (m_playerGoal.box_collider().OverlapPoint(itr.transform.position)) {
					this.blood_anim_at(itr.transform.position);
					m_looseBalls.Remove(itr);
					Destroy(itr.gameObject);
					m_playerGoal.play_eat_anim(40);
					this.player_goal_score();

				}
			}
			m_bottomReferee.sim_update();
			m_topReferee.sim_update();


		} else if (m_currentMode == LevelControllerMode.Timeout) {

			Vector3 screen = Main.GameCamera.GetComponent<Camera>().WorldToScreenPoint(this.GetLastMousePointInBallBounds());
			screen.z = 0;

			Vector3 mouse_to_center_delta = Util.vec_sub(
				screen,
				new Vector2(Screen.width/2,Screen.height/2));
			float mmouse_move_rad = (Screen.width+Screen.height)/2.0f * 0.25f;
			if (mouse_to_center_delta.magnitude > mmouse_move_rad) {
				Vector3 n_mouse_to_center_delta = mouse_to_center_delta.normalized;
				Vector3 tar_delta = Util.vec_scale(n_mouse_to_center_delta,(mouse_to_center_delta.magnitude-mmouse_move_rad)*0.3f);
				Main.GameCamera.SetTargetPos(Util.vec_add(Main.GameCamera.GetCurrentPosition(),tar_delta));
			} else {
				Main.GameCamera.SetTargetPositionToCurrent();
			}

			Main.GameCamera.SetManualOffset(new Vector3(0,0,0));
			Main.GameCamera.SetTargetZoom(800);
			Vector3 mouse_pt = GetLastMousePointInBallBounds();
			GenericFootballer select_tar = this.IsPointTouchFootballer(mouse_pt,m_playerTeamFootballers);
			if (select_tar != null && select_tar.can_take_commands()) {
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
				Main.Unpause(PauseFlags.TimeOut);
			}
		} else if (m_currentMode == LevelControllerMode.Opening) {
			if (m_matchOpeningAnimIds.Count == 0) {
				m_currentMode = LevelControllerMode.GamePlay;
				m_mouseTargetIcon.SetActive(true);
				m_enemyTeam.StartMatch();
			}
		}

		m_mouseTargetIconTheta += mouse_target_anim_speed * Util.dt_scale;
		Util.transform_set_euler_world(m_mouseTargetIcon.transform,new Vector3(0,0,m_mouseTargetIconTheta));

	}

	private BotBase _prev_ball_owner;

	public void CreateLooseBall(Vector2 start, Vector2 vel) {
		// ai msg
		{
			if (_prev_ball_owner != null) {
				_prev_ball_owner.Msg_LostBall();
				_prev_ball_owner = null;
			}
		}
		
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
		
		// ai msg
		{
			BotBase new_ball_owner = tar.GetComponent<BotBase>();
			if (_prev_ball_owner != null) {
				_prev_ball_owner.Msg_LostBall();
			}
			new_ball_owner.Msg_GotBall();
			_prev_ball_owner = new_ball_owner;
		}
		
		Destroy(looseball.gameObject);
	}
	
	private TeamBase CreateTeam(Team team) {
		GameObject neu_obj = Util.proto_clone(proto_team);
		TeamBase rtv = neu_obj.GetComponent<TeamBase>();
		rtv.Team = team;
		return rtv;
	}
	
	private BotBase CreateFootballer(TeamBase team, Vector3 pos, FootballerAnimResource anims) {
		GameObject neu_obj = Util.proto_clone(proto_genericFootballer);
		GenericFootballer rtv = neu_obj.GetComponent<GenericFootballer>();
		rtv.transform.position = pos;
		rtv.sim_initialize(anims);
		if (team.Team == Team.PlayerTeam) {
			m_playerTeamFootballers.Add(rtv);
		} else {
			m_enemyTeamFootballers.Add(rtv);
		}
		return neu_obj.GetComponent<BotBase>();
	}

	public void blood_anim_at(Vector3 pos, int reps = 32) {
		for (int i = 0; i < reps; i++ ) {
			RotateFadeOutSPParticle tmp = RotateFadeOutSPParticle.cons(proto_bloodParticle);
			tmp.transform.position = pos;
			tmp.set_ctmax(35);
			float scale = Util.rand_range(25,100);
			tmp._scmax = scale;
			tmp._scmin = scale;
			tmp._alpha.x = 0.6f;
			tmp._alpha.y = 0.0f;
			tmp._vr = Util.rand_range(-30,30);
			tmp._velocity.x = Util.rand_range(-3,3);
			tmp._velocity.y = Util.rand_range(0,7);
			tmp._gravity = 0.2f;
			m_particles.add_particle(tmp);
		}
	}
	
	public Vector3 GetMousePoint() {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Plane gamePlane = new Plane(new Vector3(0,0,-1),new Vector3(0,0,0));
		float rayout;
		gamePlane.Raycast(ray,out rayout);
		return Util.vec_add(ray.origin,Util.vec_scale(ray.direction,rayout));
	}

	private Vector3 _last_mouse_point_in_ball_bounds;
	public Vector3 GetLastMousePointInBallBounds() {
		Vector3 mpt = this.GetMousePoint();
		if (m_gameBounds.OverlapPoint(mpt)) {
			_last_mouse_point_in_ball_bounds = mpt;
		
		} else {
			_last_mouse_point_in_ball_bounds = m_gameBounds.bounds.ClosestPoint(mpt);

		}
		return _last_mouse_point_in_ball_bounds;
	}
	
	private bool IsClickAndPoint(out Vector2 point) {
		if (Input.GetMouseButtonDown(0)) {
			point = GetLastMousePointInBallBounds();
			return true;
		}
		point = Vector2.zero;
		return false;
	}
	
	private GenericFootballer IsPointTouchFootballer(Vector3 pt, List<GenericFootballer> list) {
		for (int i = 0; i < list.Count; i++) {
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
		if (tar != -1 && tar < m_playerTeamFootballers.Count) {
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
	
	public Vector3 currentLooseBallVelocity() {
		if (m_looseBalls.Count > 0) {
			return m_looseBalls[0]._vel;
		}
		return Vector3.zero;
	}

	private void enemy_goal_score() {
		_enemy_team_score++;
	}
	
	private void player_goal_score() {
		_player_team_score++;
	}

	public void set_time_remaining_seconds(int seconds) {
		TimeSpan ticks = new TimeSpan(0,0,0,seconds);
		_time_remaining = ticks.Ticks;
	}

	private void sim_update_time() {

	}

	public string get_time_remaining_formatted() {
		TimeSpan ts = TimeSpan.FromTicks(_time_remaining);
		return  string.Format(@"{0:0}:{1:00}:{2:000}",ts.Minutes,ts.Seconds,ts.Milliseconds);
	}

	public int _player_team_score = 0;
	public int _enemy_team_score = 0;
	public long _time_remaining = 0;
	public string _quarter_display = "1ST";
}

public enum Team {
	PlayerTeam,
	EnemyTeam,
	None
}

public enum Difficulty {
	Easy,
	Normal,
	Hard
}
