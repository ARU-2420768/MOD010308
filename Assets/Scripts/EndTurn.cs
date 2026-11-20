using TMPro;
using UnityEngine;

public class EndTurn : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private Player player;
    [SerializeField] private TextMeshPro PlayerMemorised;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchUser()
    {
        if (!playerManager.PlayerHasMemorisedBoard)
        {
            Debug.Log("Board has not been memorised yet");
            return;
        }
        Debug.Log($"Player: {player}, it's their turn to switch: {playerManager.IsPlayerTurn(player)}");
        if (playerManager.IsPlayerTurn(player))
        {
            #if UNITY_IOS || UNITY_ANDROID
                Handheld.Vibrate();
            #endif
            playerManager.SwitchPlayer();
            Debug.Log($"{player} ended their turn");
            PlayerMemorised.text = "";
        }
    }
}
