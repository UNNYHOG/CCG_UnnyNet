using UnityEngine;

namespace UnnyNet
{
    public abstract class LocalizationUI : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField]
        private string localizationKey;
#pragma warning restore 649

        protected abstract void OnLocalizationChanged(string localizationCode);

        protected virtual void Start()
        {
            UnnyNet.Localization.Manager.SubscribeOnLocalizationChanged(OnLocalizationChanged);
        }

        protected virtual void OnDestroy()
        {
            UnnyNet.Localization.Manager.UnsubscribeOnLocalizationChanged(OnLocalizationChanged);
        }

        protected string GetLocalization()
        {
            return UnnyNet.Localization.Manager.Get(localizationKey);
        }
    }
}