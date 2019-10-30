
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Plugins;

namespace Neo.UnitTests.Plugins
{
    class MyPlugins : Plugin
    {
        public bool Disposed = false;

        public override void Configure()
        {
        }

        public override void Dispose()
        {
            Disposed = true;
        }
    }

    [TestClass]
    public class UI_Plugins
    {
        private static NeoSystem testBlockchain;

        [ClassInitialize]
        public static void TestSetup(TestContext ctx)
        {
            testBlockchain = TestBlockchain.InitializeMockNeoSystem();
        }

        [TestMethod]
        public void TestDispose()
        {
            MyPlugins myPlugins = new MyPlugins();
            Plugin.Plugins.Add(myPlugins);
            testBlockchain.Dispose();
            myPlugins.Disposed.Should().Be(true);
        }
    }
}
