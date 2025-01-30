using System.Collections;

using CaptainCoder.DungeonCrawler.Unity;
using CaptainCoder.Dungeoneering.Game.Unity;
using CaptainCoder.Dungeoneering.Player.Unity;
using CaptainCoder.Dungeoneering.Unity;

using UnityEngine;
using UnityEngine.Tilemaps;

namespace CaptainCoder.DungeonCrawler.Combat.Unity;

public class EnemyTurnController : MonoBehaviour
{
    public static EnemyTurnController Shared { get; private set; } = default!;
    public EnemyTurnController() { Shared = this; }
    public DamageMessage DamageMessageTemplate = default!;
    public Transform DamageMessageParent = default!;
    private CombatMap Map => CombatMapController.Shared.CombatMap;
    private Queue<Position>? _enemyPositions;
    private Queue<Position> EnemyPositions => _enemyPositions ?? throw new Exception("Enemy positions is not initialized");
    private Coroutine? _enemyCoroutine;
    private Optional<(CanGuard[] options, Position enemyPosition)> _possibleGuards = new None<(CanGuard[], Position)>();
    private bool _isPaused = false;
    private ResumeInfo _resumeInfo = new Continue();
    public void Update()
    {
        PlayerInputHandler.Shared.InputMapping.OnMenuAction(HandleInput);
    }

    private void HandleInput(MenuControl control)
    {
        if (!_isPaused && _possibleGuards is Some<(CanGuard[] options, Position enemyPosition)> guards && control is MenuControl.Select or MenuControl.Cancel)
        {
            _isPaused = true;
            GuardMenuController.Shared.Initialize(guards.Value.options, guards.Value.enemyPosition);
        }
    }

    public void OnEnable()
    {
        CharacterSelectionModeController.Shared.gameObject.SetActive(false);
    }

    public void Initialize()
    {
        _enemyPositions = new Queue<Position>(Map.Enemies.Keys);
        gameObject.SetActive(true);
        DoNextMove();
    }

    private void DoNextMove()
    {
        if (!EnemyPositions.TryDequeue(out Position position))
        {
            EndEnemyTurn();
            return;
        }
        if (gameObject.activeInHierarchy)
        {
            _enemyCoroutine = StartCoroutine(PerformMove(position));
        }
    }

    public IEnumerator PerformMove(Position position)
    {
        _resumeInfo = new Continue();
        Enemy e = Map.Enemies[position];
        MessageRenderer.Shared.AddMessage($"{e.Card.Name} prepares for battle.");
        CombatMapController.Shared.SelectTiles(position);
        CombatMapController.Shared.PanTo(position);
        yield return new WaitForSeconds(CombatConstants.ShowEnemyInfoDuration);
        Position[] path = [.. Map.GetEnemyMove(position)];
        if (path.Count() > 0)
        {
            foreach (var pause in ShowMove(e, position, path)) { yield return pause; }
            position = path.Last();
        }
        // The enemy is dead
        if (_resumeInfo is EnemyIsDead) { goto cleanUp; }

        Position[] attacks = [.. Map.GetValidAttackTargets(position)];
        if (attacks.Count() > 0)
        {
            foreach (var pause in ShowAttack(e, position, attacks))
            {
                yield return pause;
            }
        }

    cleanUp:
        CombatMapController.Shared.HighlightTiles([]);
        CombatMapController.Shared.SelectTiles([]);
        DoNextMove();
    }

    public IEnumerable ShowAttack(Enemy e, Position start, Position[] options)
    {
        Position target = options.OrderBy(p => Map.PlayerCharacters[p].Health()).First();
        PlayerCharacter character = Map.PlayerCharacters[target];
        MessageRenderer.Shared.AddMessage($"{e.Card.Name} prepares to attack {character.Card.Name}.");
        CombatMapController.Shared.SelectTiles(target);

        CanGuard[] guards = [.. Map.CanGuard(start, [])];
        if (guards.Length > 0)
        {
            SFXController.Shared.PlaySound(Sound.Guard);
            string keys = PlayerInputHandler.Shared.GetKeys(MenuControl.Select);
            foreach (CanGuard guard in guards)
            {
                MessageRenderer.Shared.AddMessage($"{guard.Character.Card.Name} can <color=red>Guard</color> the enemy. Press {keys} to interrupt.");
            }
            _possibleGuards = new Some<(CanGuard[], Position)>((guards, start));
            yield return new WaitForSeconds(CombatConstants.GuardWaitDuration);
            yield return new WaitUntil(() => !_isPaused);
            if (_resumeInfo is EnemyIsDead) { yield break; }

        }
        else
        {
            yield return new WaitForSeconds(CombatConstants.ShowEnemyInfoDuration);
        }

        AttackResult result = e.Card.AttackRoll.GetRoll(IRandom.Default);
        AttackResultEvent attackEvent = Map.ApplyAttack(target, result);
        MessageRenderer.Shared.AddMessage(EventMessage(e, attackEvent));
        if (!attackEvent.IsTargetKilledEvent())
        {
            int damage = attackEvent.TotalDamage();
            if (damage > 0) { SFXController.Shared.PlaySound(Sound.Hit); }
            else { SFXController.Shared.PlaySound(Sound.Miss); }
            DamageMessage newMessage = Instantiate(DamageMessageTemplate, DamageMessageParent);
            newMessage.Render(attackEvent.TotalDamage(), target);
            yield return new WaitForSeconds(CombatConstants.ShortEnemyInfoDuration);
        }
        if (attackEvent.IsTargetKilledEvent())
        {
            SFXController.Shared.PlaySound(Sound.Die);
            CombatMapController.Shared.CharacterMap.SetTile(target.ToVector3Int(), CombatMapController.Shared.IconDatabase.Dead);
            if (CrawlingModeController.Shared.Party.IsDead)
            {
                MessageRenderer.Shared.AddMessage($"The party has fallen...");
                if (_enemyCoroutine is not null) { StopCoroutine(_enemyCoroutine); }
                Shared.gameObject.SetActive(false);
                PartySlainController.Shared.Initialize();
            }
        }
    }

    private string EventMessage(Enemy e, AttackResultEvent attackEvent) => attackEvent switch
    {
        AttackHitEvent hit => $"{hit.TargetName} takes {hit.Damage} damage.",
        ArmorAbsorbedHitEvent hit => $"{hit.TargetName}'s armor absorbs the blow.",
        TargetKilledEvent hit => $"{hit.TargetName} falls to the ground.",
        EmptyTarget => $"{e.Card.Name} misses!",
        LostGuardEvent hit => $"{hit.TargetName}'s guard was interrupted!",
        LostRestEvent hit => $"{hit.TargetName}'s rest was interrupted!",
        AttackResultEvents(var events) => string.Join("\n", events.Select(evt => EventMessage(e, evt))),
        _ => throw new NotImplementedException($"Unknown attackEvent {attackEvent}"),
    };

    public IEnumerable ShowMove(Enemy e, Position start, Position[] path)
    {
        Tilemap characterMap = CombatMapController.Shared.CharacterMap;
        MessageRenderer.Shared.AddMessage($"{e.Card.Name} moves {path.Count()} spaces.");
        CombatMapController.Shared.HighlightTiles(path, CombatMapController.Shared.IconDatabase.Yellow);
        CanGuard[] guards = [.. Map.CanGuard(start, path)];
        if (guards.Length > 0)
        {
            SFXController.Shared.PlaySound(Sound.Guard);
            string keys = PlayerInputHandler.Shared.GetKeys(MenuControl.Select);
            foreach (CanGuard guard in guards)
            {
                MessageRenderer.Shared.AddMessage($"{guard.Character.Card.Name} can <color=red>Guard</color> the enemy. Press {keys} to interrupt.");
            }
            _possibleGuards = new Some<(CanGuard[], Position)>((guards, start));
            yield return new WaitForSeconds(CombatConstants.GuardWaitDuration);
            yield return new WaitUntil(() => !_isPaused);


        }
        else
        {
            yield return new WaitForSeconds(CombatConstants.ShowEnemyInfoDuration);
        }

        TileBase tile = characterMap.GetTile(start.ToVector3Int());
        TileBase? toSet = null;
        Position last = start;
        CombatMapController.Shared.PanTo(path.Last(), CombatConstants.EnemyMoveDelay * path.Count());
        foreach (Position position in path)
        {
            characterMap.SetTile(last.ToVector3Int(), toSet);
            toSet = characterMap.GetTile(position.ToVector3Int());
            characterMap.SetTile(position.ToVector3Int(), tile);
            SFXController.Shared.PlaySound(Sound.Footstep);
            yield return new WaitForSeconds(CombatConstants.EnemyMoveDelay);
            yield return new WaitUntil(() => !_isPaused);
            last = position;
            if (_resumeInfo is EnemyIsDead)
            {
                _possibleGuards = new None<(CanGuard[], Position)>();
                characterMap.SetTile(last.ToVector3Int(), null);
                // The enemy is dead, end the turn
                yield break;
            }
        }
        Map.MoveEnemy(start, path.Last());
        _possibleGuards = new None<(CanGuard[], Position)>();
    }

    private void EndEnemyTurn()
    {
        StartPhaseController.Shared.Initialize();
    }

    public void Resume(ResumeInfo info)
    {
        _resumeInfo = info;
        _isPaused = false;
        GuardMenuController.Shared.gameObject.SetActive(false);
    }
}

public abstract record ResumeInfo;
public record Continue : ResumeInfo;
public record EnemyIsDead : ResumeInfo;

public abstract record Optional<T>;
public record Some<T>(T Value) : Optional<T>;
public record None<T> : Optional<T>;