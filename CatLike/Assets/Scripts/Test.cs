using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform parent;
    public Transform child;

    void Start()
    {

    }

    public void Todo()
    {
        Debug.Log("parent localEulerAngles: " + parent.localEulerAngles);
        Debug.Log("parent localRotation: " + parent.localRotation);
        Debug.Log("parent eulerAngles: " + parent.eulerAngles);
        Debug.Log("parent rotation: " + parent.rotation);

        Debug.Log("child localEulerAngles: " + child.localEulerAngles);
        Debug.Log("child localRotation: " + child.localRotation);
        Debug.Log("child eulerAngles: " + child.eulerAngles);
        Debug.Log("child rotation: " + child.rotation);
    }
}
