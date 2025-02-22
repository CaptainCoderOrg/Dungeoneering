using UnityEngine;

namespace CaptainCoder.Dungeoneering.DungeonMap.Unity
{
    public readonly struct SelectableMaterial
    {
        public readonly Material Unselected;
        public readonly Material Selected;

        public SelectableMaterial(Material unselected)
        {
            Unselected = unselected;
            Selected = new Material(unselected);
            Selected.EnableKeyword("_SELECTION_ON");
        }

        public Material GetMaterial(bool selected) => selected ? Selected : Unselected;

        public static implicit operator Material(SelectableMaterial mat) => mat.Unselected;
    }
}