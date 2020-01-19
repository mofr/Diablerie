using Diablerie.Engine.IO.D2Formats;
using Diablerie.Engine.UI;
using Diablerie.Game.World;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Diablerie.Game.UI.Menu.ClassSelect
{
    public class ClassSelectMenu : MonoBehaviour
    {
        public RectTransform firePlaceholder;
        public Text classNameText;
        public Text classDescriptionText;
        public ImageButton exitButton;
        public ImageButton okButton;
        
        private GameObject _fire;
        private ClassSelector _selected;
        private bool _clickable = true;

        #region Private Methods

        private void OnOkClick()
        {
            if (_selected == null || string.IsNullOrEmpty(_selected.name))
            {
                return;
            }
            
            WorldBuilder.className = _selected.name;
            SceneManager.LoadScene("Game");
        }

        private void UpdateUi(ClassSelector classSelector, string nameText, string descriptionText, ClassSelector selected, bool okDisabled)
        {
            classSelector.Toggle();
            classNameText.text = nameText;
            classDescriptionText.text = descriptionText;
            _selected = selected;
            okButton.Disabled = okDisabled;
        }

        #endregion
    
        #region Unity Lifecycle
        
        private void Awake()
        {
            _fire = UiHelper.CreateAnimatedObject("fire", @"data\global\ui\FrontEnd\fire", PaletteType.Fechar, sortingOrder: 15);
            _fire.transform.position = UiHelper.ScreenToWorldPoint(firePlaceholder.position);
            _fire.transform.parent = firePlaceholder;
            
            foreach (var classSelector in FindObjectsOfType<ClassSelector>())
            {
                classSelector.OnEnter += info => {
                    if (_selected == null)
                    {
                        classNameText.text = info.ClassName;
                        classDescriptionText.text = info.Description;
                    }
                };

                classSelector.OnExit += info => {
                    if (_selected == null)
                    {
                        classNameText.text = string.Empty;
                        classDescriptionText.text = string.Empty;
                    }
                };

                classSelector.OnClick += info =>
                {
                    if (!_clickable)
                    {
                        return;
                    }
                    
                    _clickable = false;
                    if (_selected != null)
                    {
                        if (Equals(_selected, classSelector))
                        {
                            UpdateUi(classSelector, string.Empty, string.Empty, null, true);
                            return;
                        }
                        _selected.Toggle();
                    }
                    UpdateUi(classSelector, info.ClassName, info.Description, classSelector, false);
                };

                classSelector.OnStateChanged += sender =>
                {
                    if (_selected == null || Equals(_selected, sender))
                    {
                        _clickable = true;
                    }
                };
            }

            okButton.OnClick += OnOkClick;
            okButton.Disabled = true;
        }

        #endregion
    }
}
