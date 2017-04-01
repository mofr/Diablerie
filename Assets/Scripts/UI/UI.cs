using UnityEngine;

public class UI : MonoBehaviour
{
    static public UI instance;

    public Label labelPrefab;
    public EnemyBar enemyBarPrefab;

    Label label;

    void Awake()
    {
        instance = this;
        label = Instantiate(UI.instance.labelPrefab, transform);
    }

    static public void ShowLabel(Vector2 position, string text)
    {
        instance.label.text.text = text;
        instance.label.transform.position = position;
        instance.label.gameObject.SetActive(true);
    }

    static public void HideLabel()
    {
        instance.label.gameObject.SetActive(false);
    }
}
