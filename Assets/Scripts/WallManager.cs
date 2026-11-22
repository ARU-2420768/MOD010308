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

    [ContextMenu("Damage 25")]
    private void Damage25()
    {
        DamageWall(25);
    }
    [ContextMenu("Reset Wall")]
    private void ResetWall()
    {
        wallHealth = 100;
        UpdateVisuals();
    }
    private void Start()
    {
        UpdateVisuals();
    }

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
        intactWall.SetActive(false);
        crackedWall.SetActive(false);
        smallGapWall.SetActive(false);
        largeGapWall.SetActive(false);

        if (wallHealth > 75)
        {
            intactWall.SetActive(true);
        }
        else if (wallHealth > 50)
        {
            crackedWall.SetActive(true);
        }
        else if (wallHealth > 25)
        {
            smallGapWall.SetActive(true);
        }
        else if (wallHealth > 0)
        {
            largeGapWall.SetActive(true);
        }

        Debug.Log($"Wall Health: {wallHealth}");
    }

    private void DestroyWall()
    {
        intactWall.SetActive(false);
        crackedWall.SetActive(false);
        smallGapWall.SetActive(false);
        largeGapWall.SetActive(false);

        Debug.Log("Wall Destroyed");
    }

}