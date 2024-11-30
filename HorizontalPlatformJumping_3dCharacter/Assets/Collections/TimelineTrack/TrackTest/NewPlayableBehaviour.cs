using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class NewPlayableBehaviour : PlayableBehaviour
{
    public string str;
    public float number;
    
    private bool first = true;
    // Called when the owning graph starts playing
    public override void OnGraphStart(Playable playable)
    {
        // Debug.Log("graph start!");
    }

    // Called when the owning graph stops playing
    public override void OnGraphStop(Playable playable)
    {
        // Debug.Log("graph stop!");
    }

    // Called when the state of the playable is set to Play
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        // Debug.Log("behaviour play!");
    }

    // Called when the state of the playable is set to Paused
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        // Debug.Log("behaviour pause!");
    }

    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        // Debug.Log("prepare frame!");
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        Debug.Log(number);
        // Debug.Log(str);
        // Debug.Log(playable.GetTime());
        // if (first)
        // {
        //     Debug.Log(playable.GetDuration()); 
        //     first = false;
        // }
    }
}
