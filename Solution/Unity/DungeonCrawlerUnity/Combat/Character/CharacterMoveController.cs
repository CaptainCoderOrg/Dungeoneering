using CaptainCoder.Dungeoneering.Game;
using CaptainCoder.Dungeoneering.Game.Unity;
using CaptainCoder.Dungeoneering.Player.Unity;

using UnityEngine;

namespace CaptainCoder.DungeonCrawler.Combat.Unity;
public class CharacterMoveController : MonoBehaviour
{
    public static CharacterMoveController Shared { get; private set; } = default!;
    public CharacterMoveController() { Shared = this; }
    private CharacterCard? _card = default!;
    private CharacterCard Card => _card ?? throw new Exception($"Card was not initialized");
    private Position _cursorPosition = default;
    private Position _startPosition = default!;
    private IEnumerable<Position> _previousPath = [];
    private CombatMap CombatMap => CombatMapController.Shared.CombatMap;

    public void OnEnable()
    {
        CharacterActionMenuController.Shared.gameObject.SetActive(false);
    }

    public void Update()
    {
        PlayerInputHandler.Shared.InputMapping.OnMenuAction(HandleInput);
    }

    private void HandleInput(MenuControl action)
    {
        Action toPerform = action switch
        {
            MenuControl.Up => () => MoveCursor(_cursorPosition with { Y = _cursorPosition.Y - 1 }),
            MenuControl.Down => () => MoveCursor(_cursorPosition with { Y = _cursorPosition.Y + 1 }),
            MenuControl.Left => () => MoveCursor(_cursorPosition with { X = _cursorPosition.X - 1 }),
            MenuControl.Right => () => MoveCursor(_cursorPosition with { X = _cursorPosition.X + 1 }),
            MenuControl.Select => () => TryMove(_cursorPosition),
            MenuControl.Cancel => ReturnToActionMenu,
            _ => () => { }
            ,
        };
        toPerform.Invoke();
    }

    public void TryMove(Position target)
    {
        MoveAction action = new(_startPosition, target);
        if (!CombatMap.IsValidMoveAction(action))
        {
            MessageRenderer.Shared.AddMessage(new Message($"{Card.Name} cannot move here."));
            return;
        }
        gameObject.SetActive(false);
        ConfirmDialogue.Shared.Initialize("Perform Move?", OnConfirm, OnCancel);
        void OnConfirm()
        {
            CombatMap.ApplyMoveAction(action);
            ReturnToActionMenu();
        }
        void OnCancel() { gameObject.SetActive(true); }
    }

    public void ReturnToActionMenu()
    {
        CombatMapController.Shared.HighlightTiles([]);
        CharacterActionMenuController.Shared.Initialize(Card);
        _card = null;
    }

    public void MoveCursor(Position newPosition)
    {
        _cursorPosition = newPosition;
        CombatMapController.Shared.SelectTiles(_cursorPosition);
        CombatMapController.Shared.PanToward(_cursorPosition);
        CombatMapController.Shared.HighlightTiles(_previousPath, CombatMapController.Shared.IconDatabase.Green, false);
        if (CombatMap.IsValidMoveAction(new MoveAction(_startPosition, newPosition)))
        {
            IEnumerable<Position> shortestPath = CombatMap.FindShortestPath(_startPosition, [_cursorPosition]);
            CombatMapController.Shared.HighlightTiles(shortestPath, CombatMapController.Shared.IconDatabase.Yellow, false);
            _previousPath = shortestPath;
        }
    }

    public void Initialize(CharacterCard card)
    {
        _card = card;
        _previousPath = [];
        PlayerCharacter character = CombatMap.GetCharacter(card);
        if (character.MovementPoints <= 0)
        {
            MessageRenderer.Shared.AddMessage(new Message($"{card.Name} has no movement points."));
            ReturnToActionMenu();
            return;
        }
        MessageRenderer.Shared.AddMessage(new Message($"{card.Name} has {character.MovementPoints} movement points."));
        string keys = string.Join(" or ", PlayerInputHandler.Shared.InputMapping.MenuActionMappings.Where(mapping => mapping.Action is MenuControl.Cancel).Select(m => m.Key.ToString()));
        MessageRenderer.Shared.AddMessage(new Message($"Press {keys} to cancel."));
        _startPosition = CombatMap.GetPosition(character.Card);
        _cursorPosition = _startPosition;
        HashSet<Position> validMoves = CombatMap.FindValidMoves(_startPosition, character.MovementPoints);
        CombatMapController.Shared.HighlightTiles(validMoves, CombatMapController.Shared.IconDatabase.Green);

        gameObject.SetActive(true);
    }
}