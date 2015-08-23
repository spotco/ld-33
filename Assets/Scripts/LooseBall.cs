﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CircleCollider2D))]
public class LooseBall : MonoBehaviour {

	[SerializeField] public Vector2 _vel; 
	[SerializeField] public float _z, _vz;
	[SerializeField] private GameObject _ball;
	[SerializeField] private float _initial_uncatchable_ct;

	public void sim_initialize(Vector2 position, Vector2 vel) {
		this.transform.position = position;
		_vel = vel;
		_z = 0;
		_vz = Mathf.Min(vel.magnitude * 0.25f,15);
		_initial_uncatchable_ct = 20;
	}

	private void set_ball_z(float z) {
		Vector3 set = new Vector3(0,0,(-z-5)*2);
		_ball.transform.localPosition = set;
	}

	public void sim_update() {
		Vector3 pre_position = transform.position;
		Vector2 fvel = Util.vec_scale(_vel,Util.dt_scale);
		transform.position = Util.vec_add(transform.position,fvel);
		if (!Main.LevelController.m_ballBounds.OverlapPoint(transform.position)) {
			transform.position = pre_position;
			_vel.x *= -0.15f;
			_vel.y *= -0.15f;
		}

		_vz -= 0.05f * Util.dt_scale;
		_z += _vz;
		if (_z <= 0) {
			_z = 0;
			_vz = Mathf.Abs(_vz) * 0.5f;
			_vel.x *= 0.5f;
			_vel.y *= 0.5f;
		}
		this.set_ball_z(_z);

		this._initial_uncatchable_ct -= Util.dt_scale;
		if (this._initial_uncatchable_ct <= 0 && _z < 50) {
			for (int i = 0; i < Main.LevelController.m_playerTeamFootballers.Count; i++) {
				GenericFootballer itr = Main.LevelController.m_playerTeamFootballers[i];
				if (itr.can_pickup_ball() && itr.collider_contains(this.GetComponent<CircleCollider2D>())) {
					Main.LevelController.PickupLooseBall(this,itr);
					return;
				}
			}
			for (int i = 0; i < Main.LevelController.m_enemyTeamFootballers.Count; i++) {
				GenericFootballer itr = Main.LevelController.m_enemyTeamFootballers[i];
				if (itr.can_pickup_ball() && itr.collider_contains(this.GetComponent<CircleCollider2D>())) {
					Main.LevelController.PickupLooseBall(this,itr);
					return;
				}
			}
		}
	}
}
