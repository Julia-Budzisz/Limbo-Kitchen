using System;
using System.Collections.Generic;
using UnityEngine;

/* * ORIGINAL SYSTEM BY: Kateryna Rashkovska (Team Member)
 * * Context: Included for portfolio context. My Tutorial System uses 
 * the SetupNPCOrders method to force orders at specific kitchen zones, 
 * allowing the tutorial to guide the player to exact locations.
 */
public class NPC_OrderController : InstanceBaseClass<NPC_OrderController>
{
    public static event Action<bool> ResetAfterOrderIsReady;
    
    [SerializeField] private TriggerZoneLogic[] zones;
    [field: SerializeField] public List<StructForDictionary<DishesEnum, Sprite>> Dishes {  get; private set; }
    public TriggerZoneLogic CurrentZone {  get; private set; }
    public int Index { get; private set; }

    public void SetupNPCOrders(int amount, int index)
    {
        Index = index;
        
        if (amount > 1)
        {
            foreach (var zone in zones)
            { 
                zone.SetOrdered(true); 
            }

            return;
        }
        else if(Index == -1)
        {
            Index = UnityEngine.Random.Range(0, zones.Length);
        }

        zones[Index].SetOrdered(true);
    }

    public void ResetAfterReady()
    {
        ResetAfterOrderIsReady?.Invoke(false);
    }

    public void SetupZone(TriggerZoneLogic t)
    { 
        CurrentZone = t;
    }

    public void ResetZone()
    {
        CurrentZone = null;
    }
}
