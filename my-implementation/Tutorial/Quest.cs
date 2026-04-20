using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Quest", menuName = "Scriptable Objects/Quest")]
public class Quest : ScriptableObject
{
    public List<QuestObjective> Objectives = new(); // List of steps for this tutorial quest.
    public Action OnComplete;

    private int _currentObjectiveIndex = 0;
    public void Init()
    {
        _currentObjectiveIndex = 0;

        // Creating runtime instances of Scriptable Objects 
        // to prevent overwriting project assets during gameplay.
        List<QuestObjective> tmp = new List<QuestObjective>();
        foreach (QuestObjective objective in Objectives)
            tmp.Add(Instantiate(objective));

        Objectives = tmp;

        if (Objectives.Count > 0)
        {
            ActivateObjective(_currentObjectiveIndex);
        }
    }
    private void ActivateObjective(int index)
    {
        if (index < Objectives.Count)
        {
            
            Objectives[index].OnComplete += OnQuestObjectiveCompleted;
            Objectives[index].Execute();
            
        }
        else
        {
            
            OnComplete?.Invoke(); // Triggered when all objectives are finished.
        }
    }

    private void OnQuestObjectiveCompleted()
    {
        // Unsubscribe from completed objective and move to the next one.
        Objectives[_currentObjectiveIndex].OnComplete -= OnQuestObjectiveCompleted;
        _currentObjectiveIndex++;
        ActivateObjective(_currentObjectiveIndex);
    }

    public void Update()
    {
        // Only updates the currently active objective (optimization).
        if (_currentObjectiveIndex < Objectives.Count)
        {
            Objectives[_currentObjectiveIndex].OnUpdate();
        }
    }
}
