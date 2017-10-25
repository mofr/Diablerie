using UnityEngine;

public class SkillPanel : MonoBehaviour
{
    public static SkillPanel instance;

    public SkillPanelSlot[] hotSkills;

    private void Awake()
    {
        instance = this;
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
