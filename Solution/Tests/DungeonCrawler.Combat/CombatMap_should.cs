namespace Tests;

using CaptainCoder.DungeonCrawler.Combat;

using Shouldly;

public class CombatMap_should
{
    public static HashSet<Position> NoTiles => new();

    public static HashSet<Position> Square4x4 => new()
    {
        new Position(0, 0), new Position(1, 0), new Position(2, 0), new Position(3, 0),
        new Position(0, 1), new Position(1, 1), new Position(2, 1), new Position(3, 1),
        new Position(0, 2), new Position(1, 2), new Position(2, 2), new Position(3, 2),
        new Position(0, 3), new Position(1, 3), new Position(2, 3), new Position(3, 3),
    };

    public static HashSet<Position> Cross => new()
    {
                            new Position(1, 0), new Position(2, 0),
        new Position(0, 1), new Position(1, 1), new Position(2, 1), new Position(3, 1),
        new Position(0, 2), new Position(1, 2), new Position(2, 2), new Position(3, 2),
                            new Position(1, 3), new Position(2, 3),
    };

    public static HashSet<Position> SquareAndCross => new()
    {
        new Position(0, 0), new Position(1, 0), new Position(2, 0), new Position(3, 0),
        new Position(0, 1), new Position(1, 1), new Position(2, 1), new Position(3, 1),
        new Position(0, 2), new Position(1, 2), new Position(2, 2), new Position(3, 2),
        new Position(0, 3), new Position(1, 3), new Position(2, 3), new Position(3, 3),
                            new Position(1, 4), new Position(2, 4),
        new Position(0, 5), new Position(1, 5), new Position(2, 5), new Position(3, 5),
        new Position(0, 6), new Position(1, 6), new Position(2, 6), new Position(3, 6),
                            new Position(1, 7), new Position(2, 7),
    };

    public static IEnumerable<object[]> ParseTilesFromStringData => [
        [
            string.Empty,
            NoTiles
        ],
        [
            """
            ####
            ####
            ####
            ####
            """,
            Square4x4
        ],
        [
            """
             ## 
            ####
            ####
             ## 
            """,
            Cross
        ],
        [
            """
            ####
            ####
            ####
            ####
             ## 
            ####
            ####
             ## 
            """,
            SquareAndCross
        ]
    ];

    [Theory]
    [MemberData(nameof(ParseTilesFromStringData))]
    public void parse_tiles_from_string(string toParse, HashSet<Position> expectedPositions)
    {
        HashSet<Position> actual = CombatMapExtensions.ParseTiles(toParse);
        actual.Count.ShouldBe(expectedPositions.Count);
        actual.ShouldBeSubsetOf(expectedPositions);
    }
}