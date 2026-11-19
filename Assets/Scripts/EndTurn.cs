using UnityEngine;

public class EndTurn : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private Player player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchUser()
    {
        Debug.Log($"Player: {player}, it's their turn to switch: {playerManager.IsPlayerTurn(player)}");
        if (playerManager.IsPlayerTurn(player))
        {
            playerManager.SwitchPlayer();
            Debug.Log($"{player} ended their turn");
        }
    }
}
