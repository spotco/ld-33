using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Main : Uzu.Main
{
	public static Uzu.UiPanelMgr PanelManager {
		get { return _instance._panelManager; }
	}
	public static LevelController LevelController {
		get { return _instance._levelController; }
	}
	public static FanController FanController {
		get { return _instance._fanController; }
	}
	public static FollowCamera GameCamera {
		get { return _instance._gameCamera; }
	}
	public static AudioController AudioController {
		get { return _instance._audioController; }
	}
	
  protected override void OnMainBegin ()
  {
	Application.targetFrameRate = 60;
    _instance = (Main)Uzu.Main.Instance;
    
    // InitGraphicsSettings ();
    // InitInputSettings ();
    
    // Singleton creation.
    {
			_gameCamera = GameObject.Find("GameCamera").GetComponent<FollowCamera>();
			_fanController = GetComponent<FanController>();
    }
  }
  
  protected override void OnMainBegin2 ()
  {
    // State initialization.
    {
      // CyMain.Pause (CyPauseFlags.Player);
      
      // _panelManager.ChangeCurrentPanel (PanelIds.Main);	
      _panelManager.ChangeCurrentPanel (PanelIds.Game);	
    }
  }
	
	#region Pause functionality.
	private Uzu.PauseHelper _pauseObject = new Uzu.PauseHelper ();

	public static bool IsPaused (int flag)
	{
		return Instance._pauseObject.IsPaused (flag);
	}

	public static void Pause (int flag)
	{
		Instance._pauseObject.Pause (flag);
	}

	public static void Unpause (int flag)
	{
		Instance._pauseObject.Unpause (flag);
	}
	#endregion	
  
  private static Main _instance;

  public static new Main Instance {
    get { return _instance; }
  }
	
	[SerializeField]
	private Uzu.UiPanelMgr _panelManager;
	[SerializeField]
	private LevelController _levelController;
	private FollowCamera _gameCamera;
	private FanController _fanController;
	[SerializeField]
	private AudioController _audioController;
}
