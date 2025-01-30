using CaptainCoder.Dungeoneering.Game;
using CaptainCoder.Dungeoneering.Game.Unity;
using CaptainCoder.Dungeoneering.Player.Unity;

using UnityEngine;
namespace CaptainCoder.DungeonCrawler.Combat.Unity;
public class CharacterAttackController : MonoBehaviour
{
    public static CharacterAttackController Shared { get; private set; } = default!;
    public CharacterAttackController() { Shared = this; }
    private CharacterCard? _card;
    private CharacterCard Card => _card ?? throw new Exception($"Card not initialized.");
    private CombatMap Map => CombatMapController.Shared.CombatMap;

    private Position[] _validPositions = [];
    private int _selectedIx = 0;

    public void OnEnable()
    {
        CharacterActionMenuController.Shared.gameObject.SetActive(false);
    }

    public void Update() => PlayerInputHandler.Shared.InputMapping.OnMenuAction(HandleInput);

    private void HandleInput(MenuControl control)
    {
        Action action = control switch
        {
            MenuControl.Down or MenuControl.Right => () => Next(1),
            MenuControl.Up or MenuControl.Left => () => Next(-1),
            MenuControl.Cancel => ReturnToMenuSelect,
            MenuControl.Select => () => PerformAttack(_validPositions[_selectedIx]),
            _ => () => Debug.Log($"Not implemented: {control}"),
        };
        action.Invoke();
    }

    private void Next(int delta)
    {
        _selectedIx += delta;
        if (_selectedIx < 0) { _selectedIx = _validPositions.Length - 1; }
        else if (_selectedIx >= _validPositions.Length) { _selectedIx = 0; }
        Select();
    }

    private void PerformAttack(Position target)
    {
        PerformAttackController.Shared.Initialize(Card, target, OnFinish, OnCancel);
        void OnFinish(AttackResultEvent _)
        {
            if (Map.Enemies.Count == 0)
            {
                WinCombatMenuController.Shared.Initialize();
            }
            else
            {
                ReturnToMenuSelect();
            }
        }

        void OnCancel() => ReturnToMenuSelect();

    }

    public void Initialize(CharacterCard card)
    {
        _card = card;
        if (Map.GetCharacter(card).AttackPoints <= 0)
        {
            MessageRenderer.Shared.AddMessage(new Message($"{card.Name} has no attack points."));
            ReturnToMenuSelect();
            return;
        }
        Position attackerPosition = Map.GetPosition(card);

        _validPositions = [.. Map.GetValidAttackTargets(attackerPosition)];
        if (_validPositions.Length == 0)
        {
            MessageRenderer.Shared.AddMessage(new Message("No valid targets"));
            ReturnToMenuSelect();
            return;
        }

        string keys = PlayerInputHandler.Shared.GetKeys(MenuControl.Cancel);
        MessageRenderer.Shared.AddMessage($"Select a target. Press {keys} to cancel.");
        _selectedIx = 0;
        CombatMapController.Shared.HighlightTiles(_validPositions);
        Select();
        gameObject.SetActive(true);
    }

    private void Select()
    {
        Position target = _validPositions[_selectedIx];
        CombatMapController.Shared.SelectTiles(target);
        CombatMapController.Shared.PanTo(target);
        Enemy e = Map.Enemies[target];
        MessageRenderer.Shared.AddMessage($"{e.Card.Name}: Health: {e.Health} | Armor: {e.Card.Armor}");
    }

    private void ReturnToMenuSelect()
    {
        gameObject.SetActive(false);
        CharacterActionMenuController.Shared.Initialize(Card);
        CombatMapController.Shared.SelectTiles([]);
        CombatMapController.Shared.HighlightTiles([]);
        _card = null;
        _validPositions = [];
    }
}