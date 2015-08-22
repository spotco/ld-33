using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Arrive : SteeringBehaviour
{
  public float SlowRadius = 100.0f;
  public float StopRadius = 50.0f;

  public override Vector3 GetVelocity()
  {
      float distance = Vector3.Distance(Agent.Position, Agent.TargetPoint);
      Vector3 desiredVelocity = (Agent.TargetPoint - Agent.Position).normalized;

      if (distance < StopRadius)
          desiredVelocity = Vector2.zero;
      else if (distance < SlowRadius)
          desiredVelocity = desiredVelocity * Agent.MaxVelocity * ((distance - StopRadius) / (SlowRadius - StopRadius));
      else
          desiredVelocity = desiredVelocity * Agent.MaxVelocity;

      return desiredVelocity - Agent.CurrentVelocity;
  }
}