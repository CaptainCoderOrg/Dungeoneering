using System.Linq;
using System.Text;

using NaughtyAttributes;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/Equipment/Equipment Data")]
    public class EquipmentData : ObservableSO
    {
        const string Positive = "+";
        const string PositiveColor = "<color=#1C5E02>";
        const string NegativeColor = "<color=#A60002>";
        const string EndColor = "</color>";
        private static readonly StringBuilder TooltipBuilder = new();
        private static readonly StringBuilder UIBuilder = new();
        [field: SerializeField] public string Name { get; private set; }
        [TextArea(3, 5)][SerializeField] private string _description;
        public string Description => _description;
        [field: SerializeField] public Sprite Sprite { get; private set; }
        [field: SerializeField] public int Value { get; private set; }
        [field: SerializeField] public TraitEffect[] WornTraitEffects { get; private set; }
        [field: Expandable][field: SerializeField] public AttackAbilityData[] AttackAbilities { get; private set; }
        public string TooltipText { get; private set; }
        public string PanelText { get; private set; }

        public static void CopyTo(EquipmentData from, EquipmentData to)
        {
            to.Name = from.Name;
            to._description = from._description;
            to.Sprite = from.Sprite;
            to.Value = from.Value;
            to.WornTraitEffects = from.WornTraitEffects.ToArray();
        }

        public override void OnBeforeEnterPlayMode()
        {
            base.OnBeforeEnterPlayMode();
            (TooltipText, PanelText) = BuildTooltipText(this);
        }

        private static (string Tooltip, string Panel) BuildTooltipText(EquipmentData data)
        {
            UIBuilder.Clear();
            UIBuilder.Append($"{data.Value}<sprite name=\"coin\"\\>");
            TooltipBuilder.Clear();
            TooltipBuilder.Append($"<u>{data.Name}</u> ({data.Value})");
            foreach (var effect in data.WornTraitEffects)
            {
                (string sign, string startColor, string endColor) = effect.Value switch
                {
                    0 => (string.Empty, string.Empty, string.Empty),
                    var x when effect.Value > 0 => (Positive, PositiveColor, EndColor),
                    _ => (string.Empty, NegativeColor, EndColor)
                };
                UIBuilder.Append($" {startColor}{sign}{effect.Value}{effect.TraitType.SpriteTag}{endColor}");
                TooltipBuilder.Append($"\n{sign}{effect.Value} {effect.TraitType.Name}");
            }
            TooltipBuilder.Append($"\n<b>Click for Details</b>");
            return (TooltipBuilder.ToString(), UIBuilder.ToString());
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (WornTraitEffects == null) { return; }
            for (int ix = 0; ix < WornTraitEffects.Length; ix++)
            {
                WornTraitEffects[ix].Source = Name;
            }
        }

        [Button]
        public void CopyAsHeldEquipment()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Equipment", "held-equipment", "asset", "", AssetDatabase.GetAssetPath(this));
            HeldEquipmentData data = ScriptableObject.CreateInstance<HeldEquipmentData>();
            CopyTo(this, data);
            AssetDatabase.CreateAsset(data, path);
        }

        [Button]
        public void CopyAsWornEquipment()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Equipment", "held-equipment", "asset", "", AssetDatabase.GetAssetPath(this));
            WornEquipmentData data = ScriptableObject.CreateInstance<WornEquipmentData>();
            CopyTo(this, data);
            AssetDatabase.CreateAsset(data, path);
        }

        [Button]
        public void CopyAsAccessoryEquipment()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Equipment", "held-equipment", "asset", "", AssetDatabase.GetAssetPath(this));
            AccessoryEquipmentData data = ScriptableObject.CreateInstance<AccessoryEquipmentData>();
            CopyTo(this, data);
            AssetDatabase.CreateAsset(data, path);
        }

        [Button]
        public void CopyAsEquipment()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Equipment", "held-equipment", "asset", "", AssetDatabase.GetAssetPath(this));
            EquipmentData data = ScriptableObject.CreateInstance<EquipmentData>();
            CopyTo(this, data);
            AssetDatabase.CreateAsset(data, path);
        }
#endif
    }
}