using UnityEngine;

public class HapticFeedback : IPuzzleFeedback
{
    public void RevealCube(PuzzleCube cube)
    {
        cube.PlayHaptic();
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