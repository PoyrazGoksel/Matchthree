using System;
using System.Collections.Generic;
using Services;
using Settings;
using UnityEngine;
using ViewModels;
using Zenject;

namespace Installers
{
    public class MainSceneInstaller : MonoInstaller<MainSceneInstaller>
    {
        [Inject] private PlayerVM PlayerVm{get;set;}
        [SerializeField] private Camera _camera;
        private MainSceneSettings _mainSceneSettings;
        private LevelData _currLevel;

        public override void InstallBindings()
        {
            InstallSettings();
            InstallMono();
        }

        private void InstallMono() {Container.BindInstance(_camera);}

        private void InstallSettings()
        {
            _mainSceneSettings = Resources.Load<MainSceneSettings>(EnvVar.SettingsPath + nameof(MainSceneSettings));
        }

        public override void Start()
        {
            GetCurrLevelData();
            PlayerVm.MoveCount = _currLevel.LevelMoveCount;
            InstantiateLevel();
            _mainSceneSettings.Settings.PlayerVm = PlayerVm;
        }

        private void GetCurrLevelData()
        {
            int pLevel = PlayerVm.Level;

            int levelCount = _mainSceneSettings.Settings.Levels.Count;
            
            pLevel %= levelCount;

            if(ToBeToAPI.Ins.GetGroup() == 0)
            {
                _currLevel =  _mainSceneSettings.Settings.Levels[pLevel];
            }
            else
            {
                _currLevel =  _mainSceneSettings.Settings.LevelsB[pLevel];
            }
        }

        private void InstantiateLevel()
        {
            Container.InstantiatePrefab(_currLevel.LevelPrefab);
        }

        [Serializable]
        public class Settings
        {
            [SerializeField] public PlayerVM PlayerVm;
            public List<LevelData> Levels => _levels;
            [SerializeField] private List<LevelData> _levels;
            [SerializeField] private List<LevelData> _levelsB;
            public List<LevelData> LevelsB => _levelsB;
        }

        [Serializable]
        public class LevelData
        {
            public GameObject LevelPrefab => _levelPrefab;
            public int LevelMoveCount => _levelMoveCount;
            [SerializeField] private GameObject _levelPrefab;
            [SerializeField] private int _levelMoveCount;
        }
    }
}