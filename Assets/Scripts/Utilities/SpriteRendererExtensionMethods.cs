using UnityEngine;
/// <summary>
/// A static class that contains Extension Functions for the SpriteRenderer Unity Datatype
/// </summary>
public static class SpriteRendererExtensionMethods
{
    /// <summary>
    /// Sets RGBA of the Color field of a given Sprite Renderer to the given input values<br/>
    /// Use Negative inputs to maintain the original value
    /// </summary>
    /// <param name="spriteRenderer">The Sprite Renderer to manipulate</param>
    /// <param name="newRed">The new Red value to be set</param>
    /// <param name="newGreen">The new Green value to be set</param>
    /// <param name="newBlue">The new Blue value to be set</param>
    /// <param name="newAlpha">The new Alpha value to be set</param>
    public static void SetColor
        (
            this SpriteRenderer spriteRenderer, 
            float newRed = -1.0f, 
            float newGreen = -1.0f, 
            float newBlue = -1.0f, 
            float newAlpha = -1.0f
        )
    {
        if (newRed >= 0.0f) spriteRenderer.SetRed(newRed);
        if (newGreen >= 0.0f) spriteRenderer.SetRed(newGreen);
        if (newBlue >= 0.0f) spriteRenderer.SetRed(newBlue);
        if (newAlpha >= 0.0f) spriteRenderer.SetRed(newAlpha);
    }
    /// <summary>
    /// Sets the R field of the Color field of a given Sprite Renderer to newRed
    /// </summary>
    /// <param name="spriteRenderer">The Sprite Renderer to manipulate</param>
    /// <param name="newRed">The new R value to be set</param>
    public static void SetRed(this SpriteRenderer spriteRenderer, float newRed)
    {
        spriteRenderer.color = new Color
        (
            Mathf.Clamp(newRed, 0.0f, 1.0f),
            spriteRenderer.color.g,
            spriteRenderer.color.b,
            spriteRenderer.color.a
        );
    }
    /// <summary>
    /// Sets the G field of the Color field of a given Sprite Renderer to newGreen
    /// </summary>
    /// <param name="spriteRenderer">The Sprite Renderer to manipulate</param>
    /// <param name="newGreen">The new G value to be set</param>
    public static void SetGreen(this SpriteRenderer spriteRenderer, float newGreen)
    {
        spriteRenderer.color = new Color
        (
            spriteRenderer.color.r,
            Mathf.Clamp(newGreen, 0.0f, 1.0f),
            spriteRenderer.color.b,
            spriteRenderer.color.a
        );
    }
    /// <summary>
    /// Sets the B field of the Color field of a given Sprite Renderer to newBlue
    /// </summary>
    /// <param name="spriteRenderer">The Sprite Renderer to manipulate</param>
    /// <param name="newBlue">The new B value to be set</param>
    public static void SetBlue(this SpriteRenderer spriteRenderer, float newBlue)
    {
        spriteRenderer.color = new Color
        (
            spriteRenderer.color.r,
            spriteRenderer.color.g,
            Mathf.Clamp(newBlue, 0.0f, 1.0f),
            spriteRenderer.color.a
        );
    }
    /// <summary>
    /// Sets the Alpha field of the Color field of a given Sprite Renderer to newAlpha
    /// </summary>
    /// <param name="spriteRenderer">The Sprite Renderer to manipulate</param>
    /// <param name="newAlpha">The new Alpha value to be set</param>
    public static void SetAlpha(this SpriteRenderer spriteRenderer, float newAlpha)
    {
        spriteRenderer.color = new Color
        (
            spriteRenderer.color.r,
            spriteRenderer.color.g,
            spriteRenderer.color.b, 
            Mathf.Clamp(newAlpha, 0.0f, 1.0f)
        );
    }
}
