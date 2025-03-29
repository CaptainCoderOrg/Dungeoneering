#if UNITY_EDITOR
using System.IO;
using System.Linq;

using NaughtyAttributes;

using UnityEditor;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "Sprite Merger Generator")]
    public class SpriteMerger : ObservableSO
    {
        public Texture2D[] SpriteSheets;

        [Button]
        public void Save()
        {
            string path = Path.Combine(EditorUtility.SaveFilePanel("Save Sprite Sheet", "", "merged", "png"));
            Texture2D output = GenerateTexture();
            Debug.Log(path);
            File.WriteAllBytes(path, output.EncodeToPNG());
        }

        public Texture2D GenerateTexture()
        {
            Texture2D template = SpriteSheets.First(s => s != null);
            Texture2D texture = new(template.width, template.height, template.format, false);
            texture.LoadRawTextureData(template.GetRawTextureData());
            texture.filterMode = FilterMode.Point;
            SetTransparent(texture);
            foreach (var sheet in SpriteSheets)
            {
                if (sheet == null) { continue; }
                CopyTexture(texture, sheet);
            }
            return texture;
        }

        private static void CopyTexture(Texture2D target, Texture2D layer)
        {
            for (int y = 0; y < target.height; y++)
            {
                for (int x = 0; x < target.width; x++)
                {
                    Color c = layer.GetPixel(x, y);
                    if (c.a == 0) { continue; }
                    target.SetPixel(x, y, c);
                }
            }
        }

        private static void SetTransparent(Texture2D texture)
        {
            for (int row = 0; row < texture.height; row++)
            {
                for (int col = 0; col < texture.width; col++)
                {
                    texture.SetPixel(col, row, new Color(0, 0, 0, 0));
                }
            }
        }

    }
}
#endif