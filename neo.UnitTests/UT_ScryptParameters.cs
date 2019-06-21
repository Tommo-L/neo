﻿using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.IO.Json;
using Neo.Wallets.NEP6;

namespace Neo.UnitTests
{
    [TestClass]
    public class UT_ScryptParameters
    {
        ScryptParameters uut;

        [TestInitialize]
        public void TestSetup()
        {
            uut = ScryptParameters.Default;
        }

        [TestMethod]
        public void Test_Default_ScryptParameters()
        {
            uut.N.Should().Be(16384);
            uut.R.Should().Be(8);
            uut.P.Should().Be(8);
        }

        [TestMethod]
        public void Test_ScryptParameters_Default_ToJson()
        {
            JObject json = ScryptParameters.Default.ToJson();
            json["n"].AsNumber().Should().Be(ScryptParameters.Default.N);
            json["r"].AsNumber().Should().Be(ScryptParameters.Default.R);
            json["p"].AsNumber().Should().Be(ScryptParameters.Default.P);
        }

        [TestMethod]
        public void Test_Default_ScryptParameters_FromJson()
        {
            JObject json = new JObject();
            json["n"] = 16384;
            json["r"] = 8;
            json["p"] = 8;

            ScryptParameters uut2 = ScryptParameters.FromJson(json);
            uut2.N.Should().Be(ScryptParameters.Default.N);
            uut2.R.Should().Be(ScryptParameters.Default.R);
            uut2.P.Should().Be(ScryptParameters.Default.P);
        }
    }
}
