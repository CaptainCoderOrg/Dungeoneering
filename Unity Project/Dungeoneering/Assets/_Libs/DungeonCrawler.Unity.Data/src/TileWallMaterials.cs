using System.ComponentModel;

using CaptainCoder.Dungeoneering.DungeonMap;
namespace CaptainCoder.Dungeoneering.Unity.Data;
public class TileWallTextures
{
    public TextureReference this[Facing facing]
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

    public TextureReference North { get; set; } = default!;
    public TextureReference East { get; set; } = default!;
    public TextureReference South { get; set; } = default!;
    public TextureReference West { get; set; } = default!;
}