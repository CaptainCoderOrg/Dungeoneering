namespace CaptainCoder.Dungeoneering.Raylib;

using Raylib_cs;

public class InfoOverLayScreen : IScreen
{
    private readonly Queue<OverlayMessage> _messages = new();
    private double _nextMessageAt = 0;
    public void AddMessage(string message, Color color, float duration = 2)
    {
        _messages.Enqueue(new OverlayMessage(message, color, duration));
    }
    private OverlayMessage? _currentMessage = null;
    public void HandleUserInput() { }
    public void Render()
    {
        if (Raylib.GetTime() > _nextMessageAt && _messages.TryDequeue(out _currentMessage))
        {
            _nextMessageAt = Raylib.GetTime() + _currentMessage.Duration;
        }

        if (_currentMessage is not null && _nextMessageAt > Raylib.GetTime())
        {
            _currentMessage.Message.DrawCentered(50, 36, _currentMessage.Color);
        }
    }

}

public record OverlayMessage(string Message, Color Color, double Duration);