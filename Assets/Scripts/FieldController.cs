using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldController : MonoBehaviour {
	[SerializeField]
	private Transform _rightGoalLineTop;
	[SerializeField]
	private Transform _rightGoalLineBottom;
	
	List<Vector3> _regionPositions = new List<Vector3>();
	
	public void GetRightGoalLinePositions(out Vector3 top, out Vector3 bottom) {
		top = _rightGoalLineTop.position;
		bottom = _rightGoalLineBottom.position;
	}
	
	public Vector3 GetRegionPosition(int region) {
		if (region >= _regionPositions.Count) {
			Debug.Log("Invalid region: " + region);
			return Vector3.zero;
		}
		
		return _regionPositions[region];
	}
	
	private void Awake() {
		// Grid.
		{
			BoxCollider2D[] regions = this.transform.Find("Grid").GetComponentsInChildren<BoxCollider2D>();
			
			for (int i = 0; i < regions.Length; i++) {
				Vector3 cCenter = regions[i].offset;
				Vector3 worldPos = regions[i].transform.TransformPoint(cCenter);
				_regionPositions.Add(worldPos);
			}
		}
	}
}
