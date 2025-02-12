using CaptainCoder.Dungeoneering.DungeonMap.Unity;

namespace CaptainCoder.Dungeoneering.Unity.Editor
{
    public static class UndoRedoStackDataExtensions
    {
        public static void PerformEdit(this UndoRedoStackData stack, string name, System.Action perform, System.Action undo, DungeonManifestData manifest)
        {
            if (perform == null || undo == null) { return; }
            perform += manifest.Notify;
            undo += manifest.Notify;
            stack.PerformEdit(name, perform, undo);
        }
    }
}