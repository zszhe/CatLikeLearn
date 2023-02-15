using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TestGUILayout))]
[CanEditMultipleObjects]
public class TestEditor : Editor
{
    TestGUILayout testGUILayout;

    private void OnEnable()
    {
        testGUILayout = target as TestGUILayout;
    }

    //注意访问修饰是 public
    //这个方法就是 TestGUILayout 在Inspector中序列化展开面板的定义
    public override void OnInspectorGUI()
    {
        //先绘制默认的实例化展开
        base.OnInspectorGUI();

        if (GUILayout.Button("HelloWorld"))
        {
            Debug.Log("HelloWorld");
        }

        if (GUILayout.Button("改名"))
        {
            testGUILayout.gameObject.name = "AAA";

            if(testGUILayout.GetComponent<MeshRenderer>() != null)
            {
                DestroyImmediate(testGUILayout.GetComponent<MeshRenderer>());
            }
        }

        if (GUILayout.Button("改名子物体"))
        {
            if (testGUILayout.gameObject.transform.childCount > 0)
                testGUILayout.gameObject.transform.GetChild(0).name = "BBB";
        }

        if (GUILayout.Button("批量改名子物体"))
        {

            for (int i = 0; i < testGUILayout.gameObject.transform.childCount; ++i)
                testGUILayout.gameObject.transform.GetChild(i).name = "CCC";
        }

        if (GUILayout.Button("打印选中物体名称", GUILayout.MaxWidth(200)))
        {

            var sobjs = Selection.activeGameObject;

            Debug.Log("activeGameObject" + sobjs.name);

            var tt = Selection.activeObject;

            Debug.Log("activeObject" + tt.name);

            var ww = Selection.activeContext;

            if (ww != null)
            {
                Debug.Log("activeContext" + ww.name);
            }

            var qq = Selection.activeTransform;

            Debug.Log("activeContext" + qq.name);

            var rr = Selection.activeInstanceID;

            Debug.Log("activeContext" + rr);

        }
        // UnityEditor.Selection - Unity 脚本 API
        if (GUILayout.Button("打印选中物体名称", GUILayout.MaxWidth(200)))
        {

            var sobjs = Selection.gameObjects;

            for (int i = 0; i < sobjs.Length; ++i)
                Debug.Log(sobjs[i].name);

        }

        //最后记得调一下这个方法才能保存序列化结果
        serializedObject.ApplyModifiedProperties();
    }
}
