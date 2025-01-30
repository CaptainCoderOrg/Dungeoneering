namespace CaptainCoder.DungeonCrawler.Combat;

public class CombatMap
{
    public HashSet<Position> Tiles { get; init; } = new();
    public Dictionary<Position, PlayerCharacter> PlayerCharacters { get; init; } = [];
    public Dictionary<Position, Enemy> Enemies { get; init; } = [];
    public Action<PlayerCharacter>? OnCharacterChange;
    public Action<MoveActionEvent>? OnMoveAction;
    public Action<Position>? OnEnemyRemoved;
}

public abstract record CombatMapEvent;
public record ExertEvent(PlayerCharacter Original, PlayerCharacter New, Position Position);
public record MoveActionEvent(PlayerCharacter Moving, MoveAction Move, IEnumerable<Position> Path);

public static class CombatMapExtensions
{
    public static CombatMap ParseMap(string setup, Func<char, PlayerCharacter?> characterLookup, Func<char, Enemy?> enemyLookup)
    {
        Dictionary<char, HashSet<Position>> allData = ParseCharPositions(setup);
        HashSet<Position> tiles = new();
        Dictionary<Position, PlayerCharacter> pcs = new();
        Dictionary<Position, Enemy> enemies = new();
        foreach ((char ch, HashSet<Position> positions) in allData)
        {
            if (ch == '#')
            {
                tiles.UnionWith(positions);
                continue;
            }
            if (characterLookup(ch) is PlayerCharacter pc)
            {
                tiles.UnionWith(positions);
                pcs.Add(positions.First(), pc);
                continue;
            }
            if (enemyLookup(ch) is Enemy e)
            {
                tiles.UnionWith(positions);
                foreach (Position position in positions) { enemies.Add(position, e); }
                continue;
            }
            Console.WriteLine($"Unknown character found while building CombatMap '{ch}'({(int)ch}) at {string.Join(",", positions)}");
        }

        CombatMap map = new() { Tiles = tiles, PlayerCharacters = pcs, Enemies = enemies };
        return map;
    }

    public static void RemoveEnemy(this CombatMap map, Position target)
    {
        if (map.Enemies.Remove(target))
        {
            map.OnEnemyRemoved?.Invoke(target);
        }
    }

    public static PlayerCharacter GetCharacter(this CombatMap map, CharacterCard card)
    {
        foreach (PlayerCharacter pc in map.PlayerCharacters.Values)
        {
            if (pc.Card.Name == card.Name) { return pc; }
        }
        throw new ArgumentOutOfRangeException($"No character found: {card}");
    }
    public static PlayerCharacter UpdateCharacter(this CombatMap map, PlayerCharacter character)
    {
        Position position = map.GetPosition(character.Card);
        map.PlayerCharacters[position] = character;
        map.OnCharacterChange?.Invoke(character);
        return character;
    }
    public static Position GetPosition(this CombatMap map, CharacterCard card)
    {
        foreach ((Position p, PlayerCharacter pc) in map.PlayerCharacters)
        {
            if (pc.Card.Name == card.Name) { return p; }
        }
        throw new ArgumentOutOfRangeException($"No character found: {card}");
    }
    public static HashSet<Position> ParseTiles(string toParse) => ParseCharPositions(toParse).GetValueOrDefault('#', new HashSet<Position>());

    public static Dictionary<char, HashSet<Position>> ParseCharPositions(string toParse)
    {
        string[] rows = toParse.ReplaceNewLines().Split(Environment.NewLine);
        Dictionary<char, HashSet<Position>> tiles = new();
        for (int y = 0; y < rows.Length; y++)
        {
            for (int x = 0; x < rows[y].Length; x++)
            {
                char ch = rows[y][x];
                if (ch == ' ') { continue; }
                HashSet<Position> set = GetOrInit(ch);
                set.Add(new Position(x, y));
            }
        }
        return tiles;

        HashSet<Position> GetOrInit(char ch)
        {
            if (!tiles.TryGetValue(ch, out HashSet<Position> set))
            {
                set = new HashSet<Position>();
                tiles[ch] = set;
            }
            return set;
        }
    }
}