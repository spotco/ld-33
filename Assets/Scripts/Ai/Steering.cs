using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Steering : MonoBehaviour {
	[SerializeField]
	private float _maxSpeed = 200.0f;
	[SerializeField]
	private float _maxForce = 100.0f;
	[SerializeField]
	private float _mass = 10.0f;
	[SerializeField]
	private float _friction = 0.05f;
	
	[System.Flags]
	private enum Mode {
		None = 0x0,
		Seek = 0x1,
	}
	
	private Mode _currentMode = Mode.None;
	private Vector3 _steeringForce;
	private Vector3 _currentPosition;
	private Vector3 _currentTarget;
	private Vector3 _currentVelocity;
	
	public void SetTarget(Vector3 pos) {
		_currentTarget = pos;
	}
	
	public void SeekOn() {
		_currentMode |= Mode.Seek;
	}
	
	public void SeekOff() {
		_currentMode &= ~Mode.Seek;
	}
	
	private bool IsOn(Mode mode) {
		return (mode & _currentMode) != 0;
	}
	
	public void Update() {
		_currentPosition = transform.localPosition;
		
		_steeringForce = Vector3.zero;
		_steeringForce = SumForces();
		_steeringForce = Vector3.ClampMagnitude(_steeringForce, _maxForce);
		
		_currentVelocity += _steeringForce / _mass;
		_currentVelocity -= _currentVelocity * _friction;
		_currentVelocity = Vector3.ClampMagnitude(_currentVelocity, _maxSpeed);
		
		transform.localPosition = _currentPosition + _currentVelocity * Time.deltaTime;
	}
	
	private Vector3 SumForces() {
		Vector3 deltaForce = Vector3.zero;
		
		if (IsOn(Mode.Seek)) {
			deltaForce += Seek(_currentTarget);
			
			if (!AccumulateForce(deltaForce)) {
				return _steeringForce;
			}
		}

		return _steeringForce;
	}
	
	private bool AccumulateForce(Vector3 forceToAdd) {
		float magnitudeSoFar = _steeringForce.magnitude;
		float magnitudeRemaining = _maxForce - magnitudeSoFar;

		if (magnitudeRemaining <= 0.0) {
				return false;
		}

		float deltaMagnitude = forceToAdd.magnitude;
		if (deltaMagnitude > magnitudeRemaining) {
				deltaMagnitude = magnitudeRemaining;
		}

		_steeringForce += forceToAdd.normalized * deltaMagnitude;

		return true;
	}
	
	private Vector3 Seek(Vector3 target) {
		return ((target - _currentPosition).normalized * _maxSpeed) - _currentVelocity;
	}
}
