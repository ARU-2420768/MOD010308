using UnityEngine;

public class EndTurn : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        SwitchUser();
    }

    private void SwitchUser()
    {
        Debug.Log("Change User Box Hit");
    }
}
