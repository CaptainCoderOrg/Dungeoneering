using TMPro;

using UnityEngine;

namespace CaptainCoder.DungeonCrawler.Combat.Unity;

public class CombatHelpPanel : MonoBehaviour
{
    public static CombatHelpPanel Shared { get; private set; } = default!;
    [field: SerializeField]
    public TextMeshProUGUI TextElement { get; private set; } = default!;
    public Action<bool>? OnToggled;
    public string Text
    {
        set
        {
            TextElement.text = value;
            gameObject.SetActive(IsOn);
        }
    }
    private bool _isOn = true;
    public bool IsOn
    {
        get => _isOn;
        set
        {
            _isOn = value;
            OnToggled?.Invoke(value);
            gameObject.SetActive(_isOn);
        }
    }
    public void Toggle() => IsOn = !IsOn;
    public CombatHelpPanel()
    {
        Shared = this;
    }
}

public static class HelpDialogue
{
    public static string ToggleHelpInfo = "Toggles help information on and off";
    public static string CancelActionSpending = "Cancels the current actions and returns to character selection.";
    public static string CombatMove { get; } = "Spend movement points";
    public static string MoveExert { get; } = "Spend 1 energy to gain 1 movement point";
    public static string CombatAttack { get; } = "Spend an attack point to perform an attack";
    public static string CombatEndTurn { get; } = "End this character's turn. Any unused movement or attack points are lost.";

    public static string Rest { get; } = """
    Change to the <color=#006600>Rest</color> state.

    At the start of the next turn, this character's <color=#ad4815>energy</color> is fully restored. Taking damage causes the character to exit the <color=#006600>Rest</color> state.

    This replaces any previous state.
    """;
    public static string Move { get; } = """
    Gain movement points equal to this character's speed. 

    May be used multiple times.
    """;
    public static string Attack { get; } = """
    Allows this character to perform 1 attack during their turn.

    May be used multiple times.
    """;
    public static string GuardState { get; } = """
    Change to the <color=red>guard</color> state. 

    Once while guarding, this character may interrupt an enemy move or attack. Taking damage causes the character to exit the <color=red>guard</color> state.

    This replaces any previous state.
    """;
}

[RequireComponent(typeof(TextMeshProUGUI))]
public class HelpToggleLabel : MonoBehaviour
{
    public TextMeshProUGUI Text { get; private set; } = default!;
    public void Awake()
    {
        Text = GetComponent<TextMeshProUGUI>();
    }
    public void Start()
    {
        CombatHelpPanel.Shared.OnToggled += isOn => Text.text = $"Help: {(isOn ? "On" : "Off")}";
    }
}