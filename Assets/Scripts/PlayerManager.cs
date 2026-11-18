using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    [SerializeField] private Transform wall;
    [SerializeField] private Camera arCamera;

    public bool Player1Active, Player2Active;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DetermineWhichPlayer();
    }

    private void DetermineWhichPlayer()
    {
        Vector3 p = wall.InverseTransformPoint(arCamera.transform.position);
        Player2Active = p.z > 0f;
        Player1Active = !Player2Active;
        //Debug.Log($"Player 1 active is: {Player1Active}, Player 2 active is: {Player2Active}.");
    }
}
