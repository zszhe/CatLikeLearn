using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    public float speed = 10f;

    public Vector3 orbitAngle;
    public Vector3 pos;
    // Start is called before the first frame update
    void Start()
    {
        orbitAngle = transform.rotation.eulerAngles;
        pos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 input = new Vector3(
          Input.GetAxis("Vertical Camera"),
          Input.GetAxis("Horizontal Camera"), 0f);
        const float e = 1E-3f;
        if (Mathf.Abs(input.x) > e || Mathf.Abs(input.y) > e)
        {
            orbitAngle += speed * Time.deltaTime * input;
            transform.eulerAngles = orbitAngle;
        }

        Vector3 posInput = new Vector3(
          Input.GetAxis("Horizontal"),0
          , 0f);

        if (Mathf.Abs(posInput.x) > e)
        {
            pos = transform.position;
            pos += speed * Time.deltaTime * posInput;
            transform.position = pos;
        }
    }
}
