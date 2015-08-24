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
	public static FieldController FieldController {
		get { return _instance._fieldController; }
	}
	public static FSMDebugger FSMDebugger {
		get { return _instance._fsmDebugger; }
	}
	
  protected override void OnMainBegin ()
  {
	Application.targetFrameRate = 60;
	Cursor.visible = false;
	Main._current_level = GameLevel.Level1;
	Main._current_repeat_reason = RepeatReason.None;
	SpriteResourceDB.get_footballer_anim_resource(FootballerResourceKey.Player1);
    _instance = (Main)Uzu.Main.Instance;
    
    // InitGraphicsSettings ();
    // InitInputSettings ();
    
    // Singleton creation.
    {
			_gameCamera = GameObject.Find("GameCamera").GetComponent<FollowCamera>();
			_fanController = GetComponent<FanController>();
			_fieldController = GameObject.Find("FieldController").GetComponent<FieldController>();
    }
  }
  
  protected override void OnMainBegin2 ()
  {
		Main.LevelController.CurrentDifficulty = Difficulty.Easy;
		// Main.LevelController.CurrentDifficulty = Difficulty.Normal;
		// Main.LevelController.CurrentDifficulty = Difficulty.Hard;
		
    // State initialization.
    {
      // _panelManager.ChangeCurrentPanel (PanelIds.Main);	
      //_panelManager.ChangeCurrentPanel (PanelIds.Game);	
	  _panelManager.ChangeCurrentPanel(PanelIds.Tv);
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
	private FieldController _fieldController;
	[SerializeField]
	private FSMDebugger _fsmDebugger;

	[SerializeField]
	public GameObject _particleRoot;

	[SerializeField]
	public Camera _tvCamera;

	public void Update() {
		float dt_scale = (1/60.0f)/(Time.deltaTime);
		Util.dt_scale = dt_scale;
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Confined;
	}

	public static GameLevel _current_level;
	public static RepeatReason _current_repeat_reason;
}

public enum GameLevel {
	Level1,
	Level2,
	Level3,
	End
}

public enum RepeatReason {
	Timeout,
	ScoredOn,
	None
}
