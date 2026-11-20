using UnityEngine;
public class ColourFeedback : IPuzzleFeedback
{
    public void RevealCube(PuzzleCube cube)
    {
        cube.ShowColour();
    }
    public void PlaySuccess()
    {
     #if UNITY_IOS || UNITY_ANDROID
        Handheld.Vibrate();
    #endif   
    }

    public void PlayTurnEnd()
    {
     #if UNITY_IOS || UNITY_ANDROID
        Handheld.Vibrate();
    #endif   
    }
}