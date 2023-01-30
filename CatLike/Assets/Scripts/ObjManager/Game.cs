using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{

    [SerializeField]
    Transform prefab;

    List<Transform> objects;

    public KeyCode createKey = KeyCode.C;
    public KeyCode newGameKey = KeyCode.N;

    private void Awake()
    {
        objects = new List<Transform>();
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
            CreateObject();
        }
        else if (Input.GetKeyDown(newGameKey))
        {
            BeginNewGame();
        }
    }

    private void CreateObject()
    {
        Transform obj = Instantiate(prefab);
        obj.localPosition = Random.insideUnitSphere * 5f;
        obj.localRotation = Random.rotation;
        obj.localScale = Vector3.one * Random.Range(0f, 1f);
        objects.Add(obj);
    }

    private void BeginNewGame()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            Destroy(objects[i]);
        }

        objects.Clear();
    }
}
