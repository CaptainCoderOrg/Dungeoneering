using System;
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
        [AssertIsSet][SerializeField] private SelectTacticsMenu _preperationMenu;
        [AssertIsSet][SerializeField] private EncounterSettingsData _encounterSettingsData;
        [AssertIsSet][SerializeField] private EncounterInitializer _initializer;
        [AssertIsSet][SerializeField] private HeroTurnController _heroTurnController;
        [AssertIsSet][field: SerializeField] public EncounterCamera EncounterCamera { get; private set; }
        [AssertIsSet][SerializeField] private EncounterData _encounterData;
        private readonly DungeonBuilder _builder = new();
        [AssertIsSet][SerializeField] private Transform _tileContainer;
        [AssertIsSet][SerializeField] private DungeonTile _tilePrefab;
        [SerializeField] private EncounterFigureController _selected;
        public EncounterFigureController Selected => _selected;
        private EncounterState _state;
        public EncounterState State { get => _state ??= new(); }
        public HeroTurnController HeroTurnController => _heroTurnController;

        public void Select(EncounterFigureController selected)
        {
            _selected?.Deselect();
            _selected = selected;
            _selected?.Select();
            EncounterCamera.PanTo(_selected);
        }

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
            if (controller != @event.Controller)
            {
                throw new System.Exception($"Illegal movement. Expected {@event.Controller} ({@event.Controller.GetInstanceID()}) at position {start}.");
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

        private IEnumerator<YieldInstruction> AnimateMove(EncounterFigureController controller, IEnumerable<Vector2Int> path)
        {
            float moveTime = 0;
            foreach (Vector2Int position in path)
            {
                Vector3 start = controller.transform.localPosition;
                Vector3 end = new(position.x, 0, position.y);
                moveTime += _encounterSettingsData.MovementSpeed;
                while (moveTime > 0)
                {
                    moveTime -= Time.deltaTime;
                    float percent = 1 - (moveTime / _encounterSettingsData.MovementSpeed);
                    controller.transform.localPosition = Vector3.Lerp(start, end, percent);
                    yield return null;
                }
                controller.transform.localPosition = end;
            }
        }

        internal void SelectTactics(HeroFigurePanel heroFigurePanel)
        {
            Select(heroFigurePanel.FigureController);
            _preperationMenu.SelectAndShow(heroFigurePanel);
        }
    }
}