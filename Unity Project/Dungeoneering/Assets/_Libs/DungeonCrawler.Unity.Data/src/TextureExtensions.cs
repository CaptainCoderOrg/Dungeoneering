using UnityEngine;
namespace CaptainCoder.Dungeoneering.DungeonMap.Unity
{
    public static class TextureExtensions
    {
        public static Material ToMaterial(this Dungeoneering.Texture texture)
        {
            Texture2D t2d = new(2, 2) { filterMode = FilterMode.Point };
            t2d.LoadImage(texture.Data);
            return new Material(Shader.Find("Custom/SelectableShader"))
            {
                mainTexture = t2d
            };
        }
    }
}