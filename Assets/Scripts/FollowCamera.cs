using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour {
	[SerializeField]
	private float _translateSmoothness;
	
	private Vector3 _currentTargetPos;
	private Vector3 _offset;
	private Camera _cachedCamera;
	
	public void SetTarget(Transform target) {
		_currentTargetPos = target.localPosition;
	}
	
	public void SetTargetPos(Vector3 targetPos) {
		_currentTargetPos = targetPos;
	}
	
	void Awake() {
		_offset = transform.localPosition;
		_cachedCamera = this.GetComponent<Camera>();
		_currentTargetPos = this.transform.localPosition;
	}
	
	void LateUpdate () {
		// Position interp.
		{
			float t = _translateSmoothness * Time.deltaTime;
			Vector3 newPos = Vector3.Lerp(transform.localPosition, _currentTargetPos + _offset, t);
			transform.localPosition = newPos;
		}
	}
}
