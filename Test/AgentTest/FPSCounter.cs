using UnityEngine;

public class FpsCounter : MonoBehaviour
{
    [Header("显示设置")]
    public bool show = true;                // 是否显示
    public int fontSize = 24;               // 字体大小
    public Color fontColor = Color.green;   // 字体颜色
    public TextAnchor anchor = TextAnchor.UpperLeft; // 屏幕锚点

    private float fps;
    private float updateInterval = 0.5f; // 每0.5秒更新一次FPS
    private float lastInterval;
    private int frames;

    void Start()
    {
        lastInterval = Time.realtimeSinceStartup;
        frames = 0;
    }

    void Update()
    {
        ++frames;

        float timeNow = Time.realtimeSinceStartup;
        if (timeNow > lastInterval + updateInterval)
        {
            fps = frames / (timeNow - lastInterval);
            frames = 0;
            lastInterval = timeNow;
        }
    }

    void OnGUI()
    {
        if (!show) return;

        // 设置 GUI 样式
        GUIStyle style = new GUIStyle(GUI.skin.label)
        {
            fontSize = fontSize,
            normal = { textColor = fontColor },
            alignment = anchor
        };

        // 计算位置（根据锚点自动适配）
        float x = 10;
        float y = 10;
        switch (anchor)
        {
            case TextAnchor.UpperRight:
                x = Screen.width - 150;
                break;
            case TextAnchor.LowerLeft:
                y = Screen.height - 30;
                break;
            case TextAnchor.LowerRight:
                x = Screen.width - 150;
                y = Screen.height - 30;
                break;
            case TextAnchor.UpperCenter:
                x = Screen.width / 2 - 75;
                break;
            case TextAnchor.LowerCenter:
                x = Screen.width / 2 - 75;
                y = Screen.height - 30;
                break;
        }

        GUI.Label(new Rect(x, y, 150, 30), $"FPS: {fps:F1}", style);
    }
}