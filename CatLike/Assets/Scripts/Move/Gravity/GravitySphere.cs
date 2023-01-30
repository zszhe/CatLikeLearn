using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySphere : GravitySource
{
    [SerializeField]
    float gravity = 9.81f;

    [SerializeField, Min(0f)]
    float outerRadius = 10f, outerFalloffRadius = 15f;

    float innerFalloffFactor, outerFalloffFactor;

    [SerializeField, Min(0f)]
    float innerRadius = 5f, innerFalloffRadius = 1f;

    

    // Start is called before the first frame update
    private void Awake()
    {
        
    }

    private void OnValidate()
    {
        innerFalloffRadius = Mathf.Max(0f, innerFalloffRadius);
        innerRadius = Mathf.Max(innerRadius, innerFalloffRadius);
        outerRadius = Mathf.Max(innerRadius, outerRadius);
        outerFalloffRadius = Mathf.Max(outerFalloffRadius, outerRadius);
        innerFalloffFactor = 1f / (innerRadius - innerFalloffRadius);
        outerFalloffFactor = 1f / (outerFalloffRadius - outerRadius);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override Vector3 GetGravity(Vector3 position)
    {
        Vector3 vector = transform.position - position;
        float distance = vector.magnitude;
        if(distance > outerFalloffRadius || distance < innerFalloffRadius)
        {
            return Vector3.zero;
        }

        float g = gravity / distance;

        // 处于衰减半径与正常半径中间的地带时
        if(distance > outerRadius)
        {
            g *= 1f - (distance - outerRadius) * outerFalloffFactor;
        }
        else if(distance < innerRadius)
        {
            g *= 1f - (innerRadius - distance) * innerFalloffFactor;
            g *= -1f;
        }

        return g * vector;
    }


    private void OnDrawGizmos()
    {
        Vector3 p = transform.position;
        if(innerFalloffRadius > 0f && innerFalloffRadius < innerRadius)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(p, innerFalloffRadius);
        }

        Gizmos.color = Color.yellow;
        if(innerRadius > 0f && innerRadius < outerRadius)
        {
            Gizmos.DrawWireSphere(p, innerRadius);
        }

        Gizmos.DrawWireSphere(p, outerRadius);
        if (outerFalloffRadius > outerRadius)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(p, outerFalloffRadius);
        }
    }
}
