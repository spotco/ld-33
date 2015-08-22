using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour {
	[SerializeField]
  private Transform _target;
	
	[SerializeField]
	private float _translateSmoothness;
	
	private Vector3 _offset;
	private Camera _cachedCamera;
	
	public void SetTarget(Transform target) {
		_target = target;
	}
	
	void Awake() {
		_offset = new Vector3(0.0f, 0.0f, transform.localPosition.z);
		_cachedCamera = this.GetComponent<Camera>();
	}
	
	void LateUpdate () {
		if (_target == null) {
			return;
		}
		
		float t = _translateSmoothness * Time.deltaTime;
		Vector3 newPos = Vector3.Lerp(transform.localPosition, _target.localPosition, t);
		newPos += _offset;
		transform.localPosition = newPos;
	}
}
