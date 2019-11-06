using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvailableSkillsPanel : MonoBehaviour
{
    public static AvailableSkillsPanel instance;

    public SkillPanelSlot slotPrefab;
    
    public delegate void OnSkillSelectedHandler(SkillInfo skillInfo);
    public event OnSkillSelectedHandler OnSkillSelected;

    private List<SkillInfo> skills;

    private void Awake()
    {
        instance = this;
    }

    public void AddSkill(SkillInfo skillInfo)
    {
        SkillPanelSlot slot = Instantiate(slotPrefab, transform);
        slot.SetSkill(skillInfo);
        slot.SetHotKey("");
        var button = slot.GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            Hide();
            if (OnSkillSelected != null)
                OnSkillSelected(skillInfo);
        }); 
    }

    public void Toggle()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    
    public void Show()
    {
        gameObject.SetActive(true);
    }
}
