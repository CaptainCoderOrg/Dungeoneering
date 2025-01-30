namespace Tests;

using CaptainCoder.Dungeoneering;

using Shouldly;

public class Texture_should
{

    [Theory]
    [InlineData("wall", new byte[] { 1, 2, 3 })]
    [InlineData("another", new byte[] { 7, 1 })]
    [InlineData("stone", new byte[] { 0, 5, 92, 7 })]
    public void be_equal(string name, byte[] bytes)
    {
        Texture t0 = new(name, bytes);
        byte[] bytes1 = [.. bytes];
        Texture t1 = new(name, bytes1);
        t0.ShouldBe(t1);
    }

    [Theory]
    [InlineData("wall", new byte[] { 1, 2, 3 }, "another", new byte[] { 7, 1 })]
    [InlineData("another", new byte[] { 7, 1 }, "stone", new byte[] { 0, 5, 92, 7 })]
    public void not_equal(string name0, byte[] bytes0, string name1, byte[] bytes1)
    {
        Texture t0 = new(name0, bytes0);
        Texture t1 = new(name1, bytes1);
        t0.ShouldNotBe(t1);
    }
}