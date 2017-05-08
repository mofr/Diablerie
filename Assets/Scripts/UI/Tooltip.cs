using UnityEngine;
using UnityEngine.EventSystems;

public class Tooltip : 
    MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public string text;

    private void ShowLabel()
    {
        var rect = Tools.RectTransformToScreenRect(GetComponent<RectTransform>());
        var pos = new Vector2(rect.center.x, rect.yMax);
        UI.ShowScreenLabel(pos, text);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowLabel();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UI.HideScreenLabel();
    }

    void OnDisable()
    {
        UI.HideScreenLabel();
    }
}
