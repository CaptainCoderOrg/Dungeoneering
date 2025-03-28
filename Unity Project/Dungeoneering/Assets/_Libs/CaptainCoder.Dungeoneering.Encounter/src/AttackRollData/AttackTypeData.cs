using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/AttackTypeData")]
    public class AttackTypeData : ObservableSO
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Sprite Sprite { get; private set; }
        [field: SerializeField] public DieData AttackDie { get; private set; }
        [field: SerializeField] public string Tooltip { get; private set; }
        public TextTooltipData TooltipData { get; private set; }

        public override void OnBeforeEnterPlayMode()
        {
            base.OnBeforeEnterPlayMode();
            TooltipData = ScriptableObject.CreateInstance<TextTooltipData>();
            TooltipData.Text = Tooltip;
        }
    }
}