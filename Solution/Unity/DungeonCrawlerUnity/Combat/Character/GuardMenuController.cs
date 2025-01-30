using CaptainCoder.Dungeoneering.Game.Unity;
using CaptainCoder.Dungeoneering.Unity;

namespace CaptainCoder.DungeonCrawler.Combat.Unity;

public class GuardMenuController : AbstractMenuController<GuardActions>
{
    public static GuardMenuController Shared { get; private set; } = default!;
    public GuardMenuController() { Shared = this; }
    private CanGuard[] _guards = default!;
    private Position _enemyPosition = default!;
    private CombatMap Map => CombatMapController.Shared.CombatMap;

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public void Initialize(CanGuard[] guards, Position enemyPosition)
    {
        MessageRenderer.Shared.AddMessage($"Who will guard?");
        _guards = guards;
        _enemyPosition = enemyPosition;
        gameObject.SetActive(true);
        Select(0);
    }

    protected override void SelectOption(GuardActions action)
    {
        Action toInvoke = action switch
        {
            GuardActions.Cancel => () => EnemyTurnController.Shared.Resume(new Continue()),
            var member => () => CheckMember(member.ToCharacter()),
        };
        toInvoke.Invoke();
    }

    protected override void OnSelectionChange(GuardActions selected)
    {
        base.OnSelectionChange(selected);
        if (selected is GuardActions.Cancel) { return; }
        PlayerCharacter character = selected.ToCharacter();
        CombatMapController.Shared.SelectCharacter(character);
        CombatMapController.Shared.PanTo(character);
    }

    private void PerformGuard(CanGuard guard)
    {
        MessageRenderer.Shared.AddMessage($"{guard.Character.Card.Name} <color=red>Guards</color>!");

        // Character returns to normal state
        PlayerCharacter updated = guard.Character with { State = CharacterState.Normal };
        Map.UpdateCharacter(updated);

        // Perform the attack
        PerformAttackController.Shared.Initialize(updated.Card, _enemyPosition, OnFinish, OnCancel);
        void OnCancel() => Initialize(_guards, _enemyPosition);
        void OnFinish(AttackResultEvent result)
        {

            // If the target is killed, resume enemy turn
            if (result.IsTargetKilledEvent())
            {
                EnemyTurnController.Shared.Resume(new EnemyIsDead());
                return;
            }

            // Remove this character from possible guards
            _guards = [.. _guards.Where(g => g != guard)];

            // If there are no more possible guards, resume
            if (_guards.Length == 0)
            {
                EnemyTurnController.Shared.Resume(new Continue());
                return;
            }

            // Otherwise, display who can guard
            foreach (CanGuard g in _guards)
            {
                MessageRenderer.Shared.AddMessage($"{guard.Character.Card.Name} can <color=red>Guard</color> the enemy.");
            }
        }
    }

    private void CheckMember(PlayerCharacter character)
    {
        if (character.State is not CharacterState.Guard)
        {
            MessageRenderer.Shared.AddMessage($"{character.Card.Name} is not guarding.");
            return;
        }
        foreach (CanGuard guard in _guards.Where(g => g.Character.Card.Name == character.Card.Name))
        {
            PerformGuard(guard);
            return;
        }
        MessageRenderer.Shared.AddMessage($"{character.Card.Name} cannot guard this enemy.");
    }

}

public static class GuardActionsExtensions
{
    // This is a horrible piece of code... my goodness why am I here?
    public static PlayerCharacter ToCharacter(this GuardActions action) => action switch
    {
        GuardActions.Zooperdan => CrawlingModeController.Shared.Party.TopLeft,
        GuardActions.Kordanor => CrawlingModeController.Shared.Party.TopRight,
        GuardActions.Ronadrok => CrawlingModeController.Shared.Party.BottomLeft,
        GuardActions.Nadrepooz => CrawlingModeController.Shared.Party.BottomRight,
        _ => throw new NotImplementedException($"Invalid action {action}."),
    };
}

public enum GuardActions
{
    Zooperdan,
    Kordanor,
    Nadrepooz,
    Ronadrok,
    Cancel,
}