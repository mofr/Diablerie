using Diablerie.Engine.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Diablerie.Game.UI
{
    public class EnemyBar : MonoBehaviour
    {
        public static EnemyBar instance;

        [HideInInspector]
        public Character character;

        [SerializeField]
        Slider slider;

        [SerializeField]
        Text title;

        void Awake()
        {
            instance = this;
            slider.gameObject.SetActive(false);
        }

        void LateUpdate()
        {
            slider.gameObject.SetActive(character != null);
            if (character)
            {
                title.text = character.title;
                slider.maxValue = character.maxHealth;
                slider.value = character.health;
            }
        }
    }
}
