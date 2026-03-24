using UnityEngine;
using System.Collections;
/// <summary>
/// A static class containing Helper Functions for Fading In/ Out Sprites over time
/// </summary>
public static class FadeSpriteHelper
{
    /// <summary>
    /// Fades in/ out an input Sprite Renderer to a given Alpha Level over a given Duration<br/>
    /// NOTE: This function is meant to be a Fire and Forget method, use FadeCoroutine() if you want to yield for it
    /// </summary>
    /// <param name="caller">The MonoBehavior calling this function, this is needed because Fade() uses a Coroutine</param>
    /// <param name="spriteRenderer">The Sprite Renderer to be Faded in/ out</param>
    /// <param name="alphaTarget">The target Alpha level for the Sprite Renderer's Color field</param>
    /// <param name="fadeDuration">The duration of the Fade in/ out</param>
    public static void Fade(MonoBehaviour caller, SpriteRenderer spriteRenderer, float alphaTarget = 0.0f, float fadeDuration = 0.2f)
    {
        if (spriteRenderer == null) return;
        if (fadeDuration < 0.0f) fadeDuration = 0.0f;

        caller.StartCoroutine(FadeCoroutine(spriteRenderer, alphaTarget, fadeDuration));
    }
    /// <summary>
    /// Fades in/ out an input Sprite Renderer to a given Alpha Level over a given Duration<br/>
    /// NOTE: This is a Coroutine, so be sure to use StartCoroutine(FadeSpriteHelper.FadeCoroutine(...))!
    /// </summary>
    /// <param name="spriteRenderer">The Sprite Renderer to be Faded in/ out</param>
    /// <param name="alphaTarget">The target Alpha level for the Sprite Renderer's Color field</param>
    /// <param name="fadeDuration">The duration of the Fade in/ out</param>
    public static IEnumerator FadeCoroutine(SpriteRenderer spriteRenderer, float alphaTarget = 0.0f, float fadeDuration = 0.2f)
    {
        if (spriteRenderer == null) yield break;
        if (fadeDuration < 0.0f) fadeDuration = 0.0f;

        float fadeOutStartTime = Time.time;
        float fadeOutProgress = (fadeDuration == 0.0 ? 1.0f : 0.0f);
        float startingOpacity = spriteRenderer.color.a;
        while (fadeOutProgress < 1.0f)
        {
            fadeOutProgress = (Time.time - fadeOutStartTime) / fadeDuration;
            spriteRenderer.SetAlpha(Mathf.Lerp(startingOpacity, alphaTarget, fadeOutProgress));
            yield return null;
        }
        spriteRenderer.SetAlpha(alphaTarget);
    }
}
