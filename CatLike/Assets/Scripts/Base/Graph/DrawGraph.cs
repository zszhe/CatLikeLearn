using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawGraph : MonoBehaviour
{
    const int maxResolution = 1000;
    [SerializeField, Range(10, maxResolution)]
    private int resolution = 10;
    [SerializeField]
    private Transform prefab;
    private Transform[] prefabs;

    [SerializeField, Min(0f)]
    private float functionDuration = 1.0f, transitionDuration = 1.0f;

    bool transitioning;

    private float duration;

    public GraphFunctionName transitionFunction;

    public GraphFunctionName function;

    [SerializeField]
    private TransitionMode transitionMode = TransitionMode.Ciycle;
 
    ComputeBuffer positionBuffer;

    [SerializeField]
    ComputeShader computeShader = default;

    [SerializeField]
    Material material = default;

    [SerializeField]
    Mesh mesh = null;

    static readonly int positionsId = Shader.PropertyToID("_Positions");
    static readonly int resolutionId = Shader.PropertyToID("_Resolution");
    static readonly int stepId = Shader.PropertyToID("_Step");
    static readonly int timeId = Shader.PropertyToID("_Time");
    static readonly int scaleId = Shader.PropertyToID("_Scale");
    static readonly int transitionProgress = Shader.PropertyToID("_TransitionProgress");

    // Start is called before the first frame update
    void Start()
    {
        //prefab = Resources.Load<Transform>("Prefabs/Graph/Point");
        //if(prefab == null)
        //{
        //    Debug.Log("111");
        //}
        ////ToDrawGraph();
        ////InitPrefabs(); //初始化
        //InitPrefabs3D();
    }

    private void OnEnable()
    {
        positionBuffer = new ComputeBuffer(maxResolution * maxResolution, 3 * 4);
    }

    private void OnDisable()
    {
        positionBuffer.Release();
        positionBuffer = null;
    }

    // Update is called once per frame
    void Update()
    {
        duration += Time.deltaTime;
        if (transitioning)
        {
            if (duration >= transitionDuration)
            {
                duration -= transitionDuration;
                transitioning = false;
            }
        }
        else if (duration >= functionDuration)
        {
            duration -= functionDuration;
            transitionFunction = function;
            transitioning = true;
            PickNextFunction();
        }

        //if (transitioning)
        //{
        //    DrawSin3DTransition();
        //}
        //else
        //{
        //    //DrawSin();
        //    DrawSin3D();
        //}

        UpdateFunctionOnGpu();
    }

    void UpdateFunctionOnGpu()
    {
        if (positionBuffer != null)
        {
            var kernelIndex = (int)function + (int)(transitioning ? transitionFunction : function) * FunctionLibrary.functionCount;
            float step = 2.0f / resolution;
            computeShader.SetInt(resolutionId, resolution);
            computeShader.SetFloat(stepId, step);
            computeShader.SetFloat(timeId, Time.time);
            if(transitioning)
            {
                computeShader.SetFloat(transitionProgress, Mathf.SmoothStep(0.0f, 1.0f, duration / transitionDuration));
            }
            computeShader.SetBuffer(kernelIndex, positionsId, positionBuffer);
            int groups = Mathf.CeilToInt(resolution / 8.0f);
            computeShader.Dispatch(kernelIndex, groups, groups, 1);
            material.SetBuffer(positionsId, positionBuffer);
            material.SetVector(scaleId, new Vector4(step, 1.0f / step));
            var bounds = new Bounds(Vector3.zero, Vector3.one * (2.0f + 2.0f / resolution));
            Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, positionBuffer.count);
        }
    }

    void PickNextFunction()
    {
        function = transitionMode == TransitionMode.Ciycle ? FunctionLibrary.GetGraphFunctionName(function) 
            : FunctionLibrary.GetRandomGraphFunctionName(function);
    }

    void ToDrawGraph()
    {
        float step = 2f / resolution; // 不添加后缀f会导致编译器把除法当作整型数据处理。
        // Debug.Log(step);
        var scale = Vector3.one * step;
        var position = Vector3.right;
        Transform point;
        for(int i = 0; i < resolution; i++)
        {
            point = Instantiate(prefab);
            position.x = (i + 0.5f) * step - 1f;
            position.y = position.x * position.x * position.x;
            point.localScale = scale;
            point.localPosition = position;
            point.SetParent(this.transform, false);
        }
    }

    void InitPrefabs()
    {
        float step = 2f / resolution;
        var scale = Vector3.one * step;
        var position = Vector3.zero;
        prefabs = new Transform[resolution * resolution];
        for (int i = 0, z = 0; z < resolution; z++)
        {
            position.z = (z + 0.5f) * step - 1f;
            for (int x = 0; x < resolution; x++, i++)
            {
                prefabs[i] = Instantiate(prefab);
                position.x = (x + 0.5f) * step - 1f;
                prefabs[i].localPosition = position;
                prefabs[i].localScale = scale;
                prefabs[i].SetParent(this.transform, false);
            }
        }
    }

    void InitPrefabs3D()
    {
        float step = 2f / resolution;
        var scale = Vector3.one * step;
        prefabs = new Transform[resolution * resolution];
        for (int i = 0; i < prefabs.Length; i++)
        {
            Transform point = Instantiate(prefab);
            point.localScale = scale;
            point.SetParent(transform, false);
            prefabs[i] = point;
        }
    }

    void DrawSin()
    {
        float time = Time.time;
        Vector3 position;
        Transform point;
        GraphFunction graphFunction;
        graphFunction = FunctionLibrary.graphFunctions[(int)function];
        for (int i = 0; i < prefabs.Length; i++)
        {
            position = prefabs[i].localPosition;
            point = prefabs[i];
            position = graphFunction(position.x, position.z, time);
            point.localPosition = position;
        }
    }

    void DrawSin3D()
    {
        float time = Time.time;
        GraphFunction graphFunction = FunctionLibrary.graphFunctions[(int)function];
        float step = 2f / resolution;
        for(int i = 0, z = 0; z < resolution; z++)
        {
            float v = (z + 0.5f) * step - 1;
            for (int x = 0; x < resolution; x++, i++)
            {
                float u = (x + 0.5f) * step - 1;
                prefabs[i].localPosition = graphFunction(u, v, time);
            }
        }
    }

    void DrawSin3DTransition()
    {
        float time = Time.time;
        float progress = duration / transitionDuration;
        GraphFunction from = FunctionLibrary.graphFunctions[(int)transitionFunction];
        GraphFunction to = FunctionLibrary.graphFunctions[(int)function];
        float step = 2f / resolution;
        for (int i = 0, z = 0; z < resolution; z++)
        {
            float v = (z + 0.5f) * step - 1;
            for (int x = 0; x < resolution; x++, i++)
            {
                float u = (x + 0.5f) * step - 1;
                prefabs[i].localPosition = FunctionLibrary.Morph(u, v, time, from, to, progress);
            }
        }
    }
} 

