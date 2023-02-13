using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSaver;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : PersistableObject
{
    const int saveVersion = 3;

    [SerializeField]
    ShapeFactory factory;

    List<Shape> objects;

    public PersistableStorage storage;

    public KeyCode createKey = KeyCode.C;
    public KeyCode newGameKey = KeyCode.N;
    public KeyCode saveKey = KeyCode.S;
    public KeyCode loadKey = KeyCode.L;
    public KeyCode destroyKey = KeyCode.D;

    public float CreationSpeed { get; set; }

    public float DestructionSpeed { get; set; }

    float creationProgress;
    float destructionProgress;

    [SerializeField]
    Slider creationSpeedSlider;
    [SerializeField]
    Slider destructionSpeedSlider;

    public int levelCount;
    int loadedLevelSceneIndex;

    public Vector3 SpawnPosOfLevel 
    {
        get
        {
            return GameLevel.Current.SpawnPoint; 
        }
    }

    Random.State mainRandomState;

    [SerializeField]
    bool reseedOnLoad;

    private void Awake()
    {
        objects = new List<Shape>();
    }

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
        for(int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene loadedScene = SceneManager.GetSceneAt(i);
            if (loadedScene.name.Contains("Level "))
            {
                SceneManager.SetActiveScene(loadedScene);
                loadedLevelSceneIndex = loadedScene.buildIndex;
                return;
            }
        }
#endif

        BeginNewGame();
        StartCoroutine(CreateScene(1));
    }

    private void OnEnable()
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
            StartCoroutine(CreateScene(loadedLevelSceneIndex));
        }
        else if (Input.GetKeyDown(saveKey))
        {
            storage.Save(this, saveVersion);
        }
        else if (Input.GetKeyDown(loadKey))
        {
            BeginNewGame();
            storage.Load(this);
        }
        else if (Input.GetKeyDown(destroyKey))
        {
            DestroyShape();
        }
        else
        {
            for (int i = 1; i <= levelCount; i++)
            {
                if(Input.GetKeyDown(KeyCode.Alpha0 + i))
                {
                    BeginNewGame();
                    StartCoroutine(CreateScene(i));
                    return;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        creationProgress += Time.deltaTime * CreationSpeed;
        destructionProgress += Time.deltaTime * DestructionSpeed;

        while (creationProgress >= 1f)
        {
            creationProgress -= 1f;
            CreateShape();
        }

        while (destructionProgress >= 1f)
        {
            destructionProgress -= 1f;
            DestroyShape();
        }
    }

    private IEnumerator CreateScene(int sceneIndex)
    {
        enabled = false;
        if(loadedLevelSceneIndex > 0)
        {
            yield return SceneManager.UnloadSceneAsync(loadedLevelSceneIndex);
        }

        yield return SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneIndex));
        loadedLevelSceneIndex = sceneIndex;
        enabled = true;
    }

    private void CreateShape()
    {
        Shape obj = factory.GetRandom();
        Transform trans = obj.transform;
        trans.localPosition = SpawnPosOfLevel;
        trans.localRotation = Random.rotation;
        trans.localScale = Vector3.one * Random.Range(0f, 1f);
        objects.Add(obj);
    }

    private void DestroyShape()
    {
        if (objects.Count > 0)
        {
            int random = Random.Range(0, objects.Count);
            factory.Reclaim(objects[random]);
            objects[random] = objects[objects.Count - 1];
            objects.RemoveAt(objects.Count - 1);
        }
    }

    private void BeginNewGame()
    {
        Random.state = mainRandomState;
        int seed = Random.Range(0, int.MaxValue);
        mainRandomState = Random.state;
        Random.InitState(seed);

        creationSpeedSlider.value = CreationSpeed = 0f;
        destructionSpeedSlider.value = DestructionSpeed = 0;

        for (int i = 0; i < objects.Count; i++)
        {
            Destroy(objects[i].gameObject);
        }

        objects.Clear();
    }

    public override void Save(GameDataWritter writer)
    {
        writer.Write(objects.Count);
        writer.Write(Random.state);
        writer.Write(CreationSpeed);
        writer.Write(creationProgress);
        writer.Write(DestructionSpeed);
        writer.Write(destructionProgress);
        writer.Write(loadedLevelSceneIndex);
        GameLevel.Current.Save(writer);
        for (int i = 0; i < objects.Count; i++)
        {
            writer.Write(objects[i].ShapeId);
            writer.Write(objects[i].MaterialId);
            objects[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int version = reader.Version;
        if (version > saveVersion)
        {
            Debug.LogError("Unsurpported future save version" + version);
            return;
        }

        StartCoroutine(LoadGame(reader));
    }

    IEnumerator LoadGame(GameDataReader reader)
    {
        int version = reader.Version;
        int count = version > 0 ? reader.ReadInt() : -version;
        if (version >= 3)
        {
            Random.State state = reader.ReadRandomState();
            if (!reseedOnLoad)
            {
                Random.state = state;
            }

            creationSpeedSlider.value = CreationSpeed = reader.ReadFloat();
            creationProgress = reader.ReadFloat();
            destructionSpeedSlider.value = DestructionSpeed = reader.ReadFloat();
            destructionProgress = reader.ReadFloat();
        }

        yield return StartCoroutine(CreateScene(version < 2 ? 1 : reader.ReadInt()));
        if(version >= 3)
        {
            GameLevel.Current.Load(reader);
        }

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
