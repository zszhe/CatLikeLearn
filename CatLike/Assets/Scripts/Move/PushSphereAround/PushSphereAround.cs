using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushSphereAround : MonoBehaviour
{
    Vector3 startPos;

    [SerializeField]
    Vector2 playerInput;

    [SerializeField, Range(1, 100)]
    float speedRate = 20.0f;

    [SerializeField, Range(1, 100)]
    float accelerationRate = 20.0f, airAccelerationRate = 1f;

    Vector3 velocity;

    Vector3 disiredVelocity;

    Rigidbody myBody;

    [SerializeField]
    bool desiredJump;

    [SerializeField, Range(0f, 10f)]
    float jumpHeight = 2f;

    [SerializeField, Range(0, 5)]
    int maxJumpCount = 1;

    int groundContactCount;

    [SerializeField]
    bool onGround => groundContactCount > 0;

    int jumpCount = 0;

    [SerializeField, Range(0, 90)]
    float maxGroundAngle = 25f;

    float minGroundDotProduct;

    Vector3 contactNormal;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.localPosition;
        myBody = GetComponent<Rigidbody>();
        OnValidate();
    }

    private void OnValidate()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetAll();
            return;
        }

        playerInput.y = Input.GetAxis("Horizontal");
        playerInput.x = -Input.GetAxis("Vertical");
        // 归一化
        // playerInput.Normalize();
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);
        disiredVelocity = new Vector3(playerInput.x, 0.0f, playerInput.y) * speedRate;

        //Vector3 newPosition = transform.localPosition + velocity * Time.deltaTime;
        //if(!allowedArea.Contains(new Vector2(newPosition.x, newPosition.z)))
        //{
        //    newPosition.x = Mathf.Clamp(newPosition.x, allowedArea.xMin, allowedArea.xMax);
        //    newPosition.z = Mathf.Clamp(newPosition.z, allowedArea.yMin, allowedArea.yMax);
        //}

        //transform.localPosition = newPosition;
        desiredJump |= Input.GetButtonDown("Jump") & onGround;
    }

    private void ResetAll()
    {
        myBody.velocity = Vector3.zero;
        transform.localPosition = startPos;
    }

    private void ClearState()
    {
        groundContactCount = 0;
        contactNormal = Vector3.zero;
    }

    private void FixedUpdate()
    {
        UpdateState();
        AdjustVelocity();

        if (desiredJump && onGround && jumpCount < maxJumpCount)
        {
            desiredJump = false;
            Jump();
        }

        myBody.velocity = velocity;
        ClearState();
    }

    void Jump()
    {
        jumpCount++;
        float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
        float alignedSpeed = Vector3.Dot(contactNormal, velocity);
        if(alignedSpeed > 0)
        {
            jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
        }

        velocity += contactNormal * jumpSpeed;
    }

    void UpdateState()
    {
        velocity = myBody.velocity;
        if (onGround)
        {
            jumpCount = 0;
            if (groundContactCount > 1)
            {
                contactNormal.Normalize();
            }
        }
        else
        {
            contactNormal = Vector3.up;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        EvaluateCollision(collision);
    }

    private void EvaluateCollision(Collision collison)
    {
        foreach(var contact in collison.contacts)
        {
            Vector3 normal = contact.normal;
            if(normal.y > minGroundDotProduct)
            {
                groundContactCount += 1;
                contactNormal += normal;
            }
        }
    }


    // Vector3.Dot(vector, contactNormal)点积 假设斜角为Q，直接算出来是contactNormal * -Sin Q
    // vector - 上述相乘向量后就是对应斜面的新的X轴方向
    Vector3 ProjectOnContactPlane(Vector3 vector)
    {
        return vector - contactNormal * Vector3.Dot(vector, contactNormal);
    }

    // 将速度换算到斜面上
    void AdjustVelocity()
    {
        Vector3 newXAxis = ProjectOnContactPlane(Vector3.right).normalized;
        Vector3 newZAxis = ProjectOnContactPlane(Vector3.forward).normalized;

        float currentX = Vector3.Dot(velocity, newXAxis);
        float currentZ = Vector3.Dot(velocity, newZAxis);

        float acceleration = onGround ? accelerationRate : airAccelerationRate;
        float maxSpeedChange = acceleration * Time.deltaTime;

        float newX = Mathf.MoveTowards(currentX, disiredVelocity.x, maxSpeedChange);
        float newZ = Mathf.MoveTowards(currentZ, disiredVelocity.z, maxSpeedChange);

        velocity += newXAxis * (newX - currentX) + newZAxis * (newZ - currentZ);
    }
}
