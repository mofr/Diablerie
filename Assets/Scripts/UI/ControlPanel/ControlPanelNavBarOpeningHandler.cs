using UnityEngine;
using UnityEngine.UI;

public class ControlPanelNavBarOpeningHandler : MonoBehaviour
{
    private Button buttonToggleMinibar;

    private Sprite spriteDefaultNonPressed;
    private Sprite spriteDefaultPressed;

    public Image imgBarAvailableNonPressed;
    public Image imgBarAvailablePressed;

    public void Start()
    {
        buttonToggleMinibar = GetComponent<Button>();

        //access the images from the button
        spriteDefaultNonPressed = buttonToggleMinibar.image.sprite;
        spriteDefaultPressed = buttonToggleMinibar.spriteState.pressedSprite;
    }

    public void ShowNavigationalBar(GameObject minipanelObject)
    {
        //toggle the minibar
        minipanelObject.SetActive(!minipanelObject.activeSelf);

        //change the arrow between up and down

        //has to be stored in a temporary variable otherwise not changeable
        SpriteState spriteStateClone = new SpriteState();

        //if it is now active we set the down arrow
        spriteStateClone = buttonToggleMinibar.spriteState;
        if (minipanelObject.activeSelf)
        {
            buttonToggleMinibar.image.sprite = imgBarAvailableNonPressed.sprite;
            spriteStateClone.pressedSprite = imgBarAvailablePressed.sprite;
        }
        else
        {
            buttonToggleMinibar.image.sprite = spriteDefaultNonPressed;
            spriteStateClone.pressedSprite = spriteDefaultPressed;
        }
        buttonToggleMinibar.spriteState = spriteStateClone;
    }
}
