using System.Collections.Generic;
using Extensions.System;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Extensions.Unity.MonoHelper
{
    public class UISlider : UIBase
    {
        [ShowIf("@_manualReference == true")][SerializeField]
        protected Slider _mySlider;

        [ShowIf("@_manualReference == true")][SerializeField]
        protected List<Image> _myOtherImages = new();

        [ShowIf("@_manualReference == true")][SerializeField]
        protected List<TextMeshProUGUI> _myOtherTMPs = new();

        [SerializeField][UsedImplicitly] 
        private bool _manualReference;

        private Slider.SliderEvent onValueChanged => _mySlider.onValueChanged;
        public float value{get => _mySlider.value;set => _mySlider.value = value;}
        
        private void OnValidate()
        {
            if(_manualReference) return;
            
            if (! _mySlider)
            {
                if (transform.TryGetComponent(out Slider mySlider))
                {
                    _mySlider = mySlider;
                }
                else
                {
                    Debug.LogError("UIButtonTMP needs a Slider in its children!");
                }
            }
        }

        [Button(Name = "SetActive(bool isActive)", Style = ButtonStyle.Box, Expanded = true)]
        public override void SetActive(bool isActive)
        {
            base.SetActive(isActive);
            
            _mySlider.enabled = isActive;
            _myOtherImages.DoToAll(oI => oI.enabled = isActive);
            _myOtherTMPs.DoToAll(oTmp => oTmp.enabled = isActive);
        }

        protected override void RegisterEvents()
        {
        }

        protected override void UnRegisterEvents()
        {
        }
    }
}