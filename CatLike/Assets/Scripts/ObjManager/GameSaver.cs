using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GameSaver
{
   public class GameDataWritter
    {
        BinaryWriter writer;

        public GameDataWritter(BinaryWriter writer)
        {
            this.writer = writer;
        }

        public void Write(int value)
        {
            writer.Write(value);
        }

        public void Write(Vector3 value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
        }

        public void Write(Quaternion value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
            writer.Write(value.w);
        }

        public void Write(Color value)
        {
            writer.Write(value.r);
            writer.Write(value.g);
            writer.Write(value.b);
            writer.Write(value.a);
        }
    }

    public class GameDataReader
    {
        BinaryReader reader;

        public GameDataReader(BinaryReader reader)
        {
            this.reader = reader;
        }

        public float ReadFloat()
        {
            return reader.ReadSingle();
        }

        public int ReadInt()
        {
            return reader.ReadInt32();
        }

        public Vector3 ReadVector3()
        {
            Vector3 result;
            result.x = reader.ReadSingle();
            result.y = reader.ReadSingle();
            result.z = reader.ReadSingle();
            return result;
        }

        public Quaternion ReadQuaternion()
        {
            Quaternion result;
            result.x = reader.ReadSingle();
            result.y = reader.ReadSingle();
            result.z = reader.ReadSingle();
            result.w = reader.ReadSingle();
            return result;
        }

        public Color ReadColor()
        {
            Color result;
            result.r = reader.ReadSingle();
            result.g = reader.ReadSingle();
            result.b = reader.ReadSingle();
            result.a = reader.ReadSingle();
            return result;
        }
    }
}
