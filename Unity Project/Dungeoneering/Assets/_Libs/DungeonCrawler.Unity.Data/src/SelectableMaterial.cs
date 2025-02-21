using System.Linq;

using UnityEngine;
using UnityEngine.Rendering;

namespace CaptainCoder.Dungeoneering.DungeonMap.Unity
{
    public readonly struct SelectableMaterial
    {
        public readonly Material Unselected;
        private readonly System.Lazy<Material> _selected;
        public Material Selected => _selected.Value;

        public SelectableMaterial(Material unselected)
        {
            Unselected = unselected;
            _selected = new System.Lazy<Material>(() =>
            {
                Material selected = new(unselected);
                selected.EnableKeyword("_SELECTION_ON");
                return selected;
            });
        }

        public Material GetMaterial(bool selected) => selected ? Selected : Unselected;

        public static implicit operator Material(SelectableMaterial mat) => mat.Unselected;
    }
}