using UnityEngine;
using System.Collections;

public class UiPanelGame : Uzu.UiPanel {

	public override void OnInitialize() {
		
	}
	
	public override void OnEnter(Uzu.PanelEnterContext context) {
		gameObject.SetActive(true);
		Main.LevelController.StartLevel();
	}
	
	public override void OnExit(Uzu.PanelExitContext context) {
		gameObject.SetActive(false);
	}
	
	private void Update() {
		if (Input.GetKeyDown(KeyCode.R)) {
			Main.LevelController.StartLevel();
		}
	}
}
