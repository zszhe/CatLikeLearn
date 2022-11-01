using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSphere : MonoBehaviour
{
    [SerializeField]
    Vector2 playerInput;

    [SerializeField, Range(1,100)]
    float speedRate = 20.0f;

    [SerializeField, Range(1, 100)]
    float accelerationRate = 20.0f;

    Vector3 velocity;

    [SerializeField]
    Rect allowedArea = new Rect(-5f, -5f, 10f, 10f);

    [SerializeField, Range(0,1)]
    float bounciness = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerInput.y = Input.GetAxis("Horizontal");
        playerInput.x = -Input.GetAxis("Vertical");
        // 归一化
        // playerInput.Normalize();
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);
        Vector3 disiredVelocity = new Vector3(playerInput.x, 0.0f, playerInput.y) * speedRate;
        float maxSpeedChange = accelerationRate * Time.deltaTime;

        velocity.x = Mathf.MoveTowards(velocity.x, disiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, disiredVelocity.z, maxSpeedChange);

        Vector3 newPosition = transform.localPosition + velocity * Time.deltaTime;
        //if(!allowedArea.Contains(new Vector2(newPosition.x, newPosition.z)))
        //{
        //    newPosition.x = Mathf.Clamp(newPosition.x, allowedArea.xMin, allowedArea.xMax);
        //    newPosition.z = Mathf.Clamp(newPosition.z, allowedArea.yMin, allowedArea.yMax);
        //}

        if(newPosition.x < allowedArea.xMin)
        {
            newPosition.x = allowedArea.xMin;
            velocity.x = -velocity.x * bounciness;
        }
        else if (newPosition.x > allowedArea.xMax)
        {
            newPosition.x = allowedArea.xMax;
            velocity.x = -velocity.x * bounciness;
        }
        if (newPosition.z < allowedArea.yMin)
        {
            newPosition.z = allowedArea.yMin;
            velocity.z = -velocity.z * bounciness;
        }
        else if (newPosition.z > allowedArea.yMax)
        {
            newPosition.z = allowedArea.yMax;
            velocity.z = -velocity.z * bounciness;
        }

        transform.localPosition = newPosition;
    }
}
