using Extensions.System;
using UnityEngine;

namespace Services
{
    public class ToBeToAPI
    {
        private static bool _currentGroup;
        private const float ABGroupChance = 0.5f;
        private const string ABTestPrefKey = "ABTest";
        public static ToBeToAPI Ins{get;private set;}
        
        [RuntimeInitializeOnLoadMethod(loadType: RuntimeInitializeLoadType.BeforeSplashScreen)]
        public static void RuntimeInitializeOnLoad()
        {
            Ins = new ToBeToAPI();

            float randomABGroup = Random.value;

            bool ab = randomABGroup > ABGroupChance;

            _currentGroup = ab;

            if(PlayerPrefs.HasKey(ABTestPrefKey) == false)
            {
                PlayerPrefs.SetInt(ABTestPrefKey, _currentGroup.ToInt());

                Debug.LogWarning("ABTest Init");
            }
            else
            {
                _currentGroup = PlayerPrefs.GetInt(ABTestPrefKey).ToBool();
            }

            Debug.LogWarning($"AB Group: {_currentGroup.ToInt()}");
        }

        public int GetGroup()
        {
            return _currentGroup.ToInt();
        }
    }
}