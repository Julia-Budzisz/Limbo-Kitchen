using System.Collections.Generic;
using UnityEngine;

// Manages the physical placement of the Panda character based on predefined game states.
// Inherits from InstanceBaseClass to allow global access via the Singleton pattern.
public class PandaPositionsManager : InstanceBaseClass<PandaPositionsManager>
{
    [SerializeField] private GameObject panda; // Reference to the Panda character object.

    // An array used to populate the dictionary via the Unity Inspector.
    // Demonstrates a clean workaround for Unity's lack of native Dictionary serialization.
    [SerializeField] private StructForDictionary<PandaStates, Transform>[] pandaPositions;

    private Dictionary<PandaStates, Transform> _pandaPositions = new();

    void Awake() 
    {
        // Inherited Awake logic might be present in the base class.
        InitializeDictionary();

        // Sets the default starting position for the Panda.
        MovePanda(PandaStates.Serve);
    }

    // Transfers data from the Inspector-friendly array to a performant Dictionary.
    private void InitializeDictionary()
    {
        _pandaPositions.Clear();
        foreach (var pos in pandaPositions)
        {
            if (!_pandaPositions.ContainsKey(pos.key))
                _pandaPositions.Add(pos.key, pos.value);
        }
    }

    // Updates the Panda's position and rotation based on the requested state.
    public void MovePanda(PandaStates newState)
    {
        // Fail-safe: ensures the dictionary is populated if accessed before/during initialization.
        if (_pandaPositions.Count == 0)
        {
            foreach (var pos in pandaPositions)
            {
                if (!_pandaPositions.ContainsKey(pos.key))
                    _pandaPositions.Add(pos.key, pos.value);
            }
        }

        // Moves the character transform to the target location mapped to the state.

        if (_pandaPositions.ContainsKey(newState))
        {
            Transform target = _pandaPositions[newState];
            panda.transform.position = target.position;
            panda.transform.rotation = target.rotation;
            
        }
        
    }
}
