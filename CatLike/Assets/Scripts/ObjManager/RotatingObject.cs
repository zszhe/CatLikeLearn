using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSaver;

public class RotatingObject : PersistableObject
{
    [SerializeField]
    Vector3 angularVelocity;

    private void FixedUpdate()
    {
        transform.Rotate(angularVelocity * Time.deltaTime);
    }
}
