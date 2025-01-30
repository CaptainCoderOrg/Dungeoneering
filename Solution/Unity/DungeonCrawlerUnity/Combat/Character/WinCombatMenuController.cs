using CaptainCoder.DungeonCrawler.Unity;

namespace CaptainCoder.DungeonCrawler.Combat.Unity;

public class WinCombatMenuController : AbstractMenuController<WinCombatActions>
{
    public static WinCombatMenuController Shared { get; private set; } = default!;
    public WinCombatMenuController() { Shared = this; }
    // private CharacterCard _card = default!;
    // private CombatMap CombatMap => CombatMapController.Shared.CombatMap;

    public override void OnEnable()
    {
        base.OnEnable();
        CombatMapController.Shared.DisableAllCombatControllers([this]);
        MusicPlayerController.Shared.Play(Music.Victory, false);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        CombatHelpPanel.Shared.gameObject.SetActive(false);
    }

    protected override void SelectOption(WinCombatActions action)
    {
        Action toInvoke = action switch
        {
            WinCombatActions.Continue => CombatMapController.Shared.EndCombat,
            WinCombatActions.TryAgain => CombatMapController.Shared.TryAgain,
            _ => throw new NotImplementedException($"Unknown action {action}"),
        };
        toInvoke.Invoke();
    }

    internal void Initialize()
    {
        gameObject.SetActive(true);
        Select(0);
    }
}

public enum WinCombatActions
{
    Continue,
    TryAgain
}