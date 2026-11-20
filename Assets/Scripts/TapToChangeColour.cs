using UnityEngine;

public class TapToChangeColour : MonoBehaviour
{
    private PuzzleCube puzzleCube;

    private void Awake()
    {
        puzzleCube = GetComponent<PuzzleCube>();
        if (puzzleCube == null)
        {
            Debug.LogError($"PuzzleCube missing from {gameObject.name}");
        }
    }


    public void LoopThroughColours()
    {
        puzzleCube.Data.state = (CubeState)(((int)puzzleCube.Data.state + 1) % 5);

        puzzleCube.ShowColour();

        Debug.Log($"New State: {puzzleCube.Data.state}");

        GameManager.Instance.CheckWin();
    }

}


