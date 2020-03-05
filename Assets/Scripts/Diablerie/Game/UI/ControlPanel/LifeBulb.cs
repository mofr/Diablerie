using Diablerie.Engine.World;
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
            if (WorldState.instance.Player == null)
                return;
            float currHealth = WorldState.instance.Player.unit.health;
            float maxHealth = WorldState.instance.Player.unit.maxHealth;
            imgHealthBar.fillAmount = currHealth / maxHealth;
            label.text = "Life: " + currHealth + "/" + maxHealth;
        }
    }
}
