using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public abstract class SelectablePanelController : MonoBehaviour
    {
        public GameObject defaultSelectable;

        public GameObject inputControl;
        protected GameObject m_currentSelectable;

        public IEnumerator DelayFocusSelectable()
        {
            yield return null;

            FocusCurrentSelectable();
        }

        public virtual GameObject FindFirstSelectableGameObject()
        {
            GameObject ret = null;
            Selectable[] allChildSelectables = GetComponentsInChildren<Selectable>();
            for (int i = 0; i < allChildSelectables.Length; ++i)
            {
                ret = allChildSelectables[i].gameObject;
                if (ret.activeSelf)
                {
                    break;
                }
            }

            return ret;
        }

        public virtual void FocusCurrentSelectable()
        {
            if (m_currentSelectable == null)
            {
                m_currentSelectable = defaultSelectable;
            }

            if (m_currentSelectable != null)
            {
                EventSystem.current.SetSelectedGameObject(m_currentSelectable);
            }
        }

        public void OnEnable()
        {
            StartDelayFocusSelectable();
        }

        public void StartDelayFocusSelectable()
        {
            StartCoroutine(DelayFocusSelectable());
        }
    }
}