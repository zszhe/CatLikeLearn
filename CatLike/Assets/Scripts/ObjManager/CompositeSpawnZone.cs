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

    public override void Save(GameDataWritter writer)
    {
        writer.Write(nextSequentialIndex);
    }

    public override void Load(GameDataReader reader)
    {
        nextSequentialIndex = reader.ReadInt();
    }
}
