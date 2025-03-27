using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/DieData")]
    public class DieData : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Sprite Sprite { get; private set; }
        [field: SerializeField] public Color Albedo { get; private set; }
        [field: SerializeField] public FaceData[] Faces { get; private set; }
    }
}