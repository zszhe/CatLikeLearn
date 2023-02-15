using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSaver;
using Utility;

public abstract class SpawnZone : PersistableObject
{
    [SerializeField]
    SpawnConfiguration spawnConfig;

    public abstract Vector3 SpawnPoint { get; }

    public virtual void ConfigureSpawn(Shape shape) 
    {
        Transform trans = shape.transform;
        trans.localPosition = SpawnPoint;
        trans.localRotation = Random.rotation;
        trans.localScale = Vector3.one * spawnConfig.scale.RandomValueInRange;
        shape.AngularVelocity = Random.onUnitSphere * spawnConfig.angularSpeed.RandomValueInRange;
        shape.SetColor(spawnConfig.color.RandomInRange);

        Vector3 direction = transform.forward;
        switch (spawnConfig.movementDirection)
        {
            case SpawnConfiguration.MovementDirection.Upward:
                direction = transform.up;
                break;
            case SpawnConfiguration.MovementDirection.Outward:
                direction = (trans.localPosition - transform.position).normalized;
                break;
            case SpawnConfiguration.MovementDirection.Random:
                direction = Random.onUnitSphere;
                break;
        }

        shape.Velocity = direction * spawnConfig.speed.RandomValueInRange;
    }
}
