using UnityEngine;
using System.Collections;

public class GenericFootballer : MonoBehaviour {

	enum GenericFootballerMode {
		Idle,
		CommandMoving
	}

	[SerializeField] private float _moveSpeed = 10.0f;
	
	public void sim_initialize() {

	}

	public void sim_update(float dt_scale) {

	}

	public void CommandMoveTo(Vector2 pos) {

	}

}