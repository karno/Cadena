using Cadena.Meteor._Internals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cadena.Test
{
    /// <summary>
    /// KeyCacheTreeTest の概要の説明
    /// </summary>
    [TestClass]
    public class KeyCacheTreeTest
    {
        private readonly KeyCacheTree _tree;
        private readonly IKeyCacheTreeDigger _digger;

        public KeyCacheTreeTest()
        {
            _tree = new KeyCacheTree();
            _digger = _tree.CreateDigger();
        }

        private TestContext testContextInstance;

        /// <summary>
        ///現在のテストの実行についての情報および機能を
        ///提供するテスト コンテキストを取得または設定します。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 追加のテスト属性
        //
        // テストを作成する際には、次の追加属性を使用できます:
        //
        // クラス内で最初のテストを実行する前に、ClassInitialize を使用してコードを実行してください
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // クラス内のテストをすべて実行したら、ClassCleanup を使用してコードを実行してください
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // 各テストを実行する前に、TestInitialize を使用してコードを実行してください
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // 各テストを実行した後に、TestCleanup を使用してコードを実行してください
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void KeyCacheTreeDiggingTest()
        {
            _tree.Add("coordinates");
            _tree.Add("favorited");
            _tree.Add("truncated");
            _tree.Add("created_at");
            _tree.Add("id_str");
            _tree.Add("entities");
            _tree.Add("urls");
            _tree.Add("expanded_url");
            _tree.Add("url");
            _tree.Add("indices");
            _tree.Add("display_url");
            _tree.Add("hashtags");
            _tree.Add("user_mentions");
            _tree.Add("in_reply_to_user_id_str");
            _tree.Add("contributors");
            _tree.Add("text");
            _tree.Add("retweet_count");
            _tree.Add("in_reply_to_status_id");
            _tree.Add("id");
            _tree.Add("geo");
            _tree.Add("retweeted");
            _tree.Add("possibly_sensitive");
            _tree.Add("in_reply_to_user_id");
            _tree.Add("place");
            _tree.Add("user");

            _digger.Initialize();
            Assert.IsTrue(_digger.DigNextChar('i'));
            Assert.IsTrue(_digger.DigNextChar('n'));
            Assert.IsTrue(_digger.DigNextChar('_'));
            Assert.IsTrue(_digger.DigNextChar('r'));
            Assert.IsTrue(_digger.DigNextChar('e'));
            Assert.IsTrue(_digger.DigNextChar('p'));
            Assert.IsTrue(_digger.DigNextChar('l'));
            Assert.IsTrue(_digger.DigNextChar('y'));
            Assert.IsTrue(_digger.DigNextChar('_'));
            Assert.IsTrue(_digger.DigNextChar('t'));
            Assert.IsTrue(_digger.DigNextChar('o'));
            Assert.IsTrue(_digger.DigNextChar('_'));
            Assert.IsTrue(_digger.DigNextChar('u'));
            Assert.IsTrue(_digger.DigNextChar('s'));
            Assert.IsTrue(_digger.DigNextChar('e'));
            Assert.IsTrue(_digger.DigNextChar('r'));
            Assert.IsTrue(_digger.DigNextChar('_'));
            Assert.IsTrue(_digger.DigNextChar('i'));
            Assert.IsTrue(_digger.DigNextChar('d'));
            Assert.IsTrue(_digger.DigNextChar('_'));
            Assert.IsTrue(_digger.DigNextChar('s'));
            Assert.IsTrue(_digger.DigNextChar('t'));
            Assert.IsTrue(_digger.DigNextChar('r'));

            Assert.AreEqual(_digger.PointingItem, "in_reply_to_user_id_str");
            Assert.AreEqual(_digger.ItemValidLength, "in_reply_to_user_id_str".Length);

            _digger.Initialize();

            Assert.IsTrue(_digger.DigNextChar('i'));
            Assert.IsTrue(_digger.DigNextChar('n'));
            Assert.IsTrue(_digger.DigNextChar('_'));
            Assert.IsTrue(_digger.DigNextChar('r'));
            Assert.IsTrue(_digger.DigNextChar('e'));
            Assert.IsTrue(_digger.DigNextChar('p'));
            Assert.IsTrue(_digger.DigNextChar('l'));
            Assert.IsTrue(_digger.DigNextChar('y'));
            Assert.IsTrue(_digger.DigNextChar('_'));
            Assert.IsTrue(_digger.DigNextChar('t'));
            Assert.IsTrue(_digger.DigNextChar('o'));
            Assert.IsTrue(_digger.DigNextChar('_'));
            Assert.IsTrue(_digger.DigNextChar('s'));
            Assert.IsTrue(_digger.DigNextChar('t'));
            Assert.IsTrue(_digger.DigNextChar('a'));
            Assert.IsTrue(_digger.DigNextChar('t'));
            Assert.IsTrue(_digger.DigNextChar('u'));
            Assert.IsTrue(_digger.DigNextChar('s'));
            Assert.IsTrue(_digger.DigNextChar('_'));
            Assert.IsTrue(_digger.DigNextChar('i'));
            Assert.IsTrue(_digger.DigNextChar('d'));
            Assert.IsFalse(_digger.DigNextChar('_'));

            Assert.AreEqual(_digger.PointingItem, "in_reply_to_status_id");
            Assert.AreEqual(_digger.ItemValidLength, "in_reply_to_status_id".Length);
        }
    }
}
