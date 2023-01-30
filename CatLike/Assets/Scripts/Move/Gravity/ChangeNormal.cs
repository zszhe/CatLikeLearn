using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class ChangeNormal : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int[] triangles = GetComponent<ProBuilderMesh>().GetMesh.triangles;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            int t = triangles[i];
            triangles[i] = triangles[i + 2];
            triangles[i + 2] = t;
        }

        GetComponent<ProBuilderMesh>().GetMesh.triangles = triangles;
        MeshCollider cc = gameObject.GetComponent<MeshCollider>();
        cc.sharedMesh = GetComponent<ProBuilderMesh>().GetMesh;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
