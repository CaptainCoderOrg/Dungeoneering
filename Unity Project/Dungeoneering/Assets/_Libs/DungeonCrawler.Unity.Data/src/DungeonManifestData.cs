
using System.IO;
using CaptainCoder.Dungeoneering.DungeonCrawler;
using CaptainCoder.Dungeoneering.DungeonMap.IO;
using UnityEngine;

namespace CaptainCoder.Dungeoneering.DungeonMap.Unity
{

    [CreateAssetMenu(fileName = "DungeonManifestData", menuName = "DC/Manifest")]
    public class DungeonManifestData : ScriptableObject
    {
        [field: SerializeField] 
        public TextAsset ManifestJson { get; private set; }
        public DungeonCrawlerManifest LoadManifest() => JsonExtensions.LoadModel<DungeonCrawlerManifest>(ManifestJson.text);
        public DungeonCrawlerManifest LoadFromFile(string path) => JsonExtensions.LoadModel<DungeonCrawlerManifest>(File.ReadAllText(path));
    }
}