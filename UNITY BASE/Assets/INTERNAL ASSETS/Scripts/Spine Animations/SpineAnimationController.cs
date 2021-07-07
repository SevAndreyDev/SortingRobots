using System;
using UnityEngine;
using Spine.Unity;
using Spine;

namespace EnglishKids.SortingRobots
{
    public class SpineAnimationController : MonoBehaviour
    {
        //==================================================
        // Fields
        //==================================================

        [SerializeField] private SkeletonGraphic _animation;

        protected string _currentAnimation;
        private Action<string> OnAnimationCompleteHandler;

        //==================================================
        // Properties
        //==================================================

        public bool IsPlaying { get { return !string.IsNullOrEmpty(_currentAnimation) && _animation.AnimationState.GetCurrent(0).IsComplete; } }

        //==================================================
        // Methods
        //==================================================

        protected void PlayAnimation(AnimationReferenceAsset targetAnimation, bool loop, float animationSpeed)
        {
            if (!targetAnimation.name.Equals(_currentAnimation))
            {                
                _animation.AnimationState.ClearTrack(0);
                _animation.AnimationState.SetAnimation(0, targetAnimation, loop).TimeScale = animationSpeed;

                if (!loop)
                {
                    _animation.AnimationState.Complete -= OnAnimationCompleteEvent;
                    _animation.AnimationState.Complete += OnAnimationCompleteEvent;
                }
                            
                _currentAnimation = targetAnimation.name;
            }            
        }

        #region Events
        protected virtual void OnAnimationComplete(TrackEntry trackEntry) { }

        private void OnAnimationCompleteEvent(TrackEntry trackEntry)
        {
            _animation.AnimationState.Complete -= OnAnimationCompleteEvent;
            _animation.AnimationState.ClearTrack(0);
            _currentAnimation = string.Empty;

            OnAnimationComplete(trackEntry);
        }
        #endregion
    }
}