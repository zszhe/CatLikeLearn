using UnityEditor;
using UnityEngine;

public class TestEditorGUIWindow : EditorWindow
{
    private static TestEditorGUIWindow _window;

    private Vector2 scrollViewRoot = new Vector2(120, 0);

    private GameObject obj;

    [MenuItem("LearnEditor/CreateTestEditorGUIWindow")]
    private static void CreateWindow()
    {
        _window = GetWindow<TestEditorGUIWindow>(false, "TestEditorGUIWindow", true);
        _window.Show();

        //第一个参数：bool utility
        //为true创建一个Win窗口（Windows的标准窗口，无法停靠在Editor），
        //为false创建一个Unity的浮动窗口（和Scene Game这些窗口一样可以停靠在Editor中）

        //第二个参数： string title 窗口标题

        //第三个参数：bool focus 创建后是否聚焦到这个窗口上
    }

    private void OnGUI()
    {
        scrollViewRoot =
            EditorGUILayout.BeginScrollView(scrollViewRoot, GUILayout.Width(100), GUILayout.Height(100));
        GUILayout.Label("asdfasdfasdfasdfasdfasdfasdfasdfasdfasdfa");
        EditorGUILayout.EndScrollView();
        obj = EditorGUILayout.ObjectField("一个必须带刚体的GameObj的序列化", obj, typeof(GameObject), true) as GameObject;
    }
}
