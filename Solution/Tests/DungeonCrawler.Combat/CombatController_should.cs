using CaptainCoder.DungeonCrawler;
using CaptainCoder.DungeonCrawler.Combat;

using Shouldly;

namespace Tests;
public class CombatController_should
{
    public string TestMap =
    """
     ##    ## 0
    ##########1
     ##    ## 2
    0123456789
    """;
    [Theory]
    [InlineData(0, 1, 4, 1, 1)]
    [InlineData(7, 0, 3, 7, 1)]
    [InlineData(7, 0, 3, 6, 1)]
    [InlineData(7, 0, 3, 5, 1)]
    public void validate_move_action(int x, int y, int movementPoints, int targetX, int targetY)
    {
        // Arrange
        Position start = new(x, y);
        Position end = new(targetX, targetY);
        CombatMap map = new()
        {
            Tiles = CombatMapExtensions.ParseTiles(TestMap),
            PlayerCharacters = new()
            {
                { new Position(x, y), new PlayerCharacter() { MovementPoints = movementPoints } },
                // This PlayerCharacter can be moved through, but you cannot end your move in this space
                { new Position(4, 1), new PlayerCharacter(new CharacterCard("Obstacle")) },
            }
        };

        MoveAction action = new(start, end);

        // Act
        bool actual = map.IsValidMoveAction(action);

        // Assert
        actual.ShouldBeTrue();
    }

    [Theory]
    [InlineData(0, 1, 4, 4, 1)] // Cannot end on other players
    [InlineData(0, 1, 4, 3, 0)] // Cannot move into wall space
    [InlineData(0, 1, 4, 0, 0)] // Cannot move into wall space
    [InlineData(0, 1, 4, 0, 2)] // Cannot move into wall space
    [InlineData(3, 1, 1, 5, 1)] // Cannot move into other player
    [InlineData(3, 1, 1, 5, 2)] // Moving through player counts as movement
    [InlineData(3, 1, 1, 3, 0)] // Cannot move into wall space
    [InlineData(3, 1, 1, 3, 1)] // Cannot move into wall space
    [InlineData(3, 1, 1, 1, 0)] // Too far
    [InlineData(3, 1, 1, 1, 1)] // Too far
    [InlineData(3, 1, 1, 1, 2)] // Too far
    public void invalidate_move_action(int x, int y, int movementPoints, int targetX, int targetY)
    {
        // """
        //  ##    ## 0
        // ####*#####1
        //  ##    ## 2
        // 0123456789
        // """;

        // Arrange
        Position start = new(x, y);
        Position end = new(targetX, targetY);
        CombatMap map = new()
        {
            Tiles = CombatMapExtensions.ParseTiles(TestMap),
            PlayerCharacters = new()
            {
                { new Position(x, y), new PlayerCharacter() { MovementPoints = movementPoints } },
                // This PlayerCharacter can be moved through, but you cannot end your move in this space
                { new Position(4, 1), new PlayerCharacter(new CharacterCard("Obstacle")) },
            }
        };

        MoveAction action = new(start, end);

        // Act
        bool actual = map.IsValidMoveAction(action);

        // Assert
        actual.ShouldBeFalse();
    }

    public static IEnumerable<object[]> FindValidMovesData => [
        [
            1,
            """
            ####
            ####
            ####
            ####
            """,
            """
            #**D
            #*A*
            #B**
            ##C#
            """,
        ],
        [
            4,
            """
             ##    ###
            ###########
            ###########
             ##    ###
            """,
            """
             #*    ***
            ##**B*A****
            ##**C******
             #*    *D*
            """,
        ],

        [
            3,
            """
             ##    ###
            ###########
            ###########
             ##    ###
            """,
            """
             ##    ***
            ###*B*A***#
            ###*C*****#
             ##    *D*
            """,
        ],
        [
            2,
            """
             ##    ###
            ###########
            ###########
             ##    ###
            """,
            """
             ##    **#
            ####B*A**##
            ####C****##
             ##    *D#
            """,
        ],

    ];
    // Each letter represents a PlayerCharacter
    // Each * represents where the PlayerCharacter marked as A can move
    [Theory]
    [MemberData(nameof(FindValidMovesData))]
    public void find_valid_moves(int movementPoints, string map, string expectedMap)
    {
        HashSet<Position> tiles = CombatMapExtensions.ParseTiles(map);
        Dictionary<char, HashSet<Position>> setupMap = CombatMapExtensions.ParseCharPositions(expectedMap);
        Position start = setupMap['A'].First();
        Position char2 = setupMap['B'].First();
        Position char3 = setupMap['C'].First();
        Position char4 = setupMap['D'].First();
        HashSet<Position> expectedValidMoves = setupMap['*'];
        CombatMap underTest = new()
        {
            Tiles = tiles,
            PlayerCharacters = new Dictionary<Position, PlayerCharacter>()
            {
                { start, new PlayerCharacter(){ MovementPoints = movementPoints } },
                { char2, new PlayerCharacter(){ } },
                { char3, new PlayerCharacter(){ } },
                { char4, new PlayerCharacter(){ } }
            },
        };

        HashSet<Position> actual = underTest.FindValidMoves(start, movementPoints);

        actual.Count.ShouldBe(expectedValidMoves.Count);
        actual.ShouldBeSubsetOf(expectedValidMoves);
    }

    public static IEnumerable<object[]> PlayerCannotMoveThroughEnemiesData => [
        [
            1,
            """
            ####
            ####
            ####
            ####
            """,
            """
            #**D
            #*A*
            #B**
            ##C#
            """,
        ],
        [
            4,
            """
             ##    ###
            ###########
            ###########
             ##    ###
            """,
            """
             ##    ***
            ####B*A****
            ####C******
             ##    *D*
            """,
        ],

        [
            3,
            """
             ##    ###
            ###########
            ###########
             ##    ###
            """,
            """
             ##    ***
            ####B*A***#
            ####C*****#
             ##    *D*
            """,
        ],
        [
            2,
            """
             ##    ###
            ###########
            ###########
             ##    ###
            """,
            """
             #*    ###
            ##**B**####
            ##*CAD*####
             ##    ###
            """,
        ],

    ];
    // The letter 'A' represents the PlayerCharacter moving
    // Each * represents where the PlayerCharacter marked as A can move
    // B, C, and D represents enemies
    [Theory]
    [MemberData(nameof(PlayerCannotMoveThroughEnemiesData))]
    public void prevent_player_from_moving_through_enemy(int movementPoints, string map, string expectedMap)
    {
        HashSet<Position> tiles = CombatMapExtensions.ParseTiles(map);
        Dictionary<char, HashSet<Position>> setupMap = CombatMapExtensions.ParseCharPositions(expectedMap);
        Position start = setupMap['A'].First();
        Position enemyB = setupMap['B'].First();
        Position enemyC = setupMap['C'].First();
        Position enemyD = setupMap['D'].First();
        HashSet<Position> expectedValidMoves = setupMap['*'];
        CombatMap underTest = new()
        {
            Tiles = tiles,
            PlayerCharacters = new Dictionary<Position, PlayerCharacter>()
            {
                { start, new PlayerCharacter(){ MovementPoints = movementPoints } },
            },
            Enemies = new Dictionary<Position, Enemy>()
            {
                { enemyB, new Enemy() },
                { enemyC, new Enemy() },
                { enemyD, new Enemy() },
            }
        };

        HashSet<Position> actual = underTest.FindValidMoves(start, movementPoints);

        actual.Count.ShouldBe(expectedValidMoves.Count);
        actual.ShouldBeSubsetOf(expectedValidMoves);
    }

    // A - Player that is moving
    // T - Target position to move to
    // C, B - PlayerCharacters
    // 1, 2 - Enemies
    public static IEnumerable<object[]> ApplyMoveActionData => [
        [
            """
            ####
            ####
            ####
            ####
            """,
            """
            A###
            #T##
            ##CB
            ##12
            """,
            4,
            3
        ],
        [
            """
             ##    ###
            ###########
            ###########
             ##    ###
            """,
            """
             BC    ###
            1#####T####
            2##########
             ##    ##A
            """,
            4,
            1
        ],

        [ // Enemies block movement, so the player must go AROUND the enemies
            """
             ##    ###
            ###########
            ###########
             ##    ###
            """,
            """
             ##    ###
            B#####T####
            C######21##
             ##    ##A
            """,
            4,
            0
        ],
        [ // PlayerCharacters do not block movement
            """
             ##    ###
            ###########
            ###########
             ##    ###
            """,
            """
             ##    ###
            1#####T####
            2######BC##
             ##    ##A
            """,
            4,
            1
        ],

    ];

    [Theory]
    [MemberData(nameof(ApplyMoveActionData))]
    public void apply_move_action(string map, string setup, int movementPoints, int expectedMovementPoints)
    {
        // Arrange
        HashSet<Position> tiles = CombatMapExtensions.ParseTiles(map);
        Dictionary<char, HashSet<Position>> setupMap = CombatMapExtensions.ParseCharPositions(setup);
        Position start = setupMap['A'].First();
        Position target = setupMap['T'].First();
        Position pcB = setupMap['B'].First();
        Position pcC = setupMap['C'].First();
        Position enemy1 = setupMap['1'].First();
        Position enemy2 = setupMap['2'].First();
        CombatMap underTest = new()
        {
            Tiles = tiles,
            PlayerCharacters = new Dictionary<Position, PlayerCharacter>()
            {
                { start, new PlayerCharacter(){ MovementPoints = movementPoints } },
                { pcB, new PlayerCharacter() },
                { pcC, new PlayerCharacter() },
            },
            Enemies = new Dictionary<Position, Enemy>()
            {
                { enemy1, new Enemy() },
                { enemy2, new Enemy() },
            }
        };

        MoveAction moveAction = new(start, target);
        PlayerCharacter? actualChangedData = null;
        MoveActionEvent? actualMoveActionEvent = null;
        underTest.OnCharacterChange += changeData => actualChangedData = changeData;
        underTest.OnMoveAction += @event => actualMoveActionEvent = @event;
        // Act
        underTest.ApplyMoveAction(moveAction);

        underTest.PlayerCharacters.ShouldNotContainKey(start);
        underTest.PlayerCharacters[target].ShouldBe(new PlayerCharacter() { MovementPoints = expectedMovementPoints });

        // Testing notifications
        actualChangedData.ShouldBe(new PlayerCharacter() { MovementPoints = expectedMovementPoints });
        actualMoveActionEvent.ShouldNotBeNull();
        actualMoveActionEvent.Moving.ShouldBe(new PlayerCharacter() { MovementPoints = expectedMovementPoints });
        actualMoveActionEvent.Move.ShouldBe(moveAction);
        actualMoveActionEvent.Path.SequenceEqual(underTest.FindShortestPath(moveAction.Start, [moveAction.End])).ShouldBeTrue();
    }

    // A is the PlayerCharacter moving
    // T is the target position
    // 1, 2 - Enemies
    // B, C - PlayerCharacters
    public static IEnumerable<object[]> FindShortestPathData => [
        [
            """
            ####
            ####
            ####
            ####
            """,
            """
            A##B
            ###C
            ##T1
            ###2
            """,
            (Position[])[new Position(1, 1), new Position(2, 2)]
        ],
        [ // PlayerCharacters do not block movement
            """
             ##    ###
            ###########
            ###########
             ##    ###
            """,
            """
             ##    ### 
            1######T###
            2######BC##
             ##    ##A 
            """,
            (Position[])[new Position(8, 2), new Position(7, 1)]
        ],
        [ // Must move around enemies
            """
             ##    ###
            ###########
            ###########
             ##    ###
            """,
            """
             ##    ### 
            B######T###
            C#######1##
             ##    #2A 
            """,
            (Position[])[new Position(9, 2), new Position(8, 1), new Position(7, 1)]
        ],

    ];

    [Theory]
    [MemberData(nameof(FindShortestPathData))]
    public void find_shortest_path(string map, string setup, Position[] expectedPath)
    {
        // Arrange
        HashSet<Position> tiles = CombatMapExtensions.ParseTiles(map);
        Dictionary<char, HashSet<Position>> setupMap = CombatMapExtensions.ParseCharPositions(setup);
        Position start = setupMap['A'].First();
        Position target = setupMap['T'].First();
        Position pcB = setupMap['B'].First();
        Position pcC = setupMap['C'].First();
        Position enemy1 = setupMap['1'].First();
        Position enemy2 = setupMap['2'].First();
        CombatMap underTest = new()
        {
            Tiles = tiles,
            PlayerCharacters = new Dictionary<Position, PlayerCharacter>()
            {
                { start, new PlayerCharacter(){ MovementPoints = 4 } },
                { pcB, new PlayerCharacter() },
                { pcC, new PlayerCharacter() },
            },
            Enemies = new Dictionary<Position, Enemy>()
            {
                { enemy1, new Enemy() },
                { enemy2, new Enemy() },
            }
        };

        // Act
        Position[] actual = [.. underTest.FindShortestPath(start, [target])];

        actual.SequenceEqual(expectedPath).ShouldBeTrue();
    }

    // A is the PlayerCharacter moving
    // T is the target position
    // 1, 2 - Enemies
    // B, C - PlayerCharacters
    public static IEnumerable<object[]> ThrowExceptionOnInvalidMoveActionData => [

        [ // PlayerCharacters do not block movement
            """
             ##    ###
            ###########
            ###########
             ##    ###
            """,
            """
             ##    ### 
            1##T#######
            2######BC##
             ##    ##A 
            """
        ],
        [ // Must move around enemies
            """
             ##    ###
            ###########
            ###########
             ##    ###
            """,
            """
             ##    ### 
            B####T#####
            C#######1##
             ##    #2A 
            """
        ],

    ];

    [Theory]
    [MemberData(nameof(ThrowExceptionOnInvalidMoveActionData))]
    public void throw_exception_on_invalid_move_action(string map, string setup)
    {
        // Arrange
        HashSet<Position> tiles = CombatMapExtensions.ParseTiles(map);
        Dictionary<char, HashSet<Position>> setupMap = CombatMapExtensions.ParseCharPositions(setup);
        Position start = setupMap['A'].First();
        Position target = setupMap['T'].First();
        Position pcB = setupMap['B'].First();
        Position pcC = setupMap['C'].First();
        Position enemy1 = setupMap['1'].First();
        Position enemy2 = setupMap['2'].First();
        CombatMap underTest = new()
        {
            Tiles = tiles,
            PlayerCharacters = new Dictionary<Position, PlayerCharacter>()
            {
                { start, new PlayerCharacter(){ MovementPoints = 4 } },
                { pcB, new PlayerCharacter() },
                { pcC, new PlayerCharacter() },
            },
            Enemies = new Dictionary<Position, Enemy>()
            {
                { enemy1, new Enemy() },
                { enemy2, new Enemy() },
            }
        };

        // Act
        Should.Throw<ArgumentException>(() => underTest.ApplyMoveAction(new MoveAction(start, target)));

    }

    [Theory]
    [InlineData(2, 2, 4, 0)]
    [InlineData(1, 2, 3, 2)]
    [InlineData(3, 2, 1, 0)]
    [InlineData(2, 3, 4, 3)]
    public void validate_exert_action(int x, int y, int energy, int exertion)
    {
        Position position = new(x, y);
        CombatMap underTest = new()
        {
            Tiles = CombatMapExtensions.ParseTiles(TestMap),
            PlayerCharacters = new Dictionary<Position, PlayerCharacter>()
            {
                {
                    position,
                    new PlayerCharacter() {
                        Card = new CharacterCard() { BaseEnergy = energy},
                        Exertion = exertion
                    }
                }
            }
        };
        ExertAction action = new(position);
        bool actual = underTest.IsValidExertAction(action);

        actual.ShouldBeTrue();
    }



    [Theory]
    [InlineData(2, 2, 2, 2, 4, 4)] // Not valid when character has no energy remaining
    [InlineData(1, 2, 1, 2, 3, 3)] // Not valid when character has no energy remaining
    [InlineData(3, 2, 2, 2, 4, 0)] // Not valid when position doesn't contain character
    [InlineData(3, 2, 1, 2, 3, 0)] // Not valid when position doesn't contain character
    public void invalidate_exert_action(int playerX, int playerY, int targetX, int targetY, int energy, int exertion)
    {
        Position position = new(playerX, playerY);
        CombatMap underTest = new()
        {
            Tiles = CombatMapExtensions.ParseTiles(TestMap),
            PlayerCharacters = new Dictionary<Position, PlayerCharacter>()
            {
                {
                    position,
                    new PlayerCharacter() {
                        Card = new CharacterCard() { BaseEnergy = energy},
                        Exertion = exertion
                    }
                }
            }
        };
        ExertAction action = new(new Position(targetX, targetY));
        bool actual = underTest.IsValidExertAction(action);

        actual.ShouldBeFalse();
    }

    [Theory]
    [InlineData(2, 2, 4, 0, 3, 0, 1)]
    [InlineData(1, 2, 3, 2, 0, 2, 3)]
    [InlineData(3, 2, 3, 0, 2, 1, 2)]
    [InlineData(2, 3, 4, 2, 1, 3, 4)]
    public void apply_exert_action(int x, int y, int energy, int exertion, int expectedExertion, int movementPoints, int expectedMovement)
    {
        Position position = new(x, y);
        CombatMap underTest = new()
        {
            Tiles = CombatMapExtensions.ParseTiles(TestMap),
            PlayerCharacters = new Dictionary<Position, PlayerCharacter>()
            {
                {
                    position,
                    new PlayerCharacter() {
                        Card = new CharacterCard() { BaseEnergy = energy},
                        MovementPoints = movementPoints,
                        Exertion = exertion
                    }
                }
            }
        };

        PlayerCharacter? actualChangedData = null;
        underTest.OnCharacterChange += changeData => actualChangedData = changeData;
        ExertAction action = new(position);
        underTest.ApplyExertAction(action);

        underTest.PlayerCharacters[position].Energy().ShouldBe(expectedExertion);
        underTest.PlayerCharacters[position].MovementPoints.ShouldBe(expectedMovement);

        // Test notifications
        actualChangedData.ShouldBe(new PlayerCharacter()
        {
            Card = new CharacterCard() { BaseEnergy = energy },
            MovementPoints = expectedMovement,
            Exertion = exertion + 1,
        });
    }

    [Theory]
    [InlineData(2, 2, 2, 2, 4, 4)] // Not valid when character has no energy remaining
    [InlineData(1, 2, 1, 2, 3, 3)] // Not valid when character has no energy remaining
    [InlineData(3, 2, 2, 2, 4, 0)] // Not valid when position doesn't contain character
    [InlineData(3, 2, 1, 2, 3, 0)] // Not valid when position doesn't contain character
    public void throw_exception_on_invalid_exert_action(int playerX, int playerY, int targetX, int targetY, int energy, int exertion)
    {
        Position position = new(playerX, playerY);
        CombatMap underTest = new()
        {
            Tiles = CombatMapExtensions.ParseTiles(TestMap),
            PlayerCharacters = new Dictionary<Position, PlayerCharacter>()
            {
                {
                    position,
                    new PlayerCharacter() {
                        Card = new CharacterCard() { BaseEnergy = energy},
                        Exertion = exertion
                    }
                }
            }
        };

        ExertAction action = new(new Position(targetX, targetY));
        Should.Throw<ArgumentException>(() => { underTest.ApplyExertAction(action); });
    }

    [Theory]
    [InlineData(1, 1, 3, 2, 1)]
    [InlineData(3, 2, 2, 1, 3)]
    [InlineData(2, 3, 1, 2, 3)]
    public void end_character_turn(int x, int y, int movementPoints, int actionPoints, int attackPoints)
    {
        Position pos = new(x, y);
        PlayerCharacter character = new() { MovementPoints = movementPoints, ActionPoints = actionPoints, AttackPoints = attackPoints };
        CombatMap underTest = new()
        {
            Tiles = CombatMapExtensions.ParseTiles(TestMap),
            PlayerCharacters = new Dictionary<Position, PlayerCharacter>()
            {
                { pos, character }
            }
        };

        PlayerCharacter? actualChangedData = null;
        underTest.OnCharacterChange += changed => actualChangedData = changed;
        EndTurnAction action = new(pos);


        underTest.ApplyEndCharacterTurn(action);

        PlayerCharacter expected = character with { MovementPoints = 0, ActionPoints = 0, AttackPoints = 0 };
        underTest.PlayerCharacters[pos].ShouldBe(expected);

        // Test notification
        actualChangedData.ShouldBe(expected);
    }
}