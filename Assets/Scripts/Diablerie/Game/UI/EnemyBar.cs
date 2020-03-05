using Diablerie.Engine.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Diablerie.Game.UI
{
    public class EnemyBar : MonoBehaviour
    {
        public static EnemyBar instance;

        [HideInInspector]
        public Unit unit;

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
            slider.gameObject.SetActive(unit != null);
            if (unit)
            {
                title.text = unit.title;
                slider.maxValue = unit.maxHealth;
                slider.value = unit.health;
            }
        }
    }
}
