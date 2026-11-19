using UnityEngine;

[CreateAssetMenu(menuName = "Puzzle/Game Definition")]
public class PuzzleDefinition : ScriptableObject
{
    public string gameName;
    public GameLevel gameLevel;
    public string instructionsPlayer1;
    public string instructionsPlayer2;

    public CubeState[] solutionBoard;

    public int scrambleMoves = 10;
    public PuzzleType puzzleType;
    public WinCondition winCondition;

    [Range(1,5)]
    public int columns = 5;

    [Range(1,3)]
    public int rows = 3;

    public BoardDefinition solutionBoardDefinition;
}