using System.Collections.Generic;
using System.Linq;

using CaptainCoder.Dungeoneering.Unity.Data;
using CaptainCoder.Unity.Assertions;


using UnityEngine;

namespace CaptainCoder.Dungeoneering.DungeonMap.Unity
{
    public class ThickDungeonTile : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private DungeonTileController _tileController;
        [AssertIsSet][SerializeField] private Material _transparentMaterial;
        [SerializeField] private TextureTarget[] _floorTargets;
        [SerializeField] private TextureTarget[] _northTargets;
        [SerializeField] private TextureTarget[] _eastTargets;
        [SerializeField] private TextureTarget[] _southTargets;
        [SerializeField] private TextureTarget[] _westTargets;
        [SerializeField] private TextureTarget _stepNorthFaceEast;
        [SerializeField] private TextureTarget _stepEastFaceNorth;
        [SerializeField] private TextureTarget _stepWestFaceNorth;
        [SerializeField] private TextureTarget _stepNorthFaceWest;
        [SerializeField] private TextureTarget _stepWestFaceSouth;
        [SerializeField] private TextureTarget _stepSouthFaceWest;
        [SerializeField] private TextureTarget _stepEastFaceSouth;
        [SerializeField] private TextureTarget _stepSouthFaceEast;
        private readonly Dictionary<WallReference, TextureTarget> _neighborTargets = new();
        private DungeonCrawlerData _dungeonCrawlerData;
        private TileReference _tileReference;

        void OnEnable()
        {
            _tileController.AddObserver(HandleTileChanged);
        }

        void OnDisable()
        {
            _tileController.RemoveObserver(HandleTileChanged);
        }

        private void HandleTileChanged(TileChangedEvent @event)
        {
            switch (@event)
            {
                case DungeonCrawlerDataInitialized(DungeonCrawlerData data):
                    _dungeonCrawlerData = data;
                    break;
                case TileReferenceUpdated(TileReference tileReference):
                    UpdateTileReference(tileReference);
                    break;

            }
        }

        private void BuildNeighborTargets()
        {
            _neighborTargets.Clear();
            _neighborTargets[new(_tileReference.Dungeon, _tileReference.Position.Step(Facing.North), Facing.East)] = _stepNorthFaceEast;
            _neighborTargets[new(_tileReference.Dungeon, _tileReference.Position.Step(Facing.East), Facing.North)] = _stepEastFaceNorth;
            _neighborTargets[new(_tileReference.Dungeon, _tileReference.Position.Step(Facing.North), Facing.West)] = _stepNorthFaceWest;
            _neighborTargets[new(_tileReference.Dungeon, _tileReference.Position.Step(Facing.West), Facing.North)] = _stepWestFaceNorth;
            _neighborTargets[new(_tileReference.Dungeon, _tileReference.Position.Step(Facing.South), Facing.East)] = _stepSouthFaceEast;
            _neighborTargets[new(_tileReference.Dungeon, _tileReference.Position.Step(Facing.East), Facing.South)] = _stepEastFaceSouth;
            _neighborTargets[new(_tileReference.Dungeon, _tileReference.Position.Step(Facing.South), Facing.West)] = _stepSouthFaceWest;
            _neighborTargets[new(_tileReference.Dungeon, _tileReference.Position.Step(Facing.West), Facing.South)] = _stepWestFaceSouth;
        }

        private void UpdateTileReference(TileReference newReference)
        {
            _tileReference = newReference;
            BuildNeighborTargets();
            UpdateTileTexture(_dungeonCrawlerData.GetTexture(_tileReference));
            List<WallReference> corners = new();
            foreach (Facing f in DungeonGlobals.AllFacings)
            {
                WallReference wallRef = new(_tileReference.Dungeon, _tileReference.Position, f);
                if (wallRef.WallType == WallType.None)
                {
                    corners.AddRange(HideWall(wallRef));
                }
                else
                {
                    UpdateWallTexture(wallRef);
                }
            }

            foreach (WallReference corner in corners)
            {
                CheckNeighborCorner(corner);
            }
        }

        private void UpdateTileTexture(TextureReference texture)
        {
            foreach (TextureTarget target in _floorTargets)
            {
                target.Apply(texture.Material);
            }
        }

        private void UpdateWallTexture(WallReference wallRef)
        {
            TextureTarget[] targets = GetTargets(wallRef.Facing);
            var texture = _dungeonCrawlerData.GetTexture(wallRef);
            foreach (TextureTarget target in targets)
            {
                target.Apply(texture.Material);
            }
        }

        private IEnumerable<WallReference> HideWall(WallReference wallRef)
        {
            TextureTarget[] targets = GetTargets(wallRef.Facing);
            foreach (TextureTarget target in targets) { target.Apply(_transparentMaterial); }
            // If we don't have a wall to our right, we may need to add our neighbors corner
            if ((wallRef with { Facing = wallRef.Facing.Rotate() }).WallType == WallType.None)
            {
                yield return new(wallRef.Dungeon, wallRef.Position.Step(wallRef.Facing), wallRef.Facing.Rotate());
                yield return new(wallRef.Dungeon, wallRef.Position.Step(wallRef.Facing.Rotate()), wallRef.Facing);
            }
        }

        private void CheckNeighborCorner(WallReference neighbor)
        {
            if (neighbor.WallType != WallType.None)
            {
                TextureReference rfTexture = _dungeonCrawlerData.GetTexture(neighbor);
                _neighborTargets[neighbor].Apply(rfTexture.Material);
            }
        }

        private TextureTarget[] GetTargets(Facing facing) => facing switch
        {
            Facing.North => _northTargets,
            Facing.East => _eastTargets,
            Facing.South => _southTargets,
            Facing.West => _westTargets,
            _ => throw new System.Exception($"Unexpected facing: {facing}"),
        };

        // #if UNITY_EDITOR
        //         [SerializeField] private Material _floorMaterial;
        //         [SerializeField] private Material _northMaterial;
        //         [SerializeField] private Material _southMaterial;
        //         [SerializeField] private Material _eastMaterial;
        //         [SerializeField] private Material _westMaterial;

        //         [Button("Build Targets")]
        //         private void BuildTargets()
        //         {
        //             Dictionary<string, List<TextureTarget>> targets = new() {
        //                 { _floorMaterial.name, new() },
        //                 { _northMaterial.name, new() },
        //                 { _southMaterial.name, new() },
        //                 { _eastMaterial.name, new() },
        //                 { _westMaterial.name, new() },
        //              };
        //             foreach (MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>(true))
        //             {
        //                 for (int ix = 0; ix < renderer.sharedMaterials.Length; ix++)
        //                 {
        //                     string m = renderer.sharedMaterials[ix].name.Replace(" (Instance)", "");
        //                     if (targets.TryGetValue(m, out List<TextureTarget> ts))
        //                     {
        //                         ts.Add(new TextureTarget() { Renderer = renderer, Ix = ix });
        //                     }
        //                 }
        //             }
        //             _floorTargets = targets[_floorMaterial.name].ToArray();
        //             _northTargets = targets[_northMaterial.name].ToArray();
        //             _southTargets = targets[_southMaterial.name].ToArray();
        //             _eastTargets = targets[_eastMaterial.name].ToArray();
        //             _westTargets = targets[_westMaterial.name].ToArray();
        //         }

        //         [Button("Apply Targets")]
        //         private void ApplyTargets()
        //         {
        //             foreach (TextureTarget target in _floorTargets)
        //             {
        //                 target.Renderer.sharedMaterials[target.Ix] = _floorMaterial;
        //             }
        //             foreach (TextureTarget target in _northTargets)
        //             {
        //                 target.Renderer.sharedMaterials[target.Ix] = _northMaterial;
        //             }
        //         }
        // #endif
    }

    [System.Serializable]
    public struct TextureTarget
    {
        public MeshRenderer Renderer;
        public int Ix;
        public void Apply(Material material)
        {
            int index = Ix;
            Renderer.materials = Renderer.materials.Select((original, ix) => ix == index ? material : original).ToArray();
        }
    }
}