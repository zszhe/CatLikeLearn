using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSaver
{
    [DisallowMultipleComponent]
    public class PresistableObject : MonoBehaviour
    {
        public virtual void Save(GameDataWritter writer)
        {
            writer.Write(transform.localPosition);
            writer.Write(transform.localRotation);
            writer.Write(transform.localScale);
        }

        public virtual void Load(GameDataReader reader)
        {
            transform.localPosition = reader.ReadVector3();
            transform.localRotation = reader.ReadQuaternion();
            transform.localScale = reader.ReadVector3();
        }
    }
}
