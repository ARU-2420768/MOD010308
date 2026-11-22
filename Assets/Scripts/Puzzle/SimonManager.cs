using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimonManager : MonoBehaviour
{

    [SerializeField] private WallManager wallManager;

    private static SimonManager simonManager;
    public static SimonManager Instance => simonManager;


    private List<int> sequence = new();
    private SimonState currentState;

    private List<PuzzleCube> cubes;

    private int playerIndex = 0;

    private void Awake()
    {
        simonManager = this;
    }

    public void GenerateSequence(int length)
    {
        sequence.Clear();

        for (int i = 0; i < length; i++)
        {
            sequence.Add(Random.Range(0, cubes.Count));
        }
    }

    public void StartSimon()
    {
        cubes = GameManager.Instance.GetCurrentBoard();
        Debug.Log($"Simon cubes: {cubes.Count}");
        foreach (var cube in cubes)
        {
            Debug.Log(
                cube.Data.state
            );
        }
        GenerateSequence(GameManager.Instance._puzzleDefinition.simonSequenceLength);
        StartCoroutine(ShowSequence());
        Debug.Log(string.Join(",", sequence));
    }

    public bool CheckInput(int cubeIndex)
    {

        if (currentState != SimonState.WaitingForInput)
        {
            return false;
        }

        if (cubeIndex != sequence[playerIndex])
        {
            Debug.Log($"Expected {sequence[playerIndex]}");
            Debug.Log($"Pressed {cubeIndex}");
            return false;
        }

        playerIndex++;

        if (playerIndex >= sequence.Count)
        {
            Debug.Log("Success");
            playerIndex = 0;
            wallManager.DamageWall(GameManager.Instance._puzzleDefinition.wallDamage);
            return true;
        }

        return false;
    }

    private IEnumerator ShowSequence()
    {
        currentState = SimonState.Showing;

        float flashDuration =
            GameManager.Instance
                ._puzzleDefinition
                .simonFlashDuration;

        float pauseDuration =
            GameManager.Instance
                ._puzzleDefinition
                .simonPauseDuration;

        foreach (var index in sequence)
        {
            cubes[index].Highlight();

            yield return new WaitForSeconds(
                flashDuration
            );

            cubes[index].Normal();

            yield return new WaitForSeconds(
                pauseDuration
            );
        }

        playerIndex = 0;

        currentState = SimonState.WaitingForInput;
    }

}