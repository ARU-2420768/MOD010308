public class HapticFeedback : IPuzzleFeedback
{
    public void RevealCube(PuzzleCube cube)
    {
        cube.PlayHaptic();
    }
}