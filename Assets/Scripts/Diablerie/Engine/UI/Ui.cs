using UnityEngine;

namespace Diablerie.Engine.UI
{
    public class Ui : MonoBehaviour
    {
        public static Ui instance;
        public static Canvas canvas;
    
        public SoftwareCursor softwareCursorPrefab;

        private Label label;
        private ScreenLabel screenLabel;

        void Awake()
        {
            instance = this;
            canvas = FindObjectOfType<Canvas>();
            label = new Label(transform);
            label.Hide();
            screenLabel = new ScreenLabel(canvas.transform as RectTransform);
            screenLabel.Hide();
            Instantiate(instance.softwareCursorPrefab, canvas.transform);
        }

        public static void ShowLabel(Vector2 position, string text)
        {
            instance.label.Show(position, text);
        }

        public static void HideLabel()
        {
            instance.label.Hide();
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
