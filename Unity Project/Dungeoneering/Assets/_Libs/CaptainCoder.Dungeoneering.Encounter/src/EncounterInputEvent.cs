using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter;

public abstract record class EncounterInputEvent;
public sealed record class RotateCameraClockwise : EncounterInputEvent
{
    public static readonly RotateCameraClockwise Instance = new();
}
public sealed record class RotateCameraCounterClockwise : EncounterInputEvent
{
    public static readonly RotateCameraCounterClockwise Instance = new();
}
public sealed record class CameraPitchUp : EncounterInputEvent
{
    public static readonly CameraPitchUp Instance = new();
}
public sealed record class CameraPitchDown : EncounterInputEvent
{
    public static readonly CameraPitchDown Instance = new();
}
public sealed record class StartPan(Vector2 Direction) : EncounterInputEvent;
public sealed record class EndPan : EncounterInputEvent
{
    public static readonly EndPan Instance = new();
}
public sealed record class CameraZoom(float Delta) : EncounterInputEvent;