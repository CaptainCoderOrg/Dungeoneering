using System.Linq;

using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.Unity.Data;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public class DungeonInfoPanelTestData : MonoBehaviour
    {
        public DungeonCrawlerData Data;
        public DungeonInfoPanel UnderTest;

        void Awake()
        {
            Dungeon dungeon = Data.Manifest.Dungeons.Values.First();
            UnderTest.Show(dungeon);
        }
    }
}