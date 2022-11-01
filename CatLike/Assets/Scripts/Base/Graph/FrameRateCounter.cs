using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FrameRateCounter : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI display;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float frame = Time.unscaledDeltaTime;
        display.SetText("FPS\n{0:0}\n000\n000", 1f / frame);
    }
}
