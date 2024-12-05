using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Keyboard : MonoBehaviour
{
    public RectTransform rootCanvas;
    public HorizontalLayoutGroup[] layoutGroups;
    public int maxNumKey = 10;
    public float maxKeyWidth = 58.89f;
    public float maxKeyHeight = 76.11f;
    public float minGap = 10f;
    public float maxGap = 20f;
    public float padding = 3f;

    private float currentWidth;

    public static Keyboard instance;

    private void Awake()
    {
        instance = this;   
    }

    void Start()
    {
        Update();
        currentWidth = rootCanvas.rect.width;
    }

    void Update()
    {
        if (rootCanvas.rect.width == currentWidth) return;
        currentWidth = rootCanvas.rect.width;

        float width = rootCanvas.rect.width;
        float gap = (width - 2 * padding - maxNumKey * maxKeyWidth) / (maxNumKey - 1);
        gap = Mathf.Clamp(gap, minGap, maxGap);
        float keyWidth = (width - 2 * padding - (maxNumKey - 1) * gap) / maxNumKey;
        if (keyWidth > maxKeyWidth) keyWidth = maxKeyWidth;

        float keyHeight = keyWidth / maxKeyWidth * maxKeyHeight;

        foreach(Transform child in transform)
        {
            foreach(Transform keyTr in child)
            {
                keyTr.GetComponent<RectTransform>().sizeDelta = new Vector3(keyWidth, keyHeight);
            }
        }

        foreach (var layoutGroup in layoutGroups)
        {
            layoutGroup.spacing = gap;
        }
    }

    public void OnKeyPressed(string keyValue)
    {
        WordRegion.instance.OnKeyPressed(keyValue);
    }

    public List<string> GetAllKeys()
    {
        List<string> result = new List<string>();
        var texts = GetComponentsInChildren<Text>();
        foreach(var text in texts)
        {
            if (!string.IsNullOrEmpty(text.text)) result.Add(text.text);
        }

        return result;
    }
}
