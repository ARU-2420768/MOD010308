using UnityEngine;

public class PuzzleCube : MonoBehaviour
{
    public CubeData Data;

    private Renderer cubeRenderer;

    private void Awake()
    {
        cubeRenderer = GetComponent<Renderer>();
    }

    public void Initialise(CubeData data)
    {
        Data = data;
        UpdateVisual();
    }

    public void CycleColour()
    {
        int stateCount = System.Enum.GetValues(typeof(CubeState)).Length;

        Data.state = (CubeState)(((int)Data.state + 1) % stateCount);

        UpdateVisual();
    }

    public void UpdateVisual()
    {
        Debug.Log($"Updating visual to {Data.state}");
        switch (Data.state)
        {
            case CubeState.Red:
                cubeRenderer.material.color = Color.red;
                break;

            case CubeState.Green:
                cubeRenderer.material.color = Color.green;
                break;

            case CubeState.Blue:
                cubeRenderer.material.color = Color.blue;
                break;

            case CubeState.Yellow:
                cubeRenderer.material.color = Color.yellow;
                break;
            case CubeState.Black:
                cubeRenderer.material.color = Color.black;
                break;
        }
    }
}