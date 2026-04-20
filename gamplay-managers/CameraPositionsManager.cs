using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/* * ORIGINAL SYSTEM BY: Kateryna Rashkovska Team Member
 * * Context: Included for portfolio context. My Tutorial System utilizes 
 * the public boolean properties (IsInBridgePosition, IsInServePosition, etc.) 
 * to validate player movement objectives without modifying the camera logic.
 */
public class CameraPositionsManager : InstanceBaseClass<CameraPositionsManager>
{
    [SerializeField] private Transform mainCamera;
    [SerializeField] private StructForDictionary<ViewStates, Transform>[] cameraPositions;
    [SerializeField] private float transitionDuration = 2f;

    public bool IsInBridgePosition { get { return mainCamera.transform.position == _cameraPositions[ViewStates.Bridge].position;} }  
    public bool IsInServePosition { get { return mainCamera.transform.position == _cameraPositions[ViewStates.Serve].position;} }

    public bool IsInCafePosition { get { return mainCamera.transform.position == _cameraPositions[ViewStates.Cafe].position;} }

    private Dictionary<ViewStates, Transform> _cameraPositions = new();


    void Start()
    {
        _cameraPositions.Clear();
        foreach (var camPos in cameraPositions)
        {
            _cameraPositions.Add(camPos.key, camPos.value);
        }
    }

    private void Update()
    {
        if (InputManager.Instance.ValidationAction.WasPressedThisFrame() && IsInBridgePosition)
        {
            ChangeCameraPosition(ViewStates.Serve);
            PlayerPhysicsLogic.Instance.gameObject.SetActive(false);
            AudioManager.Instance.SetViewState(ViewStates.Serve);
        }

        else if (InputManager.Instance.ValidationAction.WasPressedThisFrame() && IsInServePosition)
        {
            ChangeCameraPosition(ViewStates.Bridge);
            PlayerPhysicsLogic.Instance.gameObject.SetActive(true);
            AudioManager.Instance.SetViewState(ViewStates.Bridge);
        }
    }

    public void ChangeCameraPosition(ViewStates positionState)
    {
        StartCoroutine(ChangePosition(_cameraPositions[positionState]));
    }

    public void ChangeAvailability()
    { 
        enabled = !enabled;
    }

    private IEnumerator ChangePosition(Transform destination)
    { 
        InputManager.Instance.GameInput.Disable();

        mainCamera.GetPositionAndRotation(out Vector3 startingPos, out Quaternion startingRot);
        AudioManager.Instance.PlayCameraSwitch();
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionDuration;
            mainCamera.SetPositionAndRotation(Vector3.Lerp(startingPos, destination.position, t), Quaternion.Slerp(startingRot, destination.rotation, t));
            yield return null;
        }

        mainCamera.SetPositionAndRotation(destination.position, destination.rotation);

        AudioManager.Instance.SetViewState(_cameraPositions.FirstOrDefault(x => x.Value == destination).Key);

        InputManager.Instance.GameInput.Enable();

    }

}
