using UnityEngine;
using System.Collections;

public class UiPanelMain : Uzu.UiPanel {
	public override void OnInitialize() {
		
	}
	
	public override void OnEnter(Uzu.PanelEnterContext context) {
		gameObject.SetActive(true);
		
		Main.AudioController.PlayBgm(AudioClipIds.MenuBgm);
	}
	
	public override void OnExit(Uzu.PanelExitContext context) {
		Main.AudioController.StopBgm();
		
		gameObject.SetActive(false);
	}
	
	public void OnStartClick() {
		Main.PanelManager.ChangeCurrentPanel(PanelIds.Game);
	}
}
