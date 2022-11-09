using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CustomRigidbody : MonoBehaviour
{
    Rigidbody myBody;

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
        myBody.AddForce(CustomGravity.GetGravity(myBody.position), ForceMode.Acceleration);
    }
}
