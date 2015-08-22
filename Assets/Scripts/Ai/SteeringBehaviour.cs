using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class SteeringBehaviour {
  public float Weight = 1;
  public SteeringAgent Agent {
    get; set;
  }
  
  public bool IsEnabled { get; set; }

  public abstract Vector3 GetVelocity();
}