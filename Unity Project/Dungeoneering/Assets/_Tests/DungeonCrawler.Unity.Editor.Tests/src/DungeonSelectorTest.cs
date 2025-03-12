using System.Collections;
using System.Linq;

using CaptainCoder.Dungeoneering.Unity.Editor;
using CaptainCoder.Dungeoneering.DungeonMap;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using CaptainCoder.Unity.UI;
using CaptainCoder.Dungeoneering.Unity.Data;
using CaptainCoder.Dungeoneering.DungeonCrawler;

public class DungeonSelectorTest
{
    [SetUp]
    public void Setup()
    {
        SceneManager.LoadScene(@"Assets/_Tests/Test Scenes/Dungeon Selector Test.unity");
    }

    [UnityTest]
    public IEnumerator ShouldListAllDungeonsInManifest()
    {
        DungeonSelectorPanel panel = GameObject.FindFirstObjectByType<DungeonSelectorPanel>(FindObjectsInactive.Include);
        panel.DungeonCrawlerData.ForceInitialize();
        panel.gameObject.SetActive(true);
        yield return null;
        DungeonSelectorButton[] buttons = panel.GetComponentsInChildren<DungeonSelectorButton>();
        Assert.That(buttons.Count, Is.EqualTo(2));
        DungeonSelectorButton blank = buttons[0];
        Assert.That(blank.Label, Is.EqualTo("<b>Blank (Open)</b>"));
        DungeonSelectorButton second = buttons[1];
        Assert.That(second.Label, Is.EqualTo("Second Dungeon"));
    }

    [UnityTest]
    public IEnumerator ShouldOpenDungeonWhenButtonIsClicked()
    {
        DungeonSelectorPanel panel = GameObject.FindFirstObjectByType<DungeonSelectorPanel>(FindObjectsInactive.Include);
        panel.DungeonCrawlerData.ForceInitialize();
        Dungeon initialDungeon = panel.DungeonCrawlerData.CurrentDungeon;
        panel.gameObject.SetActive(true);
        yield return null;
        DungeonSelectorButton[] buttons = panel.GetComponentsInChildren<DungeonSelectorButton>();
        DungeonSelectorButton second = buttons[1];
        second.Select();
        yield return null;
        Assert.That(panel.gameObject.activeInHierarchy, Is.False, "Panel is not hidden");
        Dungeon loaded = panel.DungeonCrawlerData.CurrentDungeon;
        Assert.That(loaded, Is.Not.EqualTo(initialDungeon));
        Dungeon manifestVersion = panel.DungeonCrawlerData.ManifestData.Manifest.Dungeons["Second Dungeon"];
        Assert.That(loaded, Is.EqualTo(manifestVersion), "Loaded dungeon did not match manifest dungeon.");
        panel.gameObject.SetActive(true);
        yield return null;
        buttons = panel.GetComponentsInChildren<DungeonSelectorButton>();
        Assert.That(buttons.Count, Is.EqualTo(2));
        DungeonSelectorButton blank = buttons[0];
        Assert.That(blank.Label, Is.EqualTo("Blank"));
        second = buttons[1];
        Assert.That(second.Label, Is.EqualTo("<b>Second Dungeon (Open)</b>"));
    }

    [UnityTest]
    public IEnumerator ShouldLoadNewDungeonOnCreate()
    {
        DungeonSelectorPanel panel = GameObject.FindFirstObjectByType<DungeonSelectorPanel>(FindObjectsInactive.Include);
        panel.DungeonCrawlerData.ForceInitialize();
        panel.gameObject.SetActive(true);
        Dungeon initialDungeon = panel.DungeonCrawlerData.CurrentDungeon;
        yield return null;
        panel.CreateNewDungeon("New Dungeon");
        yield return null;
        Assert.That(panel.gameObject.activeInHierarchy, Is.False, "Panel is not hidden");
        Dungeon loaded = panel.DungeonCrawlerData.CurrentDungeon;
        Assert.That(loaded, Is.Not.EqualTo(initialDungeon));
        Dungeon manifestVersion = panel.DungeonCrawlerData.ManifestData.Manifest.Dungeons["New Dungeon"];
        Assert.That(loaded, Is.EqualTo(manifestVersion), "Loaded dungeon did not match manifest dungeon.");
        panel.gameObject.SetActive(true);
        yield return null;
        DungeonSelectorButton[] buttons = panel.GetComponentsInChildren<DungeonSelectorButton>();
        Assert.That(buttons.Count, Is.EqualTo(3));
        DungeonSelectorButton blank = buttons[0];
        Assert.That(blank.Label, Is.EqualTo("Blank"));
        DungeonSelectorButton second = buttons[1];
        Assert.That(second.Label, Is.EqualTo("Second Dungeon"));
        DungeonSelectorButton third = buttons[2];
        Assert.That(third.Label, Is.EqualTo("<b>New Dungeon (Open)</b>"));
    }

    [UnityTest]
    public IEnumerator TestDeletingDungeon()
    {
        DungeonSelectorPanel panel = GameObject.FindFirstObjectByType<DungeonSelectorPanel>(FindObjectsInactive.Include);
        panel.DungeonCrawlerData.ForceInitialize();
        DungeonCrawlerManifest manifest = panel.DungeonCrawlerData.ManifestData.Manifest;
        panel.gameObject.SetActive(true);
        yield return null;
        DungeonSelectorButton[] buttons = panel.GetComponentsInChildren<DungeonSelectorButton>();
        DungeonSelectorButton second = buttons[1];
        Assert.That(second.Label, Is.EqualTo("Second Dungeon"));
        Dungeon removedDungeon = manifest.Dungeons["Second Dungeon"];
        // The tile at position (0, 0) has a "white-tile.png"
        TileReference topLeftReference = new(removedDungeon, new Position(0, 0));
        Assert.That(panel.DungeonCrawlerData.HasReference(topLeftReference), Is.True, "Expected reference at (0, 0) to exist");
        second.Remove();
        yield return null;
        ConfirmPromptPanel confirmPrompt = GameObject.FindFirstObjectByType<ConfirmPromptPanel>();
        Assert.That(confirmPrompt, Is.Not.Null);
        Assert.That(confirmPrompt.gameObject.activeInHierarchy, Is.True);
        confirmPrompt.Confirm();
        yield return null;
        Assert.That(confirmPrompt.gameObject.activeInHierarchy, Is.False, "Confirm prompt was active after confirming");
        Assert.That(panel.DungeonCrawlerData.ManifestData.Manifest.Dungeons.Count, Is.EqualTo(1), "Manifest should have only 1 dungeon");
        Assert.That(panel.DungeonCrawlerData.ManifestData.Manifest.Dungeons.ContainsKey("Second Dungeon"), Is.False, "Manifest should not contain Second Dungeon");

        buttons = panel.GetComponentsInChildren<DungeonSelectorButton>();
        Assert.That(buttons.Count, Is.EqualTo(1));
        Assert.That(panel.DungeonCrawlerData.HasReference(topLeftReference), Is.False, "Expected reference at (0, 0) to be removed");

    }
}
