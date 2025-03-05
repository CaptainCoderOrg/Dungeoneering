using UnityEngine;

namespace CaptainCoder.Dungeoneering.Unity.Data
{
    [CreateAssetMenu(menuName = "DC/Material Cache Data")]
    public class MaterialCacheData : ObservableSO
    {
        public MaterialCache Cache { get; private set; } = new();

        protected override void OnExitEditMode()
        {
            base.OnExitEditMode();
            Cache = new();
        }

        protected override void OnExitPlayMode()
        {
            base.OnExitEditMode();
        }
    }
}