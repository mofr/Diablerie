using Diablerie.Engine.World;
using UnityEngine;
using UnityEngine.UI;

namespace Diablerie.Game.UI.ControlPanel
{
    public class ManaBulb : MonoBehaviour
    {
        [SerializeField]
        private Text label;
        private Image imgManaBar;

        void Awake()
        {
            imgManaBar = GetComponent<Image>();
        }

        void Update()
        {
            if (WorldState.instance.Player == null)
                return;
            float currentMana = WorldState.instance.Player.unit.mana;
            float maxMana = WorldState.instance.Player.unit.maxMana;
            imgManaBar.fillAmount = currentMana / maxMana;
            label.text = "Mana: " + currentMana + "/" + maxMana;
        }
    }
}
