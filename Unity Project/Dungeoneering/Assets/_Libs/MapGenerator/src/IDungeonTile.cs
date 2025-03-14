using CaptainCoder.Dungeoneering.Unity.Data;

using UnityEngine.Events;

namespace CaptainCoder.Dungeoneering.DungeonMap.Unity
{

    public interface IDungeonTile : ISelectable
    {
        public TileReference TileReference { get; }
        public Dungeon Dungeon { get; }
        public DungeonController DungeonController { get; }
        public Position Position { get; }
        public UnityEvent<IDungeonTile> OnClicked { get; }
        public DungeonWall NorthWall { get; }
        public DungeonWall EastWall { get; }
        public DungeonWall SouthWall { get; }
        public DungeonWall WestWall { get; }

        public DungeonWall this[Facing facing]
        {
            get
            {
                return facing switch
                {
                    Facing.North => NorthWall,
                    Facing.East => EastWall,
                    Facing.South => SouthWall,
                    Facing.West => WestWall,
                    _ => throw new System.ArgumentOutOfRangeException(nameof(facing), facing, null),
                };
            }
        }

        public string FloorTextureName { get; }

        public void SetAllWallsSelected(bool isSelected);

        public void UpdateWalls(TileWalls configuration, TileWallTextures textures);
    }
}
