using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Steering {
	private Seek _seek = new Seek();
	private SteeringAgent _agent = new SteeringAgent();
	
	public Steering() {
		_seek.Agent = _agent;
		_agent.RegisterSteeringBehaviour(_seek);
	}
	
	public void SetTarget(Vector3 pos) {
		_agent.TargetPoint = pos;
	}
	
	public void EnableSeek(bool state) {
		_seek.IsEnabled = true;
	}
	
	public void Update(BotBase bot) {
		_agent.Update(bot);
	}
}
