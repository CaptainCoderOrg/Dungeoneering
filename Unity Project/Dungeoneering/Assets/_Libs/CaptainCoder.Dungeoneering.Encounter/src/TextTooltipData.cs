using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/Text Tooltip Data")]
    public class TextTooltipData : TooltipData
    {
        [field: SerializeField] public string Text { get; set; }
        public override void Render(TooltipController controller)
        {
            controller.Text = Text;
        }
    }
}