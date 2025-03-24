using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using CaptainCoder.Dungeoneering.Unity.Data;
using CaptainCoder.Unity.Assertions;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class EncounterController : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private DungeonCrawlerData _dungeonCrawlerData;
        private readonly DungeonBuilder _builder = new();
        [AssertIsSet][SerializeField] private Transform _tileContainer;
        [AssertIsSet][SerializeField] private DungeonTile _tilePrefab;
        [SerializeField] private string _dungeonName;
        [SerializeField] private int _minX;
        [SerializeField] private int _maxX;
        [SerializeField] private int _minY;
        [SerializeField] private int _maxY;

        void Awake()
        {
            _builder.MinX = _minX;
            _builder.MaxX = _maxX;
            _builder.MinY = _minY;
            _builder.MaxY = _maxY;
            Build();
        }

        public void Build()
        {
            _dungeonCrawlerData.LoadDungeonByName(_dungeonName);
            _builder.BuildOrUpdateTiles(_tileContainer, _tilePrefab, UpdateTile, CreateTile);
        }

        private DungeonTile CreateTile(DungeonTile tilePrefab, Transform parent, Position position) => DungeonTile.Create(tilePrefab, parent, _dungeonCrawlerData, position);
        private void UpdateTile(DungeonTile tile, Position position) => DungeonTile.UpdateTile(_dungeonCrawlerData, position, tile);
    }
}