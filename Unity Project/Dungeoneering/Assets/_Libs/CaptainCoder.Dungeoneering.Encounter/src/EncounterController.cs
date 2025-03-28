using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using CaptainCoder.Dungeoneering.Unity.Data;
using CaptainCoder.Unity.Assertions;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class EncounterController : MonoBehaviour
    {
        [AssertIsSet][field: SerializeField] public EncounterCamera EncounterCamera { get; private set; }
        [AssertIsSet][SerializeField] private EncounterData _encounterData;
        private readonly DungeonBuilder _builder = new();
        [AssertIsSet][SerializeField] private Transform _tileContainer;
        [AssertIsSet][SerializeField] private DungeonTile _tilePrefab;

        void Awake()
        {
            _builder.MinX = _encounterData.MinX;
            _builder.MaxX = _encounterData.MaxX;
            _builder.MinY = _encounterData.MinY;
            _builder.MaxY = _encounterData.MaxY;
            Build();
        }

        public void Build()
        {
            _encounterData.DungeonCrawlerData.LoadDungeonByName(_encounterData.DungeonName);
            _builder.BuildOrUpdateTiles(_tileContainer, _tilePrefab, UpdateTile, CreateTile);
        }

        private DungeonTile CreateTile(DungeonTile tilePrefab, Transform parent, Position position) => DungeonTile.Create(tilePrefab, parent, _encounterData.DungeonCrawlerData, position);
        private void UpdateTile(DungeonTile tile, Position position) => DungeonTile.UpdateTile(_encounterData.DungeonCrawlerData, position, tile);
    }
}