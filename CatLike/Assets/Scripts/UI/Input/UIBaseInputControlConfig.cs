using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using UI.Input.Binding;

namespace UI.Input
{
    /// <summary>
    /// 通用绑定输入的配置信息
    /// </summary>
    [Serializable]
    public class UIBaseInputControlConfig
    {
        /// <summary>
        /// 输入绑定的配置信息
        /// </summary>
        public List<InputBindPair> inputBindPairs = new List<InputBindPair>();
    }

    [Serializable]
    public class InputBindPair
    {
        /// <summary>
        /// 绑定输入的类型
        /// </summary>
        [Tooltip("绑定输入的类型")]
        public InputBindType inputBindType;

        /// <summary>
        /// 绑定的输入名字，来源按钮绑定的配置名字，来自UNP
        /// </summary>
        [Tooltip("绑定的输入名字")]
        public string inputBindKeyName;

        /// <summary>
        /// 输入触发后执行的操作
        /// </summary>
        [Tooltip("输入后执行的操作")]
        public InputAction inputAction;

        [Tooltip("触发Confirm或Cancel的时候，针对GameObject发送Submit或者Cancel事件")]
        public GameObject inputBindGameObject;

        [Tooltip("InputAction是CallCustomFunctions时的按键相关自定义方法")]
        public UnityEvent<string> inputCustomKeyFunction = new UnityEvent<string>();

        [Tooltip("InputAction是CallCustomFunctions时的摇杆相关自定义方法")]
        public UnityEvent<string, float, float> inputCustomAxisFunction = new UnityEvent<string, float, float>();

        [Tooltip("InputAction是CallCustomFunctions时的单独Axis相关自定义方法")]
        public UnityEvent<string, float> inputCustomOneAxisFunction = new UnityEvent<string, float>();

        /// <summary>
        /// 运行时用来做remove操作的句柄
        /// </summary>
        [NonSerialized]
        public List<IInputBindingHandle> inputBindingHandler = new List<IInputBindingHandle>();

        [NonSerialized]
        public bool isBind = false;

        //[Tooltip("绑定这个按键输入的文本组件，会自动切换文本内容为按键名")]
        //public TMP_Text autoKeyText;
    }

    /// <summary>
    /// 输入绑定的类型
    /// </summary>
    [Serializable]
    public enum InputBindType
    {
        /// <summary>
        /// 绑定指定的按键输入
        /// </summary>
        KeyPress = 0,

        /// <summary>
        /// 绑定指定的按键输入
        /// </summary>
        KeyRelease = 1,

        /// <summary>
        /// 绑定导航输入，其实就是Axis2D
        /// </summary>
        Navigation = 2,

        /// <summary>
        /// 长按，只要按钮还在按，每帧都会触发
        /// </summary>
        KeyHoldCurrentFrame = 3,

        /// <summary>
        /// 长按，只触发一次，比如按下不放1秒触发一次
        /// </summary>
        KeyPressHold = 4,

        /// <summary>
        /// 区别于Navigation，这个对应纯粹的Axis，例如鼠标滚轮
        /// </summary>
        Axis = 5,
    }

    /// <summary>
    /// 输入触发后的行为
    /// </summary>
    public enum InputAction
    {
        DoNothing = 0,

        /// <summary>
        /// 关闭UI，即调用Close方法
        /// </summary>
        Close = 1,

        /// <summary>
        /// 调用Confirm方法
        /// </summary>
        Confirm = 2,

        /// <summary>
        /// 调用Cancel方法
        /// </summary>
        Cancel = 3,

        /// <summary>
        /// 选择下一个Selectable
        /// </summary>
        NavigationNextSelectable = 4,

        /// <summary>
        /// 调用对应的方法，其实就是透传
        /// KeyPress对应OnKeyPress方法
        /// KeyRelease对应OnKeyRelease方法
        /// Navigation对应OnNavigation方法
        /// Axis对应OnAxis方法
        /// </summary>
        CallUIBaseFunctions = 5,

        /// <summary>
        /// 调用绑定在Inspector上的自定义的UnityEvent方法
        /// </summary>
        CallCustomFunctions = 6,
    }
}
