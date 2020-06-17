using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class LocomotiveForeground : MonoBehaviour
{
    public Sprite[] sprites;

    private SpriteRenderer _sr;

    public void ApplyDataFromEditor(int maxlevel)
    {
        Array.Resize(ref sprites, maxlevel - 1);
    }

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Loads the Lococmotive foreground sprite for viewing in the scene
    /// </summary>
    /// <param name="locomotiveLevel">Current level of locomotive</param>
    public void LoadInstance(int locomotiveLevel)
    {
        _sr.sprite = sprites[locomotiveLevel - 1];
    }
}
