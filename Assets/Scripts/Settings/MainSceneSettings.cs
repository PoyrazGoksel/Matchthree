using Installers;
using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(fileName = nameof(MainSceneSettings), menuName = "Matchtree/" + nameof(MainSceneSettings), order = 0)]
    public class MainSceneSettings : ScriptableObject
    {
        [SerializeField] private MainSceneInstaller.Settings _settings;
        public MainSceneInstaller.Settings Settings => _settings;
    }
}