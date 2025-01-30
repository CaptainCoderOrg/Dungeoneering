namespace CaptainCoder.Dungeoneering;

public class Texture(string name, byte[] data) : IEquatable<Texture>
{
    public string Name { get; } = name;
    public byte[] Data { get; } = data;
    public bool Equals(Texture other) => Name == other.Name && Data.SequenceEqual(other.Data);
}