using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
// Based on code provided by Nahuel Gladstein
// https://www.gamedeveloper.com/design/frame-specific-attacks-in-unity
[System.Serializable]
public class FrameParser
{
    public Animator animator;
    public AnimationClip clip;
    public string animatorStateName;
    public int layerNumber;

    private string name;
    private int _totalFrames = 0;
    private int _animationFullNameHash;

    public void initialize()
    {
        //Is the problem that it thinks the clip only has 1 frame?
        float frameRate = clip.frameRate;
        if (frameRate < 2)
            frameRate = 24;
        _totalFrames = Mathf.RoundToInt(clip.length * frameRate); //Changed: clip.frameRate -> 24
        //_totalFrames = Mathf.RoundToInt(clip.length * clip.frameRate);
        //Yes it is...
        //If Light1 is 1.5 sec and it says it has 46 frames, then that means the intended fps is 30

        if (animator.isActiveAndEnabled) 
        {
            name = animator.GetLayerName(layerNumber) + "." + animatorStateName;

            _animationFullNameHash = Animator.StringToHash(name);
        }
    }

    public bool isActive() 
    {
        return animator.isPlayingOnLayer(_animationFullNameHash, 0);
    }

    public double percentageOnFrame(int frameNumber) 
    {
        return (double)frameNumber / (double)_totalFrames;
    }

    public bool isOnOrPastFrame(int frameNumber) 
    {
        double percentage = animator.normalizedTime(layerNumber);
        return (percentage >= percentageOnFrame(frameNumber));
    }

    public bool isOnLastFrame() 
    {
        double percentage = animator.normalizedTime(layerNumber);
        return (percentage > percentageOnFrame(_totalFrames - 1));
    }

    public int getTotalFrames()
    {
        return _totalFrames;
    }

    public void play() 
    {
        animator.Play(name, 0);
    }
}