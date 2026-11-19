
using TMPro;
using UnityEngine;

public class ChooseLevel : MonoBehaviour
{

    [SerializeField] private PuzzleDefinition[] puzzleDefinitions; 
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TextMeshPro levelDescription;
    private int currentPuzzleIndex = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectLevel()
    {
        if (puzzleDefinitions.Length == 0)
        {
            Debug.LogError("No Puzzle Definitions assigned.");
            return;
        }

        currentPuzzleIndex = (currentPuzzleIndex + 1) % puzzleDefinitions.Length;

        levelDescription.text = puzzleDefinitions[currentPuzzleIndex].gameLevel.ToString();

        gameManager.ChangePuzzleDefinition(puzzleDefinitions[currentPuzzleIndex]);
    }

}
