using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OrbitCamera : MonoBehaviour
{
    [SerializeField]
    private Transform focusOn = default;

    Vector3 focusPos, prefocusPos;

    [SerializeField, Range(1f, 20f)]
    private float distance = 5f;

    [SerializeField, Min(0f)]
    private float changeRadius = 1f;

    [SerializeField, Range(0f, 1f)]
    private float dampCentering = 0.5f;

    /// <summary>
    /// 只需要两个轴来描述旋转，X表示上下，Y表示左右
    /// Vector3.Forward * Quaternion.Euler(orbitAngle)来描述旋转之后空间中的朝向
    /// </summary>
    [SerializeField]
    Vector2 orbitAngle = new Vector2(45f, 0f);

    [SerializeField, Range(1f, 360f)]
    private float rotateSpeed = 90f;

    [SerializeField, Range(-89f, 89f)]
    private float minVerticalAngle = -30f;

    [SerializeField, Range(-89f, 89f)]
    private float maxVerticalAngle = 60f;

    [SerializeField, Min(0f)]
    private float alignDelay = 5f;

    float lastManualRotationTime;

    [SerializeField, Range(0f, 90f)]
    float alignSmoothRange = 45f;

    Camera regularCamera;

    Vector3 CameraHalfExtends
    {
        get
        {
            Vector3 halfExtends;
            halfExtends.y = regularCamera.nearClipPlane * Mathf.Tan(0.5f * Mathf.Deg2Rad * regularCamera.fieldOfView);
            halfExtends.x = halfExtends.y * regularCamera.aspect;
            halfExtends.z = 0f;
            return halfExtends;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        regularCamera = GetComponent<Camera>();
        focusPos = focusOn.position;
        transform.localRotation = Quaternion.Euler(orbitAngle);
    }

    private void OnValidate()
    {
        if (maxVerticalAngle < minVerticalAngle)
        {
            maxVerticalAngle = minVerticalAngle;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        UpdateFocusPoint();
        Quaternion orbit = transform.localRotation;
        if (ManualRotation() || AutomaticRotation())
        {
            ConstrainAngles();
            orbit = Quaternion.Euler(orbitAngle);
        }

        Vector3 lookDirection = orbit * Vector3.forward;
        Vector3 lookPos = focusPos - lookDirection * distance;
        //if (Physics.BoxCast(focusPos, CameraHalfExtends, -lookDirection, out RaycastHit hitInfo, orbit, distance - regularCamera.nearClipPlane))
        //{
        //    lookPos = focusPos - lookDirection * (hitInfo.distance + regularCamera.nearClipPlane);
        //}
       
        transform.SetPositionAndRotation(lookPos, orbit);
    }

    /// <summary>
    /// t越小越靠近目标位置，
    /// </summary>
    void UpdateFocusPoint()
    {
        prefocusPos = focusPos;
        Vector3 tarPos = focusOn.position;
        float distance = Vector3.Distance(tarPos, focusPos);
        float t = 1;
        if(distance > 0.01f && dampCentering > 0)
        {
            t = Mathf.Pow(1 - dampCentering, Time.unscaledDeltaTime);
        }

        if(distance > changeRadius)
        {
            t = Mathf.Min(t, changeRadius / distance);
        }

        focusPos = Vector3.Lerp(tarPos, focusPos, t);
    }

    /// <summary>
    /// 更新相机角度
    /// </summary>
    bool ManualRotation()
    {
        Vector2 input = new Vector2(
            Input.GetAxis("Vertical Camera"), 
            Input.GetAxis("Horizontal Camera"));
        const float e = 1E-3f;
        if(Mathf.Abs(input.x) > e || Mathf.Abs(input.y) > e)
        {
            lastManualRotationTime = Time.unscaledTime;
            orbitAngle += rotateSpeed * Time.unscaledDeltaTime * input;
            return true;
        }

        return false;
    }

    /// <summary>
    /// 约束相机旋转后角度
    /// </summary>
    void ConstrainAngles()
    {
        orbitAngle.x = Mathf.Clamp(orbitAngle.x, minVerticalAngle, maxVerticalAngle);

        if (orbitAngle.y < 0f)
        {
            orbitAngle.y += 360f;
        }
        else if(orbitAngle.y >= 360f)
        {
            orbitAngle.y -= 360f;
        }
    }

    bool AutomaticRotation()
    {
        if(Time.unscaledTime - lastManualRotationTime < alignDelay)
        {
            return false;
        }

        Vector2 movement = new Vector2(
            focusPos.x - prefocusPos.x,
            focusPos.z - prefocusPos.z);
        float movementDeltaSqr = movement.sqrMagnitude;
        if(movementDeltaSqr < 0.0001f)
        {
            return false;
        }

        float headingAngle = GetAngle(movement / Mathf.Sqrt(movementDeltaSqr));
        // 减少小角度偏移时相机的旋转速度
        float deltaAbs = Mathf.Abs(Mathf.DeltaAngle(orbitAngle.y, headingAngle));
        float rotationChange = rotateSpeed * Mathf.Min(Time.unscaledDeltaTime, movementDeltaSqr);
        if (deltaAbs < alignSmoothRange)
        {
            rotationChange *= deltaAbs / alignSmoothRange;
        }
        else if(180f - deltaAbs < alignSmoothRange)
        {
            rotationChange *= (180f - deltaAbs) / alignSmoothRange;
        }
        orbitAngle.y = Mathf.MoveTowardsAngle(orbitAngle.y, headingAngle, rotationChange);
        return true;
    }

    static float GetAngle(Vector2 direction)
    {
        float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;
        return direction.x < 0f ? 360f - angle : angle;
    }
}
