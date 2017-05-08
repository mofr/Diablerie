using UnityEngine;
using UnityEngine.UI;

public class ManaBulb : MonoBehaviour
{
    [SerializeField]
    private Text label;
    private Image imgManaBar;

    void Start()
    {
        imgManaBar = GetComponent<Image>();
    }

    void Update()
    {
        //TODO change this to mana once it is available
        float currHealth = PlayerController.instance.character.health;
        float maxHealth = PlayerController.instance.character.maxHealth;
        imgManaBar.fillAmount = currHealth / maxHealth;
        label.text = "Mana: " + currHealth + "/" + maxHealth;
    }
}
