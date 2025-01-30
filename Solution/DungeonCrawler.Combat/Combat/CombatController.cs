namespace CaptainCoder.DungeonCrawler.Combat;

public static class CombatController
{
    public static void ApplyEndCharacterTurn(this CombatMap map, EndTurnAction endTurn)
    {
        PlayerCharacter updated = map.PlayerCharacters[endTurn.Target] with { ActionPoints = 0, AttackPoints = 0, MovementPoints = 0 };
        map.PlayerCharacters[endTurn.Target] = updated;
        map.OnCharacterChange?.Invoke(updated);
    }

    /// <summary>
    /// A valid ExertAction is one that targets a tile with a PlayerCharacter
    /// and that PlayerCharacter has energy remaining.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public static bool IsValidExertAction(this CombatMap map, ExertAction action)
    {
        if (!map.PlayerCharacters.TryGetValue(action.Target, out PlayerCharacter pc)) { return false; }
        return pc.Energy() > 0;
    }

    public static void ApplyExertAction(this CombatMap map, ExertAction action)
    {
        if (!map.IsValidExertAction(action)) { throw new ArgumentException($"Invalid ExertAction: {action}"); }
        PlayerCharacter pc = map.PlayerCharacters[action.Target];
        PlayerCharacter updated = pc with { Exertion = pc.Exertion + 1, MovementPoints = pc.MovementPoints + 1 };
        map.PlayerCharacters[action.Target] = updated;
        map.OnCharacterChange?.Invoke(updated);
    }

    public static bool IsValidMoveAction(this CombatMap map, MoveAction moveAction)
    {
        // Start position must contain a player
        if (!map.PlayerCharacters.TryGetValue(moveAction.Start, out PlayerCharacter pc)) { return false; }
        HashSet<Position> validMoves = map.FindValidMoves(moveAction.Start, pc.MovementPoints);
        return validMoves.Contains(moveAction.End);
    }
    public static void ApplyMoveAction(this CombatMap map, MoveAction moveAction)
    {
        PlayerCharacter character = map.PlayerCharacters[moveAction.Start];
        map.PlayerCharacters.Remove(moveAction.Start);
        Position[] shortestPath = [.. map.FindShortestPath(moveAction.Start, [moveAction.End])];
        int distance = shortestPath.Length;
        if (distance > character.MovementPoints) { throw new ArgumentException("Invalid Move. No path found."); }
        PlayerCharacter updated = character with { MovementPoints = character.MovementPoints - distance };
        map.PlayerCharacters[moveAction.End] = updated;
        map.OnCharacterChange?.Invoke(updated);
        map.OnMoveAction?.Invoke(new MoveActionEvent(updated, moveAction, shortestPath));
    }

    public static IEnumerable<Position> FindShortestPath(this CombatMap map, Position start, IEnumerable<Position> targets)
    {
        HashSet<Position> ends = [.. targets];
        HashSet<Position> visited = new() { start };
        Queue<(Position, List<Position>)> queue = new();
        queue.Enqueue((start, new()));

        while (queue.TryDequeue(out var next))
        {
            (Position currentPosition, List<Position> currentPath) = next;

            if (ends.Contains(currentPosition)) { return currentPath; }

            foreach (Position neighbor in map.Neighbors(visited, currentPosition))
            {
                visited.Add(neighbor);
                List<Position> newPath = [.. currentPath, neighbor];
                queue.Enqueue((neighbor, newPath));
            }
        }
        return [];
    }

    public static HashSet<Position> FindValidMoves(this CombatMap map, Position start, int maxDistance)
    {
        HashSet<Position> visited = new() { start };
        HashSet<Position> validMoves = new() { };
        Queue<(Position, int movesRemaining)> queue = new();
        queue.Enqueue((start, maxDistance));

        while (queue.TryDequeue(out (Position, int) next))
        {
            var (nextPosition, movesRemaining) = next;
            if (movesRemaining == 0) { continue; }

            foreach (Position neighbor in map.Neighbors(visited, nextPosition))
            {
                if (!map.PlayerCharacters.ContainsKey(neighbor)) // Cannot move on other player
                {
                    validMoves.Add(neighbor);
                }
                visited.Add(neighbor);
                queue.Enqueue((neighbor, movesRemaining - 1));
            }
        }

        return validMoves;
    }

    static IEnumerable<Position> Neighbors(this CombatMap map, HashSet<Position> visited, Position position)
    {
        var (x, y) = position;
        IEnumerable<Position> neighbors =
                    [
                        new Position(x - 1, y),
                        new Position(x + 1, y),
                        new Position(x, y + 1),
                        new Position(x, y - 1),
                        new Position(x - 1, y - 1),
                        new Position(x + 1, y - 1),
                        new Position(x - 1, y + 1),
                        new Position(x + 1, y + 1),
                    ];
        return neighbors.Where(n => !visited.Contains(n))
                        .Where(map.Tiles.Contains)
                        .Where(n => !map.Enemies.ContainsKey(n));
    }
}

public record struct Position(int X, int Y);
public abstract record CombatAction;
/// <summary>
/// Represents a move from the start and end position
/// </summary>
public record MoveAction(Position Start, Position End) : CombatAction;
/// <summary>
/// Represents an exert action on a character at the specified position
/// </summary>
public record ExertAction(Position Target) : CombatAction;
public record Attack : CombatAction;
/// <summary>
/// Ends the turn of the character at the specified position. This sets the characters MovementPoints, ActionPoints, and AttackPoints to 0.
/// </summary>
public record EndTurnAction(Position Target) : CombatAction;