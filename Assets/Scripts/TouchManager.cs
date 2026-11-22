using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ARRaycastTouchManager : MonoBehaviour
{

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }


    private void Update()
    {
        //Debug.Log("ARRaycastTouchManager.Update");
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
                    if (!GameManager.Instance.CanCurrentPlayerChangeColours())
                    {
                        Debug.Log("Memorising player cannot change colours");
                        break;
                    }
                    if (GameManager.Instance.PuzzleSolved)
                    {
                        Debug.Log("Puzzle already solved");
                        break;
                    }
                    TapToChangeColour colourCube = hit.collider.GetComponent<TapToChangeColour>();
                    PuzzleCube puzzleCube = hit.collider.GetComponent<PuzzleCube>();

                    if (colourCube == null || puzzleCube == null)
                    {
                        Debug.LogError("Missing component");
                        return;
                    }

                    Debug.Log($"Cube Index = {puzzleCube.CubeIndex}");

                    Debug.Log($"Simon Manager = {SimonManager.Instance}");

                    if (GameManager.Instance._puzzleDefinition.puzzleType == PuzzleType.Simon)
                    {
                        SimonManager.Instance.CheckInput(puzzleCube.CubeIndex);

                    }
                    else
                    {
                        if (!GameManager.Instance.CanCurrentPlayerChangeColours())
                        {
                            Debug.Log("Memorising player cannot change colours");
                            break;
                        }
                        Debug.Log("Change Colour Now");

                        colourCube.LoopThroughColours();
                    }
                    break;
                case "ChangePlayerCube":
                    Debug.Log("Change Player Now");
                    EndTurn endTurn = hit.collider.GetComponent<EndTurn>();
                    if (endTurn != null)
                    {
                        endTurn.SwitchUser();
                    }
                    break;
                case "StartGame":
                    Debug.Log("Start Game");
                    StartTurn startTurn = hit.collider.GetComponent<StartTurn>();
                    if (startTurn != null)
                    {
                        startTurn.StartGame();
                    }
                    break;
                case "ChooseLevel":
                    Debug.Log("Choose Level");
                    ChooseLevel chooseLevel = hit.collider.GetComponent<ChooseLevel>();
                    if (chooseLevel != null)
                    {
                        chooseLevel.SelectLevel();
                    }
                    break;
                case "Memorised":
                    if (!PlayerManager.Instance.CurrentPlayerIsMemorising())
                    {
                        Debug.Log("Wrong player");
                        return;
                    }

                    Debug.Log("Have Memorised");

                    PlayerManager.Instance.ConfirmBoardMemorised();
                    hit.collider.GetComponentInChildren<TMP_Text>().text = "Solution Memorised";
                    /*
                    GameManager.Instance.CanCurrentPlayerChangeColours()
                    ChooseLevel chooseLevel = hit.collider.GetComponent<ChooseLevel>();
                    if (chooseLevel != null)
                    {
                        chooseLevel.SelectLevel();
                    }
                    */
                    break;
            }


        }
    }
}