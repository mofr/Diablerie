using System;
using System.IO;
using Diablerie.Engine;
using Diablerie.Engine.IO.D2Formats;
using Diablerie.Engine.UI;
using Diablerie.Game.UI.Menu.ClassSelect;
using UnityEngine;

namespace Diablerie.Game.UI.Menu
{
    public class MainMenu : MonoBehaviour
    {
        public RectTransform logoPlaceholder;
        
        public ImageButton singlePlayerButton;
        public ImageButton multiPlayerButton;
        public ImageButton cinematicsButton;
        public ImageButton exitButton;
        
        public GameObject classSelectMenu;
        public GameObject mainMenu;

        private GameObject _logo;
        private Stream _musicStream;
        
        private void Awake()
        {
            classSelectMenu.SetActive(false);
            
            singlePlayerButton.OnClick += SinglePlayerButtonOnClick;
            multiPlayerButton.Disabled = true;
            cinematicsButton.Disabled = true;
            exitButton.OnClick += ExitButtonOnClick;

            classSelectMenu.GetComponent<ClassSelectMenu>().exitButton.OnClick += ClassSelectMenuExitButtonOnClick;

            _logo = CreateLogo();
            _logo.transform.position = UiHelper.ScreenToWorldPoint(logoPlaceholder.position);
            _logo.transform.parent = logoPlaceholder;
        }

        private void ClassSelectMenuExitButtonOnClick()
        {
            mainMenu.SetActive(true);
            classSelectMenu.SetActive(false);
        }

        private void OnDestroy()
        {
            classSelectMenu.GetComponent<ClassSelectMenu>().exitButton.OnClick -= ClassSelectMenuExitButtonOnClick;
            singlePlayerButton.OnClick -= SinglePlayerButtonOnClick;
            exitButton.OnClick -= ExitButtonOnClick;
        }

        private void SinglePlayerButtonOnClick()
        {
            mainMenu.SetActive(false);
            classSelectMenu.SetActive(true);
        }
        
        private static void ExitButtonOnClick()
        {
            GameManager.QuitGame();
        }
        
        private static GameObject CreateLogo()
        {
            var logo = new GameObject("logo");

            var left = UiHelper.CreateAnimatedObject("leftLogo", @"data\global\ui\FrontEnd\d2LogoFireLeft", material: Materials.softAdditive, sortingOrder: 1);
            var right = UiHelper.CreateAnimatedObject("rightLogo", @"data\global\ui\FrontEnd\d2LogoFireRight", material: Materials.softAdditive, sortingOrder: 1);

            left.transform.SetParent(logo.transform, false);
            right.transform.SetParent(logo.transform, false);

            return logo;
        }
        
        private void OnEnable()
        {
            PlayMusic();
        }

        private void OnDisable()
        {
            StopMusic();
        }

        private void PlayMusic()
        {
            try
            {
                _musicStream = Mpq.fs.OpenFile(@"data\global\music\introedit.wav");
                var clip = Wav.Load("intro music", true, _musicStream);
                var musicObject = new GameObject("intro music");
                musicObject.transform.parent = logoPlaceholder;
                var audioSource = musicObject.AddComponent<AudioSource>();
                audioSource.clip = clip;
                audioSource.loop = true;
                audioSource.Play();
            }
            catch (FileNotFoundException)
            {
            }
        }

        private void StopMusic()
        {
            if (_musicStream == null)
            {
                return;
            }

            _musicStream.Close();
        }
    }
}
