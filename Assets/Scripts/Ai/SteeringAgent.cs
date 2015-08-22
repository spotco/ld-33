using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SteeringAgent
{
    public float MaxVelocity {
      get {
        return _hack.MaxVelocity;
      }
    }
    public float Mass {
      get {
        return _hack.Mass;
      }
    }
    public float Friction {
      get {
        return _hack.Friction;
      }
    }
    
    public bool RotateSprite = true;

    public Vector3 CurrentVelocity;
    
    public BotBase _hack;
    public Vector3 Position {
      get {
        return _hack.transform.localPosition;
      }
    }
    
    public Vector3 TargetPoint {
      get; set;
    }

    private List<SteeringBehaviour> _behaviours = new List<SteeringBehaviour>();

    public void RegisterSteeringBehaviour(SteeringBehaviour behaviour)
    {
        _behaviours.Add(behaviour);
    }

    public void Update(BotBase bot)
    {
      _hack = bot;
  
        Vector3 acceleration = Vector3.zero;

        foreach (SteeringBehaviour behaviour in _behaviours)
        {
            if (behaviour.IsEnabled) {
                acceleration += behaviour.GetVelocity() * behaviour.Weight;
            }
        }

        CurrentVelocity += acceleration / Mass;

        CurrentVelocity -= CurrentVelocity * Friction;

        if (CurrentVelocity.magnitude > MaxVelocity)
            CurrentVelocity = CurrentVelocity.normalized * MaxVelocity;

        Transform transform = bot.transform;
        transform.localPosition = transform.localPosition + (Vector3)CurrentVelocity * Time.deltaTime;
    }
}