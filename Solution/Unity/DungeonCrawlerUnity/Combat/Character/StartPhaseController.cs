using System.Collections;

using CaptainCoder.DungeonCrawler.Unity;
using CaptainCoder.Dungeoneering.Game.Unity;

using UnityEngine;

namespace CaptainCoder.DungeonCrawler.Combat.Unity;

public class StartPhaseController : MonoBehaviour
{
    public static StartPhaseController Shared { get; private set; } = default!;
    public StartPhaseController() { Shared = this; }
    private CombatMap Map => CombatMapController.Shared.CombatMap;
    public void OnEnable()
    {
        EnemyTurnController.Shared.gameObject.SetActive(false);
    }

    public void Initialize()
    {
        if (Map.Enemies.Count == 0)
        {
            WinCombatMenuController.Shared.Initialize();
        }
        else
        {
            gameObject.SetActive(true);
            MessageRenderer.Shared.AddMessage($"The party prepares for battle.");
            Queue<Position> pcs = new(Map.PlayerCharacters.Keys);
            DequeueEvent(pcs);
        }
    }

    private void DequeueEvent(Queue<Position> events)
    {
        if (events.TryDequeue(out Position position))
        {
            StartCoroutine(RefreshPlayer(position, events));
        }
        else
        {
            BeginPlayerTurn();
        }
    }

    private IEnumerator RefreshPlayer(Position position, Queue<Position> events)
    {
        PlayerCharacter character = Map.PlayerCharacters[position];
        CombatMapController.Shared.SelectTiles([position]);
        CombatMapController.Shared.PanTo(position);
        MessageRenderer.Shared.AddMessage($"{character.Card.Name} gains 2 action points.");
        character = character with { ActionPoints = 2 };
        if (character.State is CharacterState.Rest)
        {
            MessageRenderer.Shared.AddMessage($"{character.Card.Name} rests. Energy restored!");
            character = character with { Exertion = 0 };
            SFXController.Shared.PlaySound(Sound.Rest);
        }
        if (character.State is CharacterState.Guard)
        {
            MessageRenderer.Shared.AddMessage($"{character.Card.Name} is no longer guarding.");
        }
        _ = Map.UpdateCharacter(character with { State = CharacterState.Normal });
        yield return new WaitForSeconds(CombatConstants.PlayerInfoDelay);
        DequeueEvent(events);
    }

    private void BeginPlayerTurn()
    {
        CharacterSelectionModeController.Shared.Initialize();
    }

}