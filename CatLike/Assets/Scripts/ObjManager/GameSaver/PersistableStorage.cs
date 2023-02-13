using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GameSaver 
{
    public class PersistableStorage : MonoBehaviour
    {
        string savePath;

        private void Awake()
        {
            savePath = Path.Combine(Application.persistentDataPath, "saveFile");
        }

        public void Save(PersistableObject o, int version)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(savePath, FileMode.Create)))
            {
                writer.Write(-version);
                o.Save(new GameDataWritter(writer));
            }
        }

        public void Load(PersistableObject o)
        {
            byte[] data = File.ReadAllBytes(savePath);
            var reader = new BinaryReader(new MemoryStream(data));
            int version = -reader.ReadInt32();
            o.Load(new GameDataReader(reader, version));
        }
    } 
}
