using UnityEngine;
using System.Collections;

public class Referee : MonoBehaviour {

	[SerializeField] public GameObject m_RefereeTopStart;
	[SerializeField] public GameObject m_RefereeTopBase;
	[SerializeField] public GameObject m_RefereeBottomStart;
	[SerializeField] public GameObject m_RefereeBottomBase;

	public enum RefereeMode {
		Top, 
		Bottom
	}
	public RefereeMode _self_mode;
	private Vector3 _startpos,_basepos;

	public void sim_initialize(RefereeMode mode) {
		_self_mode = mode;
		if (_self_mode == RefereeMode.Bottom) {
			_startpos = m_RefereeBottomStart.transform.position;
			_basepos = m_RefereeBottomBase.transform.position;
		} else {
			_startpos = m_RefereeTopStart.transform.position;
			_basepos = m_RefereeTopBase.transform.position;
		}
		this.transform.position = _basepos;
	}

	public void sim_update() {
		Vector3 current_ball_pos = Main.LevelController.currentBallPosition();
		Vector3 target_pos = transform.position;
		if (current_ball_pos.magnitude != 0) {
			if (_self_mode == RefereeMode.Bottom && current_ball_pos.y < _basepos.y) {
				target_pos = current_ball_pos;
			} else if (_self_mode == RefereeMode.Top && current_ball_pos.y > _basepos.y) {
				target_pos = current_ball_pos;
			} else {
				target_pos = new Vector3(current_ball_pos.x,_basepos.y,_basepos.z);
			}
		}
		float speed = 8.0f * Util.dt_scale;
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
				if (_self_mode == RefereeMode.Top) {
					itr._vel = Util.vec_scale((new Vector2(Util.rand_range(-1,1),-1)).normalized,4);
				} else {
					itr._vel = Util.vec_scale((new Vector2(Util.rand_range(-1,1),1)).normalized,4);
				}

			}
		}

	}
}
