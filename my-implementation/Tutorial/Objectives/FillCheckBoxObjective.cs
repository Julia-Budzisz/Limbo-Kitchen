using UnityEngine;

// Objective focused on teaching movement and camera view switching.
[CreateAssetMenu(fileName = "FillCheckBoxObjective", menuName = "Scriptable Objects/FillCheckBoxObjective")]
public class FillCheckBoxObjective : QuestObjective
{
    public string fill1Name;
    public string fill2Name;
    public string fill3Name;
    public float requiredMovementTime = 7f; // Player must move for X seconds
    private float _currentMovementTime = 0f;
    public override void Execute()
    {
         _currentMovementTime = 0f;
    }

    public override void OnUpdate()
    {
        // Read movement input from the InputManager.
        Vector3 moveInput = InputManager.Instance.MovementAction.ReadValue<Vector3>();

        // Movement tracking logic.
        if (moveInput.magnitude > 0.1f)
        {
            _currentMovementTime += Time.deltaTime;
        }

        if (_currentMovementTime >= requiredMovementTime)
        {
            TutorialUIManager.Instance.ShowUIElement(fill1Name);
        }

        // Validating camera positions (Cafe/Serve) as objective sub-tasks.
        if (CameraPositionsManager.Instance.IsInCafePosition)
        {
            TutorialUIManager.Instance.ShowUIElement(fill2Name);
        }

        if (CameraPositionsManager.Instance.IsInServePosition)
        {
            TutorialUIManager.Instance.ShowUIElement(fill3Name);
           
        }

        CheckTasks();
    }

    private void CheckTasks()
    {
        // Verify if all UI task markers are active (completed).
        bool f1 = TutorialUIManager.Instance.CheckFill(fill1Name);
        bool f2 = TutorialUIManager.Instance.CheckFill(fill2Name);
        bool f3 = TutorialUIManager.Instance.CheckFill(fill3Name);

        if (f1 && f2 && f3)
        {
            // Auto-switch camera to serving view after teaching movement.
            CameraPositionsManager.Instance.ChangeCameraPosition(ViewStates.Serve);
            EndExecution();
        }

    }
}
