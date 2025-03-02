using System.Collections.Generic;

using CaptainCoder.Dungeoneering.DungeonMap;

namespace CaptainCoder.Dungeoneering.Unity.Data;
public static class DungeonGlobals
{
    public const int DIMENSION = 24;
    public static readonly Facing[] AllFacings = { Facing.North, Facing.East, Facing.South, Facing.West };
    private readonly static HashSet<Position> AllPositions = new();
    private static ReadOnlySetView<Position> s_allPositionsView;
    public static ReadOnlySetView<Position> Positions
    {
        get
        {
            if (s_allPositionsView == null)
            {
                s_allPositionsView = new ReadOnlySetView<Position>(AllPositions);
                for (int row = 0; row < DIMENSION; row++)
                {
                    for (int col = 0; col < DIMENSION; col++)
                    {
                        AllPositions.Add(new Position(row, col));
                    }
                }
            }
            return s_allPositionsView;
        }
    }
}