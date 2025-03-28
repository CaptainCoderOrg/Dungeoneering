using System.Collections;
using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.Unity;
using CaptainCoder.Dungeoneering.Unity.Data;
using CaptainCoder.Unity.Assertions;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class EncounterController : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private EncounterInitializer _initializer;
        [AssertIsSet][field: SerializeField] public EncounterCamera EncounterCamera { get; private set; }
        [AssertIsSet][SerializeField] private EncounterData _encounterData;
        private readonly DungeonBuilder _builder = new();
        [AssertIsSet][SerializeField] private Transform _tileContainer;
        [AssertIsSet][SerializeField] private DungeonTile _tilePrefab;

        public EncounterState State { get; internal set; } = new();

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
            _initializer.Init(_encounterData);
            _encounterData.DungeonCrawlerData.LoadDungeonByName(_encounterData.DungeonName);
            _builder.BuildOrUpdateTiles(_tileContainer, _tilePrefab, UpdateTile, CreateTile);
        }

        private DungeonTile CreateTile(DungeonTile tilePrefab, Transform parent, Position position) => DungeonTile.Create(tilePrefab, parent, _encounterData.DungeonCrawlerData, position);
        private void UpdateTile(DungeonTile tile, Position position) => DungeonTile.UpdateTile(_encounterData.DungeonCrawlerData, position, tile);


        public void HandleMovementEvent(MoveFigureEvent @event)
        {
            Vector2Int start = @event.Path.First();
            Vector2Int last = @event.Path.Last();
            if (!State.Figures.TryGetValue(start, out EncounterFigureController controller))
            {
                throw new System.Exception($"Illegal movement. No figure found at position {start}.");
            }
            if (controller.Figure != @event.Figure)
            {
                throw new System.Exception($"Illegal movement. Expected {@event.Figure.name} ({@event.Figure.GetInstanceID()}) at position {start}.");
            }
            if (State.Figures.ContainsKey(last))
            {
                throw new System.Exception($"Illegal movement. Figure found at end position {last}.");
            }
            State.Figures.Remove(start);
            State.Figures[last] = controller;
            controller.Figure.Position = last;
            StartCoroutine(AnimateMove(controller, @event.Path));
        }

        private IEnumerator AnimateMove(EncounterFigureController controller, IEnumerable<Vector2Int> path)
        {
            foreach (Vector2Int position in path)
            {
                controller.transform.localPosition = new Vector3(position.x, 0, position.y);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}