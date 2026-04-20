using System;
using UnityEngine;

// A specialized objective that manipulates the internal state of a minigame.
// Demonstrates the ability to bypass content to focus on specific tutorial teaching points.
[CreateAssetMenu(fileName = "CustomMinigameStepsObjective", menuName = "Scriptable Objects/CustomMinigameStepsObjective")]
public class CustomMinigameStepsObjective : QuestObjective
{
    [TextArea(3,10)]
    public string massage; // Instructions/Lore text for the popup.
    public string popupName;

    private bool _hasSkippedToSix = false;
    public override void Execute()
    {
        _hasSkippedToSix = false;

        // Forcefully moving the minigame to a specific starting state for the tutorial.
        DumplingMinigameManager.Instance.SkipToState(1);
        TutorialUIManager.Instance.ShowPopup(popupName, massage);
    }

    public override void OnUpdate()
    {
        // Detects when a specific minigame stage is completed/deactivated.
        if (!DumplingsSecondStepExecutable.Instance.StageSetUp.activeSelf && !_hasSkippedToSix)
        {
            _hasSkippedToSix = true;

            TutorialUIManager.Instance.HidePopup(popupName);

            // Skipping directly to the end-stage of the minigame to maintain tutorial pacing.
            DumplingMinigameManager.Instance.SkipToState(5);
            EndExecution();
        }
    }
}
