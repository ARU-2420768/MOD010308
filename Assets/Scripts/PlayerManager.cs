using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    private static PlayerManager playerManager;
    public static PlayerManager Instance => playerManager;

    [SerializeField] private Transform wall;
    [SerializeField] private Camera arCamera;
    [SerializeField] private TextMeshPro player1Instructions;
    [SerializeField] private TextMeshPro player2Instructions;
    [SerializeField] private GameObject player1StartButton;
    [SerializeField] private GameObject player2StartButton;
    [SerializeField] private TMP_Text player1MemorisedText;
    [SerializeField] private TMP_Text player2MemorisedText;
    [SerializeField] private GameManager gameManager;

    //public bool Player1Active, Player2Active;
    private bool Player1Active;
    private bool Player2Active => !Player1Active;

    private bool player1IsMemorising;
    public bool Player1IsMemorising => player1IsMemorising;

    private bool playerHasMemorisedBoard;
    public bool PlayerHasMemorisedBoard => playerHasMemorisedBoard;

    // When turn ends switch handover to true so the code can DetermineWhichPlayer()
    private bool handOverPhase = false;

    private bool gameInProgress = false;

    public bool GameInProgress => gameInProgress;

    private void Awake()
    {
        if (playerManager == null)
        {
            playerManager = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player1IsMemorising = Player1Active;
        Debug.Log($"Memorising Player = {(player1IsMemorising ? "Player1" : "Player2")}");
        DetermineWhichPlayer();
        TurnOnPlayersStartTurnButtons();
    }

    // Update is called once per frame
    void Update()
    {
        if(handOverPhase)
            DetermineWhichPlayer();
    }

    public bool IsPlayer1Active()
    {
        return Player1Active;
    }

    private void DisplayInstructions()
    {
        if (IsPlayer1Active())
        {
            player1Instructions.text = gameManager._puzzleDefinition.instructionsPlayer1;
            player2Instructions.text = gameManager._puzzleDefinition.instructionsPlayer2;
        }
        else
        {
            player1Instructions.text = gameManager._puzzleDefinition.instructionsPlayer2;
            player2Instructions.text = gameManager._puzzleDefinition.instructionsPlayer1;
            
        }
    }

    private void OnMouseDown()
    {
        SwitchPlayer();
    }


    private void DetermineWhichPlayer()
    {
        Vector3 p = wall.InverseTransformPoint(arCamera.transform.position);
        Player1Active = p.z < 0f;

        Debug.Log($"Player 1 active is: {Player1Active}, Player 2 active is: {Player2Active}.");
    }

    public void SwitchPlayer()
    {
        Player1Active = !Player1Active;
        Debug.Log($"Player 1 active is: {Player1Active}, Player 2 active is: {Player2Active}.");
        TurnOnPlayersStartTurnButtons();
        gameManager.SwitchPlayer();
    }

    public bool IsPlayerTurn(Player player)
    {
        return (player == Player.Player1 && Player1Active) || (player == Player.Player2 && Player2Active);
    }

    public void StartGame()
    {
        player1IsMemorising = Player1Active;

        playerHasMemorisedBoard = false;
        UpdateMemorisedButtonText();
        Debug.Log($"Player 1 game started is {Player1Active}; Player 2 game started is {Player2Active};");
        gameInProgress = true;
        TurnOnPlayersStartTurnButtons();
        DisplayInstructions();
        gameManager.StartGame();
    }


    private void UpdateMemorisedButtonText()
    {
        player1MemorisedText.text = "";
        player2MemorisedText.text = "";

        if (CurrentPlayerIsMemorising())
        {
            if (Player1Active)
            {
                player1MemorisedText.text = "Click when you have memorised";
            }
            else
            {
                player2MemorisedText.text = "Click when you have memorised";
            }
        }
    }

    public void ConfirmBoardMemorised()
    {
        if (!CurrentPlayerIsMemorising())
        {
            Debug.Log("Only the memorising player can press this.");
            return;
        }

        playerHasMemorisedBoard = true;
        #if UNITY_IOS || UNITY_ANDROID
            Handheld.Vibrate();
        #endif
    }

    public void PuzzleSolved()
    {
        gameInProgress = false;

        TurnOnPlayersStartTurnButtons();
    }

    private void TurnOnPlayersStartTurnButtons()
    {
        Debug.Log(
        $"GameInProgress={gameInProgress} " +
        $"Player1Active={Player1Active} " +
        $"Player2Active={Player2Active}"
    );
        if (gameInProgress)
        {
            SetStartButtonVisible(player1StartButton, false);
            SetStartButtonVisible(player2StartButton, false);
            return;
        }

        SetStartButtonVisible(player1StartButton, Player1Active);
        SetStartButtonVisible(player2StartButton, Player2Active);
    }

    private void SetStartButtonVisible(GameObject startButton, bool visible)
    {
        Renderer[] renderers = startButton.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = visible;
        }
        foreach (Collider collider in startButton.GetComponentsInChildren<Collider>(true))
        {
            collider.enabled = visible;
        }
    }

    public bool CurrentPlayerIsMemorising()
    {
        return Player1Active == player1IsMemorising;
    }


}
