using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class TapToPlace : MonoBehaviour
{

    [Tooltip("The AR Raycast Manager on the XR Origin.")]
    [SerializeField] private ARRaycastManager raycastManager;
    [Tooltip("The model to place. It is hidden until you tap.")]   
    [SerializeField] private Transform model;
    [Tooltip("Distance in meters to offset the model from the hit point.")]
    [SerializeField] private float forwardOffset = 3.0f;
    [SerializeField] private bool _isModelSet = false;


    //The raycast writes its results into this list.
    readonly List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Start()
    {    
        model.gameObject.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if(_isModelSet)
            return;

        if (ARSession.state != ARSessionState.SessionTracking)
                    return;

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
        }
    }
}
