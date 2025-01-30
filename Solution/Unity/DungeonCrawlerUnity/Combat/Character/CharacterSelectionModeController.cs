using CaptainCoder.DungeonCrawler.Unity;
using CaptainCoder.Dungeoneering.Game.Unity;
using CaptainCoder.Dungeoneering.Player.Unity;
using CaptainCoder.Dungeoneering.Unity;

using UnityEngine;

namespace CaptainCoder.DungeonCrawler.Combat.Unity;

public class CharacterSelectionModeController : MonoBehaviour
{
    public static CharacterSelectionModeController Shared { get; private set; } = default!;
    private CharacterSelectionMode _characterSelectionMode = default!;
    public PlayerInputMapping PlayerControls = default!;
    public CharacterSelectionModeController() { Shared = this; }

    public void OnEnable()
    {
        StartPhaseController.Shared.gameObject.SetActive(false);
        SpendActionPointsModeController.Shared.gameObject.SetActive(false);
    }

    public void Update()
    {
        foreach (MenuActionMapping mapping in PlayerControls.MenuActionMappings)
        {
            if (Input.GetKeyDown(mapping.Key))
            {
                HandleUserInput(mapping.Action);
            }
        }
    }

    public void Initialize(PlayerCharacter? selected = null)
    {
        _characterSelectionMode = new(CrawlingModeController.Shared.Party, selected);
        if (!_characterSelectionMode.HasSelection)
        {
            foreach (var card in Overlay.Shared.Cards)
            {
                card.IsSelected = false;
                card.IsFinished = true;
            }
            EnemyTurnController.Shared.Initialize();
            return;
        }
        (int charIx, PlayerCharacter character) = _characterSelectionMode.FirstSelected(selected);
        SelectCard(charIx, character);
        _characterSelectionMode.OnSelectionChange += SelectCard;
        _characterSelectionMode.OnSelected += (ix, selected) => CombatMapController.Shared.StartSpendActionPoints(selected);
        MessageRenderer.Shared.AddMessage($"Select a Character");
        gameObject.SetActive(true);
    }
    private void SelectCard(int _, PlayerCharacter character)
    {
        for (int jx = 0; jx < Overlay.Shared.Cards.Length; jx++)
        {
            var card = Overlay.Shared.Cards[jx];
            card.IsSelected = _characterSelectionMode.IsSelected(jx);
            card.IsFinished = _characterSelectionMode.IsFinished(jx);
        }
        CombatMapController.Shared.SelectCharacter(character);
    }
    public void HandleUserInput(MenuControl input)
    {
        Action action = input switch
        {
            MenuControl.Right => () => _characterSelectionMode.HandleInput(CharacterSelectionControl.Next),
            MenuControl.Down => () =>
            {
                _characterSelectionMode.HandleInput(CharacterSelectionControl.Next);
                _characterSelectionMode.HandleInput(CharacterSelectionControl.Next);
            }
            ,
            MenuControl.Up => () =>
            {
                _characterSelectionMode.HandleInput(CharacterSelectionControl.Previous);
                _characterSelectionMode.HandleInput(CharacterSelectionControl.Previous);
            }
            ,
            MenuControl.Left => () => _characterSelectionMode.HandleInput(CharacterSelectionControl.Previous),
            MenuControl.Select => () => _characterSelectionMode.HandleInput(CharacterSelectionControl.Select),
            _ => () => { }
        };
        action.Invoke();
    }
}