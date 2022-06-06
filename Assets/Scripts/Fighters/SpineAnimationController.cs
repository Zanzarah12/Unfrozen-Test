using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpineAnimationController : MonoBehaviour
{
	public Fighter model;
	public SkeletonAnimation skeletonAnimation;

	public AnimationReferenceAsset idle, takeDamage, grabbed;	

	void Start()
	{
		if (skeletonAnimation == null) 
			return;

		skeletonAnimation.AnimationState.Event += HandleEvent;
	}

    void HandleEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
		if (e.data.name == "Hit")
			ActionController.currentTarget.TryTakeDamage(ActionController.currentAbility.GetDamage());		
	} 

    public void PlayIdle()
    {
		skeletonAnimation.AnimationState.SetAnimation(0, idle, true);
	}

	public void PlayTakeDamage()
	{
		skeletonAnimation.AnimationState.SetAnimation(0, takeDamage, false);
	}

    public void PlayGrab()
    {
        if (grabbed != null)
            skeletonAnimation.AnimationState.SetAnimation(0, grabbed, false);
        else
            PlayTakeDamage();
    }

    public IEnumerator PlayAnimation(AnimationReferenceAsset animationReference)
	{		
		skeletonAnimation.AnimationState.SetAnimation(0, animationReference, false);
        yield return new WaitForSeconds(animationReference.Animation.duration);
	}   
}
