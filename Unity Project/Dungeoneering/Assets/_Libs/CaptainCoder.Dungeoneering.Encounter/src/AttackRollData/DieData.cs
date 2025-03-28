using System.Linq;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/DieData")]
    public class DieData : ObservableSO
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Sprite Sprite { get; private set; }
        [field: SerializeField] public Color UIAlbedo { get; private set; }
        [field: SerializeField] public Color Albedo { get; private set; }
        [field: SerializeField] public FaceData[] Faces { get; private set; }
        [SerializeField] private Texture2D _texture2D;
        public Texture2D Texture => LazyGet();
        [SerializeField] private DieMaterialGenerator _dieMaterialGenerator;
        private Sprite[] _sprites;

        public override void OnAfterEnterPlayMode()
        {
            base.OnAfterEnterPlayMode();
            LazyGet();
        }

        public Sprite GetFaceSprite(int ix) => _sprites[ix];

        // For some reasone ??= does not work here, Unity must be doing something with null checks
        private Texture2D LazyGet()
        {
            if (_texture2D == null)
            {
                _texture2D = _dieMaterialGenerator.GenerateTexture(this);
                _sprites = Enumerable.Range(0, 6).Select(ix => Sprite.Create(_texture2D, DieMaterialGenerator.FaceIxToRect(ix), default)).ToArray();
            }
            return _texture2D;
        }
    }
}