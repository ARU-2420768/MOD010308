using System.Collections;
using UnityEngine;

public class PuzzleCube : MonoBehaviour
{
    public CubeData Data;

    public int CubeIndex;
    private Color originalColour;

    private Renderer cubeRenderer;

    private AudioSource audioSource;

    [SerializeField] private AudioClip beepClip;

    private void Awake()
    {
        cubeRenderer = GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();
        originalColour = cubeRenderer.material.color;
    }

    public void Normal()
    {
        ShowColour();
    }
    public void Highlight()
    {
        cubeRenderer.material.color = Color.cyan;

        PlaySound();
    }
    public void Initialise(CubeData data)
    {
        Data = data;
        ShowColour();
    }

    public void CycleColour()
    {
        int stateCount = System.Enum.GetValues(typeof(CubeState)).Length;

        Data.state = (CubeState)(((int)Data.state + 1) % stateCount);

        ShowColour();
    }

    public void ShowColour()
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



    public void PlaySound()
    {
        switch (Data.state)
        {
            case CubeState.Red:
                StartCoroutine(PlayPattern(1));
                break;

            case CubeState.Green:
                StartCoroutine(PlayPattern(2));
                break;

            case CubeState.Blue:
                StartCoroutine(PlayPattern(3));
                break;

            case CubeState.Yellow:
                StartCoroutine(PlayPattern(4));
                break;

            case CubeState.Black:
                StartCoroutine(PlayPattern(5));
                break;
        }
    }

    private IEnumerator PlayPattern(int count)
    {
        for (int i = 0; i < count; i++)
        {
            audioSource.PlayOneShot(beepClip);

            yield return new WaitForSeconds(0.3f);
        }
    }

    public void PlayHaptic()
    {
        switch (Data.state)
        {
            case CubeState.Red:
                StartCoroutine(HapticPulse(1));
                break;

            case CubeState.Green:
                StartCoroutine(HapticPulse(2));
                break;

            case CubeState.Blue:
                StartCoroutine(HapticPulse(3));
                break;

            case CubeState.Yellow:
                StartCoroutine(HapticPulse(4));
                break;

            case CubeState.Black:
                StartCoroutine(HapticPulse(5));
                break;
        }
    }

    private IEnumerator HapticPulse(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Handheld.Vibrate();

            yield return new WaitForSeconds(0.2f);
        }
    }

    public void Reveal(PuzzleType puzzleType)
    {
        switch (puzzleType)
        {
            case PuzzleType.ColourMatch:
                ShowColour();
                break;

            case PuzzleType.SoundMatch:
                PlaySound();
                break;

            case PuzzleType.HapticMatch:
                PlayHaptic();
                break;

            case PuzzleType.Mixed:
                ShowColour();
                PlaySound();
                PlayHaptic();
                break;
        }
    }

}