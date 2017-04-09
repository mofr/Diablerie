using UnityEngine;
using UnityEngine.UI;

public class LevelEntryTitle : MonoBehaviour
{
    public Text text;
    float elapsed = 0;
    float duration = 0;

    static LevelEntryTitle instance;

    private void Awake()
    {
        instance = this;
        instance.text.gameObject.SetActive(false);
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed > duration)
            instance.text.gameObject.SetActive(false);
    }

    public static void Show(string title, float duration = 3.75f)
    {
        instance.elapsed = 0;
        instance.duration = duration;
        instance.text.text = title;
        instance.text.gameObject.SetActive(true);
    }
}
