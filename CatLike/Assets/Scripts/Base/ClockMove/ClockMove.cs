using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ClockMove : MonoBehaviour
{
    private const int perSecondAngel = 1;
    private const int perMinuteAngel = 1;
    private const int perHourAngel = 30;
    private Transform hour;
    [SerializeField]
    private Transform minute;

    private Transform second;
    private void Awake()
    {
        Debug.Log(DateTime.Now);
        Debug.Log(DateTime.Now.TimeOfDay);
    }

    // Start is called before the first frame update
    void Start()
    {
        hour = GameObject.Find("Clock/Hour Aim").transform;
        // hour = GameObject.Find("Hour Aim").transform;
        second = GameObject.Find("Clock").transform.Find("Second Aim");
        // second = GameObject.Find("Clock").transform.FindChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        MakeClockMove();
    }

    void MakeClockMove()
    {
        TimeSpan time = DateTime.Now.TimeOfDay;
        if (hour != null)
        {
            hour.localRotation = Quaternion.Euler(0, (float)time.TotalHours * perHourAngel, 0);
        }
        if (minute != null)
        {
            minute.localRotation = Quaternion.Euler(0, (float)time.TotalMinutes * perMinuteAngel, 0);
        }
        if (second != null)
        {
            second.localRotation = Quaternion.Euler(0, (float)time.TotalSeconds * perSecondAngel, 0);
        }
    }
}
