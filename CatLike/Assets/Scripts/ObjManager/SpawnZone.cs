using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSaver;

public abstract class SpawnZone : PersistableObject
{
    public abstract Vector3 SpawnPoint { get; }

}
