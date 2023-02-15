using GameSaver;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCube : SpawnZone
{
    [SerializeField]
    bool surfaceOnly;

    public override Vector3 SpawnPoint 
    {
        get
        {
            Vector3 p;
            p.x = Random.Range(-0.5f, 0.5f);
            p.y = Random.Range(-0.5f, 0.5f);
            p.z = Random.Range(-0.5f, 0.5f);

            if (surfaceOnly)
            {
                int index = Random.Range(0, 3);
                p[index] = p[index] >= 0 ? 0.5f : -0.5f;
            }

            return transform.TransformPoint(p);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
}
