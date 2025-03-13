using System.Collections.Generic;

using UnityEngine;
namespace CaptainCoder.Unity
{
    [CreateAssetMenu(menuName = "CaptainCoder/UndoRedoStack")]
    public class UndoRedoStackData : ObservableSO
    {
        private readonly Stack<EditorOperation> _undoStack = new();
        private readonly Stack<EditorOperation> _redoStack = new();

        public void Clear()
        {
            _redoStack.Clear();
            _undoStack.Clear();
        }

        /// <summary>
        /// Pushes the edit to the stack without calling perform. This assumes the action was performed before calling this method
        /// </summary>
        public void PushEdit(string name, System.Action perform, System.Action undo)
        {
            EditorOperation operation = new() { Name = name, Perform = perform, Undo = undo };
            _undoStack.Push(operation);
            _redoStack.Clear();
        }

        public void PerformAndPushEdit(string name, System.Action perform, System.Action undo)
        {
            EditorOperation operation = new() { Name = name, Perform = perform, Undo = undo };
            operation.Perform?.Invoke();
            _undoStack.Push(operation);
            _redoStack.Clear();
        }

        public void Undo()
        {
            if (_undoStack.Count == 0) { return; }
            EditorOperation operation = _undoStack.Pop();
            operation.Undo?.Invoke();
            _redoStack.Push(operation);
        }

        public void Redo()
        {
            if (_redoStack.Count == 0) { return; }
            EditorOperation operation = _redoStack.Pop();
            operation.Perform?.Invoke();
            _undoStack.Push(operation);
        }

        protected override void OnExitPlayMode()
        {
            base.OnExitPlayMode();
            Clear();
        }


        private class EditorOperation
        {
            public string Name { get; set; }
            public System.Action Perform { get; set; }
            public System.Action Undo { get; set; }
        }

    }
}