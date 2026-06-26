using UnityEngine;

public abstract class Player_OnEngineFullyDepletes_BehaviorBase : PlayerComponent
{
    protected virtual void OnEnable()
    {
        player.playerEvents.engineFullyDepletes += OnEngineFullyDepletes;
    }
    protected virtual void OnDisable()
    {
        player.playerEvents.engineFullyDepletes -= OnEngineFullyDepletes;
    }
    protected abstract void OnEngineFullyDepletes(GameObject depleter);
}
