using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/Tooltip Element Data")]
    public class TooltipElementData : ObservableSO
    {
        private TooltipController _tooltip;
        public TooltipController Tooltip => _tooltip ??= FindFirstObjectByType<TooltipController>();

        public override void OnBeforeEnterPlayMode()
        {
            base.OnBeforeEnterPlayMode();
            _tooltip = null;
        }
    }
}