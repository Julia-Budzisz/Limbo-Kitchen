using System;
using UnityEngine;

/* * ORIGINAL SYSTEM BY: Kateryna Rashkovska (Team Member)
 * * Context: Included to provide context for the portfolio. 
 * My Tutorial System hooks into this script's public properties 
 * (like StageSetUp.activeSelf) to track the player's minigame progress 
 * without modifying the core logic of the step itself.
 */
public class DumplingsSecondStepExecutable : InstanceBaseClass<DumplingsSecondStepExecutable>, IExecutable
{
    public static Action OnStepStarted;
    public static Action OnStepCompleted;

    [SerializeField] private SpriteRenderer bowl;
    [SerializeField] private IngredientManager[] ingredients;
    [SerializeField] private Sprite[] bowls;
    [SerializeField] private SpoonController spoon;
    [SerializeField] private int maxIngredientAmount = 3;

    [field: SerializeField] public GameObject StageSetUp { get; private set; }
    [field: SerializeField] public string TargetTag { get; private set; }
    public IStateHandler StateHandler { get; set; }
    public bool IngredientsUsedUp => _usedIngredients == maxIngredientAmount;

    private int _usedIngredients = 0;
    private bool _isSpoonUsed = false;

    #region UnityMethods

    private void OnEnable()
    {
        InputManager.Instance.MouseAction.performed += Check;
    }

    private void OnDisable()
    {
        InputManager.Instance.MouseAction.performed -= Check;
        StageSetUp.SetActive(false);
    }

    #endregion

    public void Execute()
    {
        StageSetUp.SetActive(true);
        gameObject.SetActive(true);

        _usedIngredients = 0;
        StopMGButton.Instance.SetExecutable(this);

        bowl.sprite = bowls[0];
        _isSpoonUsed = false;

        OnStepStarted?.Invoke();
    }

    private void Check(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if(!_isSpoonUsed)
        {
            foreach (var ingr in ingredients)
            {
                if (Raycaster.CheckRaycastResult(ingr, RaycastUtility.GetRay()))
                    ingr.OnRaycast();
            }

            if (Raycaster.CheckRaycastResult(spoon, RaycastUtility.GetRay()) && IngredientsUsedUp)
                spoon.OnRaycast();
        }
        
    }

    public void IngredientUsed()
    {
        _usedIngredients++;

        if(IngredientsUsedUp)
            bowl.sprite = bowls[1];
    }

    public void EndExecution()
    {
        OnStepCompleted?.Invoke();

        gameObject.SetActive(false);
    }

    public void SpoonIsUsed()
    {
        SwitchBTNLogic.Instance.RequirementsCompleted(this);
        _isSpoonUsed = true;
    }
}
