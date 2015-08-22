using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	[SerializeField]
	private float _moveSpeed = 10.0f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 moveVec = Vector3.zero;
		if(Input.GetKey(KeyCode.UpArrow)) {
			moveVec += Vector3.up * Time.deltaTime * _moveSpeed;
		}
		if(Input.GetKey(KeyCode.DownArrow)) {
			moveVec -= Vector3.up * Time.deltaTime * _moveSpeed;
		}
		if(Input.GetKey(KeyCode.RightArrow)) {
			moveVec += Vector3.right * Time.deltaTime * _moveSpeed;
		}
		if(Input.GetKey(KeyCode.LeftArrow)) {
			moveVec -= Vector3.right * Time.deltaTime * _moveSpeed;
		}
		
		// TODO: omg
		if (!Mathf.Approximately(moveVec.magnitude, 0.0f)) {
			float rad = 25.0f * 0.5f;
			Vector3 newPos = transform.localPosition + moveVec * rad;
			bool coll = Physics2D.Linecast(transform.localPosition, newPos, 1 << LayerMask.NameToLayer("Ground"));  
			
			if (!coll) {
				transform.localPosition += moveVec;
			}
		}
	}
}
