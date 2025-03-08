using UnityEngine;

namespace CaptainCoder.Dungeoneering.Unity.Data
{
    [CreateAssetMenu(menuName = "DC/Material Cache Data")]
    public class MaterialCacheData : ScriptableObject
    {
        public MaterialCache Cache { get; private set; } = new();

        public void Initialize()
        {
            Cache.Clear();
        }
    }
}