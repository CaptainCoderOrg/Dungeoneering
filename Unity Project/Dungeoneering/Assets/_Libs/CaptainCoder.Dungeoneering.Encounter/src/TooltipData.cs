using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/Tooltip Data")]
    public class TooltipData : ObservableSO
    {
        public virtual void Render(TooltipController controller)
        {
        }
    }
}