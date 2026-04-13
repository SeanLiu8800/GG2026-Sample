using UnityEngine;

[CreateAssetMenu(fileName = "Soundtrack", menuName = "Scriptable Objects/Soundtrack")]
public class Soundtrack : ScriptableObject
{
    [Range(0.0f, 200.0f)] public float tempo = 140.0f;

    public AudioClip intro;
    public AudioClip mainLoop;
    public AudioClip interlude;
    public AudioClip outro;
}
