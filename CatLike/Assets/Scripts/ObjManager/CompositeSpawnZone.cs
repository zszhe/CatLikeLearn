using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSaver;

public class CompositeSpawnZone : SpawnZone
{
    [SerializeField]
    SpawnZone[] spawnZones;

    [SerializeField]
    bool sequential;

    [SerializeField]
    bool overrideConfig;

    int nextSequentialIndex;

    public override Vector3 SpawnPoint
    {
        get
        {
            int index = sequential ? nextSequentialIndex++ : Random.Range(0, spawnZones.Length);
            if(nextSequentialIndex >= spawnZones.Length)
            {
                nextSequentialIndex = 0;
            }

            return spawnZones[index].SpawnPoint;
        }
    }

    public override void ConfigureSpawn(Shape shape)
    {
        if (overrideConfig)
        {
            base.ConfigureSpawn(shape);
        }
        else
        {
            int index;
            if (sequential)
            {
                index = nextSequentialIndex++;
                if (nextSequentialIndex >= spawnZones.Length)
                {
                    nextSequentialIndex = 0;
                }
            }
            else
            {
                index = Random.Range(0, spawnZones.Length);
            }

            spawnZones[index].ConfigureSpawn(shape);
        }
    }

    public override void Save(GameDataWritter writer)
    {
        writer.Write(nextSequentialIndex);
    }

    public override void Load(GameDataReader reader)
    {
        nextSequentialIndex = reader.ReadInt();
    }
}
