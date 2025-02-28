using System.Collections.Generic;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.DungeonMap.Unity;
public readonly struct SelectableMaterial
{
    private static List<SelectableMaterial> s_materials = new();
    private static int s_nextID = 1;
    public readonly int Id;
    public readonly Material Unselected;
    public readonly Material Selected;

    public SelectableMaterial(Material unselected)
    {
        Id = s_nextID++;
        Unselected = unselected;
        Selected = new Material(unselected);
        Selected.EnableKeyword("_SELECTION_ON");
        s_materials.Add(this);
    }

    public Material GetMaterial(bool selected) => selected ? Selected : Unselected;
    public static SelectableMaterial GetById(int id) => s_materials[id];
    public static implicit operator Material(SelectableMaterial mat) => mat.Unselected;
}