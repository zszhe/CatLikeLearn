using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Input
{
    [RequireComponent(typeof(Transform), typeof(CanvasGroup))]
    public class UIBaseInputController : MonoBehaviour
    {
        [Tooltip("打开UI后是否默认支持导航")]
        public bool navigationEnableOnAwake = true;

        [Range(0.0f, 1.0f)]
        [Tooltip("导航时超过这个输入值才会响应，低于不响应")]
        public float navigationAxisDeadZone = 0.6f;

        [Tooltip("导航时是否支持当前选中的元素能通过UIConfirm触发点击")]
        public bool navigationSupportConfirm = true;

        [Tooltip("打开UI后默认选中的UI元素")]
        public GameObject defaultSelectable;

        [Tooltip("是否屏蔽玩家输入，UI输入和玩家输入是同时生效的，如果打开了全屏UI，一般来说是需要屏蔽玩家输入的")]
        public bool enableUIOnlyInputMode;

        private Transform m_uiBase;
        private bool m_navigationEnabled;
        private int m_navigationInputPairCount = 0;
        private Vector2 m_lastMoveVector = Vector2.zero;
        private int m_consecutiveMoveCount;
        private float m_prevActionTime = 0.0f;

        private CanvasGroup m_canvasGroup;

        private Action m_navigationInputConfirm;
        private Action m_navigationInputCancel;

        private static float navigationRepeatDelay = 0.5f;
        private static float inputActionsPerSecond = 10f;

        private new void Awake()
        {
            m_uiBase = GetComponent<Transform>();
            m_canvasGroup = GetComponent<CanvasGroup>();
            m_navigationEnabled = navigationEnableOnAwake;
        }
      

        private void OnAxis(InputBindPair inputBindPair, float axis)
        {
            if (Mathf.Approximately(axis, 0f))
            {
                return;
            }

            if (ProcessAction(inputBindPair))
            {
                return;
            }

            if (inputBindPair.inputAction == InputAction.CallUIBaseFunctions)
            {
                if (m_uiBase != null)
                {
                    //m_uiBase.OnAxis(inputBindPair.inputBindKeyName, axis);
                }
            }
            else if (inputBindPair.inputAction == InputAction.CallCustomFunctions)
            {
                if (inputBindPair.inputCustomOneAxisFunction != null)
                {
                    inputBindPair.inputCustomOneAxisFunction.Invoke(inputBindPair.inputBindKeyName, axis);
                }
            }
        }

        private void OnNavigation(InputBindPair inputBindPair, float axisX, float axisY)
        {
            if (!m_navigationEnabled || (Mathf.Approximately(axisX, 0f) && Mathf.Approximately(axisY, 0f)))
            {
                m_consecutiveMoveCount = 0;
                return;
            }

            if (ProcessAction(inputBindPair))
            {
                return;
            }

            // 默认的导航直接用Selectable.FindSelectable来处理
            if (inputBindPair.inputAction == InputAction.NavigationNextSelectable ||
                inputBindPair.inputAction == InputAction.CallUIBaseFunctions ||
                inputBindPair.inputAction == InputAction.CallCustomFunctions)
            {
                // 时间间隔的判断直接从StandaloneInputModule复制过来，那边实在是很难改写
                float time = Time.unscaledTime;

                Vector2 movement = new Vector2(axisX, axisY);

                bool similarDir = (Vector2.Dot(movement, m_lastMoveVector) > 0);

                // If direction didn't change at least 90 degrees, wait for delay before allowing consecutive event.
                if (similarDir && m_consecutiveMoveCount == 1)
                {
                    if (time <= m_prevActionTime + navigationRepeatDelay)
                    {
                        return;
                    }
                }
                // If direction changed at least 90 degree, or we already had the delay, repeat at repeat rate.
                else
                {
                    if (time <= m_prevActionTime + 1f / inputActionsPerSecond)
                    {
                        return;
                    }
                }

                if (!similarDir)
                {
                    m_consecutiveMoveCount = 0;
                }

                AxisEventData axisEventData = GetAxisEventData(movement.x, movement.y, navigationAxisDeadZone);

                if (axisEventData.moveDir != MoveDirection.None)
                {
                    if (inputBindPair.inputAction == InputAction.CallUIBaseFunctions)
                    {
                        if (m_uiBase != null)
                        {
                            //m_uiBase.OnNavigation(inputBindPair.inputBindKeyName, axisX, axisY);
                        }
                    }
                    else if (inputBindPair.inputAction == InputAction.CallCustomFunctions)
                    {
                        if (inputBindPair.inputCustomAxisFunction != null)
                        {
                            inputBindPair.inputCustomAxisFunction.Invoke(inputBindPair.inputBindKeyName, axisX, axisY);
                        }
                    }
                    else
                    {
                        ExecuteEvents.Execute(EventSystem.current.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);
                    }

                    if (!similarDir)
                    {
                        m_consecutiveMoveCount = 0;
                    }

                    m_consecutiveMoveCount++;
                    m_prevActionTime = time;
                    m_lastMoveVector = movement;
                }
                else
                {
                    m_consecutiveMoveCount = 0;
                }
            }
        }

        private AxisEventData m_axisEventData;

        protected virtual AxisEventData GetAxisEventData(float x, float y, float moveDeadZone)
        {
            if (m_axisEventData == null)
            {
                m_axisEventData = new AxisEventData(EventSystem.current);
            }

            m_axisEventData.Reset();
            m_axisEventData.moveVector = new Vector2(x, y);
            m_axisEventData.moveDir = DetermineMoveDirection(x, y, moveDeadZone);
            return m_axisEventData;
        }

        protected static MoveDirection DetermineMoveDirection(float x, float y, float deadZone)
        {
            // if vector is too small... just return
            if (new Vector2(x, y).sqrMagnitude < deadZone * deadZone)
            {
                return MoveDirection.None;
            }

            if (Mathf.Abs(x) > Mathf.Abs(y))
            {
                return x > 0 ? MoveDirection.Right : MoveDirection.Left;
            }

            return y > 0 ? MoveDirection.Up : MoveDirection.Down;
        }

        private BaseEventData m_baseEventData;

        protected virtual BaseEventData GetBaseEventData()
        {
            if (m_baseEventData == null)
            {
                m_baseEventData = new BaseEventData(EventSystem.current);
            }

            m_baseEventData.Reset();
            return m_baseEventData;
        }

        private void OnConfirmSelectable()
        {
            if (m_navigationEnabled && gameObject.activeSelf)
            {
                ExecuteEvents.Execute(EventSystem.current.currentSelectedGameObject, GetBaseEventData(),
                    ExecuteEvents.submitHandler);
            }
        }

        private void OnCancelSelectable()
        {
            if (m_navigationEnabled && gameObject.activeSelf)
            {
                ExecuteEvents.Execute(EventSystem.current.currentSelectedGameObject, GetBaseEventData(),
                    ExecuteEvents.cancelHandler);
            }
        }

        private bool ProcessAction(InputBindPair inputBindPair)
        {
            if (!gameObject.activeInHierarchy)
            {
                return true;
            }

            switch (inputBindPair.inputAction)
            {
                case InputAction.DoNothing:
                    return true;
                case InputAction.Confirm:

                    if (inputBindPair.inputBindGameObject != null)
                    {
                        ExecuteEvents.Execute(inputBindPair.inputBindGameObject, GetBaseEventData(),
                            ExecuteEvents.submitHandler);
                    }
                    else if (m_uiBase != null)
                    {
                        //m_uiBase.OnConfirmInput();
                    }

                    return true;
                case InputAction.Cancel:

                    if (inputBindPair.inputBindGameObject != null)
                    {
                        ExecuteEvents.Execute(inputBindPair.inputBindGameObject, GetBaseEventData(),
                            ExecuteEvents.cancelHandler);
                    }
                    else if (m_uiBase != null)
                    {
                        //m_uiBase.OnCancelInput();
                    }

                    return true;
                default:
                    return false;
            }
        }

        public void EnableNavigation()
        {
            if (!m_navigationEnabled && defaultSelectable != null)
            {
                EventSystem.current.SetSelectedGameObject(defaultSelectable);
            }

            m_navigationEnabled = true;
        }

        public void DisableNavigation()
        {
            if (m_navigationEnabled)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }

            m_navigationEnabled = false;
        }

        protected Transform GetUIBase()
        {
            return m_uiBase;
        }
    }
}
