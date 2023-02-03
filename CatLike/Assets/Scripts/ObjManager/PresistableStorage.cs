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

        public void Save(PresistableObject o)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(savePath, FileMode.Create)))
            {
                o.Save(new GameDataWritter(writer));
            }
        }

        public void Load(PresistableObject o)
        {
            using (BinaryReader reader = new BinaryReader(File.Open(savePath, FileMode.Open)))
            {
                o.Load(new GameDataReader(reader));
            }
        }
    } 
}
