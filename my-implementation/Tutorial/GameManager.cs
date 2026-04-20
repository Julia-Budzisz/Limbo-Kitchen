using System;
using UnityEngine;

// Central coordinator for the overall game state.
// Designed to manage global modes and facilitate communication between systems.
public class GameManager : InstanceBaseClass<GameManager>
{
    // The current active mode of the game. 
    // Using a private setter ensures that the state can only be modified 
    // through controlled methods, maintaining data integrity.
    public GameMode CurrentMode { get; private set; } = GameMode.Gameplay;

    // A global gatekeeper used to prevent or allow minigame activation.
    // Crucial for tutorial sequences where player interaction needs to be restricted.
    public bool CanStartMinigame = true;

    // Defines the primary operating modes of the application.
    public enum GameMode
    {
        Gameplay, // Standard game loop with all mechanics enabled.
        Tutorial, // Restricted mode where modular tutorial logic takes priority.      
    }

    // Changes the global game mode and updates dependent systems.
    public void ChangeMode(GameMode mode)
    {
        CurrentMode = mode;
        Debug.Log("Current Mode is: " + mode);
    }
}
