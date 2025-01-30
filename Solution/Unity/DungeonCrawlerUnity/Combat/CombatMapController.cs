using System.Collections;
using System.Diagnostics;

using CaptainCoder.DungeonCrawler.Unity;
using CaptainCoder.Dungeoneering.Game;
using CaptainCoder.Dungeoneering.Game.Unity;
using CaptainCoder.Dungeoneering.Unity;

using UnityEngine;
using UnityEngine.Tilemaps;

namespace CaptainCoder.DungeonCrawler.Combat.Unity;

public class CombatMapController : MonoBehaviour
{
    public static CombatMapController Shared { get; private set; } = default!;
    public float MoveAnimationSpeed = 0.1f;
    public Tilemap MapInfo = default!;
    public Tilemap Grid = default!;
    [field: SerializeField]
    public Tilemap TileMap { get; set; } = default!;
    [field: SerializeField]
    public Tilemap WallMap { get; set; } = default!;
    [field: SerializeField]
    public Tilemap CursorMap { get; set; } = default!;
    [field: SerializeField]
    public Tilemap CharacterMap { get; set; } = default!;
    [field: SerializeField]
    public CombatIconDatabase IconDatabase { get; set; } = default!;
    [field: SerializeField]
    public Camera CombatCamera { get; private set; } = default!;
    public CombatMap CombatMap { get; private set; } = default!;

    public CombatMapController() { Shared = this; }

    private Party? _originalParty;
    private Party OriginalParty => _originalParty ?? throw new Exception($"Party was not initialized");
    private string? _originalSetup;
    private string OriginalSetup => _originalSetup ?? throw new Exception("Setup was not initialized");
    private string _onWinScript = string.Empty;
    private string _onGiveUpScript = string.Empty;

    public void Initialize(string setup, string onWinScript, string onGiveUpScript)
    {
        MusicPlayerController.Shared.Play(Music.Combat);
        CombatModeController.Shared.Initialize();
        _onGiveUpScript = onGiveUpScript;
        _onWinScript = onWinScript;
        CrawlingModeController.Shared.gameObject.SetActive(false);
        CrawlingViewPortController.Shared.gameObject.SetActive(false);
        _originalSetup = setup;
        _originalParty = CrawlingModeController.Shared.Party.Copy();
        StartCombat(_originalParty, _originalSetup);
    }
    public void TryAgain() => StartCombat(OriginalParty, OriginalSetup);
    public void StartCombat(Party party, string setup)
    {
        MusicPlayerController.Shared.Play(Music.Combat);
        CrawlingModeController.Shared.Party.ApplyValues(party);
        CombatMap map = CombatMapExtensions.ParseMap(setup, PCMapping, EnemyMapping);
        RemoveNobody(map);
        BuildMap(map);
        map.OnCharacterChange += CrawlingModeController.Shared.Party.UpdateCharacter;
        map.OnMoveAction += HandleMove;
        map.OnEnemyRemoved += RemoveEnemy;
        gameObject.SetActive(true);
        DisableAllCombatControllers([StartPhaseController.Shared]);
        StartPhaseController.Shared.Initialize();
    }

    private void RemoveNobody(CombatMap map)
    {
        var toRemove = map.PlayerCharacters.Where(kvp => kvp.Value.Card == Characters.NoBody).Select(kvp => kvp.Key).ToArray();
        foreach (Position p in toRemove)
        {
            map.PlayerCharacters.Remove(p);
        }
    }

    private void RemoveEnemy(Position position) => CharacterMap.SetTile(position.ToVector3Int(), null);

    private void HandleMove(MoveActionEvent @event)
    {
        MessageRenderer.Shared.AddMessage(new Message($"{@event.Moving.Card.Name} moves {@event.Path.Count()} spaces."));
        StartCoroutine(AnimateMove(@event.Move.Start, @event.Path));
    }

    private IEnumerator AnimateMove(Position start, IEnumerable<Position> positions)
    {
        TileBase tile = CharacterMap.GetTile(start.ToVector3Int());
        TileBase? toSet = null;
        Position last = start;
        foreach (var position in positions)
        {
            CharacterMap.SetTile(last.ToVector3Int(), toSet);
            toSet = CharacterMap.GetTile(position.ToVector3Int());
            CharacterMap.SetTile(position.ToVector3Int(), tile);
            SFXController.Shared.PlaySound(Sound.Footstep);
            yield return new WaitForSeconds(MoveAnimationSpeed);
            last = position;
        }
    }

    public void BuildMap(CombatMap toBuild)
    {
        CombatMap = toBuild;
        Grid.ClearAllTiles();
        WallMap.ClearAllTiles();
        TileMap.ClearAllTiles();
        MapInfo.ClearAllTiles();
        CharacterMap.ClearAllTiles();

        TileMap.SetTiles(toBuild.Tiles, IconDatabase.Floor);
        WallMap.SetTiles(Grow(toBuild.Tiles), IconDatabase.Wall);
        Grid.SetTiles(toBuild.Tiles, IconDatabase.Outline);
        CharacterMap.SetTiles(toBuild.PlayerCharacters.Keys, p => IconDatabase.GetTile(toBuild.PlayerCharacters[p]));
        CharacterMap.SetTiles(toBuild.Enemies.Keys, p => IconDatabase.GetTile(toBuild.Enemies[p]));
    }

    private static HashSet<Position> Grow(HashSet<Position> toGrow)
    {
        HashSet<Position> grown = [];
        foreach (Position p in toGrow) { grown.UnionWith(Grow(p)); }
        return grown;
    }

    private static IEnumerable<Position> Grow(Position p)
    {
        return [
            new Position(p.X, p.Y - 1),
            new Position(p.X, p.Y + 1),
            new Position(p.X - 1, p.Y),
            new Position(p.X + 1, p.Y),
            new Position(p.X - 1, p.Y - 1),
            new Position(p.X - 1, p.Y + 1),
            new Position(p.X + 1, p.Y - 1),
            new Position(p.X + 1, p.Y + 1),
        ];
    }
    public void SelectTiles(params Position[] position)
    {
        CursorMap.ClearAllTiles();
        CursorMap.SetTiles(position, IconDatabase.Selected);
    }

    public void HighlightTiles(IEnumerable<Position> positions, TileBase? tile = null, bool clear = true)
    {
        if (tile == null) { tile = IconDatabase.Green; }
        if (clear) { MapInfo.ClearAllTiles(); }
        MapInfo.SetTiles(positions, tile);
    }

    private Coroutine? _cameraCoroutine;
    internal void PanTo(PlayerCharacter character, float duration = 0.3f) => PanTo(CombatMap.GetPosition(character.Card), duration);
    public void PanTo(Position position, float duration = 0.3f)
    {
        Vector3 end = CombatCamera.transform.localPosition;
        end.x = position.ToVector3Int().x;
        end.y = position.ToVector3Int().y;
        if (_cameraCoroutine is not null) { StopCoroutine(_cameraCoroutine); };
        _cameraCoroutine = StartCoroutine(CombatCamera.transform.PanTo(end, duration));
    }

    internal void PanToward(PlayerCharacter character, float distance = 2) => PanToward(CombatMap.GetPosition(character.Card), distance);

    internal void PanToward(Position cursorPosition, float distance = 2)
    {
        Vector2 target = cursorPosition.ToVector2();
        Vector2 current = CombatCamera.transform.localPosition;
        // Do not pan if we are close enough
        if (Vector2.Distance(target, current) < distance) { return; }
        Vector3 end = Vector2.Lerp(current, target, 0.5f);
        end.z = CombatCamera.transform.localPosition.z;
        if (_cameraCoroutine is not null) { StopCoroutine(_cameraCoroutine); };
        _cameraCoroutine = StartCoroutine(CombatCamera.transform.PanTo(end));
    }

    internal void SelectCharacter(PlayerCharacter character)
    {
        Position position = CombatMap.GetPosition(character.Card);
        SelectTiles(position);
        PanTo(position);
    }

    public void StartSpendActionPoints(PlayerCharacter selected)
    {
        SpendActionPointsModeController.Shared.Initialize(selected);
        CharacterSelectionModeController.Shared.gameObject.SetActive(false);
        SpendActionPointsModeController.Shared.gameObject.SetActive(true);
    }

    internal static PlayerCharacter? PCMapping(char ch) => ch switch
    {
        '1' => CrawlingModeController.Shared.Party[0],
        '2' => CrawlingModeController.Shared.Party[1],
        '3' => CrawlingModeController.Shared.Party[2],
        '4' => CrawlingModeController.Shared.Party[3],
        _ => null,
    };
    public Difficulty Difficulty = Difficulty.Easy;
    public void SetDifficulty(int ix)
    {
        Difficulty = ix switch
        {
            0 => Difficulty.Easy,
            1 => Difficulty.Normal,
            2 => Difficulty.Hard,
            _ => throw new Exception($"Unknown difficulty {ix}"),
        };
    }
    internal Enemy? EnemyMapping(char ch)
    {
        IEnemyCards enemies = Difficulty switch
        {
            Difficulty.Easy => EasyEnemies.Enemies,
            Difficulty.Normal => NormalEnemies.Enemies,
            Difficulty.Hard => HardEnemies.Enemies,
            _ => throw new Exception($"Unknown difficulty: {Difficulty}"),
        };
        return ch switch
        {
            'P' => new Enemy() { Card = enemies.WeakSkeletonCard },
            'B' => new Enemy() { Card = enemies.MouthBreather },
            'H' => new Enemy() { Card = enemies.HangryEmployee },
            'D' => new Enemy() { Card = enemies.EmployeeCard },
            'S' => new Enemy() { Card = enemies.SkeletonCard },
            'E' => new Enemy() { Card = enemies.EyeKeyUh },
            'M' => new Enemy() { Card = enemies.Meatball },
            'W' => new Enemy() { Card = enemies.Wardrobe },
            'Y' => new Enemy() { Card = enemies.CosmicBed },
            'K' => new Enemy() { Card = enemies.Karen },
            'C' => new Enemy() { Card = enemies.Chad },
            'F' => new Enemy() { Card = enemies.BloodShot },
            _ => null,
        };
    }

    internal void DisableAllCombatControllers(IEnumerable<MonoBehaviour> skip)
    {
        HashSet<MonoBehaviour> keepActive = [.. skip];
        IEnumerable<MonoBehaviour> objects =
        [
            SpendActionPointsModeController.Shared,
            CharacterSelectionModeController.Shared,
            CharacterActionMenuController.Shared,
            CharacterMoveController.Shared,
            CharacterAttackController.Shared,
            EnemyTurnController.Shared,
            StartPhaseController.Shared,
            PartySlainController.Shared,
            WinCombatMenuController.Shared,
        ];
        foreach (MonoBehaviour obj in objects)
        {
            if (keepActive.Contains(obj)) { continue; }
            obj.gameObject.SetActive(false);
        }
    }

    internal void EndCombat()
    {
        DisableAllCombatControllers([]);
        gameObject.SetActive(false);
        CombatModeController.Shared.gameObject.SetActive(false);
        CrawlingModeController.Shared.Initialize(_onWinScript);
        CrawlingModeController.Shared.Party.ClearActions();
    }

    internal void GiveUpCombat()
    {
        CrawlingModeController.Shared.Party.ClearActions();
        DisableAllCombatControllers([]);
        gameObject.SetActive(false);
        CombatModeController.Shared.gameObject.SetActive(false);
        CrawlingModeController.Shared.Initialize(_onGiveUpScript);
        CrawlingModeController.Shared.Party.ClearActions();
    }


    internal void ExitCombat()
    {
        DisableAllCombatControllers([]);
        gameObject.SetActive(false);
        CombatModeController.Shared.gameObject.SetActive(false);
        CrawlingModeController.Shared.Initialize();
        CrawlingModeController.Shared.Party.ClearActions();
    }
}

public enum Difficulty
{
    Easy,
    Normal,
    Hard
}

public static class PositionExtensions
{
    public static readonly YieldInstruction WaitForEndOfFrame = new WaitForEndOfFrame();
    public static void SetTiles(this Tilemap map, IEnumerable<Position> positions, TileBase tile) => map.SetTiles([.. positions.Select(p => p.ToVector3Int())], [.. positions.Select(_ => tile)]);
    public static void SetTiles(this Tilemap map, IEnumerable<Position> positions, Func<Position, TileBase> lookup) => map.SetTiles([.. positions.Select(p => p.ToVector3Int())], [.. positions.Select(lookup)]);
    public static void SetTiles(this Tilemap map, IEnumerable<TileData> data) => map.SetTiles([.. data.Select(t => t.Position.ToVector3Int())], [.. data.Select(t => t.Tile)]);
    public static Vector3Int ToVector3Int(this Position position) => new(position.X, -position.Y, 0);
    public static Vector2 ToVector2(this Position position) => new(position.X, -position.Y);
    public static IEnumerator PanTo(this Transform transform, Vector3 end, float duration = 0.3f)
    {
        Vector3 start = transform.localPosition;
        float elapsedTime = 0;
        while (Percent() < 1)
        {
            transform.localPosition = Vector3.Lerp(start, end, Percent());
            elapsedTime += Time.deltaTime;
            yield return WaitForEndOfFrame;
        }
        transform.localPosition = Vector3.Lerp(start, end, 1);
        float Percent() => elapsedTime / duration;
    }


}

public record class TileData(Position Position, TileBase Tile);