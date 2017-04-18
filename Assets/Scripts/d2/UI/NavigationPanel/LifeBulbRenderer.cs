using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LifeBulbRenderer : MonoBehaviour {


    private Image imgHealthBar;

    // Update is called once per frame
    void Update () {

     double currHealth=   PlayerController.instance.character.health;
        double maxHealth = PlayerController.instance.character.maxHealth;
        Debug.Log((currHealth / maxHealth));
        imgHealthBar.fillAmount = (float)(currHealth / maxHealth);


    }
 
    void Start()
    {
        imgHealthBar = GetComponent<Image>();
    }
}
