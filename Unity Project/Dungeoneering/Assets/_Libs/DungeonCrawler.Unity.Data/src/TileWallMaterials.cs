using System.ComponentModel;
namespace CaptainCoder.Dungeoneering.DungeonMap.Unity
{
    public class TileWallMaterials
    {
        public SelectableMaterial this[Facing facing]
        {
            get
            {
                return facing switch
                {
                    Facing.North => North,
                    Facing.East => East,
                    Facing.South => South,
                    Facing.West => West,
                    _ => throw new InvalidEnumArgumentException(nameof(facing), (int)facing, facing.GetType()),
                };
            }
        }

        public SelectableMaterial North { get; set; } = default!;
        public SelectableMaterial East { get; set; } = default!;
        public SelectableMaterial South { get; set; } = default!;
        public SelectableMaterial West { get; set; } = default!;
    }
}