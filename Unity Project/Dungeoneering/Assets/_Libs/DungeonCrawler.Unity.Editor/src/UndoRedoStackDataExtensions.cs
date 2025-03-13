using System;

using CaptainCoder.Dungeoneering.DungeonCrawler;
using CaptainCoder.Dungeoneering.DungeonMap;
using CaptainCoder.Dungeoneering.Unity.Data;
using CaptainCoder.Unity;

namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public static class UndoRedoStackDataExtensions
    {

        /// <summary>
        /// Serializes the state of the manifest before performing the specified action. The undo action loads the entire previous state.
        /// </summary>
        public static void PerformEditSerializeState(this UndoRedoStackData stack, string name, Action perform, DungeonCrawlerData data)
        {
            Dungeon originalDungeon = data.CurrentDungeon.Copy();
            DungeonCrawlerManifest originalManifest = data.Manifest.Copy();

            data.PreventNotify = true;
            perform.Invoke();
            data.PreventNotify = false;

            Dungeon redoDungeon = data.CurrentDungeon.Copy();
            DungeonCrawlerManifest redoManifest = data.Manifest.Copy();

            void Redo()
            {
                data.LoadManifest(redoManifest);
                data.LoadDungeon(redoDungeon);
            }

            void Undo()
            {
                data.LoadManifest(originalManifest);
                data.LoadDungeon(originalDungeon);
            }
            stack.PushEdit(name, Redo, Undo);
        }
    }
}