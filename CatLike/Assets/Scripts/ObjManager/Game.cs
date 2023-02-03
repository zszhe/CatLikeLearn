using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSaver;

public class Game : PresistableObject
{
    const int saveVersion = 1;

    [SerializeField]
    ShapeFactory factory;

    List<Shape> objects;

    public PresistableStorage storage;

    public KeyCode createKey = KeyCode.C;
    public KeyCode newGameKey = KeyCode.N;
    public KeyCode saveKey = KeyCode.S;
    public KeyCode loadKey = KeyCode.L;

    private void Awake()
    {
        objects = new List<Shape>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(createKey))
        {
            CreateShape();
        }
        else if (Input.GetKeyDown(newGameKey))
        {
            BeginNewGame();
        }
        else if (Input.GetKeyDown(saveKey))
        {
            storage.Save(this);
        }
        else if (Input.GetKeyDown(loadKey))
        {
            BeginNewGame();
            storage.Load(this);
        }
    }

    private void CreateShape()
    {
        Shape obj = factory.GetRandom();
        Transform trans = obj.transform;
        trans.localPosition = Random.insideUnitSphere * 5f;
        trans.localRotation = Random.rotation;
        trans.localScale = Vector3.one * Random.Range(0f, 1f);
        objects.Add(obj);
    }

    private void BeginNewGame()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            Destroy(objects[i].gameObject);
        }

        objects.Clear();
    }

    public override void Save(GameDataWritter writer)
    {
        writer.Write(-saveVersion);
        writer.Write(objects.Count);
        for (int i = 0; i < objects.Count; i++)
        {
            writer.Write(objects[i].ShapeId);
            writer.Write(objects[i].MaterialId);
            objects[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int version = -reader.ReadInt();
        if(version > saveVersion)
        {
            Debug.LogError("Unsurpported future save version" + version);
            return;
        }

        int count = version > 0 ? reader.ReadInt() : -version;
        for (int i = 0; i < count; i++)
        {
            int shapeId = version > 0 ? reader.ReadInt() : 0;
            int matId = version > 0 ? reader.ReadInt() : 0;
            Shape o = factory.Get(shapeId, matId);
            o.Load(reader);
            objects.Add(o);
        }
    }
}
