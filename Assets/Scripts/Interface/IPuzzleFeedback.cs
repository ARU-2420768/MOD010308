public interface IPuzzleFeedback
{
    void RevealCube(PuzzleCube cube);
    void PlaySuccess();
    void PlayTurnEnd();
}