using UnityEngine;
using UnityEngine.UI;

public class Label : MonoBehaviour
{
    public Text text;
    public RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
}
