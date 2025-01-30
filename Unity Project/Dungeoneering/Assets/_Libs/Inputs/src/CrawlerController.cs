using UnityEngine;
using CaptainCoder.Dungeoneering.DungeonCrawler;
using CaptainCoder.Dungeoneering.Player;

public class CrawlerController : MonoBehaviour
{
    public void HandleInput(MovementAction action)
    {
        Vector3 movement = action switch
        {
            MovementAction.StepForward => transform.forward,
            MovementAction.StrafeLeft => -transform.right,
            MovementAction.StepBackward => -transform.forward,
            MovementAction.StrafeRight => transform.right,
        };
        transform.position += movement;
    }
}
