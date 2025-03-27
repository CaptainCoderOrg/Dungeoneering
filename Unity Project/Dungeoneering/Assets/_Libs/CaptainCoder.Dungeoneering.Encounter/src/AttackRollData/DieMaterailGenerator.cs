using System.IO;

using NaughtyAttributes;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Encounter
{
    [CreateAssetMenu(menuName = "DC/Die Material Generator")]
    public class DieMaterialGenerator : ObservableSO
    {
        public Sprite[] DamageSprites;
        public Sprite[] AimSprites;
        public Sprite MissSprite;
        public Sprite PowerSprite;
        public Sprite SplitSprite;
        public Texture2D DieTextureTemplate;

        const int TextureSize = 32;


#if UNITY_EDITOR

        public DieData DieData;
        public string FileName;
        [Button]
        public void GenerateTexture()
        {
            Texture2D output = GenerateTexture(DieData);
            string path = Path.Combine(Path.Combine(AssetDatabase.GetAssetPath(this).Split("/")[0..^1]), FileName);
            Debug.Log(path);
            File.WriteAllBytes(path, output.EncodeToPNG());
        }

#endif

        public Texture2D GenerateTexture(DieData die)
        {
            const int rows = 2;
            const int columns = 3;
            Texture2D texture = new(DieTextureTemplate.width, DieTextureTemplate.height, DieTextureTemplate.format, false);
            texture.LoadRawTextureData(texture.GetRawTextureData());
            texture.filterMode = FilterMode.Point;
            SetTransparent(texture);
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    FaceData face = die.Faces[row * columns + col];
                    if (face.IsMiss)
                    {
                        CopyTexture(texture, MissSprite, row, col);
                        continue;
                    }

                    if (face.IsSplit)
                    {
                        CopyTexture(texture, SplitSprite, row, col);
                    }

                    if (face.Power > 0)
                    {
                        CopyTexture(texture, PowerSprite, row, col);
                    }

                    if (face.Damage > 0)
                    {
                        CopyTexture(texture, DamageSprites[face.Damage - 1], row, col);
                    }

                    CopyTexture(texture, AimSprites[face.Aim], row, col);
                }
            }
            texture.Apply();
            return texture;
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

        private static void CopyTexture(Texture2D target, Sprite origin, int row, int col)
        {
            for (int y = 0; y < TextureSize; y++)
            {
                for (int x = 0; x < TextureSize; x++)
                {
                    Color c = origin.texture.GetPixel(x + (int)origin.rect.x, y + (int)origin.rect.y);
                    if (c.a == 0) { continue; }
                    target.SetPixel(x + TextureSize * col, y + TextureSize * row, c);
                }
            }
        }
    }
}