using UnityEngine;

// Objective that teaches or forces the "Customer Kick/Push" mechanic.
// Syncs player position, customer count, and specialized gameplay managers.
[CreateAssetMenu(fileName = "KickCustomerObjective", menuName = "Scriptable Objects/KickCustomerObjective")]
public class KickCustomerObjective : QuestObjective
{
    [TextArea(3, 10)]
    public string pandaMessage;
    public string popupName;
    private bool _pushStarted = false;
    public override void Execute()
    {
        // Ensuring the validator doesn't block progress during this specific tutorial step.
        OrderValidator.Instance.ForceFail = false;
        TutorialUIManager.Instance.ShowPopup(popupName, pandaMessage);
        _pushStarted = false;
    }

    public override void OnUpdate()
    {
        // Condition: Trigger the push logic only when the queue is full enough 
        // and the player is looking at the correct area (Serving Position).
        if (!_pushStarted && QueueManager.Instance.Customers.Count >= 2 && CameraPositionsManager.Instance.IsInServePosition)
        {
            _pushStarted = true;
            // Force-triggering a specific gameplay mechanic through its manager.
            PushingManager.Instance.TutorialForcePush();
            
        }

        // Completion: Detect when the customer count has decreased (successful push).
        if (_pushStarted && QueueManager.Instance.Customers.Count == 1)
        {
            TutorialUIManager.Instance.HidePopup(popupName);
            EndExecution();
        }
    }
}
