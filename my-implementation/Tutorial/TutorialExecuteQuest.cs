using UnityEngine;

// MonoBehavior responsible for starting and updating the tutorial Quest system.
// This acts as the bridge between the scene and the Quest Scriptable Object.
public class TutorialExecuteQuest : MonoBehaviour
{
    public Quest questToStart;

    private void Start()
    {
        // We instantiate the Quest to create a unique runtime copy,
        // ensuring tutorial progress doesn't permanently modify the project asset.
        questToStart = Instantiate(questToStart);
        questToStart.Init();

        // Subscribing to the completion event to handle end-of-tutorial logic.
        questToStart.OnComplete += OnComplete;
    }

    private void Update()
    {
        // Pumping the update cycle into the active quest.
        questToStart.Update();
    }

    private void OnComplete()
    {
        // Cleaning up the event subscription to prevent memory leaks.
        questToStart.OnComplete -= OnComplete;
    }
}
