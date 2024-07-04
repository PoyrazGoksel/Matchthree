using DG.Tweening;
using Events;
using Extensions.DoTween;
using Extensions.Unity.MonoHelper;
using UnityEngine;
using Zenject;

namespace Components.UI
{
    public class PlayerScoreTMP : UITMP, ITweenContainerBind
    {
        [Inject] private GridEvents GridEvents{get;set;}
        private Tween _counterTween;
        public ITweenContainer TweenContainer{get;set;}
        private int _currCounterVal;
        private int _playerScore;

        private void Awake()
        {
            TweenContainer = TweenContain.Install(this);
        }

        protected override void RegisterEvents()
        {
            GridEvents.MatchGroupDespawn += OnMatchGroupDespawn;
        }

        private void OnMatchGroupDespawn(int arg0)
        {
            Debug.LogWarning($"{arg0}");
            
            _playerScore += arg0;

            if(_counterTween.IsActive()) _counterTween.Kill();
            
            _counterTween = DOVirtual.Int
            (
                _currCounterVal,
                _playerScore,
                1f,
                OnCounterUpdate
            );

            TweenContainer.AddTween = _counterTween;
        }

        private void OnCounterUpdate(int val)
        {
            _currCounterVal = val;
            _myTMP.text = $"Score: {_currCounterVal}";
        }

        protected override void UnRegisterEvents()
        {
            GridEvents.MatchGroupDespawn -= OnMatchGroupDespawn;
        }
    }
}