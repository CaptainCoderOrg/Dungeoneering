using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/SpriteSheetData")]
    public class SpriteSheetData : ObservableSO
    {
        [field: SerializeField] public Texture2D SpriteSheet { get; private set; }
        [field: SerializeField] public int Rows { get; private set; }
        [field: SerializeField] public int Columns { get; private set; }
        public float XScale { get; private set; }
        public float YScale { get; private set; }

        public override void OnAfterEnterPlayMode()
        {
            base.OnAfterEnterPlayMode();
            XScale = 1f / Columns;
            YScale = 1f / Rows;
        }

        public Vector2 GetTextureOffset(int frameIx)
        {
            (int column, int row) = IndexToOffset(frameIx);
            return new(XScale * column, 1 - (YScale * (row + 1)));
        }

        private (int Column, int Row) IndexToOffset(int ix) => (ix % Columns, ix / Columns);
        private int OffsetToIndex((int column, int row) offset) => offset.row * Columns + offset.column;
    }
}