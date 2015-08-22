using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class GenericFootballer : MonoBehaviour {

	private float _select_reticule_theta;
	[SerializeField] private SpriteRenderer _select_reticule;
	private float _select_reticule_target_alpha = 0.0f;
	public void set_select_reticule_alpha(float tar) {
		_select_reticule_target_alpha = tar;
	}

	enum GenericFootballerMode {
		Idle,
		CommandMoving
	}

	[SerializeField] private GenericFootballerMode _current_mode;

	private static int __alloct = 0;
	[SerializeField] private int _id = 0;
	
	public void sim_initialize() {
		_id = ++__alloct;
		_current_mode = GenericFootballerMode.Idle;
		Color color = _select_reticule.color;
		color.a = 0;
		_select_reticule.color = color;
		this.set_select_reticule_alpha(0);
	}

	void Update() {

		float reticule_anim_speed = 0.5f;
		float reticule_scale = 180;
		if (Main.LevelController.m_playerControlledFootballer == this) {
		} else if (Main.LevelController.m_timeoutSelectedFootballer == this) {
			reticule_anim_speed = 1.5f;
			reticule_scale = 250;

		}
	
		_select_reticule_theta += reticule_anim_speed * Util.dt_scale;
		Util.transform_set_euler_world(_select_reticule.transform,new Vector3(0,0,_select_reticule_theta));
		Color color = _select_reticule.color;
		color.a = Util.drpt(color.a,_select_reticule_target_alpha,1/4.0f);
		_select_reticule.color = color;
		
		_select_reticule.transform.localScale = Util.valv(reticule_scale);
		
	}

	public void sim_update() {
		this.set_select_reticule_alpha(0.0f);

		if (Main.LevelController.m_playerControlledFootballer == this) {
			if (Input.GetKey(KeyCode.Z)) {

			} else {
				Vector3 delta =  Util.vec_sub(Main.LevelController.GetMousePoint(),transform.position);
				Vector3 dir = delta.normalized;
				float mag = delta.magnitude;
				float scf = mag/200.0f;
				float speed = 5.0f * Util.dt_scale;
				dir.Scale(Util.valv(scf*speed));
				transform.position = Util.vec_add(transform.position,dir);
			}

		} else if (_current_mode == GenericFootballerMode.Idle) {

		} else if (_current_mode == GenericFootballerMode.CommandMoving) {
			float speed = 5.0f * Util.dt_scale;
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
		if (Main.LevelController.m_playerControlledFootballer == this) {
			this.set_select_reticule_alpha(0.0f);
		} else if (Main.LevelController.m_timeoutSelectedFootballer == this) {
			this.set_select_reticule_alpha(0.6f);
		} else {
			this.set_select_reticule_alpha(0.15f);
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
		SphereCollider col = this.GetComponent<SphereCollider>();
		return Vector3.Distance(pt,this.transform.position) < col.radius;
	}

}