using UnityEngine;

[CreateAssetMenu(fileName = "Soundtrack", menuName = "Scriptable Objects/Soundtrack")]
public class Soundtrack : ScriptableObject
{
    public AudioClip intro;
    public AudioClip mainLoop;
    public AudioClip interlude;
    public AudioClip outro;
}
