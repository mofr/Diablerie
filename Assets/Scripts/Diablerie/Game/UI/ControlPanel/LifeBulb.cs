using Diablerie.Engine;
using UnityEngine;
using UnityEngine.UI;

namespace Diablerie.Game.UI.ControlPanel
{
    public class LifeBulb : MonoBehaviour
    {
        [SerializeField]
        private Text label;
        private Image imgHealthBar;

        void Awake()
        {
            imgHealthBar = GetComponent<Image>();
        }

        void Update()
        {
            float currHealth = PlayerController.instance.character.health;
            float maxHealth = PlayerController.instance.character.maxHealth;
            imgHealthBar.fillAmount = currHealth / maxHealth;
            label.text = "Life: " + currHealth + "/" + maxHealth;
        }
    }
}
