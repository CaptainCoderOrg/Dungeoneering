using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.IO;
using CaptainCoder.Dungeoneering.Unity.Data;
using CaptainCoder.Unity;

namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public static class UndoRedoStackDataExtensions
    {
        public static void PerformEdit(this UndoRedoStackData stack, string name, System.Action perform, System.Action undo, DungeonData dungeon)
        {
            if (perform == null || undo == null) { return; }
            perform += dungeon.Notify;
            undo += dungeon.Notify;
            stack.PerformEdit(name, perform, undo);
        }

        /// <summary>
        /// Serializes the state of the manifest before performing the specified action. The undo action loads the entire previous state.
        /// </summary>
        public static void PerformEditSerializeState(this UndoRedoStackData stack, string name, System.Action perform, DungeonCrawlerData data)
        {
            string originalDungeonJson = JsonExtensions.ToJson(data.CurrentDungeon.Dungeon);
            string originalManifestJson = JsonExtensions.ToJson(data.ManifestData.Manifest);
            System.Action undo = () =>
            {
                data.ManifestData.TryLoadManifest(originalManifestJson, out _);
                data.CurrentDungeon.Dungeon = JsonExtensions.LoadModel<Dungeon>(originalDungeonJson);
            };
            stack.PerformEdit(name, perform, undo, data.CurrentDungeon);
        }
    }
}