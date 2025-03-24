using CaptainCoder.Dungeoneering.Unity.Data;
using CaptainCoder.Unity.Assertions;

using UnityEngine;
namespace CaptainCoder.Dungeoneering.Encounter
{
    public class EncounterController : MonoBehaviour
    {
        [AssertIsSet][SerializeField] private DungeonCrawlerData _dungeonCrawlerData;
        [SerializeField] private string _dungeonName;
        [SerializeField] private int _minX;
        [SerializeField] private int _maxX;
        [SerializeField] private int _minY;
        [SerializeField] private int _maxY;

        void Awake() => Build();

        public void Build()
        {

        }

    }
}