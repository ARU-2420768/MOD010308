using UnityEngine;

public class SimonFeedback : IPuzzleFeedback
{
    public void RevealCube(PuzzleCube cube)
    {
        cube.PlaySound();
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