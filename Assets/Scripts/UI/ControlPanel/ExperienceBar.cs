using UnityEngine;
using UnityEngine.UI;

public class ExperienceBar : MonoBehaviour
{
    [SerializeField]
    private Image filler;

    [SerializeField]
    private Tooltip tooltip;

    void Update()
    {
        uint currExp = PlayerController.instance.charStat.experience;
        uint currLevelExp = PlayerController.instance.charStat.currentLevelExp;
        uint nextLevelExp = PlayerController.instance.charStat.nextLevelExp;
        filler.fillAmount = (currExp - currLevelExp) / (float)(nextLevelExp - currLevelExp);
        tooltip.text = "Experience: " + (currExp - currLevelExp) + " / " + (nextLevelExp - currLevelExp);
    }
}
