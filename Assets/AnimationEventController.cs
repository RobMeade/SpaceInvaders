using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationEventController : MonoBehaviour
{
    public enum AnimationClipType : int { CommandShipDestroyed, InvaderDestroyed, PlayerDestroyed };

    public delegate void AnimationCompleteEventHandler(object sender, AnimationCompleteEventArgs e);
    public event AnimationCompleteEventHandler OnAnimationComplete;


    public void AnimationCompleted(AnimationClipType animationClipType)
    {
        if (OnAnimationComplete != null)
        {
            OnAnimationComplete(this, new AnimationCompleteEventArgs(animationClipType));
        }
    }
}