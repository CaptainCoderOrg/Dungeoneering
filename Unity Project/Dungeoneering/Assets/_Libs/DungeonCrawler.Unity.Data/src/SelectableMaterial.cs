using CaptainCoder.Dungeoneering.Unity.Data;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.DungeonMap.Unity;
public readonly struct SelectableMaterial
{
    private static int s_nextID = 1;
    public readonly TextureId Id;
    public readonly Material Unselected;
    public readonly Material Selected;

    public SelectableMaterial(Material unselected)
    {
        Id = new TextureId(s_nextID++);
        Unselected = unselected;
        Selected = new Material(unselected);
        Selected.EnableKeyword("_SELECTION_ON");
    }

    public Material GetMaterial(bool selected) => selected ? Selected : Unselected;
    public static implicit operator Material(SelectableMaterial mat) => mat.Unselected;
}