using UnityEngine;
using System.Collections;

/*
TODO:
- allow setable target and lerp around
- allow zooming in on target
- allow clamping to edge of world
 */

public class FollowCamera : MonoBehaviour {
	[SerializeField]
  private Transform _target;
	
	[SerializeField]
	private float _translateSmoothness;
	[SerializeField]
	private float _sizeSmoothness;
	
	private Vector3 _offset;
	private Camera _cachedCamera;
	private float _targetSize;
	
	public void SetTarget(Transform target) {
		_target = target;
	}
	
	public float TargetSize {
		get { return _targetSize; }
		set { _targetSize = value; }
	}
	
	void Awake() {
		_offset = new Vector3(0.0f, 0.0f, transform.localPosition.z);
		_cachedCamera = this.GetComponent<Camera>();
		_targetSize = _cachedCamera.orthographicSize;
	}
	
	private string _t = "T1";
	private float[] _t_size = { 250, 125 };
	private int _sizeIdx = 0;
	
	void Update() {
		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			Transform newTarget = null;
			if (_t == "T1") {
				newTarget = GameObject.Find("T2").transform;
				_t = "T2";
			} else {
				newTarget = GameObject.Find("T1").transform;
				_t = "T1";
			}
			SetTarget(newTarget);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2)) {
			_sizeIdx = (_sizeIdx + 1) % _t_size.Length;
			TargetSize = _t_size[_sizeIdx];
		}
	}
	
	void LateUpdate () {
		if (_target == null) {
			return;
		}
		
		// Position interp.
		{
			float t = _translateSmoothness * Time.deltaTime;
			Vector3 newPos = Vector3.Lerp(transform.localPosition, _target.localPosition, t);
			newPos += _offset;
			transform.localPosition = newPos;
		}
		
		// Size lerp.
		{
			float t = _sizeSmoothness * Time.deltaTime;
			_cachedCamera.orthographicSize = Mathf.Lerp(_cachedCamera.orthographicSize, _targetSize, t);
		}
	}
}
