using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityPanel : GravitySource
{
    [SerializeField]
    float gravity = 9.81f;

    [SerializeField, Min(0f)]
    float range = 1f;
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
        Vector3 up = transform.up;
        float distance = Vector3.Dot(up, position - transform.position);
        if(distance > range)
        {
            return Vector3.zero;
        }

        float g = -gravity;
        if (distance > 0f)
        {
            g *= 1f - distance / range;
        }

        return g * up;
    }

    private void OnDrawGizmos()
    {
        Vector3 size = new Vector3(9f, 0f, 9f);
        Vector3 scale = transform.localScale;
        scale.y = range;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, scale);
        Gizmos.color = Color.yellow;
        Vector3 offset = new Vector3(2.5f, 0f, 2.5f);
        Gizmos.DrawWireCube(Vector3.zero + offset, size);
        if (range > 0)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(Vector3.up + offset, size);
        }
    }
}
