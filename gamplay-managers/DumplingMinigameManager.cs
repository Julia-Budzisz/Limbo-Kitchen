using System.Collections.Generic;
using UnityEngine;

/* * ORIGINAL SYSTEM BY: Kateryna Rashkovska (Team Member)
 * TUTORIAL INTEGRATION & EXTENSIONS BY: Julia Budzisz
 * * Context: State Machine architecture created by a team member. 
 * I extended it by adding a mechanism to forcefully skip states, 
 * which was necessary to streamline the player's tutorial experience.
 */
public class DumplingMinigameManager : InstanceBaseClass<DumplingMinigameManager>, IStateHandler
{
    [SerializeField] private int _stages;
    private BaseStateMachine _stateMachine;
    private List<State> _states;
    private int _stateIndex;

    [SerializeField] private List<MonoBehaviour> executables;

    void Start()
    {
        _stateMachine = new BaseStateMachine();
        _states = new List<State>();
        CreateAllStates();

        foreach (var state in _states)
        {
            _stateMachine.AddState(state);
        }

        enabled = false;
    }

    public void StartDumplingsMinigame()
    {
        _stateIndex = 0;
        _stateMachine.SetState(_states[_stateIndex++]);
        enabled = true;
    }

    private void CreateAllStates()
    {
        for (int i = 0; i < _stages; i++)
        {
            _states.Add(new State((IExecutable)executables[i], i, this));
            executables[i].gameObject.SetActive(false);
        }
    }

    public void CompleteStep()
    {

        if (_stateIndex >= _states.Count)
        {
            _stateMachine.SetState();
            MinigamesManager.Instance.EndMinigame(true);
            enabled = false;
            return;
        }

        _stateMachine.SetState(_states[_stateIndex++]);
    }

    // =========================================================================
    // --- TUTORIAL EXTENSION START ---
    // Injected a state-skipping mechanism to allow the tutorial objectives
    // to bypass specific minigame phases for a focused learning experience.
    // =========================================================================

    public void SkipToState(int index)
    {
        if (index >= 0 && index < _states.Count)
        {
            // Reset visuals for all steps to prevent overlapping UI/Objects
            foreach (var e in executables) e.gameObject.SetActive(false);

            _stateMachine.SetState(); // Clear current state
            _stateIndex = index;
            _stateMachine.SetState(_states[_stateIndex++]); // Force new state

            Debug.Log($"[Tutorial] Switched to step: {index}");
        }
    }
    // --- TUTORIAL EXTENSION END ---
}
