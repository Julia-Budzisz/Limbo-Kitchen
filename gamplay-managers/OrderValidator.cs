using System.Collections.Generic;
using UnityEngine;

/* * ORIGINAL SYSTEM ARCHITECTURE BY: Kateryna Rashkovska (Team Member)
 * TUTORIAL INTEGRATION & EXTENSIONS BY: [Julia Budzisz
 * * Context: I extended the validation logic to support tutorial-specific 
 * edge cases, ensuring the game doesn't crash when testing mechanics 
 * without a live NPC. I also added the 'ForceFail' flag to control 
 * tutorial outcomes for demonstration purposes.
 */
public class OrderValidator : InstanceBaseClass<OrderValidator>
{
    [SerializeField] private List<StructForDictionary<int, int>> requirements;
    
    private int _maxSkillCheckPoints; 
    private int _maxPreferencesPoints;
    private int _currentPreferencesPoints;
    private int _currentSkillCheckPoints;
    private int _skillCheck;
    private int _preferencesCheck;

    private FinalResultEnum _currentOrderResult = FinalResultEnum.None;
    // --- TUTORIAL EXTENSION ---
    // Added to allow the Tutorial System to force a specific result (e.g., failure)
    // regardless of actual player performance for teaching purposes.
    public bool ForceFail { get; set; }

    public void ValidateIngredient(PreferencesEnum ingr)
    {
        if (OrderManager.Instance.CurrentCustomer != null)
        {
            if (OrderManager.Instance.CurrentCustomer.FeedingType == ingr) _currentPreferencesPoints++;
        }
        // --- TUTORIAL EXTENSION ---
        // Allows testing the cooking mechanic during tutorial phases 
        // where a customer might not be present at the counter yet.
        else
        {
            _currentPreferencesPoints++;
            Debug.Log("Tutorial: skip validation");
        }
            
    }

    public void ValidateSpices(SpicesEnum spices)
    {
        if (OrderManager.Instance.CurrentCustomer != null)
        {
            if (OrderManager.Instance.CurrentCustomer.SpicesType == spices) _currentPreferencesPoints++;
        }
        // --- TUTORIAL EXTENSION ---
        else
        {
            _currentPreferencesPoints++;
            Debug.Log("Tutorial: skip validation");
        }

           
    }

    public void SetSuccessfulSkillCheck()
    {
        _currentSkillCheckPoints++;
    }

    public void SetOrderResult()
    {
        // --- TUTORIAL EXTENSION ---
        if (ForceFail)
        {
            _currentOrderResult = FinalResultEnum.TerribleOrder;
            ForceFail = false; 
        }
        else
        {
            SetUpMaxPoints();
            CountPoints();
            ResetPoints();

            var result = (FinalResultEnum)(_skillCheck + _preferencesCheck);

            if (_currentOrderResult == FinalResultEnum.None)
                _currentOrderResult = result;
            else if (_currentOrderResult != result)
                _currentOrderResult = FinalResultEnum.AverageOrder;
        }
    }


    public FinalResultEnum GetFinalResult()
    {
        return _currentOrderResult;
    }

    private void SetUpMaxPoints()
    {
        _maxSkillCheckPoints = requirements[NPC_OrderController.Instance.CurrentZone.Index].key;
        _maxPreferencesPoints = requirements[NPC_OrderController.Instance.CurrentZone.Index].value;
    }

    private void CountPoints()
    {
        _skillCheck = _currentSkillCheckPoints == _maxSkillCheckPoints ? (int)QualityCheckEnum.GoodSkillCheck : (int)QualityCheckEnum.BadSkillCheck;
        _preferencesCheck = _currentPreferencesPoints == _maxPreferencesPoints ? (int)QualityCheckEnum.CorrectIngredient : (int)QualityCheckEnum.IncorrectIngredient;
    }

    public int GetDishResultIndex()
    {
        SetUpMaxPoints();

        if (_currentSkillCheckPoints == _maxSkillCheckPoints && _currentPreferencesPoints == _maxPreferencesPoints) return 0;
        else return 1;
    }

    private void ResetPoints()
    {
        _currentSkillCheckPoints = 0;
        _currentPreferencesPoints = 0;
    }

}
