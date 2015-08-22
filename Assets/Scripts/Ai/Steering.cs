using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Steering : MonoBehaviour {
	[SerializeField]
	private float _maxSpeed = 200.0f;
	[SerializeField]
	private float _maxForce = 100.0f;
	[SerializeField]
	private float _mass = 1.0f;
	[SerializeField]
	private float _slowRadius = 100.0f;
	[SerializeField]
	private float _stopRadius = 50.0f;
	
	[System.Flags]
	private enum Mode {
		None = 0x0,
		Seek = 0x1,
		Arrive = 0x2,
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
	public void ArriveOn() {
		_currentMode |= Mode.Arrive;
	}
	public void ArriveOff() {
		_currentMode &= ~Mode.Arrive;
	}
	
	private bool IsOn(Mode mode) {
		return (mode & _currentMode) != 0;
	}
	
	public void Update() {
		_currentPosition = transform.localPosition;
		
		_steeringForce = Vector3.zero;
		_steeringForce = SumForces();
		_steeringForce = Vector3.ClampMagnitude(_steeringForce, _maxForce);
		
		_currentVelocity += (_steeringForce / _mass) * Time.deltaTime;
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
		
		if (IsOn(Mode.Arrive)) {
			deltaForce += Arrive(_currentTarget);
			
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
	
	private Vector3 Seek(Vector3 targetPos) {
		return ((targetPos - _currentPosition).normalized * _maxSpeed) - _currentVelocity;
	}
	
	private Vector3 Arrive(Vector3 targetPos) {
		float distance = Vector3.Distance(_currentPosition, targetPos);
		Vector3 desiredVelocity = (targetPos - _currentPosition).normalized;
		
		if (distance < _stopRadius) {
			desiredVelocity = Vector2.zero;
		} else if (distance < _slowRadius) {
			desiredVelocity = desiredVelocity * _maxSpeed * ((distance - _stopRadius) / (_slowRadius - _stopRadius));
		} else {
			desiredVelocity = desiredVelocity * _maxSpeed;
		}

		return desiredVelocity - _currentVelocity;
	}
}
