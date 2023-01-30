using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CustomRigidbody : MonoBehaviour
{
    Rigidbody myBody;

    float delay;

    [SerializeField]
    bool allowDelay = false;

    private void Awake()
    {
        myBody = GetComponent<Rigidbody>();
        myBody.useGravity = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (allowDelay)
        {
            if (myBody.IsSleeping())
            {
                delay = 0f;
                return;
            }

            if (myBody.velocity.sqrMagnitude < 0.0001f)
            {
                delay += Time.deltaTime;
                if (delay >= 1f)
                {
                    return;
                }
            }
            else
            {
                delay = 0f;
            }
        }

        myBody.AddForce(CustomGravity.GetGravity(myBody.position), ForceMode.Acceleration);
    }
}
