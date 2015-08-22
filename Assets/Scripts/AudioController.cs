using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour
{
	private const int MAX_AUDIO_SOURCES = 8;
	
	private const float FADE_IN_TIME = 1.0f;
	private const float FADE_OUT_TIME = 0.1f;

	private const float BGM_VOLUME = 0.7f;
	private const float SFX_VOLUME = 0.6f;

	/// <summary>
	/// Gets the base controller used by this object.
	/// Prevents having to re-expose all the base controller functionality.
	/// </summary>
	public Uzu.AudioController BaseController {
		get { return _audioController; }
	}

	public void PlayBgm (string clipId)
	{
		// Play sound.
		{
			Uzu.AudioController.PlayOptions options = new Uzu.AudioController.PlayOptions ();
			options.Loop = true;
			options.Volume = BGM_VOLUME;
			options.FadeInTime = FADE_IN_TIME;
			_activeBGMHandle = _audioController.Play (clipId, options);
		}
	}

	public void StopBgm ()
	{
		_audioController.Stop (_activeBGMHandle, FADE_OUT_TIME);
	}
	
	public void PlayEffect (string clipId)
	{
		Uzu.AudioController.PlayOptions options = new Uzu.AudioController.PlayOptions ();
		options.Volume = SFX_VOLUME;
		_audioController.Play (clipId, options);
	}
	
	#region Implementation.
	private Uzu.AudioLoader _audioLoader;
	private Uzu.AudioController _audioController;
	private Uzu.AudioHandle _activeBGMHandle;
	
	protected void Awake ()
	{
		_audioLoader = GetComponent<Uzu.AudioLoader> ();
		_audioController = this.gameObject.AddComponent<Uzu.AudioController> ();
		
		// Initialize audio controller.
		{
			Uzu.AudioControllerConfig config = new Uzu.AudioControllerConfig ();
			config.AudioSourceMaxCount = MAX_AUDIO_SOURCES;
			config.AudioLoader = _audioLoader;
			_audioController.Initialize (config);
		}
	}
	#endregion
}