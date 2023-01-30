using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushSphereAround : MonoBehaviour
{
    #region Unit2
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

    // steepContactCount：陡峭的墙壁，不是天花板类型，很陡的坡
    int groundContactCount, steepContactCount;

    [SerializeField]
    bool onGround => groundContactCount > 0;

    bool OnSteep => steepContactCount > 0;

    int jumpCount = 0;

    [SerializeField, Range(0, 90)]
    float maxGroundAngle = 25f, maxStairsAngle = 25f;

    //表面水平时，其法线向量的Y分量为1。对于完全垂直的墙，Y分量为零
    [SerializeField]
    float minGroundDotProduct, minStairsDotProduct;

    Vector3 contactNormal, steepNormal;
    #endregion

    #region Unit3
    int stepsSinceLastGrouded, stepsSinceLastJump;

    [SerializeField, Range(0f, 100f)]
    float maxSnapSpeed = 100f;

    [SerializeField, Min(0f)]
    float probeDistance = 1f;

    [SerializeField]
    LayerMask probeMask = -1, stairsMask = -1;
    #endregion

    #region Unit4
    [SerializeField]
    Transform playerInputSpace = default;
    #endregion

    #region Unit5
    Vector3 upAxis, rightAxis, forwadAxis;
    #endregion

    #region MyProperty
    Renderer myRender;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.localPosition;
        myBody = GetComponent<Rigidbody>();
        myBody.useGravity = false;
        myRender = GetComponent<Renderer>();
        OnValidate();
    }

    private void OnValidate()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
        minStairsDotProduct = Mathf.Cos(maxStairsAngle * Mathf.Deg2Rad);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetAll();
            return;
        }

        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");
        // 归一化
        // playerInput.Normalize();
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);

        // 坐标空间转换到摄像头的方向
        if (playerInputSpace)
        {
            rightAxis = ProjecDirectionOnPlane(playerInputSpace.right, upAxis);
            forwadAxis = ProjecDirectionOnPlane(playerInputSpace.forward, upAxis);
            // 前后左右方向转向摄像头的本地坐标
            
        }
        else
        {
            rightAxis = ProjecDirectionOnPlane(Vector3.right, upAxis);
            forwadAxis = ProjecDirectionOnPlane(Vector3.forward, upAxis);
        }

        disiredVelocity = new Vector3(playerInput.x, 0.0f, playerInput.y) * speedRate;

        //Vector3 newPosition = transform.localPosition + velocity * Time.deltaTime;
        //if(!allowedArea.Contains(new Vector2(newPosition.x, newPosition.z)))
        //{
        //    newPosition.x = Mathf.Clamp(newPosition.x, allowedArea.xMin, allowedArea.xMax);
        //    newPosition.z = Mathf.Clamp(newPosition.z, allowedArea.yMin, allowedArea.yMax);
        //}

        //transform.localPosition = newPosition;
        desiredJump |= Input.GetButtonDown("Jump");

        myRender.material.SetColor("_BaseColor", onGround ? Color.black : Color.white);
    }

    private void ResetAll()
    {
        myBody.velocity = Vector3.zero;
        transform.localPosition = startPos;
    }

    private void ClearState()
    {
        groundContactCount = steepContactCount = 0;
        contactNormal = steepNormal = Vector3.zero;
    }

    private void FixedUpdate()
    {
        Vector3 gravity = CustomGravity.GetGravity(myBody.position, out upAxis);
        UpdateState();
        AdjustVelocity();

        if (desiredJump)
        {
            desiredJump = false;
            Jump(gravity);
        }

        velocity += gravity * Time.deltaTime;

        myBody.velocity = velocity;
        ClearState();
    }

    void Jump(Vector3 gravity)
    {
        Vector3 jumpDirection;
        if (onGround)
        {
            jumpDirection = contactNormal;
        }
        else if(OnSteep)
        {
            jumpDirection = steepNormal;
            jumpCount = 0;
        }
        else if(maxJumpCount > 0 && jumpCount <= maxJumpCount)
        {
            if(jumpCount == 0)
            {
                jumpCount = 1;
            }
            jumpDirection = contactNormal;
        }
        else
        {
            return;
        }

        jumpCount++;
        stepsSinceLastJump = 0;
        float jumpSpeed = Mathf.Sqrt(2f * gravity.magnitude * jumpHeight);
        jumpDirection = (jumpDirection + upAxis).normalized;
        float alignedSpeed = Vector3.Dot(jumpDirection, velocity);
        if(alignedSpeed > 0)
        {
            jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
        }

        velocity += jumpDirection * jumpSpeed;
    }

    void UpdateState()
    {
        stepsSinceLastGrouded += 1;
        stepsSinceLastJump += 1;
        velocity = myBody.velocity;
        if (onGround || SnapToGround() || CheckSteepContacts())
        {
            if (stepsSinceLastJump > 1)
            {
                jumpCount = 0;
            }

            stepsSinceLastGrouded = 0;
            if (groundContactCount > 1)
            {
                contactNormal.Normalize();
            }
        }
        else
        {
            // 球体处于空中
            contactNormal = upAxis;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        EvaluateCollision(collision);
    }

    private void EvaluateCollision(Collision collison)
    {
        float minDot = GetMinDot(collison.gameObject.layer);
        foreach(var contact in collison.contacts)
        {
            Vector3 normal = contact.normal;
            float upDot = Vector3.Dot(normal, upAxis);
            if(upDot >= minDot)
            {
                groundContactCount += 1;
                contactNormal += normal;
            }
            else if (upDot > -0.01f)
            {
                steepContactCount += 1;
                steepNormal += normal;
            }
        }
    }


    // Vector3.Dot(vector, contactNormal)点积 假设斜角为Q，直接算出来是contactNormal * -Sin Q
    // vector - 上述相乘向量后就是对应斜面的新的X轴方向
    Vector3 ProjectOnContactPlane(Vector3 vector)
    {
        return vector - contactNormal * Vector3.Dot(vector, contactNormal);
    }

    Vector3 ProjecDirectionOnPlane(Vector3 direction, Vector3 normal)
    {
        return (direction - normal * Vector3.Dot(direction, normal)).normalized;
    }

    // 将速度换算到斜面上
    void AdjustVelocity()
    {
        //Vector3 newXAxis = ProjectOnContactPlane(Vector3.right).normalized;
        //Vector3 newZAxis = ProjectOnContactPlane(Vector3.forward).normalized;
        Vector3 newXAxis = ProjecDirectionOnPlane(rightAxis, contactNormal);
        Vector3 newZAxis = ProjecDirectionOnPlane(forwadAxis, contactNormal);

        float currentX = Vector3.Dot(velocity, newXAxis);
        float currentZ = Vector3.Dot(velocity, newZAxis);

        float acceleration = onGround ? accelerationRate : airAccelerationRate;
        float maxSpeedChange = acceleration * Time.deltaTime;

        float newX = Mathf.MoveTowards(currentX, disiredVelocity.x, maxSpeedChange);
        float newZ = Mathf.MoveTowards(currentZ, disiredVelocity.z, maxSpeedChange);

        velocity += newXAxis * (newX - currentX) + newZAxis * (newZ - currentZ);
    }

    bool SnapToGround()
    {
        if (stepsSinceLastGrouded > 1 || stepsSinceLastJump <= 2)
        {
            return false;
        }

        float speed = velocity.magnitude;
        if(speed > maxSnapSpeed)
        {
            return false;
        }

        if (!Physics.Raycast(myBody.position, -upAxis, out RaycastHit hitInfo, probeDistance, probeMask))
        {
            return false;
        }

        // 表面水平时，其法线向量的Y分量为1。对于完全垂直的墙，Y分量为零
        float upDot = Vector3.Dot(upAxis, hitInfo.normal);
        // upDot代表normal在对应转换后的upAxis坐标系里y方向标量
        if (upDot < GetMinDot(hitInfo.collider.gameObject.layer))
        {
            return false;
        }

        groundContactCount = 1;
        contactNormal = hitInfo.normal;
        float dot = Vector3.Dot(velocity, contactNormal);
        if(dot > 0f)
        {
            velocity = (velocity - hitInfo.normal * dot).normalized * speed;
        }
        return true;
    }

    /// <summary>
    /// 通过目标Layer获取当前该使用法线的最小值
    /// </summary>
    /// <param name="layer"></param>
    /// <returns></returns>
    float GetMinDot(int layer)
    {
        return (stairsMask & (1 << layer)) == 0 ? minGroundDotProduct : minStairsDotProduct;
    }

    /// <summary>
    /// 检查是否处于接触过陡的坡，可将其总法线方向视作一块虚拟地面的法线
    /// </summary>
    /// <returns></returns>
    bool CheckSteepContacts()
    {
        if(steepContactCount > 1)
        {
            steepNormal.Normalize();
            float upDot = Vector3.Dot(upAxis, steepNormal);
            if (upDot >= minGroundDotProduct)
            {
                groundContactCount = 1;
                contactNormal = steepNormal;
                return true;
            }
        }

        return false;
    }
}
