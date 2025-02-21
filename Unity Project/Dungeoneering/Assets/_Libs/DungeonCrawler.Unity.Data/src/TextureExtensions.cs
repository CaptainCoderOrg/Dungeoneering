using UnityEngine;
namespace CaptainCoder.Dungeoneering.DungeonMap.Unity
{
    public static class TextureExtensions
    {
        public static Material ToMaterial(this Dungeoneering.Texture texture)
        {
            Texture2D t2d = new(2, 2) { filterMode = FilterMode.Point };
            t2d.LoadImage(texture.Data);
            Shader shader = Shader.Find("Custom/SelectableShader");
            if (shader == null)
            {
                Debug.LogErrorFormat("Can't find shader 'Custom/SelectableShader'.");
                throw new System.Exception("Can't find shader!");
            }
            return new Material(shader)
            {
                mainTexture = t2d
            };
        }
    }
}