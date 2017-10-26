using UnityEngine;
using UnityEngine.UI;

public class LifeBulb : MonoBehaviour
{
    [SerializeField]
    private Text label;
    private Image imgHealthBar;

    void Awake()
    {
        imgHealthBar = GetComponent<Image>();
    }

    void Update()
    {
        float currHealth = PlayerController.instance.character.health;
        float maxHealth = PlayerController.instance.character.maxHealth;
        imgHealthBar.fillAmount = currHealth / maxHealth;
        label.text = "Life: " + currHealth + "/" + maxHealth;
    }
}
