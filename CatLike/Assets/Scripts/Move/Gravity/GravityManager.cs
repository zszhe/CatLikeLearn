using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityManager : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    KeyCode back;

    [SerializeField]
    KeyCode down;

    [SerializeField]
    KeyCode up;

    [SerializeField]
    KeyCode right;

    [SerializeField]
    KeyCode left;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(back))
        {
            Physics.gravity = new Vector3(9.81f, 0f, 0f);
        }
        else if (Input.GetKeyDown(down))
        {
            Physics.gravity = new Vector3(0f, -9.81f, 0f);
        }
        else if (Input.GetKeyDown(up))
        {
            Physics.gravity = new Vector3(0f, 9.81f, 0f);
        }
        else if (Input.GetKeyDown(left))
        {
            Physics.gravity = new Vector3(0f, 0f ,9.81f);
        }
        else if (Input.GetKeyDown(right))
        {
            Physics.gravity = new Vector3(0f ,0f, -9.81f);
        }
    }
}
