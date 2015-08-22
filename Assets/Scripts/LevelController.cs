using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class LevelController : MonoBehaviour {

	public static LevelController inst;
	void Start() {
		LevelController.inst = this;
	}

	[SerializeField] private GameObject proto_genericFootballer;

	private List<GenericFootballer> m_allFootballers = new List<GenericFootballer>();

	public void StartLevel() {
		m_allFootballers.Add(this.CreateFootballer(new Vector3(0,0)));

	}

	private GenericFootballer CreateFootballer(Vector3 pos) {
		GameObject neu_obj = Util.proto_clone(proto_genericFootballer);
		GenericFootballer rtv = neu_obj.GetComponent<GenericFootballer>();
		rtv.transform.position = pos;
		return rtv;
	}

	void Update () {

	}

}