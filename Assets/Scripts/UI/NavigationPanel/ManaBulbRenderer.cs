using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBulbRenderer : MonoBehaviour {

    private Image imgManaBar;

    // Update is called once per frame
    void Update()
    {
        //TODO change this to mana once it is available
        float currHealth = PlayerController.instance.character.health;
        float maxHealth = PlayerController.instance.character.maxHealth;
        imgManaBar.fillAmount = (float)(currHealth / maxHealth);
    }

    void Start()
    {
        imgManaBar = GetComponent<Image>();
    }
}
