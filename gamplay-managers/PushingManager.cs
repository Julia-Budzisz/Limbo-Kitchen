using System.Collections;
using System.Data;
using UnityEngine;

/* * ORIGINAL SYSTEM BY: Kateryna Rashkovska (Team Member)
 * TUTORIAL INTEGRATION & EXTENSIONS BY: Julia Budzisz
 * * Context: State Machine architecture created by a team member. 
 * I extended it by adding a mechanism to forcefully skip states, 
 * which was necessary to streamline the player's tutorial experience.
 */
public class PushingManager : InstanceBaseClass<PushingManager> 
{
    [SerializeField] private float waitingLimit = 85f;
    [SerializeField] private float peakWaitingLimit = 70f;
    [SerializeField] private Transform deathPoint;
    [SerializeField] private Transform pushPoint;
    [SerializeField] private AnimationCurve curvePickUp;
    [SerializeField] private AnimationCurve curveThrow;
    [SerializeField] private float durationPickUp;
    [SerializeField] private float durationThrow;

    public float CurrentCookingTime { get; private set; }
    public float CurrentWaitingTime { get; private set; }
    private Transform _targetPoint;
    private bool _isObserving;

    private Transform _victim;
    private Transform _killer;

    protected override void OnInstanceBaseClassAwake()
    {
        enabled = false;
    }
    
    public void StartObserving()
    { 
        CurrentCookingTime = 0f;
        CurrentWaitingTime = ShiftManager.Instance.IsPeakHour ? peakWaitingLimit : waitingLimit;
        CurrentWaitingTime *= OrderManager.Instance.CurrentCustomer.OrdersCount;
        enabled = true;
        _isObserving = true;
    }

    public void StopObserving()
    {
        _isObserving = false;
        enabled = false;
    }

    public bool IsWaitingTimeExceeded()
    {
        if (QueueManager.Instance.Customers.Count <= 1) return false;
        return CurrentCookingTime >= CurrentWaitingTime;
    }

    public void PushCustomer()
    {
        InputManager.Instance.InteractionAction.Disable();
        _victim = QueueManager.Instance.Customers[0].transform;
        _killer = QueueManager.Instance.Customers[1].transform;

        QueueManager.Instance.Customers[1].PrepareAnimator();
        _targetPoint = QueueManager.Instance.Customers[1].CarryPoint;
        QueueManager.Instance.Customers[0].PrepareCustomer();

        StartCoroutine(PickUpCustomer());
    }

    private IEnumerator PickUpCustomer()
    {
        _victim.GetPositionAndRotation(out var customerStart, out var rotationStart);
        var elapsed = 0f;

        while (elapsed < durationPickUp)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / durationPickUp;
            _victim.SetPositionAndRotation(Vector3.Lerp(customerStart, _targetPoint.position, curvePickUp.Evaluate(t)), Quaternion.Slerp(rotationStart, _targetPoint.rotation, curvePickUp.Evaluate(t)));
            yield return null;
        }

        _victim.SetParent(_killer);
        QueueManager.Instance.Customers[1].PrepareAnimator();
        QueueManager.Instance.Customers[1].SetDestination(pushPoint);

        if (GameManager.Instance.CurrentMode == GameManager.GameMode.Gameplay)
        {
            enabled = true;
        }
        
    }

    public IEnumerator EliminateCustomer()
    {
        _victim.SetParent(null);
        AudioManager.Instance.PlayNPCPush();

        Vector3 start = _victim.position;
        Vector3 end = deathPoint.position;

        float elapsed = 0f;

        OrderManager.Instance.CheckOutCustomer();
        QueueManager.Instance.DequeueCustomer();
        InputManager.Instance.InteractionAction.Enable();

        while (elapsed < durationThrow)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / durationThrow;

            Vector3 pos = Vector3.Lerp(start, end, t);

            float height = curveThrow.Evaluate(t);
            pos.y += height;

            _victim.position = pos;

            yield return null;
        }

        _victim.gameObject.SetActive(false);
        NPC_Manager.Instance.CustomerCompleted();
        AudioManager.Instance.PlayBoiling();
    }

    private void Update()
    {
        if (_isObserving)
        { 
            CurrentCookingTime += Time.deltaTime;
            return;
        }

        // Bypasses regular update logic if we are in Tutorial Mode.
        if (GameManager.Instance.CurrentMode == GameManager.GameMode.Tutorial) return;

        if (!_isObserving)
        {
            if (QueueManager.Instance.Customers[1].HasTheRightDistance)
            {
                StartCoroutine(EliminateCustomer());
                enabled = false;
            }
        }

    }

    // =========================================================================
    // --- TUTORIAL EXTENSION START ---
    // Specifically created for the tutorial phase. It forces the pushing 
    // sequence regardless of timers, ensuring the player learns the mechanic.
    // =========================================================================


    public void TutorialForcePush()
    {
        _isObserving = false;

        InputManager.Instance.InteractionAction.Disable();
        _victim = QueueManager.Instance.Customers[0].transform;
        _killer = QueueManager.Instance.Customers[1].transform;

        QueueManager.Instance.Customers[1].PrepareAnimator();
        _targetPoint = QueueManager.Instance.Customers[1].CarryPoint;
        QueueManager.Instance.Customers[0].PrepareCustomer();

        StartCoroutine(TutorialPushSequence());
    }

    private IEnumerator TutorialPushSequence()
    {
        GameManager.Instance.CanStartMinigame = false;

        NPC_Base killerNPC = QueueManager.Instance.Customers[1];

        // Reusing the base coroutines but dictating the exact flow and timing.
        yield return StartCoroutine(PickUpCustomer());
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(EliminateCustomer());

        

        InputManager.Instance.InteractionAction.Disable();

        // Repositioning the 'killer' NPC to the serving spot to continue the tutorial.
        if (killerNPC != null)
        {
            Transform serveSpot = QueueManager.Instance.GetTheSpot(killerNPC);
            killerNPC.SetDestination(serveSpot);
        }

        AudioManager.Instance.StopBoiling();
    }

    // --- TUTORIAL EXTENSION END ---
}


