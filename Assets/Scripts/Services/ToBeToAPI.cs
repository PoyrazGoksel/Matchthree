using Extensions.System;
using UnityEngine;

namespace Services
{
    public class ToBeToAPI
    {
        private const float ABGroupChance = 0.5f;
        private const string ABTestPrefKey = "ABTest";
        public static ToBeToAPI Ins{get;private set;}
        
        [RuntimeInitializeOnLoadMethod(loadType: RuntimeInitializeLoadType.BeforeSplashScreen)]
        public static void RuntimeInitializeOnLoad()
        {
            Ins = new ToBeToAPI();

            float randomABGroup = Random.value;

            bool ab = randomABGroup > ABGroupChance;

            if(PlayerPrefs.HasKey(ABTestPrefKey) == false)
            {
                PlayerPrefs.SetInt(ABTestPrefKey, ab.ToInt());

                Debug.LogWarning("ABTest Init");
            }
            else
            {
                ab = PlayerPrefs.GetInt(ABTestPrefKey).ToBool();
            }

            Debug.LogWarning($"AB Group: {ab.ToInt()}");
        }
    }
}