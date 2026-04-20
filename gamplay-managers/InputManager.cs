using UnityEngine.InputSystem;

/* * ORIGINAL SYSTEM BY: Kateryna Rashkovska (Team Member)
 * TUTORIAL INTEGRATION BY: Julia Budzisz
 * * Context: Standard Input System wrapper. I added helper methods to 
 * selectively enable/disable controls during the tutorial to prevent 
 * unintended player movement while popups or instructions are active.
 */
public class InputManager : InstanceBaseClass<InputManager>
{
    public GameInput GameInput { get; private set; }

    public InputAction MovementAction { get; private set; }

    public InputAction MouseAction { get; private set; }
    public InputAction InteractionAction { get; private set; }

    public InputAction ValidationAction { get; private set; }

    public InputAction MenuListenerAction { get; private set; }

    private void OnValidate()
    {
        GameInput = new GameInput();
        MovementAction = GameInput.Gameplay.Movement;
        InteractionAction = GameInput.Gameplay.Interaction;
        ValidationAction = GameInput.Gameplay.Validation;
        MouseAction = GameInput.Gameplay.MouceUsage;
        MenuListenerAction = GameInput.Gameplay.MenuListener;
    }

    protected override void OnInstanceBaseClassAwake()
    {
        DontDestroyOnLoad(this);
        GameInput = new GameInput();
        MovementAction = GameInput.Gameplay.Movement;
        InteractionAction = GameInput.Gameplay.Interaction;
        ValidationAction = GameInput.Gameplay.Validation;
        MouseAction = GameInput.Gameplay.MouceUsage;
        MenuListenerAction = GameInput.Gameplay.MenuListener;
    }

    private void OnEnable()
    {
        GameInput.Enable();
    }

    private void OnDisable()
    {
        GameInput.Disable();
    }

    public void OnMinigameStarted()
    {
        MovementAction.Disable();
        InteractionAction.Disable();
    }

    public void OnMinigameFinished()
    {
        MovementAction.Enable();
        InteractionAction.Enable();
    }

    // =========================================================================
    // --- TUTORIAL EXTENSION START (My Contribution) ---
    // A utility method to bulk-manage input states during tutorial sequences.
    // Useful for freezing the player while a dialogue or UI popup is shown.
    // =========================================================================

    public void TutorialControls(bool enable)
    {
        if (enable)
        {
            MovementAction.Enable();
            InteractionAction.Enable();
            ValidationAction.Enable();
        }
        else
        {
            MovementAction.Disable();
            InteractionAction.Disable();
            ValidationAction.Disable();
        }    
    }
    // --- TUTORIAL EXTENSION END ---
}
