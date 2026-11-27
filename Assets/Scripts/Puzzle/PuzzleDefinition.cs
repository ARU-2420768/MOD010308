using UnityEngine;

[CreateAssetMenu(menuName = "Puzzle/Game Definition")]
public class PuzzleDefinition : ScriptableObject
{
    [Header("Barrier & Mission")]
    public string gameName;
    public string unlockObjective; // e.g., "Unlock Bar Lock 1/3: Sync Phase Nodes"
    public int unlockStageNumber = 1; // 1 to 3 distinct actions
    public GameLevel gameLevel;
    public string instructionsPlayer1;
    public string instructionsPlayer2;
    public PuzzleType puzzleType;

    [Header("Cubes Dimensions")]
    [Range(1, 5)]
    public int columns = 5;
    [Range(1, 3)]
    public int rows = 3;

    [Header("Phase / Pattern Match")]
    public int scrambleMoves = 10;
    public WinCondition winCondition;

    [Header("Barrier Breach")]
    public int wallDamage = 5;

    [Header("Sequence Override")]
    public int simonSequenceLength = 5;
    public int targetRounds = 10;
    public float startingTimer = 8f;
    public float timerReductionPerRound = 0.5f;
    public float minimumTimer = 2f;
    public float simonFlashDuration = 0.5f;
    public float simonPauseDuration = 0.25f;
    public bool randomiseSequence = true;

    [Header("Anchored Core Shift")]
    [Tooltip("Enable world-tracked AR anchors on individual cube slots.")]
    public bool useAnchoredSlots = true;

    [Tooltip("Number of swap moves used to scramble cubes at puzzle start.")]
    [Min(1)]
    public int swapScrambleMoves = 3;

    [Tooltip("Optional move limit (0 = unlimited moves).")]
    [Min(0)]
    public int maxSwapMoves = 0;

    [Tooltip("Duration in seconds for smooth cube swap animation.")]
    [Min(0.05f)]
    public float swapDuration = 0.2f;

    [Tooltip("Spacing between anchored slots on X and Z axes (in metres).")]
    public Vector2 slotSpacing = new Vector2(0.2f, 0.3f);

    public CubeState[] targetPattern;
}