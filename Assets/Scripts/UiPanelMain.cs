using UnityEngine;
using System.Collections;

public class UiPanelMain : Uzu.UiPanel {
	public override void OnInitialize() {
		Debug.Log("INIT");
	}
	
	public override void OnActivate() {
		Debug.Log("ENTER MAIN");
	}
	
	public override void OnDeactivate() {
		
	}
}
