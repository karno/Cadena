using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Cadena.Engine._Internals;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Cadena.Test
{
    /// <summary>
    /// TaskFactoryDistrictTest の概要の説明
    /// </summary>
    [TestClass]
    public class TaskFactoryDistrictTest
    {
        private readonly TaskFactoryDistrict _district;
        private readonly List<string> _resultList;

        public TaskFactoryDistrictTest()
        {
            _resultList = new List<string>();
            _district = new TaskFactoryDistrict(3);
        }

        private TestContext _testContextInstance;

        /// <summary>
        ///現在のテストの実行についての情報および機能を
        ///提供するテスト コンテキストを取得または設定します。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return _testContextInstance;
            }
            set
            {
                _testContextInstance = value;
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
        public async Task TestTaskFactoryDistrict()
        {
            var tasks = new[]{
                _district.Run(() => Debug.WriteLine("DummyStick"), 0),
                _district.Run(() => WaitAndEnqueue(10, "0"), 0),
                _district.Run(() => WaitAndEnqueue(15, "1"), 0),
                _district.Run(() => WaitAndEnqueue(20, "2"), 0),

                _district.Run(() => WaitAndEnqueue(20, "6"), 1),
                _district.Run(() => WaitAndEnqueue(20, "7"), 1),
                _district.Run(() => WaitAndEnqueue(20, "8"), 1),

                _district.Run(() => WaitAndEnqueue(20, "3"), 0),
                _district.Run(() => WaitAndEnqueue(20, "4"), 0),
                _district.Run(() => WaitAndEnqueue(20, "5"), 0),

                _district.Run(() => WaitAndEnqueue(20, "9"), 2),
                _district.Run(() => WaitAndEnqueue(20, "a"), 2),
                _district.Run(() => WaitAndEnqueue(20, "b"), 2),
            };
            await Task.WhenAny(Task.WhenAll(tasks), Task.Delay(TimeSpan.FromSeconds(20)));
            var success = "0123456789ab";
            foreach (var str in _resultList)
            {
                Debug.WriteLine(str);
            }
            Assert.AreEqual(success.Length, _resultList.Count);
            CollectionAssert.AreEqual(success.Select(s => s.ToString()).ToArray(), _resultList.ToArray());
        }

        public async Task WaitAndEnqueue(int wait, string msg)
        {
            Debug.WriteLine("* executing " + msg);
            await Task.Delay(wait * 10);
            Debug.WriteLine("* completed " + msg);
            lock (_resultList)
            {
                _resultList.Add(msg);
            }
        }

    }
}
