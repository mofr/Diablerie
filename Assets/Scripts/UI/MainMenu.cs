using CrystalMpq;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    }

    void Start()
    {
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
                World.className = info.className;
                SceneManager.LoadScene("Game");
            };
        }
	}

    private void Update()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(logoPlaceholder.position);
        pos.z = 0;
        logo.transform.position = pos;
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
        MpqFile file = Mpq.fs.FindFile(@"data\global\music\introedit.wav");
        if (file == null)
            return;
        musicStream = file.Open();
        AudioClip clip = Wav.Load("intro music", true, musicStream);
        var musicObject = new GameObject();
        var audioSource = musicObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
    }

    void StopMusic()
    {
        if (musicStream == null)
            return;

        musicStream.Close();
    }

    static GameObject CreateLogo()
    {
        Palette.LoadPalette(0);

        GameObject logo = new GameObject("logo");

        var leftLogo = Spritesheet.Load(@"data\global\ui\FrontEnd\d2LogoFireLeft");
        var rightLogo = Spritesheet.Load(@"data\global\ui\FrontEnd\d2LogoFireRight");

        var left = new GameObject("leftLogo");
        var right = new GameObject("rightLogo");

        var leftLogoAnimator = left.AddComponent<SpriteAnimator>();
        var rightLogoAnimator = right.AddComponent<SpriteAnimator>();

        leftLogoAnimator.sprites = leftLogo.GetSprites(0);
        rightLogoAnimator.sprites = rightLogo.GetSprites(0);

        left.transform.SetParent(logo.transform, false);
        right.transform.SetParent(logo.transform, false);

        return logo;
    }
}
