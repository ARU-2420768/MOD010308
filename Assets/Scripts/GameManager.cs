using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;

public class GameManager : MonoBehaviour
{
    private static GameManager gameManager;
    public static GameManager Instance => gameManager;


    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private Transform player1Table;
    [SerializeField] private Transform player2Table;
    [SerializeField] private PuzzleDefinition puzzleDefinition;


    public PuzzleDefinition _puzzleDefinition => puzzleDefinition;
    private List<PuzzleCube> player1Cubes = new();
    private List<PuzzleCube> player2Cubes = new();

    public void Awake()
    {
        if(gameManager == null)
            gameManager = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {

    }

    public void ChangePuzzleDefinition(PuzzleDefinition _puzzleDefinition)
    {
        puzzleDefinition = _puzzleDefinition;
        StartGame();
    }

    private void ClearBoards()
    {
        foreach (PuzzleCube cube in player1Cubes)
        {
            Destroy(cube.gameObject);
        }

        foreach (PuzzleCube cube in player2Cubes)
        {
            Destroy(cube.gameObject);
        }

        player1Cubes.Clear();
        player2Cubes.Clear();
    }


    public void StartGame()
    {
        ClearBoards();
        BoardDefinition solutionBoard = GenerateSolutionBoard();
        CreatePlayerBoard(player1Table, solutionBoard, player1Cubes);
        CreatePlayerBoard(player2Table, solutionBoard, player2Cubes);
//        CreatePlayerBoard(player1Table, puzzleDefinition.solutionBoardDefinition, player1Cubes);
//        CreatePlayerBoard(player2Table, puzzleDefinition.solutionBoardDefinition, player2Cubes);
        SetupPuzzle();
        SwitchPlayer();
    }

    public void SwitchPlayer()
    {
        //SetBoardVisible(player1Cubes, !GameManager.Instance.CanCurrentPlayerChangeColours());
        //SetBoardVisible(player2Cubes, !GameManager.Instance.CanCurrentPlayerChangeColours());
        SetBoardVisible(player1Cubes, playerManager.IsPlayer1Active());
        SetBoardVisible(player2Cubes, !playerManager.IsPlayer1Active());       
    }

    private BoardDefinition GenerateSolutionBoard()
    {
        BoardDefinition board = new BoardDefinition();

        int cellCount = puzzleDefinition.rows * puzzleDefinition.columns;

        board.cells = new CellDefinition[cellCount];

        for (int i = 0; i < cellCount; i++)
        {
            board.cells[i] = new CellDefinition
            {
                active = true,
                startingState = GetRandomColourState()
            };
        }

        return board;
    }


    private void SetBoardVisible(List<PuzzleCube> cubes, bool visible)
    {
        foreach (PuzzleCube cube in cubes)
        {   
            cube.GetComponent<Renderer>().enabled = visible;
        }
    }



    private List<PuzzleCube> GetGuessingPlayerBoard()
    {
        return playerManager.IsPlayer1Active() ? player2Cubes : player1Cubes;
    }

    private void SetupPuzzle()
    {
        Debug.Log(
            $"Scrambling board. Player1Active={playerManager.IsPlayer1Active()}"
        );
        List<PuzzleCube> board = GetGuessingPlayerBoard();

        Debug.Log($"Guessing board has {board.Count} cubes");

        //ScrambleBoard(board, 3);
        ScrambleBoard(GetGuessingPlayerBoard(), 3);
    }

    private void ScrambleBoard(List<PuzzleCube> cubes, int moves)
    {
        for (int i = 0; i < moves; i++)
        {
            int cubeIndex = Random.Range(0, cubes.Count);

            cubes[cubeIndex].Data.state = GetRandomColourState();

            // Refresh visual
            cubes[cubeIndex].UpdateVisual();
        }
    }

    public bool CanCurrentPlayerChangeColours()
    {
        bool result = playerManager.IsPlayer1Active() != playerManager.Player1IsMemorising;
        Debug.Log($"CanChangeColours: Player1Active={playerManager.IsPlayer1Active()}, Memorising={playerManager.Player1IsMemorising}, Result={result}");
        return result;

    }

    private CubeState GetRandomColourState()
    {
        CubeState[] colours =
        {
            CubeState.Red,
            CubeState.Green,
            CubeState.Blue,
            CubeState.Yellow,
            CubeState.Black
        };

        return colours[Random.Range(0, colours.Length)];
    }

    private CubeState GetNextState(CubeState currentState)
    {
        int stateCount = System.Enum.GetValues(typeof(CubeState)).Length;

        int nextState = ((int)currentState + 1) % stateCount;

        return (CubeState)nextState;
    }


    public bool CheckWin()
    {
        Debug.Log("Check Win Called!");

        for (int i = 0; i < player1Cubes.Count; i++)
        {
            Debug.Log(
                $"Cube {i}: " +
                $"P1={player1Cubes[i].Data.state}, " +
                $"P2={player2Cubes[i].Data.state}");
        }

        if (player1Cubes.Count != player2Cubes.Count)
        {
            Debug.LogError("Player board sizes differ.");
            return false;
        }

        for (int i = 0; i < player1Cubes.Count; i++)
        {
            if (player1Cubes[i].Data.state !=
                player2Cubes[i].Data.state)
            {
                Debug.Log(
                    $"Cube {i} mismatch. " +
                    $"P1={player1Cubes[i].Data.state}, " +
                    $"P2={player2Cubes[i].Data.state}"
                );

                return false;
            }
        }

        Debug.Log("Puzzle solved!");
        playerManager.PuzzleSolved();
        return true;
    }

    private void CreatePlayerBoard(
        Transform parent,
        BoardDefinition board,
        List<PuzzleCube> cubeList)
    {
        const float spacingx = 0.2f;
        const float spacingz = 0.3f;

        int columns = puzzleDefinition.columns;
        int rows = puzzleDefinition.rows;

        float boardWidth = (columns - 1) * spacingx;
        float boardHeight = (rows - 1) * spacingz;

        for (int i = 0; i < board.cells.Length; i++)
        {
            if (!board.cells[i].active)
                continue;

            GameObject cube = Instantiate(cubePrefab, parent);

            int column = i % columns;
            int row = i / columns;

            float x = column * spacingx - boardWidth * 0.5f;
            float z = row * spacingz - boardHeight * 0.5f;

            cube.transform.localPosition = new Vector3(
                x,
                0.55f,
                z
            );

            PuzzleCube puzzleCube = cube.GetComponent<PuzzleCube>();

            if (puzzleCube == null)
            {
                Debug.LogError("PuzzleCube component missing from prefab");
                return;
            }

            puzzleCube.Initialise(
                new CubeData
                {
                    active = true,
                    state = board.cells[i].startingState
                });

            cubeList.Add(puzzleCube);
        }
    }


}
