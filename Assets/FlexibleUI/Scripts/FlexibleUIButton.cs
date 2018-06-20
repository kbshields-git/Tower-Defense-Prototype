using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class FlexibleUIButton : FlexibleUI
{

    //protected Button button;
    //protected Image image;

    public enum ButtonType
    {
        Default,
        Confirm,
        Decline,
        Warning,
        Text
    }

    
    public Image image;
    public Image icon;
    public Button button;
    public Text text;

    public ButtonType buttonType;

    protected override void OnSkinUI()
    {
        base.OnSkinUI();

        //image = GetComponent<Image>();
        //icon = transform.Find("Icon").GetComponent<Image>();
        //button = GetComponent<Button>();

        button.transition = Selectable.Transition.SpriteSwap;
        button.targetGraphic = image;

        image.sprite = skinData.buttonSprite;
        image.type = Image.Type.Sliced;
        button.spriteState = skinData.buttonSpriteState;

        if (text != null)
        {
            text.text = skinData.text;
        }

        switch (buttonType)
        {
            case ButtonType.Confirm:
                image.color = skinData.confirmColor;
                icon.sprite = skinData.confirmIcon;
                ToggleText(false);
                break;
            case ButtonType.Decline:
                image.color = skinData.declineColor;
                icon.sprite = skinData.declineIcon;
                ToggleText(false);
                break;
            case ButtonType.Default:
                image.color = skinData.defaultColor;
                icon.sprite = skinData.defaultIcon;
                ToggleText(false);
                break;
            case ButtonType.Warning:
                image.color = skinData.warningColor;
                icon.sprite = skinData.warningIcon;
                ToggleText(false);
                break;
            case ButtonType.Text:
                image.color = skinData.textColor;
                icon.sprite = null;
                ToggleText(true);
                break;
        }
        

    }

    private void ToggleText(bool textActive)
    {
        if (!textActive)
        {
            text.gameObject.SetActive(false);
            icon.gameObject.SetActive(true);
        }
        else
        {
            text.gameObject.SetActive(true);
            icon.gameObject.SetActive(false);
        }
    }


}
