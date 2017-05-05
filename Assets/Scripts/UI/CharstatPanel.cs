using UnityEngine;

public class CharstatPanel : MonoBehaviour
{
    static public CharstatPanel instance;

    public GameObject panel;

    void Awake()
    {
        instance = this;
    }

    public void ToggleVisibility()
    {
        visible ^= true;
    }

    public bool visible
    {
        set { panel.SetActive(value); }
        get { return panel.activeSelf; }
    }
}
