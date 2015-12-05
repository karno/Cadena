using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Engine;
using Cadena.Engine.Requests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cadena.Test
{
    [TestClass]
    public class RequestEngineTest
    {
        public List<string> ResultList { get; } = new List<string>();


        [TestMethod]
        public async Task TestRequestEngineAsync()
        {
            var engine = new RequestEngine(2);
            var batch0 = new SequentialRequestBatch();
            var result0 = batch0.Enqueue(new TestRequest("0", this));
            var result1 = batch0.Enqueue(new TestRequest("1", this));
            var result2 = batch0.Enqueue(new TestRequest("2", this));
            var result3 = batch0.Enqueue(new TestRequest("3", this));
            var batch1 = new SequentialRequestBatch();
            var result4 = batch1.Enqueue(new TestRequest("4", this));
            var result5 = batch1.Enqueue(new TestRequest("5", this));
            var result6 = batch1.Enqueue(new TestRequest("6", this));
            var result7 = batch1.Enqueue(new TestRequest("7", this));
            var batch2 = new SequentialRequestBatch();
            var result8 = batch2.Enqueue(new TestRequest("8", this));
            var result9 = batch2.Enqueue(new TestRequest("9", this));
            var resulta = batch2.Enqueue(new TestRequest("a", this));
            var resultb = batch2.Enqueue(new TestRequest("b", this));
            var batch3 = new SequentialRequestBatch();
            var resultc = batch3.Enqueue(new TestRequest("c", this));
            var resultd = batch3.Enqueue(new TestRequest("d", this));
            var resulte = batch3.Enqueue(new TestRequest("e", this));
            var resultf = batch3.Enqueue(new TestRequest("f", this));
            var br1 = engine.SendRequest(batch0);
            var br2 = engine.SendRequest(batch1);
            var br3 = engine.SendRequest(batch2);
            var br4 = engine.SendRequest(batch3);
            await br1;
            await br2;
            await br3;
            await br4;
            foreach (var line in ResultList)
            {
                Debug.WriteLine(line);
            }
        }

    }

    public class TestRequest : RequestBase
    {
        private static Random rand = new Random();

        private readonly string _id;
        private readonly RequestEngineTest _parent;

        public TestRequest(string id, RequestEngineTest parent)
        {
            _id = id;
            _parent = parent;
        }

        public override async Task Send(CancellationToken token)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(rand.Next(300)), token);
            _parent.ResultList.Add(_id);
        }
    }
}
