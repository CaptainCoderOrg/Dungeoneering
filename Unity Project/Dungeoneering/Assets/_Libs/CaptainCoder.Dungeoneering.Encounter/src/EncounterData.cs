using System.Collections.Generic;

using CaptainCoder.Dungeoneering.Unity.Data;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/Encounter Data")]
    public class EncounterData : ObservableSO
    {
        [field: SerializeField] public List<FigureData> Figures { get; private set; }
        [field: SerializeField] public DungeonCrawlerData DungeonCrawlerData { get; private set; }
        [field: SerializeField] public string DungeonName { get; private set; }
        [field: SerializeField] public int MinX { get; private set; }
        [field: SerializeField] public int MaxX { get; private set; }
        [field: SerializeField] public int MinY { get; private set; }
        [field: SerializeField] public int MaxY { get; private set; }
    }
}