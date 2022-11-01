using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fractal : MonoBehaviour
{
    [SerializeField, Range(1, 8)]
    int depth = 4;
    // Start is called before the first frame update
    void Start()
    {
        if (depth == 0)
        {
            return;
        }
        name = "Fractal_" + depth;

        Fractal child = CreateChild(Vector3.up, Quaternion.identity);
        Fractal child1 = CreateChild(Vector3.right, Quaternion.Euler(0, 0, -90));
        Fractal child2 = CreateChild(Vector3.left, Quaternion.Euler(0, 0, 90));
        //Fractal child3 = CreateChild(Vector3.forward, Quaternion.Euler(90, 0, 0));
        //Fractal child4 = CreateChild(Vector3.back, Quaternion.Euler(-90, 0, 0));
    }

    Fractal CreateChild(Vector3 direction, Quaternion rotation)
    {
        Fractal child = Instantiate(this);
        child.depth = depth - 1;
        child.transform.localPosition = 0.5f * direction;
        child.transform.localScale = 0.5f * Vector3.one;
        child.transform.localRotation = rotation;

        child.transform.SetParent(this.transform, false);
        return child;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
