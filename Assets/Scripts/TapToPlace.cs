using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class TapToPlace : MonoBehaviour
{

    [Tooltip("The AR Raycast Manager on the XR Origin.")]
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private ARPlaneManager planeManager;

    [Tooltip("The model to place. It is hidden until you tap.")]   
    [SerializeField] private Transform model;
    [SerializeField] private LineRenderer areaBoundary;

    [Tooltip("Size of game area in metres")]
    [SerializeField] private float areaSize = 3f;


    [Tooltip("Distance in meters to offset the model from the hit point.")]
    [SerializeField] private float forwardOffset = 3.0f;
    [SerializeField] private bool _isModelSet = false;


    //The raycast writes its results into this list.
    readonly List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Start()
    {    
        model.gameObject.SetActive(false);
        if (areaBoundary != null)
        {
            areaBoundary.positionCount = 5;
            areaBoundary.gameObject.SetActive(false);
        }
    }

 private void Update()
    {
        if (ARSession.state != ARSessionState.SessionTracking)
            return;

        ShowPlacementPreview();

        if (_isModelSet)
            return;

        if (Pointer.current == null ||
            !Pointer.current.press.wasPressedThisFrame)
            return;

        Vector2 screenPosition =
            Pointer.current.position.ReadValue();

        if (raycastManager.Raycast(
                screenPosition,
                hits,
                TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;

            Vector3 spawnPosition = hitPose.position;

            if (Camera.main != null)
            {
                Vector3 camForward =
                    Camera.main.transform.forward;

                camForward.y = 0f;
                camForward.Normalize();

                spawnPosition +=
                    camForward * forwardOffset;
            }

            model.SetPositionAndRotation(
                spawnPosition,
                hitPose.rotation);

            model.gameObject.SetActive(true);

            DrawBoundary(
                spawnPosition,
                hitPose.rotation);

            _isModelSet = true;

            raycastManager.enabled = false;
            planeManager.enabled = false;

            foreach (var plane in planeManager.trackables)
            {
                plane.gameObject.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    private void Update1()
    {
        if(_isModelSet)
            return;

        if (ARSession.state != ARSessionState.SessionTracking)
                    return;

        ShowPlacementPreview();

        // Pointer means a touchscreen on the phone, or the mouse in the Editor.
        if (Pointer.current == null || !Pointer.current.press.wasPressedThisFrame)
            return;
        
        Vector2 screenPosition = Pointer.current.position.ReadValue();

        if (screenPosition == Vector2.zero || 
                    screenPosition.x < 0 || screenPosition.x >= Screen.width || 
                    screenPosition.y < 0 || screenPosition.y >= Screen.height)
                {
                    return;
                }

        if (raycastManager == null)
                    return;

        // Fire a ray from the screen into the real world. Did it hit a detected surface?
        if (raycastManager.Raycast(screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose; // hits[0] is the closest surface
            Vector3 spawnPosition = hitPose.position;
            if (Camera.main != null)
            {
                // Get camera forward direction, flatten it on the Y-axis so it stays on the floor plane
                Vector3 camForward = Camera.main.transform.forward;
                camForward.y = 0; 
                camForward.Normalize();

                // Apply the 1-meter forward offset from the hit point
                spawnPosition += camForward * forwardOffset;
            }
            else
            {
                // Fallback if Camera.main is missing: use the hit plane's forward vector
                spawnPosition += hitPose.forward * forwardOffset;
            }

            model.SetPositionAndRotation(spawnPosition, hitPose.rotation);
            model.gameObject.SetActive(true);
            _isModelSet = true;
            raycastManager.enabled = false;
            planeManager.enabled = false;

            foreach (var plane in planeManager.trackables)
            {
                plane.gameObject.SetActive(false);
            }
        }
    }
    private void ShowPlacementPreview()
    {
        if (_isModelSet)
            return;

        Vector2 centreScreen =
            new Vector2(
                Screen.width * 0.5f,
                Screen.height * 0.5f);

        if (raycastManager.Raycast(
                centreScreen,
                hits,
                TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;

            Vector3 previewPosition =
                hitPose.position;

            if (Camera.main != null)
            {
                Vector3 camForward =
                    Camera.main.transform.forward;

                camForward.y = 0f;
                camForward.Normalize();

                previewPosition +=
                    camForward * forwardOffset;
            }

            areaBoundary.gameObject.SetActive(true);

            DrawBoundary(
                previewPosition,
                hitPose.rotation);
        }
    }

    private void DrawBoundary(
        Vector3 centre,
        Quaternion rotation)
    {
        float half = areaSize * 0.5f;
        float yOffset = -1.5f;

        Vector3 p1 =
            centre + rotation * new Vector3(-half, yOffset, -half);

        Vector3 p2 =
            centre + rotation * new Vector3(-half, yOffset, half);

        Vector3 p3 =
            centre + rotation * new Vector3(half, yOffset, half);

        Vector3 p4 =
            centre + rotation * new Vector3(half, yOffset, -half);

        areaBoundary.SetPosition(0, p1);
        areaBoundary.SetPosition(1, p2);
        areaBoundary.SetPosition(2, p3);
        areaBoundary.SetPosition(3, p4);
        areaBoundary.SetPosition(4, p1);
    }
}
