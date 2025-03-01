using CaptainCoder.Dungeoneering.Unity;

using UnityEngine;

namespace CaptainCoder.DungeonCrawler.Unity.Data
{
    [CreateAssetMenu(menuName = "DC/Material Cache Data")]
    public class MaterialCacheData : ObservableSO
    {
        public MaterialCache Cache { get; private set; } = new();

        protected override void OnExitEditMode()
        {
            base.OnExitEditMode();
            Cache.Clear();
        }

        protected override void OnExitPlayMode()
        {
            base.OnExitEditMode();
            Cache.Clear();
        }
    }
}