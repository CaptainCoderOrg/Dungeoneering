
using CaptainCoder.Dungeoneering;
using CaptainCoder.Dungeoneering.DungeonMap.IO;


Texture stoneWall = new("stone-wall", File.ReadAllBytes("stone-wall.png"));

File.WriteAllText("stone-wall.json", JsonExtensions.ToJson(stoneWall));