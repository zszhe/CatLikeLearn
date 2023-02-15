using UnityEditor;
using UnityEngine;

public class TestEditorWindow : EditorWindow
{
    private static TestEditorWindow _window;

    private bool toggle;

    private int nowSelectionIdx = 0;
    private int lastSelectionIdx = -1; //保证启动时响应默认选择
    private int nowToggleIdx = 0;
    private int lastToggleIdx = -1;
    private string[] buttonNames = new string[] { "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
    private int maxButtonPerLine = 3;

    private string areaText = "areaText";
    private string fieldText = "fieldText";

    private int heigth = 30;//高度递推

    private float barHorizontalValue = 100;
    private float barVerticlaValue = 0;

    private Vector2 scrollViewRoot;

    [SerializeField]
    private Rect windowRect1 = new Rect(720, 30, 120, 50);
    [SerializeField]
    private Rect windowRect2 = new Rect(870, 30, 120, 50);

    [MenuItem("LearnEditor/CreateTestEditorWindow")]
    private static void CreateWindow()
    {
        _window = GetWindow<TestEditorWindow>(false, "TutorialWindow", true);
        _window.Show();

        //第一个参数：bool utility
        //为true创建一个Win窗口（Windows的标准窗口，无法停靠在Editor），
        //为false创建一个Unity的浮动窗口（和Scene Game这些窗口一样可以停靠在Editor中）

        //第二个参数： string title 窗口标题

        //第三个参数：bool focus 创建后是否聚焦到这个窗口上
    }

    private void OnGUI()
    {

        Debug.Log("On Draw");
        // 如果要实现动态检测，可以使用 Update 或者 OnInspectorUpdate 这两个Event方法

        heigth = 0;
        toggle = GUI.Toggle(new Rect(0, heigth, 150, 30), toggle, "是否展开Area");
        heigth += 30;
        if (toggle)
        {
            GUILayout.BeginArea(new Rect(0, heigth, 300, 500));

            if (GUILayout.Button("测试按钮", GUILayout.Width(100), GUILayout.Height(25)))
            {
                Debug.Log("Click 按钮 ！");
            }

            nowSelectionIdx = GUILayout.SelectionGrid(nowSelectionIdx, buttonNames, maxButtonPerLine);
            if (nowSelectionIdx != lastSelectionIdx)
            {
                Debug.Log("SelectionGrid 选择了：" + buttonNames[nowSelectionIdx]);
                lastSelectionIdx = nowSelectionIdx;
            }

            nowToggleIdx = GUILayout.Toolbar(nowToggleIdx, buttonNames);

            if (nowToggleIdx != lastToggleIdx)
            {
                Debug.Log("Toolbar 选择了：" + buttonNames[nowToggleIdx]);
                lastToggleIdx = nowToggleIdx;
            }

            GUILayout.Label("Hi 这是一个Label");
            GUILayout.Box("但是Label太不明显了所以可以用Box代替", GUILayout.Width(280), GUILayout.Height(25));
            //使用GUILayout 下的 TextArea TextField
            //常需要结合 MinWidth Option 否则在输入字符过短时宽度可能收缩到无法使用
            areaText = GUILayout.TextArea(areaText, GUILayout.MinWidth(200), GUILayout.MaxHeight(30));

            fieldText = GUILayout.TextField(fieldText, GUILayout.MinWidth(200), GUILayout.MaxHeight(30));

            //这个横条不加MinWidth会出错
            //Slider的特点是，当我们拉到末端时一定会给出 min/max 读数
            barHorizontalValue = GUILayout.HorizontalSlider(barHorizontalValue, 0f, 500f, GUILayout.MinWidth(200));
            barVerticlaValue = GUILayout.VerticalSlider(barVerticlaValue, 0f, 500f, GUILayout.Height(40));

            GUILayout.Box("横条读数：" + barHorizontalValue);
            GUILayout.Box("纵条读数：" + barVerticlaValue);

            //【注意】上下两块代码应注释掉其中一块，否则最终输出值按后方的 Scrollbar 影响

            //Scrollbar的特点是通过一个 blockSize 控制滑块的大小
            //由于滑块自身大小的限制，最终读数范围是 [min,max-blcokSize] 
            barHorizontalValue = GUILayout.HorizontalScrollbar(barHorizontalValue, 25, 0f, 500f, GUILayout.MinWidth(200));
            barVerticlaValue = GUILayout.VerticalScrollbar(barVerticlaValue, 25, 0f, 500f);

            GUILayout.Box("横条读数：" + barHorizontalValue);
            GUILayout.Box("纵条读数：" + barVerticlaValue);

            GUILayout.EndArea();
            //使用GUI版本的 TextArea TextField 是定死的宽度
            heigth += 205;
            //areaText = GUI.TextArea(new Rect(0, heigth, 100, 30), areaText);
            //heigth += 30;
            //fieldText = GUI.TextField(new Rect(0, heigth, 100, 30), fieldText);
            //heigth += 30;

            //一定记得上面Begin展开Area
            //下面就要End结束 ，不然会报错
            GUILayout.BeginArea(new Rect(300, 30, 120, 500));
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            GUILayout.Box("1");
            GUILayout.Box("2");
            GUILayout.Box("3");
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Box("4");
            GUILayout.Box("5");
            GUILayout.Box("6");
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Box("7");
            GUILayout.Box("8");
            GUILayout.Box("9");
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            if (GUILayout.RepeatButton("啊", GUILayout.MaxWidth(200)))
                Debug.Log("啊啊啊");

            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(420, 30, 300, 400));

            //scrollViewRoot 是一个Vector2 Mask内互动区域当前的位置锚点
            scrollViewRoot = GUILayout.BeginScrollView(scrollViewRoot);


            //默认纵向布局
            //Heigth 总计600 >> 400 因此纵向就会展开滑动条
            //Width 总计300 << 400 因此横向不会展开滑动条
            GUILayout.Button("Buttons", GUILayout.Height(200), GUILayout.Width(300));
            GUILayout.Button("Buttons", GUILayout.Height(200), GUILayout.Width(300));
            GUILayout.Button("Buttons", GUILayout.Height(200), GUILayout.Width(300));

            GUILayout.EndScrollView();

            GUILayout.EndArea();

            windowRect1 = GUILayout.Window(0, windowRect1, DoMyWindow, "My Window");
            windowRect2 = GUILayout.Window(1, windowRect2, DoMyWindow, "My Window", GUILayout.Width(100));
        }

        // Make the contents of the window
        void DoMyWindow(int windowID)
        {
            // This button is too large to fit the window
            // Normally, the window would have been expanded to fit the button, but due to
            // the GUILayout.Width call above the window will only ever be 100 pixels wide
            if (GUILayout.Button("Please click me a lot"))
            {
                Debug.Log("Got a click" + windowID);
            }
        }
    }
}
