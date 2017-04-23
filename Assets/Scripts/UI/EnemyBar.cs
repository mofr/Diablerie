using UnityEngine;
using UnityEngine.UI;

public class EnemyBar : MonoBehaviour
{
    static public EnemyBar instance;

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
