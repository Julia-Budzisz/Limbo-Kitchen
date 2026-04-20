using System;
using System.Collections.Generic;
using UnityEngine;

/* * ORIGINAL SYSTEM BY:  Kateryna Rashkovska - Team Member
 * * Context: Included for portfolio context. My tutorial objectives hook 
 * into the public 'Customers' list to verify state changes, such as checking 
 * if a customer has been successfully pushed out of the queue.
 */
public class QueueManager : InstanceBaseClass<QueueManager>
{
    public static event Action OnCustomerDequeued;

    [SerializeField] private List<Transform> queuePositions;
    public List<NPC_Base> Customers { get; private set; } = new List<NPC_Base>();

    public void EnqueueCustomer(NPC_Base npc)
    {
        Customers.Add(npc);
        Debug.Log($"Customer Enqueued. Total Customers: {Customers.Count}");
    }

    public void DequeueCustomer()
    {
        if (Customers.Count == 0) return;

        Debug.Log($"Customer Dequeued. Total Customers: {Customers.Count - 1}");

        NPC_Base customer = Customers[0];
        Customers.RemoveAt(0);

        OnCustomerDequeued?.Invoke();
    }

    public bool IsFirstCustomer(NPC_Base npc)
    {
        return Customers[0] == npc;
    }

    public Transform GetTheSpot(NPC_Base npc)
    { 
        return queuePositions[Customers.IndexOf(npc)];
    }
}
