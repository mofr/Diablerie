using UnityEngine;

namespace Diablerie.Engine.UI
{
    public class Ui : MonoBehaviour
    {
        public static Ui instance;
        public static Canvas canvas;
    
        public Label labelPrefab;
        public SoftwareCursor softwareCursorPrefab;

        private Label label;
        private ScreenLabel screenLabel;

        void Awake()
        {
            instance = this;
            canvas = FindObjectOfType<Canvas>();
            label = Instantiate(instance.labelPrefab, transform);
            label.gameObject.SetActive(false);
            screenLabel = new ScreenLabel(canvas.transform as RectTransform);
            screenLabel.Hide();
            Instantiate(instance.softwareCursorPrefab, canvas.transform);
        }

        public static void ShowLabel(Vector2 position, string text)
        {
            instance.label.text.text = text;
            instance.label.transform.position = position;
            instance.label.gameObject.SetActive(true);
        }

        public static void HideLabel()
        {
            instance.label.gameObject.SetActive(false);
        }

        public static void ShowScreenLabel(Vector2 position, string text)
        {
            instance.screenLabel.Show(position, text);
        }

        public static void HideScreenLabel()
        {
            instance.screenLabel.Hide();
        }
    }
}
