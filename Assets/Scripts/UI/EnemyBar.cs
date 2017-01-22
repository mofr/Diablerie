using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBar : MonoBehaviour {

    [SerializeField]
    Slider slider;

    [SerializeField]
    Text title;

    [HideInInspector]
    public Character character;

    static public EnemyBar instance;

    void Awake()
    {
        instance = this;
        slider.gameObject.SetActive(false);
    }

	void LateUpdate () {
        slider.gameObject.SetActive(character != null);
        if (character)
        {
            title.text = character.name;
            slider.maxValue = character.maxHealth;
            slider.value = character.health;
        }
    }
}
