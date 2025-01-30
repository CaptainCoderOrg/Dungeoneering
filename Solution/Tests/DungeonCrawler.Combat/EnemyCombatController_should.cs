using CaptainCoder.DungeonCrawler;
using CaptainCoder.DungeonCrawler.Combat;

using Shouldly;

namespace Tests;
public class EnemyCombatController_should
{

    // In this setup, A is a player Character.
    // 1 is the enemy who has a speed of 3
    public static IEnumerable<object[]> GetEnemyMoveData => [
        [
            """
            #########
            #########
            #########
            #########
            """,
            """
            #########
            #########
            A########
            1########
            """,
            // Enemy is already adjacent to a PC, does not move
            (Position[])[],
        ],
        [
            """
            #########
            #########
            #########
            #########
            """,
            """
            A########
            #########
            #########
            1########
            """,
            // Enemy moves 2 spaces to reach player
            (Position[])[new Position(0, 2), new Position(0, 1)],
        ]
        ,
        [
            """
            #########
            #########
            #########
            #########
            """,
            """
            #########
            #########
            #########
            1###A####
            """,
            // Enemy moves 3 spaces to reach player
            (Position[])[new Position(1, 3), new Position(2, 3), new Position(3, 3),],
        ]
        ,
        [
            """
            #########
            #########
            #########
            #########
            """,
            """
            #########
            #########
            #########
            1#######A
            """,
            // Enemy moves 3 spaces toward player
            (Position[])[new Position(1, 3), new Position(2, 3), new Position(3, 3),],
        ]
        ,
        [
            """
            #########
            #########
            #########
            #########
            """,
            """
            ###A#####
            #########
            #########
            1########
            """,
            // Enemy moves 2 spaces diagonally
            (Position[])[new Position(1, 2), new Position(2, 1)],
        ]
    ];
    [Theory]
    [MemberData(nameof(GetEnemyMoveData))]
    public void get_enemy_move_no_obstacles(string map, string setup, Position[] expectedMoves)
    {
        Dictionary<char, HashSet<Position>> setupMap = CombatMapExtensions.ParseCharPositions(setup);
        CombatMap underTest = new()
        {
            Tiles = CombatMapExtensions.ParseTiles(map),
            PlayerCharacters = new Dictionary<Position, PlayerCharacter>()
            {
                { setupMap['A'].First(), new PlayerCharacter() },
            },
            Enemies = new Dictionary<Position, Enemy>()
            {
                { setupMap['1'].First(), new Enemy() { Card = new EnemyCard() { Speed = 3 }}}
            }
        };

        IEnumerable<Position> actualMoves = underTest.GetEnemyMove(setupMap['1'].First());

        bool result = actualMoves.SequenceEqual(expectedMoves);
        result.ShouldBeTrue();
    }

    // // In this setup, A is a player Character.
    // // 1 is the enemy who has a speed of 3
    // // 2 is an enemy that blocks
    // public static IEnumerable<object[]> AllowEnemyToMoveThroughEnemiesData => [
    //     [
    //         """
    //         #########
    //         #########
    //         #########
    //         #########
    //         """,
    //         """
    //         A########
    //         #########
    //         2########
    //         1########
    //         """,
    //         // Enemy moves 2 spaces to reach player
    //         (Position[])[new Position(0, 2), new Position(0, 1)],
    //     ]
    //     ,
    //     [
    //         """
    //         #########
    //         #########
    //         #########
    //         #########
    //         """,
    //         """
    //         #########
    //         #########
    //         #########
    //         1#2#A####
    //         """,
    //         // Enemy moves 3 spaces to reach player
    //         (Position[])[new Position(1, 3), new Position(2, 3), new Position(3, 3),],
    //     ]
    //     ,
    //     [
    //         """
    //         #########
    //         #########
    //         #########
    //         #########
    //         """,
    //         """
    //         #########
    //         #########
    //         #########
    //         1##2####A
    //         """,
    //         // Enemy moves 3 spaces toward player moving around the other enemy
    //         (Position[])[new Position(1, 3), new Position(2, 3), new Position(3, 2),],
    //     ]
    //     ,
    //     [
    //         """
    //         #########
    //         #########
    //         #########
    //         #########
    //         """,
    //         """
    //         ###A#####
    //         ##2######
    //         #########
    //         1########
    //         """,
    //         // Enemy moves 2 spaces diagonally
    //         (Position[])[new Position(1, 2), new Position(2, 1), new Position(3, 1)],
    //     ]
    // ];
    // [Theory]
    // [MemberData(nameof(AllowEnemyToMoveThroughEnemiesData))]
    // public void allow_enemy_to_move_through_enemies(string map, string setup, Position[] expectedMoves)
    // {
    //     // TODO: This is trickier than expected. For now, enemies will not be allowed to pass

    //     Dictionary<char, HashSet<Position>> setupMap = CombatMapExtensions.ParseCharPositions(setup);
    //     CombatMap underTest = new()
    //     {
    //         Tiles = CombatMapExtensions.ParseTiles(map),
    //         PlayerCharacters = new Dictionary<Position, PlayerCharacter>()
    //         {
    //             { setupMap['A'].First(), new PlayerCharacter() },
    //         },
    //         Enemies = new Dictionary<Position, Enemy>()
    //         {
    //             { setupMap['1'].First(), new Enemy() { Card = new EnemyCard() { Speed = 3 }}},
    //             { setupMap['2'].First(), new Enemy() }
    //         }
    //     };

    //     IEnumerable<Position> actualMoves = underTest.GetEnemyMove(setupMap['1'].First());

    //     bool result = actualMoves.SequenceEqual(expectedMoves);
    //     result.ShouldBeTrue();
    // }
}