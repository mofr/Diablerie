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
            float currentMana = WorldState.instance.Player.character.mana;
            float maxMana = WorldState.instance.Player.character.maxMana;
            imgManaBar.fillAmount = currentMana / maxMana;
            label.text = "Mana: " + currentMana + "/" + maxMana;
        }
    }
}
