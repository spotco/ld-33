using UnityEngine;
using System.Collections;

public class Referee : MonoBehaviour {

	[SerializeField] public GameObject m_center;
	[SerializeField] public GameObject m_RefereeTopBase;
	[SerializeField] public GameObject m_leftGoalLine;
	[SerializeField] public GameObject m_rightGoalLine;
	[SerializeField] public GameObject m_RefereeBottomBase;

	[SerializeField] private SpriteRenderer _renderer;

	public enum RefereeMode {
		Top, 
		Bottom
	}
	public RefereeMode _self_mode;
	private Vector3 _center,_basepos,_left_goal_line,_right_goal_line;

	public void Update() {
		_renderer.sortingOrder = (int)(-transform.position.y * 100);	
	}

	public void sim_initialize(RefereeMode mode) {
		_self_mode = mode;
		if (_self_mode == RefereeMode.Bottom) {
			_center = m_center.transform.position;
			_left_goal_line = m_leftGoalLine.transform.position;
			_right_goal_line = m_rightGoalLine.transform.position;
			_basepos = m_RefereeBottomBase.transform.position;
		} else {
			_center = m_center.transform.position;
			_left_goal_line = m_leftGoalLine.transform.position;
			_right_goal_line = m_rightGoalLine.transform.position;
			_basepos = m_RefereeTopBase.transform.position;
		}
		this.transform.position = _basepos;
		_movespeed = 8.0f;
	}

	[SerializeField] public float _movespeed ;

	public void sim_update() {
		Vector3 current_ball_pos = Main.LevelController.currentBallPosition();
		Vector3 target_pos = transform.position;
		if (current_ball_pos.magnitude != 0) {
			if (!Main.LevelController.m_gameBounds.OverlapPoint(current_ball_pos)) {
				if (_self_mode == RefereeMode.Bottom && current_ball_pos.y < _center.y) {
					target_pos = current_ball_pos;
				} else if (_self_mode == RefereeMode.Top && current_ball_pos.y > _center.y) {
					target_pos = current_ball_pos;
				} else {
					target_pos = new Vector3(current_ball_pos.x,_basepos.y,_basepos.z);
				}
			} else {
				target_pos = new Vector3(current_ball_pos.x,_basepos.y + (_self_mode == RefereeMode.Top?40:-40),_basepos.z);
			}

		}
		float speed = _movespeed * Util.dt_scale;
		Vector3 delta =  Util.vec_sub(target_pos,transform.position);
		if (delta.magnitude < speed) {
			transform.position = target_pos;
		} else {
			Vector3 dir = delta.normalized;
			float mag = delta.magnitude;
			dir.Scale(Util.valv(speed));
			transform.position = Util.vec_add(transform.position,dir);
		}

		for (int i = 0; i < Main.LevelController.m_looseBalls.Count; i++ ) {
			LooseBall itr = Main.LevelController.m_looseBalls[i];
			if (Vector3.Distance(itr.transform.position,this.transform.position) < 40 && itr._vel.magnitude < 0.1f) {
				itr._vz = 3;
				itr._z = 0;
				Main.AudioController.PlayEffect("sfx_throw");
				if (_self_mode == RefereeMode.Top) {
					if (itr.transform.position.x < _left_goal_line.x) {
						itr._vel = Util.vec_scale((new Vector2(1,Util.rand_range(-0.15f,0))).normalized,4);

					} else if (itr.transform.position.x > _right_goal_line.x) {
						itr._vel = Util.vec_scale((new Vector2(-1,Util.rand_range(-0.15f,0))).normalized,4);

					} else {
						itr._vel = Util.vec_scale((new Vector2(Util.rand_range(-1,1),-1)).normalized,4);
					}


				} else {
					if (itr.transform.position.x < _left_goal_line.x) {
						itr._vel = Util.vec_scale((new Vector2(1,Util.rand_range(0,0.15f))).normalized,4);

					} else if (itr.transform.position.x > _right_goal_line.x) {
						itr._vel = Util.vec_scale((new Vector2(-1,Util.rand_range(0,0.15f))).normalized,4);

					} else {
						itr._vel = Util.vec_scale((new Vector2(Util.rand_range(-1,1),1)).normalized,4);
					}

				}

			}
		}

	}
}
