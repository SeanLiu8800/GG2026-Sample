using UnityEngine;
using System;
public struct RoomEventsStruct
{
    public Action roomStarts;
    
    public Action waveCompleted;
    public Action allWavesCompleted;

    public Action roomEnds;
}
