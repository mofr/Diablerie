using Diablerie.Engine.UI;
using Diablerie.Engine.World;
using UnityEngine;
using UnityEngine.UI;

namespace Diablerie.Game.UI.ControlPanel
{
    public class ExperienceBar : MonoBehaviour
    {
        [SerializeField]
        private Image filler;

        [SerializeField]
        private Tooltip tooltip;

        void Update()
        {
            int level = WorldState.instance.Player.charStat.level;
            uint currExp = WorldState.instance.Player.charStat.experience;
            uint currLevelExp = WorldState.instance.Player.charStat.currentLevelExp;
            uint nextLevelExp = WorldState.instance.Player.charStat.nextLevelExp;
            filler.fillAmount = (currExp - currLevelExp) / (float)(nextLevelExp - currLevelExp);
            tooltip.text = "Level " + level + "\nExperience: " + (currExp - currLevelExp) + " / " + (nextLevelExp - currLevelExp);
        }
    }
}
