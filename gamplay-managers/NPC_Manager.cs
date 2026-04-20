using System.Collections.Generic;
using UnityEngine;

/* * ORIGINAL SYSTEM ARCHITECTURE BY: Kateryna Rashkovska (Team Member)
 * TUTORIAL INTEGRATION & EXTENSIONS BY: Julia Budzisz
 * * Context: I extended this existing team script to allow my modular 
 * Tutorial System to inject predefined NPC lists and bypass the standard 
 * randomized spawning logic during the tutorial phase.
 */
public class NPC_Manager : InstanceBaseClass<NPC_Manager> 
{
    [SerializeField] private List<Transform> tablesSpots;
    [SerializeField] private NPC_Spawner spawner;
    [SerializeField] private List<StructForDictionary<FinalResultEnum, OrderResultManager>> objects;
    [SerializeField] private Transform endPositionTransform;

    [field: SerializeField] public float TableTime { get; private set; } = 40f;

    public bool Completed => _completedCustomers >= _customersPerStageCount;

    private List<NPC> _customersPerLevel;
    private List<SpecialGuest> _specialGuests;
    private int _customersPerStageCount; 
    private float _spawnDelayInSeconds; 
    private int _completedCustomers = 0;
    private Dictionary<FinalResultEnum, OrderResultManager> _orderResultManagers = new();
    private float _currentDelay = 0;
    private int _index = 0;
    private IExecutable _currentExecutable;

    public void  PrepareManager()
    {
        foreach (var item in objects)
        {
            _orderResultManagers.Add(item.key, item.value);
        }

        _customersPerLevel = spawner.CreateCustomers();
        _specialGuests = spawner.CreateSpecialGuest();

        enabled = false;
    }

    void Update()
    {
        if (_customersPerLevel == null || _customersPerLevel.Count == 0 || _index >= _customersPerStageCount)
        {
            enabled = false;
            return;
        }
        if(_currentDelay >= _spawnDelayInSeconds)
        {
            SpawnNextCustomer();
            _currentDelay = 0;
        }
        else _currentDelay += Time.deltaTime;
    }

    public void SetUpStage(int customers, float delay)
    {
        _customersPerStageCount = customers;
        _spawnDelayInSeconds = delay;
        _index = 0;
        _completedCustomers = 0;
        _currentDelay = 0;
        enabled = true;
    }

    public void SetCurrentExecutable(IExecutable exe)
    {
        _currentExecutable = exe;
    }

    public void SpawnNextCustomer()
    {
        if (_customersPerLevel == null || _customersPerLevel.Count == 0)
        { 
            return;
        }

        NPC npc = _customersPerLevel[0];

        _customersPerLevel.RemoveAt(0);
        SetUpAfterSpawned(npc);
    }

    public void SpawnSpecialGuest()
    {
        SpecialGuest guest = _specialGuests[0];

        _specialGuests.RemoveAt(0);
        SetUpAfterSpawned(guest);
    }

    private void SetUpAfterSpawned(NPC_Base customer)
    {
        customer.transform.position = transform.position;
        customer.gameObject.SetActive(true);
        customer.Initialize();
        ShiftManager.Instance.OrdersPerLevel += customer.OrdersCount;
        Debug.Log("Orders: " + ShiftManager.Instance.OrdersPerLevel);
        _index++;
    }

    public OrderResultManager GetOrderResultManager(FinalResultEnum resultEnum) 
    {

        return _orderResultManagers[resultEnum];
    }

    public Transform GetTableSpot()
    {
        Transform table = tablesSpots[0];
        tablesSpots.Remove(table);
        tablesSpots.Add(table);

        return table;
    }

    public Transform GetEndPosition()
    {
        return endPositionTransform;
    }

    public void CustomerCompleted()
    { 
        _completedCustomers++;
        if (Completed) 
        {
            _currentExecutable?.SetAsCompleted();
        }
    }

    // =========================================================================
    // --- TUTORIAL EXTENSION START  ---
    // Added specific initialization methods to feed the manager with predefined
    // tutorial data instead of randomized standard gameplay data.
    // =========================================================================

    public void PrepareTutorialManager(List<NPC> tutorialNPC)
    {
        foreach (var item in objects)
        {
            if (!_orderResultManagers.ContainsKey(item.key))
                _orderResultManagers.Add(item.key, item.value);
        }

        // Bypassing standard spawner to use specific tutorial customers
        _customersPerLevel = spawner.CreateTutorialCustomers(tutorialNPC);
        _customersPerStageCount = tutorialNPC.Count;
        _spawnDelayInSeconds = 2f;

        _index = 0;
        _currentDelay = 0;
        _completedCustomers = 0;


        enabled = true; 
    }

    public void PrepareTutorialSPManager(List<SpecialGuest> tutorialSP)
    {
        foreach (var item in objects)
        {
            if (!_orderResultManagers.ContainsKey(item.key))
                _orderResultManagers.Add(item.key, item.value);
        }

        _specialGuests = spawner.CreateTutorialSPCustomers(tutorialSP);
        _customersPerStageCount = tutorialSP.Count;
        _spawnDelayInSeconds = 2f;

        enabled = true;
    }

    // --- TUTORIAL EXTENSION END ---
}

