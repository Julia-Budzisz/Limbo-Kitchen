using System;
using UnityEngine;

// Abstract base class for all tutorial objectives.
// Allows creating modular, data-driven goals as Scriptable Objects.
public abstract class QuestObjective : ScriptableObject
{
    public bool IsCompleted = false;
    public Action OnComplete; // Event triggered when the objective requirements are met.

    // Core methods to be overridden (Execute - initialization, OnUpdate - frame-by-frame logic).
    public abstract void Execute();
    public abstract void OnUpdate();

    // Marks the objective as finished and notifies the Quest system.
    public virtual void EndExecution()
    {
        IsCompleted = true;
        OnComplete?.Invoke();
    }

}
