using CaptainCoder.DungeonCrawler.Unity;
using CaptainCoder.Dungeoneering.Unity;

using UnityEngine;

namespace CaptainCoder.DungeonCrawler.Combat.Unity;

public class CombatModeController : MonoBehaviour
{
    public static CombatModeController Shared { get; private set; } = default!;
    public CombatModeController() { Shared = this; }

    public void OnEnable()
    {
        CombatViewPortController.Shared.gameObject.SetActive(true);
    }

    public void OnDisable()
    {
        CombatViewPortController.Shared.gameObject.SetActive(false);
        UnselectPartyAndRevive();
    }

    public void Initialize()
    {
        gameObject.SetActive(true);

    }

    private void UnselectPartyAndRevive()
    {
        foreach (var card in Overlay.Shared.Cards)
        {
            card.IsFinished = false;
            card.IsSelected = false;
        }
        foreach (var character in CrawlingModeController.Shared.Party)
        {
            if (character.Card == Characters.NoBody) { continue; }
            if (character.Health() > 0) { continue; }
            CrawlingModeController.Shared.Party.UpdateCharacter(character with { Wounds = character.Card.BaseHealth - 1, Exertion = character.Card.BaseEnergy - 1 });
        }
    }

    public void UpdateEnemySpeed(float speed)
    {
        CombatConstants.ShowEnemyInfoDuration = 0.01f + speed * 2;
        CombatConstants.ShortEnemyInfoDuration = 0.01f + speed;
        CombatConstants.GuardWaitDuration = 1.5f + speed * 2;
    }
}