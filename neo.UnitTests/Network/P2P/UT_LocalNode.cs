using Akka.Actor;
using Akka.IO;
using Akka.TestKit.Xunit2;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.IO;
using Neo.Network.P2P;
using Neo.Network.P2P.Capabilities;
using Neo.Network.P2P.Payloads;
using System.Linq;
using System.Net;
using Akka.TestKit;
using System.Collections.Generic;

namespace Neo.UnitTests.Network.P2P
{
    [TestClass]
    public class UT_LocalNode : TestKit
    {
        private static NeoSystem testBlockchain;

        [ClassInitialize]
        public static void TestSetup(TestContext ctx)
        {
            testBlockchain = TestBlockchain.InitializeMockNeoSystem();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // clear all remote nodes in LocalNode
            var testProbe = CreateTestProbe();
            foreach (var remoteActor in LocalNode.Singleton.RemoteNodes.Keys)
            {
                testProbe.Send(remoteActor, new Tcp.ConnectionClosed());
            }
        }

        [TestMethod]
        public void Test_GetRandomConnectedPeers()
        {
            var localNode = testBlockchain.LocalNode;
            var local = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);
            var remote = new IPEndPoint(IPAddress.Parse("133.133.133.133"), 8080);
            var connected = new Tcp.Connected(remote, local);

            // create a remote node connection by sending tcp.connected message
            var testProbe = CreateTestProbe();
            testProbe.Send(localNode, connected);
            testProbe.ExpectMsg<Tcp.Register>(); // register msg is earlier than version message
            testProbe.ExpectMsg<Tcp.Write>();    // remote ndoe send version message

            // send version to remote node
            var payload = new VersionPayload()
            {
                UserAgent = "Unit Test".PadLeft(1024, '0'),
                Nonce = 1,
                Magic = ProtocolSettings.Default.Magic,
                Timestamp = 5,
                Version = 6,
                Capabilities = new NodeCapability[]
               {
                    new ServerCapability(NodeCapabilityType.TcpServer, 8080)
               }
            };
            var message = Message.Create(MessageCommand.Version, payload);
            var versionReceived = new Tcp.Received(ByteString.FromBytes(message.ToArray()));
            var remoteNodeActor = LocalNode.Singleton.RemoteNodes.Keys.First();
            testProbe.Send(remoteNodeActor, versionReceived);
            testProbe.ExpectMsg<Tcp.Write>(); // remote node will send verack and change its listenerPort

            // check connected peers
            int connectedCount = LocalNode.Singleton.ConnectedCount;
            NetworkAddressWithTime[] addressWithTimes = LocalNode.Singleton.GetRandomConnectedPeers(connectedCount);
            addressWithTimes.Where(p => p.EndPoint.Equals(remote)).Count().Should().Be(1);  // `remote` must be contained

        }

        [TestMethod]
        public void Test_Peer_Max_Per_Address_Connection_Reached()
        {
            var localNode = testBlockchain.LocalNode;
            var local = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);

            // Create the maximum number of per address connections
            IPEndPoint remote = null;
            Tcp.Connected connected = null;
            var senderDict = new Dictionary<IPEndPoint, TestProbe>();
            for (int i = 1; i <= LocalNode.Singleton.MaxConnectionsPerAddress; i++)
            {
                remote = new IPEndPoint(IPAddress.Parse("192.168.1.1"), 8080 + i);
                connected = new Tcp.Connected(remote, local);

                var proble = CreateTestProbe();
                proble.Send(localNode, connected);
                proble.ExpectMsg<Tcp.Register>(); // register msg is earlier than version msg
                proble.ExpectMsg<Tcp.Write>();    // remote ndoe send version msg

                senderDict[remote] = proble;
            }

            // send version message to one remote node, which will contain listenerport
            // and can be selected in `GetRandomConnectedPeers`
            var payload = new VersionPayload()
            {
                UserAgent = "Unit Test".PadLeft(1024, '0'),
                Nonce = 1,
                Magic = ProtocolSettings.Default.Magic,
                Timestamp = 5,
                Version = 6,
                Capabilities = new NodeCapability[]
               {
                    new ServerCapability(NodeCapabilityType.TcpServer, 8080)
               }
            };
            var message = Message.Create(MessageCommand.Version, payload);
            var versionReceived = new Tcp.Received(ByteString.FromBytes(message.ToArray()));
            var remoteAddr = LocalNode.Singleton.RemoteNodes.Values.First().Remote;
            var remoteNodeActor = LocalNode.Singleton.RemoteNodes.Keys.First();
            var testProbe = senderDict[remoteAddr];
            testProbe.Send(remoteNodeActor, versionReceived);
            testProbe.ExpectMsg<Tcp.Write>(); // remote node will send verack and change its listenerPort

            // create one more remote connection and localnode will disconnect with `MaxPerAddressConnectionReached`
            remote = new IPEndPoint(IPAddress.Parse("192.168.1.1"), 8079);
            connected = new Tcp.Connected(remote, local);
            testProbe.Send(localNode, connected);

            var tcpWrite = testProbe.ExpectMsg<Tcp.Write>();
            message = tcpWrite.Data.ToArray().AsSerializable<Message>();
            message.Command.Should().Be(MessageCommand.Disconnect);

            var disconnectionPayload = (DisconnectPayload)message.Payload;
            disconnectionPayload.Reason.Should().Be(DisconnectReason.MaxPerAddressConnectionReached);

            // return addr
            var count = DisconnectPayload.MaxDataSize;
            NetworkAddressWithTime[] addrs = disconnectionPayload.Data.AsSerializableArray<NetworkAddressWithTime>(count);
            addrs.Length.Should().Be(1);
            addrs[0].EndPoint.Address.Should().BeEquivalentTo(IPAddress.Parse("192.168.1.1"));

            testProbe.ExpectMsg<Tcp.Close>();
        }

        [TestMethod]
        public void Test_Peer_MaxConnection_Reached()
        {
            var localNode = testBlockchain.LocalNode;
            var local = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);

            // Create the maximum number of connections
            IPEndPoint remote = null;
            Tcp.Connected connected = null;
            var senderDict = new Dictionary<IPEndPoint, TestProbe>();
            for (int i = 1; i <= LocalNode.Singleton.MaxConnections; i++)
            {
                remote = new IPEndPoint(IPAddress.Parse("192.168.1." + i), 8080);
                connected = new Tcp.Connected(remote, local);
                var proble = CreateTestProbe();
                proble.Send(localNode, connected);
                proble.ExpectMsg<Tcp.Register>(); // register msg is earlier than version msg
                proble.ExpectMsg<Tcp.Write>();    // remote ndoe send version msg

                senderDict[remote] = proble;
            }

            // send version message to one remote node, which will contain listenerport
            // and can be selected in `GetRandomConnectedPeers`
            var payload = new VersionPayload()
            {
                UserAgent = "Unit Test".PadLeft(1024, '0'),
                Nonce = 1,
                Magic = ProtocolSettings.Default.Magic,
                Timestamp = 5,
                Version = 6,
                Capabilities = new NodeCapability[]
               {
                    new ServerCapability(NodeCapabilityType.TcpServer, 8080)
               }
            };
            var versionReceived = new Tcp.Received(ByteString.FromBytes(Message.Create(MessageCommand.Version, payload).ToArray()));
            var remoteAddr = LocalNode.Singleton.RemoteNodes.Values.First().Remote;
            var remoteNodeActor = LocalNode.Singleton.RemoteNodes.Keys.First();
            var testProbe = senderDict[remoteAddr];

            testProbe.Send(remoteNodeActor, versionReceived);
            testProbe.ExpectMsg<Tcp.Write>(); // remote node will send verack and change its listenerPort

            // create one more remote connection and localnode will disconnect with `MaxConnectionReached`
            remote = new IPEndPoint(IPAddress.Parse("192.168.2.1"), 8080);
            connected = new Tcp.Connected(remote, local);
            testProbe.Send(localNode, connected);

            var tcpWrite = testProbe.ExpectMsg<Tcp.Write>();
            Message message = tcpWrite.Data.ToArray().AsSerializable<Message>();
            message.Command.Should().Be(MessageCommand.Disconnect);

            var disconnectionPayload = (DisconnectPayload)message.Payload;
            disconnectionPayload.Reason.Should().Be(DisconnectReason.MaxConnectionReached);

            // return addr
            var count = DisconnectPayload.MaxDataSize;
            NetworkAddressWithTime[] addrs = disconnectionPayload.Data.AsSerializableArray<NetworkAddressWithTime>(count);
            addrs.Length.Should().Be(1);
            addrs[0].EndPoint.Port.Should().Be(8080);

            testProbe.ExpectMsg<Tcp.Close>();
        }

        [TestMethod]
        public void Test_Received_MaxConnectionReached_Disconnection()
        {
            //send MaxConnectionReached disconnection
            NetworkAddressWithTime[] addressWithTimes = new NetworkAddressWithTime[]
            {
                new NetworkAddressWithTime()
                {
                    Timestamp = 0,
                    Address = IPAddress.Parse("192.167.1.1"),
                    Capabilities = new NodeCapability[]
                    {
                        new ServerCapability(NodeCapabilityType.TcpServer, 8080)
                    }
                }
            };

            var payload = DisconnectPayload.Create(DisconnectReason.MaxConnectionReached, "The maximum number of connections reached!", addressWithTimes.ToByteArray());
            var testProbe = CreateTestProbe();
            testProbe.Send(testBlockchain.LocalNode, payload);

            testProbe.ExpectNoMsg();
            LocalNode.Singleton.GetUnconnectedPeers().Any(p => p.Equals(new IPEndPoint(IPAddress.Parse("192.167.1.1"), 8080))).Should().BeTrue();
        }
    }
}