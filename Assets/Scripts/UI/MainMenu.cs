using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public RectTransform logoPlaceholder;
    public Text headerText;
    string defaultText;
    GameObject logo;
    
	void Start()
    {
        defaultText = headerText.text;
        logo = CreateLogo();
        foreach (ClassSelectButton classSelector in FindObjectsOfType<ClassSelectButton>())
        {
            classSelector.OnEnter += (CharStatsInfo info) => {
                headerText.text = "Play " + info.className;
            };

            classSelector.OnExit += (CharStatsInfo info) => {
                headerText.text = defaultText;
            };
        }
	}

    private void Update()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(logoPlaceholder.position);
        pos.z = 0;
        logo.transform.position = pos;
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
