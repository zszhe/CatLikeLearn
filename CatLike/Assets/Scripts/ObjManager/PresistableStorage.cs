using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GameSaver 
{
    public class PresistableStorage : MonoBehaviour
    {
        string savePath;

        private void Awake()
        {
            savePath = Path.Combine(Application.persistentDataPath, "saveFile");
        }

        public void Save(PresistableObject o, int version)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(savePath, FileMode.Create)))
            {
                writer.Write(-version);
                o.Save(new GameDataWritter(writer));
            }
        }

        public void Load(PresistableObject o)
        {
            using (BinaryReader reader = new BinaryReader(File.Open(savePath, FileMode.Open)))
            {
                int version = -reader.ReadInt32();
                o.Load(new GameDataReader(reader, version));
            }
        }
    } 
}
