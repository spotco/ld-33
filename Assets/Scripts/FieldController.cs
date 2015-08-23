using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldController : MonoBehaviour {
	List<Vector3> _regionPositions = new List<Vector3>();
	
	public Vector3 GetRegionPosition(int region) {
		if (region >= _regionPositions.Count) {
			Debug.Log("Invalid region: " + region);
			return Vector3.zero;
		}
		
		return _regionPositions[region];
	}
	
	private void Awake() {
		BoxCollider2D[] regions = this.GetComponentsInChildren<BoxCollider2D>();
		
		for (int i = 0; i < regions.Length; i++) {
			Vector3 cCenter = regions[i].offset;
			Vector3 worldPos = regions[i].transform.TransformPoint(cCenter);
			_regionPositions.Add(worldPos);
		}
		
		// for (int i = 0; i < _regionPositions.Count; i++) {
		// 	Uzu.Dbg.DrawSphere (_regionPositions[i], 50.0f, Color.red);
		// 	Debug.Log(_regionPositions[i]);
		// }
	}
}
