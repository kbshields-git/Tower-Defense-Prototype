using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName ="Flexible UI Data")]
public class FlexibleUIData : ScriptableObject
{
    [Header("Flexible Button")]
    public Sprite buttonSprite;
    public SpriteState buttonSpriteState;

    public Color defaultColor;
    public Sprite defaultIcon;

    public Color confirmColor;
    public Sprite confirmIcon;

    public Color declineColor;
    public Sprite declineIcon;

    public Color warningColor;
    public Sprite warningIcon;

    public Color textColor;

    [Header("Text")]
    public string text;

}
