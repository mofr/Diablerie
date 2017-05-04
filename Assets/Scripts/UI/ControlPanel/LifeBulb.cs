using UnityEngine;
using UnityEngine.UI;

public class LifeBulb : MonoBehaviour
{
    private Image imgHealthBar;

    void Start()
    {
        imgHealthBar = GetComponent<Image>();
    }

    void Update()
    {
        float currHealth = PlayerController.instance.character.health;
        float maxHealth = PlayerController.instance.character.maxHealth;
        imgHealthBar.fillAmount = currHealth / maxHealth;
    }
}
