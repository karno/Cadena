namespace Cadena.Meteor._Internals
{
    /// <summary>
    /// Red-black tree implementation for caching object key.
    /// </summary>
    internal class KeyCacheTree
    {
    }

    internal class KeyCacheTreeNode
    {
        private string Item { get; }

        private KeyCacheTreeNode Left { get; set; }

        private KeyCacheTreeNode Right { get; set; }

        public KeyCacheTreeNode(string item)
        {
            Item = item;
        }

        public KeyCacheTreeNode(string item, KeyCacheTreeNode left, KeyCacheTreeNode right)
            : this(item)
        {
            Left = left;
            Right = right;
        }
    }

    /// <summary>
    /// Mining the KeyCacheTree node.
    /// </summary>
    internal unsafe class KeyCacheTreeMiner
    {
        private uint _currentLength = 0;

        /// <summary>
        /// Mining next leaf.
        /// </summary>
        /// <param name="ptr">pointer of current character</param>
        private void Mine(ref char* ptr)
        {
        }
    }
}
