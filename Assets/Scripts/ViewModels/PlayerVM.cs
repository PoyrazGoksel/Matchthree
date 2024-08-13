using System;
using Events;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace ViewModels
{
    [Serializable]
    public class PlayerVM : IInitializable, IDisposable
    {
        [Inject] private GridEvents GridEvents{get;set;}
        [Inject] private ProjectEvents ProjectEvents{get;set;}
        public int Level => _level;
        [ShowInInspector]public int MoveCount{get;set;}
        [SerializeField] private int _level;

        public void Dispose()
        {
            GridEvents.PlayerMoved -= OnPlayerMoved;
        }

        public void Initialize()
        {
            GridEvents.PlayerMoved += OnPlayerMoved;
        }

        private void OnPlayerMoved()
        {
            MoveCount --;

            if(MoveCount <= 0)
            {
                _level ++;
                ProjectEvents.LevelComplete?.Invoke();
            }
        }
    }
}