using System.Collections;
using UnityEngine;

public class TempGrabCinematic : GrabCinematicBase
{
    protected override IEnumerator GrabCinematic()
    {
        Debug.Log("Temp Grab Cinematic is properly initialized!");
        yield break;
    }
}
