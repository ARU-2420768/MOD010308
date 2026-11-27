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
