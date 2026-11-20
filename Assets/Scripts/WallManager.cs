using UnityEngine;
public class WallManager : MonoBehaviour
{
    [SerializeField] private GameObject wall;
    [SerializeField] private GameObject intactWall;
    [SerializeField] private GameObject crackedWall;
    [SerializeField] private GameObject smallGapWall;
    [SerializeField] private GameObject largeGapWall;

    [SerializeField]
    private int wallHealth = 100;

    public void DamageWall(int amount)
    {
        wallHealth -= amount;

        UpdateVisuals();

        if (wallHealth <= 0)
        {
            DestroyWall();
        }
    }

    private void UpdateVisuals()
    {
        Debug.Log("Wall Manager Update Visuals");
    }

    private void DestroyWall()
    {
        Debug.Log("Wall Manager Destroy Wall");
    }

}