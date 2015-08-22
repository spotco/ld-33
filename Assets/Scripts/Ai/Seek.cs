using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Seek : SteeringBehaviour
{
    public override Vector3 GetVelocity()
    {
        Vector3 r = ((Agent.TargetPoint - Agent.Position).normalized * Agent.MaxVelocity) - Agent.CurrentVelocity;
        return r;
    }
}