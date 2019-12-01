using UnityEngine;

namespace Diablerie.Game.UI
{
    public class CharstatPanel : MonoBehaviour
    {
        public static CharstatPanel instance;

        public GameObject panel;

        void Awake()
        {
            instance = this;
        }

        public void ToggleVisibility()
        {
            visible ^= true;
        }

        public bool visible
        {
            set { panel.SetActive(value); }
            get { return panel.activeSelf; }
        }
    }
}
