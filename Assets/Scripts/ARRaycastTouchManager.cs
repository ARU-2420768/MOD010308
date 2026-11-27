using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;

public class ARRaycastTouchManager : MonoBehaviour
{
    [Header("AR & References")]
    [SerializeField] private Camera arCamera;
    [SerializeField] private ARRaycastManager arRaycastManager;
    [SerializeField] private AnchoredCubeSwapper cubeSwapper;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private StartTurn startTurn;
    [SerializeField] private EndTurn endTurn;

    private readonly List<ARRaycastHit> arHits = new();
    private bool isDraggingPatternCube;

    private void Awake()
    {
        if (arCamera == null)
        {
            arCamera = Camera.main;
        }

        if (cubeSwapper == null)
        {
            cubeSwapper = FindFirstObjectByType<AnchoredCubeSwapper>();
        }
    }

    private void Update()
    {
        Camera cam = arCamera != null ? arCamera : Camera.main;
        if (cam == null)
        {
            return;
        }

        if (IsPatternShiftActive())
        {
            if (HandlePatternShiftPointer(cam))
            {
                return;
            }
        }
        else if (isDraggingPatternCube)
        {
            isDraggingPatternCube = false;
        }

        if (TryGetPointerDownPosition(out Vector2 pointerDownPos))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            HandleTap(pointerDownPos, cam);
        }
    }

    private bool IsPatternShiftActive()
    {
        return GameManager.Instance != null &&
               GameManager.Instance._puzzleDefinition != null &&
               GameManager.Instance._puzzleDefinition.puzzleType == PuzzleType.PatternShift;
    }

    private bool HandlePatternShiftPointer(Camera cam)
    {
        if (cubeSwapper == null)
        {
            cubeSwapper = FindFirstObjectByType<AnchoredCubeSwapper>();
        }

        if (cubeSwapper == null)
        {
            return false;
        }

        // 1. Touch input (Mobile / AR device)
        Touchscreen touchScreen = Touchscreen.current;
        if (touchScreen != null)
        {
            var primaryTouch = touchScreen.primaryTouch;
            var phase = primaryTouch.phase.ReadValue();
            Vector2 touchPos = primaryTouch.position.ReadValue();

            if (primaryTouch.press.wasPressedThisFrame)
            {
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                {
                    return true;
                }

                Ray ray = cam.ScreenPointToRay(touchPos);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    PuzzleCube puzzleCube = hit.collider.GetComponentInParent<PuzzleCube>();
                    if (puzzleCube != null)
                    {
                        cubeSwapper.BeginDrag(puzzleCube);
                        isDraggingPatternCube = cubeSwapper.IsDragging;
                        return true;
                    }
                }
            }
            else if (primaryTouch.press.isPressed && (isDraggingPatternCube || cubeSwapper.IsDragging))
            {
                cubeSwapper.UpdateDrag(touchPos, cam);
                return true;
            }
            else if (primaryTouch.press.wasReleasedThisFrame && (isDraggingPatternCube || cubeSwapper.IsDragging))
            {
                cubeSwapper.EndDrag(touchPos, cam);
                isDraggingPatternCube = false;
                return true;
            }
        }

        // 2. Mouse input (Editor / Desktop testing)
        Mouse mouse = Mouse.current;
        if (mouse != null)
        {
            Vector2 mousePos = mouse.position.ReadValue();

            if (mouse.leftButton.wasPressedThisFrame)
            {
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                {
                    return true;
                }

                Ray ray = cam.ScreenPointToRay(mousePos);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    PuzzleCube puzzleCube = hit.collider.GetComponentInParent<PuzzleCube>();
                    if (puzzleCube != null)
                    {
                        cubeSwapper.BeginDrag(puzzleCube);
                        isDraggingPatternCube = cubeSwapper.IsDragging;
                        return true;
                    }
                }
            }
            else if (mouse.leftButton.isPressed && (isDraggingPatternCube || cubeSwapper.IsDragging))
            {
                cubeSwapper.UpdateDrag(mousePos, cam);
                return true;
            }
            else if (mouse.leftButton.wasReleasedThisFrame && (isDraggingPatternCube || cubeSwapper.IsDragging))
            {
                cubeSwapper.EndDrag(mousePos, cam);
                isDraggingPatternCube = false;
                return true;
            }
        }

        return false;
    }

    private bool TryGetPointerDownPosition(out Vector2 position)
    {
        Touchscreen touchScreen = Touchscreen.current;
        if (touchScreen != null && touchScreen.primaryTouch.press.wasPressedThisFrame)
        {
            position = touchScreen.primaryTouch.position.ReadValue();
            return true;
        }

        Mouse mouse = Mouse.current;
        if (mouse != null && mouse.leftButton.wasPressedThisFrame)
        {
            position = mouse.position.ReadValue();
            return true;
        }

        position = default;
        return false;
    }

    private void HandleTap(Vector2 screenPosition, Camera cam)
    {
        Ray ray = cam.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject hitObject = hit.collider.gameObject;

            switch (hitObject.tag)
            {
                case "ColourCube":
                    HandleColourCubeTap(hit.collider);
                    return;

                case "StartGame":
                    Debug.Log("Start Game");
                    StartTurn startTurn = hit.collider.GetComponent<StartTurn>();
                    if (startTurn != null)
                    {
                        startTurn.StartGame();
                    }
                    return;

                case "ChangePlayerCube":
                    EndTurn endTurn = hit.collider.GetComponent<EndTurn>();
                    if (endTurn != null)
                    {
                        endTurn.SwitchUser();
                    }
                    return;

                case "Memorised":
                    Debug.Log("Have Memorised");
                    if (playerManager != null)
                    {
                        playerManager.ConfirmBoardMemorised();
                    }
                    return;
            }
        }

        if (arRaycastManager != null &&
            arRaycastManager.Raycast(screenPosition, arHits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = arHits[0].pose;
            // Optional: handle AR plane placement here if needed
            _ = hitPose;
        }
    }
    private void HandleColourCubeTap(Collider hitCollider)
    {
        PuzzleCube puzzleCube = hitCollider.GetComponentInParent<PuzzleCube>();
        TapToChangeColour colourCube = hitCollider.GetComponentInParent<TapToChangeColour>();

        if (GameManager.Instance == null || GameManager.Instance._puzzleDefinition == null)
        {
            return;
        }

        PuzzleType currentType = GameManager.Instance._puzzleDefinition.puzzleType;

        if (currentType == PuzzleType.Simon)
        {
            if (SimonManager.Instance != null && puzzleCube != null)
            {
                SimonManager.Instance.CheckInput(puzzleCube.CubeIndex);
            }
            return;
        }

        if (currentType == PuzzleType.PatternShift)
        {
            // Dragging is handled in HandlePatternShiftPointer(); if a quick tap occurs,
            // you can leave this empty or forward to BeginDrag(puzzleCube).
            return;
        }

        if (!GameManager.Instance.CanCurrentPlayerChangeColours())
        {
            Debug.Log("Memorising player cannot change colours");
            return;
        }

        if (colourCube != null)
        {
            colourCube.LoopThroughColours();
        }
    }
}