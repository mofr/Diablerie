using Diablerie.Engine;
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
            float currentMana = PlayerController.instance.character.mana;
            float maxMana = PlayerController.instance.character.maxMana;
            imgManaBar.fillAmount = currentMana / maxMana;
            label.text = "Mana: " + currentMana + "/" + maxMana;
        }
    }
}
