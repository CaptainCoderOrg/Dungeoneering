using CaptainCoder.Dungeoneering.DungeonMap.Unity;
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
    }
}