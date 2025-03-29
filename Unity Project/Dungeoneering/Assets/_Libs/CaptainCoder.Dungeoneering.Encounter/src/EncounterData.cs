using System.Collections.Generic;

using CaptainCoder.Dungeoneering.Unity.Data;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/Encounter Data")]
    public class EncounterData : ObservableSO
    {
        [field: SerializeField] public List<EnemyFigure> EnemyFigures { get; private set; }
        [field: SerializeField] public List<HeroFigure> HeroFigures { get; private set; }
        [field: SerializeField] public DungeonCrawlerData DungeonCrawlerData { get; private set; }
        [field: SerializeField] public string DungeonName { get; private set; }
        [field: SerializeField] public int MinX { get; private set; }
        [field: SerializeField] public int MaxX { get; private set; }
        [field: SerializeField] public int MinY { get; private set; }
        [field: SerializeField] public int MaxY { get; private set; }
    }

    [System.Serializable]
    public struct EnemyFigure
    {
        public EnemyEntityData EnemyEntityTemplate;
        public Vector2Int Position;
    }

    [System.Serializable]
    public struct HeroFigure
    {
        public HeroEntityData HeroEntity;
        public Vector2Int Position;
    }
}