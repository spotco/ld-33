using UnityEngine;
using System.Collections;

public class UIPanelTV : Uzu.UiPanel {

	[SerializeField] private GameObject _MNM_logo;

	public override void OnInitialize() {
	}

	public override void OnEnter(Uzu.PanelEnterContext context) {
		gameObject.SetActive(true);
		Main.GameCamera.gameObject.SetActive(false);
		Main.Instance._tvCamera.gameObject.SetActive(true);
		_MNM_logo.SetActive(true);

	}
	
	public override void OnExit(Uzu.PanelExitContext context) {
		gameObject.SetActive(false);
		Main.GameCamera.gameObject.SetActive(true);
		Main.Instance._tvCamera.gameObject.SetActive(false);
		_MNM_logo.SetActive(false);
	}

	
	
	private void Update() {

	}
}