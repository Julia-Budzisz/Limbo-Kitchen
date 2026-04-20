using System.Collections.Generic;
using UnityEngine;

// Tutorial objective managing the "Rush Hour" phase.
// Synchronizes game modes, NPC spawning, and stage-end conditions.
[CreateAssetMenu(fileName = "StartRushHourObjective", menuName = "Scriptable Objects/StartRushHourObjective")]
public class StartRushHourObjective : QuestObjective
{
    private PeakHourStage _cachedPeakHourStage;
    [SerializeField] private List<NPC> rushHourNPCs;
    public override void Execute()
    {
        // Switch to tutorial mode and prepare the NPC list for the manager.
        GameManager.Instance.ChangeMode(GameManager.GameMode.Tutorial);
        NPC_Manager.Instance.PrepareTutorialManager(rushHourNPCs);
        _cachedPeakHourStage = FindFirstObjectByType<PeakHourStage>();

        if (_cachedPeakHourStage != null)
        {
            _cachedPeakHourStage.Execute();
            // Setup stage with a dynamic count based on the provided NPC list.
            NPC_Manager.Instance.SetUpStage(rushHourNPCs.Count, 2f);

            if (NPC_OrderController.Instance != null)
            {
                // Force specific orders for tutorial purposes.
                NPC_OrderController.Instance.SetupNPCOrders(1, 0);
            }
        }
    }

    public override void OnUpdate()
    {
        if (_cachedPeakHourStage == null) return;

        // Completion condition: all customers served and the queue is empty.
        bool allCustomersServed = NPC_Manager.Instance.Completed;

        if (allCustomersServed && QueueManager.Instance.Customers.Count == 0)
        {
            
            _cachedPeakHourStage.EndExecution();
            EndExecution();
        }
        // Fail-safe: if the manager is disabled, terminate the stage gracefully.
        else if (!NPC_Manager.Instance.enabled && QueueManager.Instance.Customers.Count == 0)
        {
            _cachedPeakHourStage.EndExecution();
            EndExecution();
        }
    }
}
