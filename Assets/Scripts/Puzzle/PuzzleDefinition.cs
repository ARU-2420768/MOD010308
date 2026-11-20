using UnityEngine;

[CreateAssetMenu(menuName = "Puzzle/Game Definition")]
public class PuzzleDefinition : ScriptableObject
{
    [Header("General")]
    public string gameName;
    public GameLevel gameLevel;
    public string instructionsPlayer1;
    public string instructionsPlayer2;
    public PuzzleType puzzleType;

    [Header("Cubes Dimensions")]
    [Range(1,5)]
    public int columns = 5;
    [Range(1,3)]
    public int rows = 3;

    [Header("Colour Match")]
    public int scrambleMoves = 10;
    public WinCondition winCondition;


    [Header("Simon")]
    public int simonSequenceLength = 5;
    public float simonFlashDuration = 0.5f;
    public float simonPauseDuration = 0.25f;
    public bool randomiseSequence = true;
}