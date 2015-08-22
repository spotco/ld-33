using UnityEngine;
using System.Collections;

public class UiPanelGame : Uzu.UiPanel {
	public override void OnInitialize() {
		
	}
	
	public override void OnEnter(Uzu.PanelEnterContext context) {
		gameObject.SetActive(true);
	}
	
	public override void OnExit(Uzu.PanelExitContext context) {
		gameObject.SetActive(false);
	}
}
