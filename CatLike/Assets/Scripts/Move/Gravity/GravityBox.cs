using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityBox : GravitySource
{
    [SerializeField]
    float gravity = 9.81f;

    [SerializeField]
    Vector3 boundDis = Vector3.one;

    [SerializeField, Min(0f)]
    float innerDis = 0f, innerFallOffDis = 0f;

    float innerFallOffFactor;

    private void Awake()
    {
        OnValidate();
    }

    private void OnValidate()
    {
        boundDis = Vector3.Max(boundDis, Vector3.zero);
        float maxInner = Mathf.Min(Mathf.Min(boundDis.x, boundDis.y), boundDis.z);
        innerDis = Mathf.Min(innerDis, maxInner);
        innerFallOffDis = Mathf.Max(Mathf.Min(maxInner, innerFallOffDis), innerDis);

        innerFallOffFactor = 1f / (innerFallOffDis - innerDis);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override Vector3 GetGravity(Vector3 position)
    {
        //position -= transform.position;
        position = transform.InverseTransformDirection(position - transform.position);
        Vector3 vector = Vector3.zero;
        Vector3 distances;
        distances.x = boundDis.x - Mathf.Abs(position.x);
        distances.y = boundDis.y - Mathf.Abs(position.y);
        distances.z = boundDis.z - Mathf.Abs(position.z);
        if(distances.x < distances.y)
        {
            if(distances.x < distances.z)
            {
                vector.x = GetGravityComponent(position.x, distances.x);
            }
            else
            {
                vector.z = GetGravityComponent(position.z, distances.z);
            }
        }
        else if (distances.y < distances.z)
        {
            vector.y = GetGravityComponent(position.y, distances.y);
        }
        else
        {
            vector.z = GetGravityComponent(position.z, distances.z);
        }
        return transform.TransformDirection(vector);
    }

    float GetGravityComponent(float coordinate, float distance)
    {
        if(distance > innerFallOffDis)
        {
            return 0f;
        }

        float g = gravity;
        if(distance > innerDis)
        {
            g *= 1f - (distance - innerDis) * innerFallOffFactor;
        }

        return coordinate > 0f ? -g : g;
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Vector3 size;
        if(innerFallOffDis > innerDis)
        {
            Gizmos.color = Color.cyan;
            size.x = 2f * (boundDis.x - innerFallOffDis);
            size.y = 2f * (boundDis.y - innerFallOffDis);
            size.z = 2f * (boundDis.z - innerFallOffDis);
            Gizmos.DrawWireCube(Vector3.zero, size);
        }

        if (innerDis > 0f)
        {
            Gizmos.color = Color.yellow;
            size.x = 2f * (boundDis.x - innerDis);
            size.y = 2f * (boundDis.y - innerDis);
            size.z = 2f * (boundDis.z - innerDis);
            Gizmos.DrawWireCube(Vector3.zero, size);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, 2f * boundDis);
    }
}
