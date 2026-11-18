using UnityEngine;
using UnityEngine.InputSystem;

public class ARRaycastTouchManager : MonoBehaviour
{
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // Pointer means a touchscreen on the phone, or the mouse in the Editor.
        if (Pointer.current == null || !Pointer.current.press.wasPressedThisFrame)
            return;

        Vector2 screenPosition = Pointer.current.position.ReadValue();

        Ray ray = mainCamera.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {

            switch (hit.collider.gameObject.tag)
            {
                case "ColourCube":
                    TapToChangeColour colourCube = hit.collider.GetComponent<TapToChangeColour>();
                    if (colourCube != null)
                    {
                        colourCube.LoopThroughColours();
                    }
                    break;
                case "ChangePlayerCube":
                    Debug.Log("Change Player");
                    break;
            }


        }
    }
}