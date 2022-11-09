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

    [SerializeField]
    LayerMask layer = -1;

    Quaternion gravityAlignment = Quaternion.identity;

    Quaternion orbitRotation;
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
        transform.localRotation = orbitRotation = Quaternion.Euler(orbitAngle);
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
        gravityAlignment = Quaternion.FromToRotation(gravityAlignment * Vector3.up, -CustomGravity.GetGravity(focusPos).normalized) * gravityAlignment;

        UpdateFocusPoint();
        if (ManualRotation() || AutomaticRotation())
        {
            ConstrainAngles();
            orbitRotation = Quaternion.Euler(orbitAngle);
        }

        Quaternion lookRotation = gravityAlignment * orbitRotation;
        Vector3 lookDirection = lookRotation * Vector3.forward;
        Vector3 lookPos = focusPos - lookDirection * distance;

        Vector3 rectOffset = lookDirection * regularCamera.nearClipPlane;
        Vector3 rectPos = focusOn.position + rectOffset;
        Vector3 castFrom = focusOn.position;
        Vector3 castLine = rectPos - castFrom;
        float castDistance = castLine.magnitude;
        Vector3 castDirection = castLine / castDistance;

        if (Physics.BoxCast(castFrom, CameraHalfExtends, castDirection, out RaycastHit hitInfo, lookRotation, castDistance, layer))
        {
            rectPos = castFrom + castDirection * hitInfo.distance;
            lookPos = rectPos - rectOffset;
        }

        transform.SetPositionAndRotation(lookPos, lookRotation);
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

        Vector3 alignedDelta =
             Quaternion.Inverse(gravityAlignment) *
             (focusPos - prefocusPos);
        Vector2 movement = new Vector2(alignedDelta.x, alignedDelta.z);
        float movementDeltaSqr = movement.sqrMagnitude;
        if(movementDeltaSqr < 0.0001f)
        {
            return false;
        }

        //float headingAngle = GetAngle(movement.normalized);
        //// 减少小角度偏移时相机的旋转速度
        //float deltaAbs = Mathf.Abs(Mathf.DeltaAngle(orbitAngle.y, headingAngle));
        //float rotationChange = rotateSpeed * Mathf.Min(Time.unscaledDeltaTime, movementDeltaSqr);
        //if (deltaAbs < alignSmoothRange)
        //{
        //    rotationChange *= deltaAbs / alignSmoothRange;
        //}
        //else if(180f - deltaAbs < alignSmoothRange)
        //{
        //    rotationChange *= (180f - deltaAbs) / alignSmoothRange;
        //}
        //orbitAngle.y = Mathf.MoveTowardsAngle(orbitAngle.y, headingAngle, rotationChange);
        return true;
    }

    /// <summary>
    /// direction默认标准化，angle反余弦来计算角度，通过x正负来判断是否进一步换算到0-360内
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    static float GetAngle(Vector2 direction)
    {
        float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;
        return direction.x < 0f ? 360f - angle : angle;
    }
}
