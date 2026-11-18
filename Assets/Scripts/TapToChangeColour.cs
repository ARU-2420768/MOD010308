using UnityEngine;

public class TapToChangeColour : MonoBehaviour
{

    public enum CubeColour
    {
        Red,
        Yellow,
        Green,
        Blue,
        Black
    }

    [SerializeField] private CubeColour currentColour = CubeColour.Red;

    private Renderer cubeRenderer;
    

    private void Awake()
    {
        cubeRenderer = GetComponent<Renderer>();
        UpdateVisuals();
    }

    private void OnMouseDown()
    {
        LoopThroughColours();
    }

    public void LoopThroughColours()
    {
        // cast to CubeColour at correct position (CubeColour)0 = Red
        currentColour = (CubeColour)(((int)currentColour + 1) % 5);
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        switch (currentColour)
        {
            case CubeColour.Red:
                cubeRenderer.material.color = Color.red;
                break;

            case CubeColour.Yellow:
                cubeRenderer.material.color = Color.yellow;
                break;

            case CubeColour.Green:
                cubeRenderer.material.color = Color.green;
                break;

            case CubeColour.Blue:
                cubeRenderer.material.color = Color.blue;
                break;

            case CubeColour.Black:
                cubeRenderer.material.color = Color.black;
                break;
        }
    }

    public CubeColour GetCurrentColour()
    {
        return currentColour;
    }
}
