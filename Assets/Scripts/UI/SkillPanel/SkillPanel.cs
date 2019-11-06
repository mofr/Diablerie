using UnityEngine;

public class SkillPanel : MonoBehaviour
{
    public static SkillPanel instance;
    
    public delegate void OnClickHandler(int index);
    public event OnClickHandler OnClick;

    public SkillPanelSlot[] hotSkills;

    private void Awake()
    {
        instance = this;
        for (int i = 0; i < hotSkills.Length; ++i)
        {
            int clickedIndex = i;
            hotSkills[i].button.onClick.AddListener(() =>
            {
                if (OnClick != null)
                    OnClick(clickedIndex);
            });
        }
    }

    public void SetHotSkill(int index, SkillInfo skill)
    {
        hotSkills[index].SetSkill(skill);
    }

    public void SetHotKey(int index, string text)
    {
        hotSkills[index].SetHotKey(text);
    }
}
