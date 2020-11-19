using UnityEngine;
using UnityEngine.UI;

namespace UnnyNet
{
    [RequireComponent(typeof(Text))]
    public class LocalizationText : LocalizationUI
    {
        private Text _component;

        private void Awake()
        {
            _component = GetComponent<Text>();
            SetTextValue();
        }

        protected override void OnLocalizationChanged(string localizationCode)
        {
            SetTextValue();
        }

        void SetTextValue()
        {
            if (_component != null)
                _component.text = GetLocalization();
        }
    }
}