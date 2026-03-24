using UnityEngine;
/// <summary>
/// A static class that contains Extension Functions for the SpriteRenderer Unity Datatype
/// </summary>
public static class SpriteRendererExtensionMethods
{
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
