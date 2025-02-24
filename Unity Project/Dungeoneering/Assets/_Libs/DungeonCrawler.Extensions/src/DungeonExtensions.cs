using CaptainCoder.Dungeoneering.DungeonMap.IO;
namespace CaptainCoder.Dungeoneering.DungeonMap
{
    public static class DungeonExtensions
    {
        public static Dungeon Copy(this Dungeon toCopy)
        {
            string jsonified = JsonExtensions.ToJson(toCopy);
            return JsonExtensions.LoadModel<Dungeon>(jsonified);
        }
    }
}