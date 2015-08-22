﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(CircleCollider2D))]
public class GenericFootballer : MonoBehaviour {

	[SerializeField] private Sprite _image_noball;
	[SerializeField] private Sprite _image_holding_ball;
	[SerializeField] private Sprite _image_enemy_noball;
	[SerializeField] private Sprite _image_enemy_holding_ball;

	[SerializeField] private SpriteRenderer _renderer;
	[SerializeField] public float _waitdelay;

	private float _select_reticule_theta;
	[SerializeField] private SpriteRenderer _select_reticule;
	private float _select_reticule_target_alpha = 0.0f;
	public void set_select_reticule_alpha(float tar) {
		_select_reticule_target_alpha = tar;
	}

	public enum GenericFootballerMode {
		Idle,
		CommandMoving
	}

	[SerializeField] public GenericFootballerMode _current_mode;

	private static int __alloct = 0;
	[SerializeField] private int _id = 0;
	
	public void sim_initialize() {
		_id = ++__alloct;
		_current_mode = GenericFootballerMode.Idle;
		Color color = _select_reticule.color;
		color.a = 0;
		_select_reticule.color = color;
		this.set_select_reticule_alpha(0);
		_throw_charge_ct = 0;
	}

	public void image_update() {
		if (Main.LevelController.get_footballer_team(this) == Team.PlayerTeam) {
			if (Main.LevelController.footballer_has_ball(this)) {
				_renderer.sprite = _image_holding_ball;
			} else {
				_renderer.sprite = _image_noball;
			}

		} else if (Main.LevelController.get_footballer_team(this) == Team.EnemyTeam) {
			if (Main.LevelController.footballer_has_ball(this)) {
				_renderer.sprite = _image_enemy_holding_ball;
			} else {
				_renderer.sprite = _image_enemy_noball;
			}

		}
	}

	void Update() {
		float reticule_anim_speed = 0.5f;
		float reticule_scale = 160;
		if (Main.LevelController.footballer_has_ball(this)) {
		} else if (Main.LevelController.m_timeoutSelectedFootballer == this) {
			reticule_anim_speed = 1.5f;
			reticule_scale = 250;

		} else if (_selected_ct > 0) {
			reticule_anim_speed = 1.5f;
			reticule_scale = 200;

		}
		_selected_ct -= Util.dt_scale;

	
		_select_reticule_theta += reticule_anim_speed * Util.dt_scale;
		Util.transform_set_euler_world(_select_reticule.transform,new Vector3(0,0,_select_reticule_theta));
		Color color = _select_reticule.color;
		color.a = Util.drpt(color.a,_select_reticule_target_alpha,1/4.0f);
		_select_reticule.color = color;
		
		_select_reticule.transform.localScale = Util.valv(reticule_scale);
		
	}

	[SerializeField] public float _throw_charge_ct;
	private List<Vector3> __tmp = new List<Vector3>();
	private bool _ball_charging = false;
	public void sim_update() {
		this.image_update();
		if (Main.LevelController.get_footballer_team(this) == Team.PlayerTeam) {
			this.playerteam_sim_update();
		} else {
			this.enemyteam_sim_update();
		}
	}

	private void enemyteam_sim_update() {

	}

	private void playerteam_sim_update() {
		this.set_select_reticule_alpha(0.0f);
		Vector3 last_pos = transform.position;
		
		if (_waitdelay > 0) {
			_waitdelay -= Util.dt_scale;
			
		} else if (Main.LevelController.footballer_has_ball(this)) {
			Vector3 delta =  Util.vec_sub(Main.LevelController.GetMousePoint(),transform.position);
			Vector3 dir = delta.normalized;
			float mag = delta.magnitude;
			Main.LevelController.m_pathRenderer.clear_path(_id);
			
			if (Input.GetMouseButton(0)) {
				_ball_charging = true;
				_throw_charge_ct = Mathf.Min(_throw_charge_ct+Util.dt_scale*10.0f,mag);
				
				Vector3 p0 = this.transform.position;
				Vector3 p3 = Util.vec_add(p0,Util.vec_scale(dir,_throw_charge_ct));
				Vector3 p1 = new Vector3(
					Util.lerp(p0.x,p3.x,0.25f),
					Util.lerp(p0.y,p3.y,0.25f),
					-150
					);
				Vector3 p2 = new Vector3(
					Util.lerp(p0.x,p3.x,0.75f),
					Util.lerp(p0.y,p3.y,0.75f),
					-150
					);
				__tmp.Clear();
				for (float i = 0; i < 1.0f; i += 0.125f) {
					__tmp.Add(Util.bezier_val_for_t(p0,p1,p2,p3,i));
				}
				__tmp.Add(p3);
				Main.LevelController.m_pathRenderer.id_draw_path(_id,this.transform.position,__tmp.ToArray());
				
			} else if (!Input.GetKey(KeyCode.Z) && _ball_charging) {
				if (_throw_charge_ct > 100) {
					
					float vel = Mathf.Clamp(_throw_charge_ct/2000.0f * 6 + 3,3,10);
					Main.LevelController.CreateLooseBall(
						this.transform.position,
						Util.vec_scale(dir,vel)
						);
					Main.LevelController.m_playerTeamFootballersWithBall.Remove(this);
				}
				_ball_charging = false;
				_throw_charge_ct = 0;
				
			} else {
				_throw_charge_ct = 0;
				
				float scf = Mathf.Clamp(mag,0,200)/200.0f;
				float speed = 2.0f * Util.dt_scale;
				dir.Scale(Util.valv(scf*speed));
				transform.position = Util.vec_add(transform.position,dir);
			}
			
		} else if (_current_mode == GenericFootballerMode.Idle) {
			
		} else if (_current_mode == GenericFootballerMode.CommandMoving) {
			float speed = 2.0f * Util.dt_scale;
			Vector3 delta = Util.vec_sub(new Vector3(_command_move_to_point.x,_command_move_to_point.y),transform.position);
			if (delta.magnitude <= speed) {
				transform.position = new Vector3(_command_move_to_point.x,_command_move_to_point.y);
				_current_mode = GenericFootballerMode.Idle;
				
			} else {
				Vector3 dir = delta.normalized;
				dir.Scale(Util.valv(speed));
				transform.position = Util.vec_add(transform.position,dir);
			}
			
		}
		
		Vector3 rts = _renderer.transform.localScale;
		if (transform.position.x > last_pos.x) {
			rts.x = Mathf.Abs(rts.x) * 1;
		} else if (transform.position.x < last_pos.x) {
			rts.x = Mathf.Abs(rts.x) * -1;
		}
		_renderer.transform.localScale = rts;
	}

	public void timeout_start() {
		Main.LevelController.m_pathRenderer.clear_path(_id);
		if (_current_mode == GenericFootballerMode.CommandMoving) {
			Vector3 tar_pos = new Vector3(_command_move_to_point.x,_command_move_to_point.y);                         
			Main.LevelController.m_pathRenderer.id_draw_path(_id,this.transform.position,new Vector3[] { tar_pos });
		}
	}

	public void timeout_end() {
		Main.LevelController.m_pathRenderer.clear_path(_id);
	}

	public void timeout_update() {
		if (Main.LevelController.footballer_has_ball(this)) {
			this.set_select_reticule_alpha(0.0f);
		} else if (Main.LevelController.m_timeoutSelectedFootballer == this) {
			this.set_select_reticule_alpha(0.75f);
		} else {
			this.set_select_reticule_alpha(0.25f);
		}

	}

	[SerializeField] private Vector2 _command_move_to_point;
	public void CommandMoveTo(Vector2 pos) {
		_command_move_to_point = pos;
		_current_mode = GenericFootballerMode.CommandMoving;

		Main.LevelController.m_pathRenderer.clear_path(_id);
		Vector3 tar_pos = new Vector3(_command_move_to_point.x,_command_move_to_point.y);                         
		Main.LevelController.m_pathRenderer.id_draw_path(_id,this.transform.position,new Vector3[] { tar_pos });
	}

	public bool ContainsPoint(Vector3 pt) {
		CircleCollider2D col = this.GetComponent<CircleCollider2D>();
		return Vector3.Distance(pt,this.transform.position) < col.radius;
	}

	[SerializeField] private float _selected_ct;
	public void SetSelectedForAFrame() {
		_selected_ct = 4;
	}

	public bool collider_contains(CircleCollider2D col) {
		float rads = col.radius + this.GetComponent<CircleCollider2D>().radius;
		return Vector3.Distance(col.transform.position,this.transform.position) < rads;
	}

}