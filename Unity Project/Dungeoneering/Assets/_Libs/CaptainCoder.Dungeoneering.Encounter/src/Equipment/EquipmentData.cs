using System.Linq;

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
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public Sprite Sprite { get; private set; }
        [field: SerializeField] public int Value { get; private set; }
        [field: SerializeField] public TraitEffect[] WornTraitEffects { get; private set; }

        public static void CopyTo(EquipmentData from, EquipmentData to)
        {
            to.Name = from.Name;
            to.Description = from.Description;
            to.Sprite = from.Sprite;
            to.Value = from.Value;
            to.WornTraitEffects = from.WornTraitEffects.ToArray();
        }

#if UNITY_EDITOR
        void OnValidate()
        {
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