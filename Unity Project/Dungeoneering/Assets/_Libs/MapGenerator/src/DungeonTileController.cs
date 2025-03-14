
using System;

using CaptainCoder.Dungeoneering.Unity.Data;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.DungeonMap.Unity
{
    public class DungeonTileController : MonoBehaviour //, ISelectable
    {
        private Action<TileChangedEvent> _onDataChanged;
        private DungeonCrawlerData _dungeonCrawlerData;
        private TileChangedEvent _initialized;
        private TileChangedEvent _latestTileEvent;
        public DungeonCrawlerData DungeonCrawlerData
        {
            get => _dungeonCrawlerData;
            internal set
            {
                _dungeonCrawlerData = value;
                _initialized = new DungeonCrawlerDataInitialized(_dungeonCrawlerData);
                _onDataChanged?.Invoke(_initialized);
            }
        }
        private TileReference _tileReference;
        public TileReference TileReference
        {
            get => _tileReference;
            internal set
            {
                _tileReference = value;
                _onDataChanged?.Invoke(new TileReferenceUpdated(_tileReference));
            }
        }

        public void AddObserver(Action<TileChangedEvent> handler)
        {
            _onDataChanged += handler;
            if (_initialized != null) { handler.Invoke(_initialized); }
            if (_latestTileEvent != null) { handler.Invoke(_latestTileEvent); }
        }

        public void RemoveObserver(Action<TileChangedEvent> handler) => _onDataChanged -= handler;
    }

    public abstract record class TileChangedEvent;
    public sealed record class DungeonCrawlerDataInitialized(DungeonCrawlerData Data) : TileChangedEvent;
    public sealed record class TileReferenceUpdated(TileReference TileRef) : TileChangedEvent;
}