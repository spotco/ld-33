using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class SpriteAnimator : MonoBehaviour {

	private class SpriteAnimator_Animation {
		public List<Sprite> _frames;
		public float _speed;
	}

	[SerializeField] private SpriteRenderer _tar;
	private Dictionary<string,SpriteAnimator_Animation> _anim_name_to_anim = new Dictionary<string,SpriteAnimator_Animation>();
	private float _ct;
	private int _i;
	private string _current_anim_name = "";

	public void add_anim(string name, List<Sprite> frames, float speed) {
		_anim_name_to_anim[name] = new SpriteAnimator_Animation() {
			_frames = frames,
			_speed = speed
		};
	}

	public void play_anim(string name) {
		if (_current_anim_name != name) {
			_current_anim_name = name;
			_ct = 0;
			_i = 0;
		}
	}

	void Update () {
		if (!this._anim_name_to_anim.ContainsKey(this._current_anim_name)) return;
		SpriteAnimator_Animation animation = this._anim_name_to_anim[this._current_anim_name];
		if (_i >= animation._frames.Count) _i = 0;
		_tar.sprite = animation._frames[_i];
		_ct -= Util.dt_scale;

		while (_ct <= 0) {
			_ct += animation._speed;
			_i++;
			if (_i >= animation._frames.Count) _i = 0;
		}
	}
}