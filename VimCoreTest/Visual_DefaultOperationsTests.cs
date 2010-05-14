﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;
using Vim.Modes.Visual;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text;
using VimCore.Test.Utils;
using Microsoft.VisualStudio.Text.Operations;
using Vim;
using Microsoft.VisualStudio.Text.Outlining;

namespace VimCore.Test
{
    [TestFixture]
    public class Visual_DefaultOperationsTests
    {
        private IWpfTextView _textView;
        private MockFactory _factory;
        private Mock<IEditorOperations> _editorOpts;
        private Mock<IVimHost> _host;
        private Mock<IJumpList> _jumpList;
        private Mock<IVimLocalSettings> _settings;
        private Mock<IOutliningManager> _outlining;
        private Mock<IUndoRedoOperations> _undoRedoOperations;
        private IOperations _operations;

        private void Create(params string[] lines)
        {
            Create(ModeKind.VisualCharacter, lines);
        }

        private void Create(ModeKind kind, params string[] lines)
        {
            _textView = EditorUtil.CreateView(lines);
            _factory = new MockFactory(MockBehavior.Strict);
            _editorOpts = _factory.Create<IEditorOperations>();
            _jumpList = _factory.Create<IJumpList>();
            _host = _factory.Create<IVimHost>();
            _outlining = _factory.Create<IOutliningManager>();
            _settings = _factory.Create<IVimLocalSettings>();
            _undoRedoOperations = _factory.Create<IUndoRedoOperations>();
            _operations = new DefaultOperations(_textView, _editorOpts.Object, _outlining.Object, _host.Object, _jumpList.Object, _settings.Object, _undoRedoOperations.Object, kind);
        }

        [Test]
        public void DeleteSelection1()
        {
            Create("foo", "bar");
            _textView.Selection.Select(new SnapshotSpan(_textView.TextSnapshot, 0, 2),false);
            var reg = new Register('c');
            _operations.DeleteSelection(reg);
            Assert.AreEqual("fo", reg.StringValue);
            Assert.AreEqual("o", _textView.TextSnapshot.GetLineFromLineNumber(0).GetText());
            _factory.Verify();
        }

        [Test]
        public void DeleteSelection2()
        {
            Create(ModeKind.VisualLine, "a", "b", "c");
            _textView.Selection.Select(_textView.GetLine(0).ExtentIncludingLineBreak, false);
            var reg = new Register('c');
            _operations.DeleteSelection(reg);
            Assert.AreEqual(OperationKind.LineWise, reg.Value.OperationKind);
        }

        [Test]
        public void DeleteSelectedLines1()
        {
            Create("foo", "bar");
            var span = _textView.GetLine(0).ExtentIncludingLineBreak;
            _textView.Selection.Select(span, false);
            var reg = new Register('c');
            _operations.DeleteSelectedLines(reg);
            Assert.AreEqual( span.GetText(), reg.StringValue);
            Assert.AreEqual(1, _textView.TextSnapshot.LineCount);
            _factory.Verify();
        }
    }
}
