using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

namespace Extensions.Unity
{
    public static class AnimatorExt
    {
        /// <summary>
        /// Overrides the animation clips of the Animator with custom animation clips.
        /// </summary>
        /// <param name="animator">The Animator component to override the clips of.</param>
        /// <param name="animationClips">A list of AnimationClips to override the existing clips with.</param>
        /// <param name="animCompleteFuncName">The name of the function to be called when the animation clip completes. Related Func Req AnimationClip as param.</param>
        [Obsolete]public static void OverrideClips
        (
            this Animator animator,
            List<AnimationClip> animationClips,
            string animCompleteFuncName
        )
        {
            AnimatorOverrideController animatorOverrideController = new() {runtimeAnimatorController = animator.runtimeAnimatorController};
            List<KeyValuePair<AnimationClip, AnimationClip>> overrides = new();

            foreach(AnimationClip animationClip in animationClips)
            {
                AnimationEvent endEvent = new()
                {
                    functionName = animCompleteFuncName,
                    time = animationClip.length,
                    objectReferenceParameter = animationClip
                };

                animationClip.AddEvent(endEvent);
                overrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(null, animationClip));
            }

            animatorOverrideController.ApplyOverrides(overrides);
            animator.runtimeAnimatorController = animatorOverrideController;
        }
        
        public static void AddAnimClip(this AnimatorController toAnimatorController, AnimationClip newStateMotion, string newStateName, string exitTrig,
            string anyTrig,
            float exitDur,
            float anyDur
        )
        {
            toAnimatorController.AddParameter(exitTrig, AnimatorControllerParameterType.Trigger);
            toAnimatorController.AddParameter(anyTrig, AnimatorControllerParameterType.Trigger);

            AnimatorStateMachine rootStateMachine = toAnimatorController.layers[0].stateMachine;

            AnimatorState newRootState = rootStateMachine.AddState(newStateName);
            newRootState.motion = newStateMotion;
            
            AnimatorTransition entryTransition = rootStateMachine.AddEntryTransition(newRootState);
            
            AnimatorStateTransition anyStateTransition = rootStateMachine.AddAnyStateTransition
            (newRootState);
            anyStateTransition.AddCondition(AnimatorConditionMode.If, 0, anyTrig);
            anyStateTransition.duration = anyDur;
            
            AnimatorStateTransition exitTransition = newRootState.AddExitTransition();
            exitTransition.AddCondition(AnimatorConditionMode.If, 0, exitTrig);
            exitTransition.duration = exitDur;
        }

    }
}