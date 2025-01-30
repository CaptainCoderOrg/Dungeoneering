using CaptainCoder.DungeonCrawler.Unity;
using CaptainCoder.Dungeoneering.Game.Unity;
using CaptainCoder.Dungeoneering.Player.Unity;

using UnityEngine;
namespace CaptainCoder.DungeonCrawler.Combat.Unity;
public class PerformAttackController : MonoBehaviour
{
    public static PerformAttackController Shared { get; private set; } = default!;
    public PerformAttackController() { Shared = this; }
    public DamageMessage DamageMessageTemplate = default!;
    public Transform DamageMessageParent = default!;
    private CombatMap Map => CombatMapController.Shared.CombatMap;
    private int _exert = 0;
    private Action<AttackResultEvent> _onFinish = default!;
    private Action _onCancel = default!;
    private CharacterCard _card = default!;
    private Position _target = default!;
    public float SkillUpChance = 0.5f;
    public AttackRollRenderer AttackRollRenderer = default!;
    public void OnEnable()
    {
        CharacterAttackController.Shared.gameObject.SetActive(false);
        GuardMenuController.Shared.gameObject.SetActive(false);
        AttackRollRenderer.gameObject.SetActive(true);
    }
    public void OnDisable()
    {
        AttackRollRenderer.gameObject.SetActive(false);
    }

    public void Update() => PlayerInputHandler.Shared.InputMapping.OnMenuAction(HandleInput);
    public void Initialize(CharacterCard card, Position target, Action<AttackResultEvent> onFinish, Action onCancel)
    {
        _target = target;
        _card = card;
        _onFinish = onFinish;
        _onCancel = onCancel;
        _exert = 0;

        var keys = string.Join(" or ", [PlayerInputHandler.Shared.GetKeys(MenuControl.Up), PlayerInputHandler.Shared.GetKeys(MenuControl.Down)]);
        MessageRenderer.Shared.AddMessage($"Increase/Decrease Energy: {keys}");
        MessageRenderer.Shared.AddMessage($"Press {PlayerInputHandler.Shared.GetKeys(MenuControl.Cancel)} to cancel.");
        MessageRenderer.Shared.AddMessage($"Press {PlayerInputHandler.Shared.GetKeys(MenuControl.Select)} to confirm.");
        PlayerCharacter pc = Map.GetCharacter(_card);
        AttackRollRenderer.Render(Map.Enemies[_target], _exert, pc.Energy(), GetAttackRoll());
        gameObject.SetActive(true);
    }

    private void HandleInput(MenuControl control)
    {
        Action action = control switch
        {
            MenuControl.Down => () => Exert(-1),
            MenuControl.Up => () => Exert(1),
            MenuControl.Cancel => Cancel,
            MenuControl.Select => PerformAttack,
            _ => () => Debug.Log($"Not implemented: {control}"),
        };
        action.Invoke();
    }

    private IAttackRoll GetAttackRoll()
    {
        PlayerCharacter pc = Map.GetCharacter(_card);
        SimpleAttack roll = new(pc.Weapon.AttackRoll.Min + (_exert / 2), pc.Weapon.AttackRoll.Max + _exert);
        return roll;
    }

    private void PerformAttack()
    {
        EnemyCard enemyCard = Map.Enemies[_target].Card;
        PlayerCharacter pc = Map.GetCharacter(_card);
        _ = Map.UpdateCharacter(pc with { Exertion = pc.Exertion + _exert });
        AttackResultEvent result = Map.DoAttack(_card, GetAttackRoll(), _target);
        pc = Map.GetCharacter(_card);
        if (!result.IsTargetKilledEvent())
        {
            int damage = result.TotalDamage();
            if (damage > 0) { SFXController.Shared.PlaySound(Sound.Hit); }
            else { SFXController.Shared.PlaySound(Sound.Miss); }
            DamageMessage newMessage = Instantiate(DamageMessageTemplate, DamageMessageParent);
            newMessage.Render(damage, _target);
        }
        if (result.IsTargetKilledEvent())
        {
            SFXController.Shared.PlaySound(Sound.Die);
            CheckForSkillUp(pc, enemyCard);
        }
        _onFinish.Invoke(result);
        gameObject.SetActive(false);
    }

    private void CheckForSkillUp(PlayerCharacter playerCharacter, EnemyCard enemyCard)
    {
        float chance = UnityEngine.Random.Range(0, 1);
        if (chance < SkillUpChance)
        {
            for (int ix = 0; ix < 4; ix++)
            {
                int choice = UnityEngine.Random.Range(0, 4);
                Func<bool> action = choice switch
                {
                    0 => () => TryIncreaseArmor(playerCharacter, enemyCard.Level, out _),
                    1 => () => TryIncreaseEnergy(playerCharacter, enemyCard.Level, out _),
                    2 => () => TryIncreaseHealth(playerCharacter, enemyCard.Level, out _),
                    3 => () => TryIncreaseSpeed(playerCharacter, enemyCard.Level, out _),
                    _ => () => { Debug.Log($"Unknown skill increase!"); return false; }
                };
                if (action.Invoke()) { break; }
            }
            // Armor
            // Energy
            // Health
            // Speed
        }
    }

    private bool TryIncreaseArmor(PlayerCharacter playerCharacter, int level, out PlayerCharacter updated)
    {
        updated = playerCharacter;
        int max = (int)Math.Floor(level * 1.5);
        int current = playerCharacter.Card.BaseArmor;
        if (current >= max) { return false; }
        CharacterCard newCard = playerCharacter.Card with { BaseArmor = current + 1 };
        updated = playerCharacter with { Card = newCard };
        Map.UpdateCharacter(updated);
        MessageRenderer.Shared.AddMessage($"<color=yellow>{playerCharacter.Card.Name}</color> gained a skill point! <color=yellow>Armor</color> increased by 1.");
        SFXController.Shared.PlaySound(Sound.LevelUp);
        return true;
    }

    private bool TryIncreaseEnergy(PlayerCharacter playerCharacter, int level, out PlayerCharacter updated)
    {
        updated = playerCharacter;
        int max = level * 2;
        int current = playerCharacter.Card.BaseEnergy;
        if (current >= max) { return false; }
        CharacterCard newCard = playerCharacter.Card with { BaseEnergy = current + 1 };
        updated = playerCharacter with { Card = newCard };
        Map.UpdateCharacter(updated);
        MessageRenderer.Shared.AddMessage($"<color=yellow>{playerCharacter.Card.Name}</color> gained a skill point! <color=yellow>Max Energy</color> increased by 1.");
        SFXController.Shared.PlaySound(Sound.LevelUp);
        return true;
    }

    private bool TryIncreaseHealth(PlayerCharacter playerCharacter, int level, out PlayerCharacter updated)
    {
        updated = playerCharacter;
        int max = level * 10;
        int current = playerCharacter.Card.BaseHealth;
        if (current >= max) { return false; }
        CharacterCard newCard = playerCharacter.Card with { BaseHealth = current + 1 };
        updated = playerCharacter with { Card = newCard };
        Map.UpdateCharacter(updated);
        MessageRenderer.Shared.AddMessage($"<color=yellow>{playerCharacter.Card.Name}</color> gained a skill point! <color=yellow>Max Health</color> increased by 1.");
        SFXController.Shared.PlaySound(Sound.LevelUp);
        return true;
    }

    private bool TryIncreaseSpeed(PlayerCharacter playerCharacter, int level, out PlayerCharacter updated)
    {
        updated = playerCharacter;
        int max = (int)Math.Floor(level * 1.5);
        int current = playerCharacter.Card.BaseSpeed;
        if (current >= max) { return false; }
        CharacterCard newCard = playerCharacter.Card with { BaseSpeed = current + 1 };
        updated = playerCharacter with { Card = newCard };
        Map.UpdateCharacter(updated);
        MessageRenderer.Shared.AddMessage($"<color=yellow>{playerCharacter.Card.Name}</color> gained a skill point! <color=yellow>Speed</color> increased by 1.");
        SFXController.Shared.PlaySound(Sound.LevelUp);
        return true;
    }

    private void Exert(int delta)
    {
        PlayerCharacter pc = Map.GetCharacter(_card);
        _exert = Math.Clamp(_exert + delta, 0, pc.Energy());
        AttackRollRenderer.Render(Map.Enemies[_target], _exert, pc.Energy(), GetAttackRoll());
    }

    private void Cancel()
    {
        gameObject.SetActive(false);
        _onCancel.Invoke();
    }

}