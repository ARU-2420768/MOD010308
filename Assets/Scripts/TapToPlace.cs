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


    //The raycast writes its results into this list.
    readonly List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Start()
    {    
        model.gameObject.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        // Pointer means a touchscreen on the phone, or the mouse in the Editor.
        if (Pointer.current == null || !Pointer.current.press.wasPressedThisFrame)
            return;
        
        Vector2 screenPosition = Pointer.current.position.ReadValue();

        // Fire a ray from the screen into the real world. Did it hit a detected surface?
        if (raycastManager.Raycast(screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose; // hits[0] is the closest surface
            model.SetPositionAndRotation(hitPose.position, hitPose.rotation);
            model.gameObject.SetActive(true);
        }

        
    }
}
