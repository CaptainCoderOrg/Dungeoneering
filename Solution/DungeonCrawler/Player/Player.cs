namespace CaptainCoder.Dungeoneering.Player;

using CaptainCoder.Dungeoneering.DungeonMap;

public record PlayerView(Position Position, Facing Facing)
{
    public PlayerView(int x, int y, Facing facing) : this(new Position(x, y), facing) { }
}