namespace CaptainCoder.DungeonCrawler.Combat;

public static class EnemyCombatController
{
    public static void MoveEnemy(this CombatMap map, Position start, Position end)
    {
        Enemy toMove = map.Enemies[start];
        map.Enemies[end] = toMove;
        map.Enemies.Remove(start);
    }
    /// <summary>
    /// Retrieves the moves an enemy plans to take. An enemy moves toward the nearest PlayerCharacter.
    /// If there is a tie, moves toward the PlayerCharacter with the least HP. If there is a further
    /// tie, arbitrary tie breaker.
    /// </summary>
    public static IEnumerable<Position> GetEnemyMove(this CombatMap map, Position enemyPosition)
    {
        HashSet<Position> targets = map.PlayerCharacters.Keys.FindNeighbors();
        IEnumerable<Position> shortestPath = map.FindShortestPath(enemyPosition, targets);
        int speed = map.Enemies[enemyPosition].Card.Speed;
        return shortestPath.Take(speed);
    }

    internal static HashSet<Position> FindNeighbors(this IEnumerable<Position> positions)
    {
        HashSet<Position> all = new();
        foreach (Position pos in positions)
        {
            all.UnionWith(pos.Neighbors());
        }
        return all;
    }

    internal static IEnumerable<Position> Neighbors(this Position p) =>
        [
            p with { X = p.X + 1 },
            p with { X = p.X - 1 },
            p with { Y = p.Y + 1 },
            p with { Y = p.Y - 1 },
            p with { X = p.X + 1, Y = p.Y - 1 },
            p with { X = p.X + 1, Y = p.Y + 1 },
            p with { X = p.X - 1, Y = p.Y - 1 },
            p with { X = p.X - 1, Y = p.Y + 1 },
        ];
}