using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AnimatedGoalPost : MonoBehaviour {

	[SerializeField] private Sprite _sprite_mouth_open;
	[SerializeField] private Sprite _sprite_mouth_closed;

	[SerializeField] private GameObject _tongue;
	[SerializeField] private SpriteRenderer _top_renderer;

	private Vector3 _tongue_original_position;
	private float _tongue_theta, _tongue_theta_2;

	void Start () {
		_tongue_original_position = _tongue.transform.position;
	}

	void Update () {
		_tongue_theta += Util.dt_scale * 0.05f;
		_tongue_theta_2 += Util.dt_scale * 0.02f;
		_tongue.transform.position = _tongue_original_position + new Vector3(20 * Mathf.Sin(_tongue_theta),10 * Mathf.Sin(_tongue_theta_2),0);
	}
}
