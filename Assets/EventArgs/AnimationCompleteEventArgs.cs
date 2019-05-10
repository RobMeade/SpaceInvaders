using System;

public class AnimationCompleteEventArgs : EventArgs
{
    private AnimationEventController.AnimationClipType _animationClipType;


    public AnimationCompleteEventArgs(AnimationEventController.AnimationClipType animationClipType)
    {
        _animationClipType = animationClipType;
    }


    public AnimationEventController.AnimationClipType AnimationClipType
    {
        get { return _animationClipType; }
    }
}