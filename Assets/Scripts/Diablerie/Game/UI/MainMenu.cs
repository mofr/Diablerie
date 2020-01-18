using System.IO;
using Diablerie.Engine;
using Diablerie.Engine.Datasheets;
using Diablerie.Engine.Entities;
using Diablerie.Engine.IO.D2Formats;
using Diablerie.Game.World;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Diablerie.Game.UI
{
    public class MainMenu : MonoBehaviour
    {
        public RectTransform logoPlaceholder;
        public Text headerText;

        static SoundInfo selectSound;

        string defaultText;
        GameObject logo;
        Stream musicStream;

        private void Awake()
        {
            selectSound = SoundInfo.Find("cursor_pass");
            defaultText = headerText.text;
            logo = CreateLogo();
            foreach (ClassSelectButton classSelector in FindObjectsOfType<ClassSelectButton>())
            {
                classSelector.OnEnter += (CharStatsInfo info) => {
                    headerText.text = "Play " + info.className;
                    AudioManager.instance.Play(selectSound);
                };

                classSelector.OnExit += (CharStatsInfo info) => {
                    headerText.text = defaultText;
                };

                classSelector.OnClick += (CharStatsInfo info) =>
                {
                    WorldBuilder.className = info.className;
                    SceneManager.LoadScene("Game");
                };
            }
        }

        private void LateUpdate()
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(logoPlaceholder.position);
            pos.z = 0;
            logo.transform.position = pos;
            
            if (Input.GetKeyDown(KeyCode.Escape))
                GameManager.QuitGame();
        }

        private void OnEnable()
        {
            PlayMusic();
        }

        private void OnDisable()
        {
            StopMusic();
        }

        void PlayMusic()
        {
            try
            {
                musicStream = Mpq.fs.OpenFile(@"data\global\music\introedit.wav");
                AudioClip clip = Wav.Load("intro music", true, musicStream);
                var musicObject = new GameObject();
                var audioSource = musicObject.AddComponent<AudioSource>();
                audioSource.clip = clip;
                audioSource.loop = true;
                audioSource.Play();
            }
            catch (FileNotFoundException)
            {
            }
        }

        void StopMusic()
        {
            if (musicStream == null)
                return;

            musicStream.Close();
        }

        static GameObject CreateLogo()
        {
            GameObject logo = new GameObject("logo");

            var leftLogo = Spritesheet.Load(@"data\global\ui\FrontEnd\d2LogoFireLeft");
            var rightLogo = Spritesheet.Load(@"data\global\ui\FrontEnd\d2LogoFireRight");

            var left = new GameObject("leftLogo");
            var right = new GameObject("rightLogo");

            var leftLogoAnimator = left.AddComponent<SpriteAnimator>();
            var rightLogoAnimator = right.AddComponent<SpriteAnimator>();

            leftLogoAnimator.SetSprites(leftLogo.GetSprites(0));
            rightLogoAnimator.SetSprites(rightLogo.GetSprites(0));

            left.transform.SetParent(logo.transform, false);
            right.transform.SetParent(logo.transform, false);

            return logo;
        }
    }
}
