using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.DungeonMap.IO;
using CaptainCoder.Dungeoneering.Unity.Data;
using CaptainCoder.Unity;

namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public static class UndoRedoStackDataExtensions
    {

        /// <summary>
        /// Serializes the state of the manifest before performing the specified action. The undo action loads the entire previous state.
        /// </summary>
        public static void PerformEditSerializeState(this UndoRedoStackData stack, string name, System.Action perform, DungeonCrawlerData data)
        {
            string originalDungeonJson = JsonExtensions.ToJson(data.CurrentDungeon.Dungeon);
            string originalManifestJson = JsonExtensions.ToJson(data.ManifestData.Manifest);

            data.CurrentDungeon.PreventNotify = true;
            perform.Invoke();
            data.CurrentDungeon.PreventNotify = false;

            string redoDungeonJson = JsonExtensions.ToJson(data.CurrentDungeon.Dungeon);
            string redoManifestJson = JsonExtensions.ToJson(data.ManifestData.Manifest);

            void Redo()
            {
                data.ManifestData.TryLoadManifest(redoManifestJson, out _);
                data.CurrentDungeon.Dungeon = JsonExtensions.LoadModel<Dungeon>(redoDungeonJson);
            }

            void Undo()
            {
                data.ManifestData.TryLoadManifest(originalManifestJson, out _);
                data.CurrentDungeon.Dungeon = JsonExtensions.LoadModel<Dungeon>(originalDungeonJson);
            }
            stack.PushEdit(name, Redo, Undo);
        }
    }
}