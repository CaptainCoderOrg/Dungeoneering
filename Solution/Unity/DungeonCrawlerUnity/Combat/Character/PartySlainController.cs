using CaptainCoder.DungeonCrawler.Unity;

namespace CaptainCoder.DungeonCrawler.Combat.Unity;

public class PartySlainController : AbstractMenuController<PartySlainAction>
{
    public static PartySlainController Shared { get; private set; } = default!;
    public PartySlainController() { Shared = this; }

    public override void OnEnable()
    {
        base.OnEnable();
        CombatMapController.Shared.DisableAllCombatControllers([this]);
        MusicPlayerController.Shared.Play(Music.GameOver, false);
    }

    public override void OnDisable()
    {
        base.OnDisable();
    }

    public void Initialize()
    {
        gameObject.SetActive(true);
        Select(0);
    }

    protected override void SelectOption(PartySlainAction action)
    {
        Action invoke = action switch
        {
            PartySlainAction.TryAgain => () => CombatMapController.Shared.TryAgain(),
            PartySlainAction.GiveUp => () => CombatMapController.Shared.GiveUpCombat(),
            _ => throw new NotImplementedException($"Action not implemented {action}"),
        };
        invoke.Invoke();
    }
}

public enum PartySlainAction
{
    TryAgain,
    GiveUp,
}