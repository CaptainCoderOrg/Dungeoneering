using UnityEngine;
namespace CaptainCoder.Unity
{
    public static class TextureExtensions
    {
        public static Material ToMaterial(this Dungeoneering.Texture texture)
        {
            Texture2D t2d = new(2, 2) { filterMode = FilterMode.Point };
            ImageConversion.LoadImage(t2d, texture.Data);
            Material material = new(Shader.Find("Universal Render Pipeline/Lit"));
            material.SetFloat("_Smoothness", 0); // Smoothness
            material.SetFloat("_SpecularHighlights", 0); // Specular Highlights
            material.SetFloat("_GlossyReflections", 0); // Reflections
            material.mainTexture = t2d;
            return material;
        }
    }
}