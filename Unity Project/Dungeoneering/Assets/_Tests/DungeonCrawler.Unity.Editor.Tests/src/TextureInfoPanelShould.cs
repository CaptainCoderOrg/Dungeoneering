using System.Collections;

using CaptainCoder.Dungeoneering.Unity.Data;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class TextureInfoPanelShould
{

    [SetUp]
    public void Setup()
    {
        SceneManager.LoadScene(@"Assets/_Tests/Test Scenes/Texture Info Panel Test/Texture Info Panel Test Scene.unity");
    }

    private TextureInfoPanelTestData Init()
    {
        TextureInfoPanelTestData data = GameObject.FindFirstObjectByType<TextureInfoPanelTestData>(FindObjectsInactive.Include);
        data.DungeonCrawlerData.ForceInitialize();
        return data;
    }

    [UnityTest]
    public IEnumerator DisableDeleteIfDefaultTexture()
    {
        TextureInfoPanelTestData data = Init();
        data.Panel.Texture = data.DungeonCrawlerData.GetTexture("default-tile.png");
        yield return null;
        Assert.That(data.DeleteButton.interactable, Is.False);
    }

    [UnityTest]
    public IEnumerator EnableDeleteIfNotDefaultTexture()
    {
        TextureInfoPanelTestData data = Init();
        data.Panel.Texture = data.DungeonCrawlerData.GetTexture("white-tile.png");
        yield return null;
        Assert.That(data.DeleteButton.interactable, Is.True);
    }
}