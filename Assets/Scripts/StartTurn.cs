using UnityEngine;

public class StartTurn : MonoBehaviour
{

    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private Player player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartGame()
    {
        Debug.Log($"Player: {player}, it's their turn: {playerManager.IsPlayerTurn(player)}");
        if (playerManager.IsPlayerTurn(player))
        {
            playerManager.StartGame();
            Debug.Log($"{player} started their game");
        }
    }
}
