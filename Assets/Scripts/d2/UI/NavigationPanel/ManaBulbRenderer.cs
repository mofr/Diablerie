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
        double currHealth = PlayerController.instance.character.health;
        double maxHealth = PlayerController.instance.character.maxHealth;
        Debug.Log((currHealth / maxHealth));
        imgManaBar.fillAmount = (float)(currHealth / maxHealth);


    }

    void Start()
    {
        imgManaBar = GetComponent<Image>();
    }
}
