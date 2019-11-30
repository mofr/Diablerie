using Diablerie.Game.UI;
using UnityEngine;

namespace Diablerie.Engine.UI
{
    public class Ui : MonoBehaviour
    {
        static public Ui instance;
        static public Canvas canvas;
    
        public Label labelPrefab;
        public Label screenLabelPrefab;
        public EnemyBar enemyBarPrefab;
        public SoftwareCursor softwareCursorPrefab;

        [HideInInspector]
        public Label label;

        [HideInInspector]
        public Label screenLabel;

        void Awake()
        {
            instance = this;
            canvas = FindObjectOfType<Canvas>();
            label = Instantiate(instance.labelPrefab, transform);
            screenLabel = Instantiate(instance.screenLabelPrefab, canvas.transform);
            label.gameObject.SetActive(false);
            screenLabel.gameObject.SetActive(false);
            Instantiate(instance.softwareCursorPrefab, canvas.transform);
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

        static public void ShowScreenLabel(Vector2 position, string text)
        {
            instance.screenLabel.text.text = text;
            instance.screenLabel.rectTransform.anchoredPosition = position;
            instance.screenLabel.gameObject.SetActive(true);
        }

        static public void HideScreenLabel()
        {
            instance.screenLabel.gameObject.SetActive(false);
        }
    }
}
