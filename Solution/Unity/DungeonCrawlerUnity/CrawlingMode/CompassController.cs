using CaptainCoder.Dungeoneering.Player;

using TMPro;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Unity;

public class CompassController : MonoBehaviour
{
    public static CompassController Shared { get; private set; } = default!;
    public CompassController() { Shared = this; }
    public TextMeshProUGUI Text = default!;

    public void Render(PlayerView view)
    {
        Text.text = view.Facing.ToString()[0].ToString();
    }
}