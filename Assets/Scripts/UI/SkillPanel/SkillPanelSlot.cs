using UnityEngine;
using UnityEngine.UI;

public class SkillPanelSlot : MonoBehaviour
{
    public Image image;
    public Tooltip tooltip;
    public Button button;
    public Text hotkeyText;

    private void Awake()
    {
        Clear();
    }

    public void Clear()
    {
        image.color = new Color(0, 0, 0, 0);

        tooltip.enabled = false;
        tooltip.text = "";

        button.enabled = false;

        hotkeyText.enabled = false;
    }

    public void SetSkill(SkillInfo skill)
    {
        if (skill == null)
        {
            Clear();
        }
        else
        {
            image.sprite = skill.GetIcon();
            image.color = Color.white;

            tooltip.enabled = true;
            tooltip.text = skill.name;

            button.enabled = true;
            var spriteState = button.spriteState;
            spriteState.pressedSprite = skill.GetPressedIcon();
            button.spriteState = spriteState;

            hotkeyText.enabled = true;
        }
    }

    public void SetHotKey(string text)
    {
        hotkeyText.text = text;
    }
}
