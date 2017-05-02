using System;
using System.Runtime.CompilerServices;

namespace Cadena.Meteor._Internals
{
    /// <summary>
    /// Left-leaning red-black tree implementation for caching object key.
    /// </summary>
    internal class KeyCacheTree
    {
        private KeyCacheTreeNode _root;

        public int Count { get; private set; }
        public int AddRequestCount { get; private set; }

        public IKeyCacheTreeDigger CreateDigger()
        {
            return new KeyCacheTreeDigger(this);
        }

        public void Add(string key)
        {
            AddRequestCount++;
            _root = Add(_root, key);
            _root.IsBlack = true;
        }

        private KeyCacheTreeNode Add(KeyCacheTreeNode node, string key)
        {
            if (node == null)
            {
                Count++;
                return new KeyCacheTreeNode(key);
            }

            if (IsRedNode(node.Left) && IsRedNode(node.Right))
            {
                FlipRedBlack(node);
            }

            var cr = String.Compare(node.Value, key, StringComparison.Ordinal);
            if (cr == 0)
            {
                // this node is target node -> nothing to do.
                return node;
            }
            if (cr > 0)
            {
                node.Left = Add(node.Left, key);
            }
            else
            {
                node.Right = Add(node.Right, key);
            }

            // rotate
            if (IsRedNode(node.Right))
            {
                node = RotateLeft(node);
            }

            if (IsRedNode(node.Left) && IsRedNode(node.Left.Left))
            {
                node = RotateRight(node);
            }

            return node;
        }

        private KeyCacheTreeNode RotateLeft(KeyCacheTreeNode node)
        {
            //    node
            //      pick
            //     |
            //     V
            //    pick
            //  node
            var pick = node.Right;
            node.Right = pick.Left;
            pick.Left = node;
            pick.IsBlack = node.IsBlack;
            node.IsBlack = false;
            return pick;
        }

        private KeyCacheTreeNode RotateRight(KeyCacheTreeNode node)
        {
            //   node
            // pick
            //   |
            //   V
            //   pick
            //     node
            var pick = node.Left;
            node.Left = pick.Right;
            pick.Right = node;
            pick.IsBlack = node.IsBlack;
            node.IsBlack = false;
            return pick;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsRedNode(KeyCacheTreeNode node)
        {
            return node != null && !node.IsBlack;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void FlipRedBlack(KeyCacheTreeNode node)
        {
            node.IsBlack = !node.IsBlack;
            node.Left.IsBlack = !node.Left.IsBlack;
            node.Right.IsBlack = !node.Right.IsBlack;
        }

        public void Clear()
        {
            _root = null;
            Count = 0;
        }

        private sealed class KeyCacheTreeDigger : IKeyCacheTreeDigger
        {
            private readonly KeyCacheTree _parent;

            private KeyCacheTreeNode _targetNode;

            private int _digIndex;

            public int ItemValidLength => _digIndex;

            public string PointingItem => _targetNode?.Value;

            public KeyCacheTreeDigger(KeyCacheTree parent)
            {
                _parent = parent;
                _digIndex = 0;
                Initialize();
            }

            public void Initialize()
            {
                _targetNode = _parent._root;
                _digIndex = 0;
            }

            public bool DigNextChar(char c)
            {
                var node = DigChar(_targetNode, c);
                if (node == null)
                {
                    return false;
                }
                _digIndex++;
                _targetNode = node;
                return true;
            }

            private KeyCacheTreeNode DigChar(KeyCacheTreeNode node, char nextchar)
            {
                if (node == null) return null;
                var item = node.Value;
                if (item.Length < _digIndex + 1)
                {
                    // go to right node
                    return DigText(node.Right, item, nextchar);
                }
                var diff = nextchar - item[_digIndex];
                if (diff == 0)
                {
                    // this node has same(or partial same) string
                    return node;
                }
                return DigText(diff < 0 ? node.Left : node.Right, item, nextchar);
            }

            private KeyCacheTreeNode DigText(KeyCacheTreeNode node, string text, char nextchar)
            {
                if (node == null) return null;
                var item = node.Value;
                var diff = String.Compare(text, 0, item, 0, _digIndex, StringComparison.Ordinal);
                if (diff == 0)
                {
                    return DigChar(node, nextchar);
                }
                return DigText(diff < 0 ? node.Left : node.Right, text, nextchar);
            }

            public void Complete()
            {
                if (_targetNode == null || _targetNode.Value.Length == _digIndex) return;
                // target node is too long to specified cache string.
                // let's digging left descendants.
                _targetNode = CompleteText(_targetNode.Value, _targetNode.Left) ?? _targetNode;
            }

            private KeyCacheTreeNode CompleteText(string reference, KeyCacheTreeNode node)
            {
                if (node == null) return null;
                var diff = String.Compare(reference, 0, node.Value, 0, _digIndex, StringComparison.Ordinal);
                if (diff == 0 && node.Value.Length == _digIndex)
                {
                    return node;
                }
                return CompleteText(reference, diff <= 0 ? node.Left : node.Right);
            }
        }
    }

    internal class KeyCacheTreeNode
    {
        public string Value { get; }

        public KeyCacheTreeNode Left { get; set; }

        public KeyCacheTreeNode Right { get; set; }

        public bool IsBlack { get; set; }

        public KeyCacheTreeNode(string item)
        {
            Value = item;
        }
    }
}