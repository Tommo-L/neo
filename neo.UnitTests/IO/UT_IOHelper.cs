using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Neo.UnitTests.IO
{
    [TestClass]
    public class UT_IOHelper
    {
        [TestMethod]
        public void TestAsSerializableGeneric()
        {
            byte[] caseArray = new byte[] { 0x00,0x00,0x00,0x00,0x00,
                                            0x00,0x00,0x00,0x00,0x00,
                                            0x00,0x00,0x00,0x00,0x00,
                                            0x00,0x00,0x00,0x00,0x00 };
            UInt160 result = Neo.IO.Helper.AsSerializable<UInt160>(caseArray);
            Assert.AreEqual(UInt160.Zero, result);
        }

        [TestMethod]
        public void TestAsSerializable()
        {
            for (int i = 0; i < 2; i++)
            {
                if (i == 0)
                {
                    byte[] caseArray = new byte[] { 0x00,0x00,0x00,0x00,0x00,
                                                    0x00,0x00,0x00,0x00,0x00,
                                                    0x00,0x00,0x00,0x00,0x00,
                                                    0x00,0x00,0x00,0x00,0x00 };
                    ISerializable result = Neo.IO.Helper.AsSerializable(caseArray, typeof(UInt160));
                    Assert.AreEqual(UInt160.Zero, result);
                }
                else
                {
                    Action action = () => Neo.IO.Helper.AsSerializable(new byte[0], typeof(Double));
                    action.Should().Throw<InvalidCastException>();
                }
            }
        }

        [TestMethod]
        public void TestAsSerializableArray()
        {
            byte[] byteArray = Neo.IO.Helper.ToByteArray<UInt160>(new UInt160[] { UInt160.Zero });
            UInt160[] result = Neo.IO.Helper.AsSerializableArray<UInt160>(byteArray);
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(UInt160.Zero, result[0]);
        }

        [TestMethod]
        public void TestGetVarSizeInt()
        {
            for (int i = 0; i < 3; i++)
            {
                if (i == 0)
                {
                    int result = Neo.IO.Helper.GetVarSize(1);
                    Assert.AreEqual(1, result);
                }
                else if (i == 1)
                {
                    int result = Neo.IO.Helper.GetVarSize(0xFFFF);
                    Assert.AreEqual(3, result);
                }
                else
                {
                    int result = Neo.IO.Helper.GetVarSize(0xFFFFFF);
                    Assert.AreEqual(5, result);
                }
            }
        }
        enum TestEnum0 : sbyte
        {
            case1 = 1, case2 = 2
        }

        enum TestEnum1 : byte
        {
            case1 = 1, case2 = 2
        }

        enum TestEnum2 : short
        {
            case1 = 1, case2 = 2
        }

        enum TestEnum3 : ushort
        {
            case1 = 1, case2 = 2
        }

        enum TestEnum4 : int
        {
            case1 = 1, case2 = 2
        }

        enum TestEnum5 : uint
        {
            case1 = 1, case2 = 2
        }

        enum TestEnum6 : long
        {
            case1 = 1, case2 = 2
        }

        [TestMethod]
        public void TestGetVarSizeGeneric()
        {
            for (int i = 0; i < 9; i++)
            {
                if (i == 0)
                {
                    int result = Neo.IO.Helper.GetVarSize(new UInt160[] { UInt160.Zero });
                    Assert.AreEqual(21, result);
                }
                else if (i == 1)//sbyte
                {
                    List<TestEnum0> initList = new List<TestEnum0>
                    {
                        TestEnum0.case1
                    };
                    IReadOnlyCollection<TestEnum0> testList = initList.AsReadOnly();
                    int result = Neo.IO.Helper.GetVarSize(testList);
                    Assert.AreEqual(2, result);
                }
                else if (i == 2)//byte
                {
                    List<TestEnum1> initList = new List<TestEnum1>
                    {
                        TestEnum1.case1
                    };
                    IReadOnlyCollection<TestEnum1> testList = initList.AsReadOnly();
                    int result = Neo.IO.Helper.GetVarSize(testList);
                    Assert.AreEqual(2, result);
                }
                else if (i == 3)//short
                {
                    List<TestEnum2> initList = new List<TestEnum2>
                    {
                        TestEnum2.case1
                    };
                    IReadOnlyCollection<TestEnum2> testList = initList.AsReadOnly();
                    int result = Neo.IO.Helper.GetVarSize(testList);
                    Assert.AreEqual(3, result);
                }
                else if (i == 4)//ushort
                {
                    List<TestEnum3> initList = new List<TestEnum3>
                    {
                        TestEnum3.case1
                    };
                    IReadOnlyCollection<TestEnum3> testList = initList.AsReadOnly();
                    int result = Neo.IO.Helper.GetVarSize(testList);
                    Assert.AreEqual(3, result);
                }
                else if (i == 5)//int
                {
                    List<TestEnum4> initList = new List<TestEnum4>
                    {
                        TestEnum4.case1
                    };
                    IReadOnlyCollection<TestEnum4> testList = initList.AsReadOnly();
                    int result = Neo.IO.Helper.GetVarSize(testList);
                    Assert.AreEqual(5, result);
                }
                else if (i == 6)//uint
                {
                    List<TestEnum5> initList = new List<TestEnum5>
                    {
                        TestEnum5.case1
                    };
                    IReadOnlyCollection<TestEnum5> testList = initList.AsReadOnly();
                    int result = Neo.IO.Helper.GetVarSize(testList);
                    Assert.AreEqual(5, result);
                }
                else if (i == 7)//long
                {
                    List<TestEnum6> initList = new List<TestEnum6>
                    {
                        TestEnum6.case1
                    };
                    IReadOnlyCollection<TestEnum6> testList = initList.AsReadOnly();
                    int result = Neo.IO.Helper.GetVarSize(testList);
                    Assert.AreEqual(9, result);
                }
                else if (i == 8)
                {
                    List<int> initList = new List<int>
                    {
                        1
                    };
                    IReadOnlyCollection<int> testList = initList.AsReadOnly();
                    int result = Neo.IO.Helper.GetVarSize<int>(testList);
                    Assert.AreEqual(5, result);
                }
            }
        }

        [TestMethod]
        public void TestGetVarSizeString()
        {
            int result = Neo.IO.Helper.GetVarSize("AA");
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void TestReadBytesWithGrouping()
        {
            for (int i = 0; i < 2; i++)
            {
                if (i == 0)
                {
                    byte[] caseArray = new byte[] { 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA,
                                                    0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA,
                                                    0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA,
                                                    0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA,
                                                    0xAA, 0xAA, 0xAA, 0xAA};
                    MemoryStream stream = new MemoryStream();
                    BinaryWriter writer = new BinaryWriter(stream);
                    Neo.IO.Helper.WriteBytesWithGrouping(writer, caseArray);
                    stream.Seek(0, SeekOrigin.Begin);
                    BinaryReader reader = new BinaryReader(stream);
                    byte[] result = Neo.IO.Helper.ReadBytesWithGrouping(reader);
                    Assert.AreEqual(Encoding.Default.GetString(caseArray), Encoding.Default.GetString(result));
                }
                else
                {
                    byte[] caseArray = new byte[] { 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA,
                                                    0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA,0x10,
                                                    0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA,
                                                    0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA,0x10,
                                                    0xAA, 0xAA, 0xAA, 0xAA, 0x00, 0x00, 0x00, 0x00,
                                                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,0x11};
                    MemoryStream stream = new MemoryStream();
                    BinaryWriter writer = new BinaryWriter(stream);
                    writer.Write(caseArray);
                    stream.Seek(0, SeekOrigin.Begin);
                    BinaryReader reader = new BinaryReader(stream);
                    Action action = () => Neo.IO.Helper.ReadBytesWithGrouping(reader);
                    action.Should().Throw<FormatException>();
                }
            }
        }

        [TestMethod]
        public void TestReadFixedString()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            Neo.IO.Helper.WriteFixedString(writer, "AA", Encoding.UTF8.GetBytes("AA").Length + 1);
            stream.Seek(0, SeekOrigin.Begin);
            BinaryReader reader = new BinaryReader(stream);
            string result = Neo.IO.Helper.ReadFixedString(reader, Encoding.UTF8.GetBytes("AA").Length + 1);
            Assert.AreEqual("AA", result);
        }

        [TestMethod]
        public void TestReadSerializable()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            Neo.IO.Helper.Write(writer, UInt160.Zero);
            stream.Seek(0, SeekOrigin.Begin);
            BinaryReader reader = new BinaryReader(stream);
            UInt160 result = Neo.IO.Helper.ReadSerializable<UInt160>(reader);
            Assert.AreEqual(UInt160.Zero, result);
        }

        [TestMethod]
        public void TestReadSerializableArray()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            Neo.IO.Helper.Write(writer, new UInt160[] { UInt160.Zero });
            stream.Seek(0, SeekOrigin.Begin);
            BinaryReader reader = new BinaryReader(stream);
            UInt160[] resultArray = Neo.IO.Helper.ReadSerializableArray<UInt160>(reader);
            Assert.AreEqual(1, resultArray.Length);
            Assert.AreEqual(UInt160.Zero, resultArray[0]);
        }

        [TestMethod]
        public void TestReadVarBytes()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            Neo.IO.Helper.WriteVarBytes(writer, new byte[] { 0xAA, 0xAA });
            stream.Seek(0, SeekOrigin.Begin);
            BinaryReader reader = new BinaryReader(stream);
            byte[] byteArray = Neo.IO.Helper.ReadVarBytes(reader, 10);
            Assert.AreEqual(Encoding.Default.GetString(new byte[] { 0xAA, 0xAA }), Encoding.Default.GetString(byteArray));
        }

        [TestMethod]
        public void TestReadVarInt()
        {
            for (int i = 0; i < 4; i++)
            {
                if (i == 0)
                {
                    MemoryStream stream = new MemoryStream();
                    BinaryWriter writer = new BinaryWriter(stream);
                    Neo.IO.Helper.WriteVarInt(writer, 0xFFFF);
                    stream.Seek(0, SeekOrigin.Begin);
                    BinaryReader reader = new BinaryReader(stream);
                    ulong result = Neo.IO.Helper.ReadVarInt(reader, 0xFFFF);
                    Assert.AreEqual((ulong)0xFFFF, result);
                }
                else if (i == 1)
                {
                    MemoryStream stream = new MemoryStream();
                    BinaryWriter writer = new BinaryWriter(stream);
                    Neo.IO.Helper.WriteVarInt(writer, 0xFFFFFFFF);
                    stream.Seek(0, SeekOrigin.Begin);
                    BinaryReader reader = new BinaryReader(stream);
                    ulong result = Neo.IO.Helper.ReadVarInt(reader, 0xFFFFFFFF);
                    Assert.AreEqual(0xFFFFFFFF, result);
                }
                else
                {
                    MemoryStream stream = new MemoryStream();
                    BinaryWriter writer = new BinaryWriter(stream);
                    Neo.IO.Helper.WriteVarInt(writer, 0xFFFFFFFFFF);
                    stream.Seek(0, SeekOrigin.Begin);
                    BinaryReader reader = new BinaryReader(stream);
                    Action action = () => Neo.IO.Helper.ReadVarInt(reader, 0xFFFFFFFF);
                    action.Should().Throw<FormatException>();
                }
            }
        }

        [TestMethod]
        public void TestReadVarString()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            Neo.IO.Helper.WriteVarString(writer, "AAAAAAA");
            stream.Seek(0, SeekOrigin.Begin);
            BinaryReader reader = new BinaryReader(stream);
            string result = Neo.IO.Helper.ReadVarString(reader, 10);
            stream.Seek(0, SeekOrigin.Begin);
            Assert.AreEqual("AAAAAAA", result);
        }

        [TestMethod]
        public void TestToArray()
        {
            byte[] byteArray = Neo.IO.Helper.ToArray(UInt160.Zero);
            Assert.AreEqual(Encoding.Default.GetString(new byte[] { 0x00,0x00,0x00,0x00,0x00,
                                                                    0x00,0x00,0x00,0x00,0x00,
                                                                    0x00,0x00,0x00,0x00,0x00,
                                                                    0x00,0x00,0x00,0x00,0x00}), Encoding.Default.GetString(byteArray));
        }

        [TestMethod]
        public void TestToByteArrayGeneric()
        {
            byte[] byteArray = Neo.IO.Helper.ToByteArray(new UInt160[] { UInt160.Zero });
            Assert.AreEqual(Encoding.Default.GetString(new byte[] { 0x01,0x00,0x00,0x00,0x00,0x00,
                                                                         0x00,0x00,0x00,0x00,0x00,
                                                                         0x00,0x00,0x00,0x00,0x00,
                                                                         0x00,0x00,0x00,0x00,0x00}), Encoding.Default.GetString(byteArray));
        }

        [TestMethod]
        public void TestWrite()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            Neo.IO.Helper.Write(writer, UInt160.Zero);
            stream.Seek(0, SeekOrigin.Begin);
            byte[] byteArray = new byte[stream.Length];
            stream.Read(byteArray, 0, (int)stream.Length);
            Assert.AreEqual(Encoding.Default.GetString(new byte[] { 0x00,0x00,0x00,0x00,0x00,
                                                                    0x00,0x00,0x00,0x00,0x00,
                                                                    0x00,0x00,0x00,0x00,0x00,
                                                                    0x00,0x00,0x00,0x00,0x00}), Encoding.Default.GetString(byteArray));
        }

        [TestMethod]
        public void TestWriteGeneric()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            Neo.IO.Helper.Write(writer, new UInt160[] { UInt160.Zero });
            stream.Seek(0, SeekOrigin.Begin);
            byte[] byteArray = new byte[stream.Length];
            stream.Read(byteArray, 0, (int)stream.Length);
            Assert.AreEqual(Encoding.Default.GetString(new byte[] { 0x01,0x00,0x00,0x00,0x00,0x00,
                                                                         0x00,0x00,0x00,0x00,0x00,
                                                                         0x00,0x00,0x00,0x00,0x00,
                                                                         0x00,0x00,0x00,0x00,0x00}), Encoding.Default.GetString(byteArray));
        }


        [TestMethod]
        public void TestWriteBytesWithGrouping()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            Neo.IO.Helper.WriteBytesWithGrouping(writer, new byte[] { 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA,
                                                                      0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA,
                                                                      0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA,
                                                                      0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA,
                                                                      0xAA, 0xAA, 0xAA, 0xAA});
            stream.Seek(0, SeekOrigin.Begin);
            byte[] byteArray = new byte[stream.Length];
            stream.Read(byteArray, 0, (int)stream.Length);
            Assert.AreEqual(Encoding.Default.GetString(new byte[] { 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA,
                                                                    0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA,0x10,
                                                                    0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA,
                                                                    0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA,0x10,
                                                                    0xAA, 0xAA, 0xAA, 0xAA, 0x00, 0x00, 0x00, 0x00,
                                                                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,0x04}), Encoding.Default.GetString(byteArray));
        }

        [TestMethod]
        public void TestWriteFixedString()
        {
            for (int i = 0; i < 5; i++)
            {
                if (i == 0)
                {
                    MemoryStream stream = new MemoryStream();
                    BinaryWriter writer = new BinaryWriter(stream);
                    Action action = () => Neo.IO.Helper.WriteFixedString(writer, null, 0);
                    action.Should().Throw<ArgumentNullException>();
                }
                else if (i == 1)
                {
                    MemoryStream stream = new MemoryStream();
                    BinaryWriter writer = new BinaryWriter(stream);
                    Action action = () => Neo.IO.Helper.WriteFixedString(writer, "AA", Encoding.UTF8.GetBytes("AA").Length - 1);
                    action.Should().Throw<ArgumentException>();
                }
                else if (i == 2)
                {
                    MemoryStream stream = new MemoryStream();
                    BinaryWriter writer = new BinaryWriter(stream);
                    Action action = () => Neo.IO.Helper.WriteFixedString(writer, "拉拉", Encoding.UTF8.GetBytes("拉拉").Length - 1);
                    action.Should().Throw<ArgumentException>();
                }
                else if (i == 3)
                {
                    MemoryStream stream = new MemoryStream();
                    BinaryWriter writer = new BinaryWriter(stream);
                    Neo.IO.Helper.WriteFixedString(writer, "AA", Encoding.UTF8.GetBytes("AA").Length + 1);
                    stream.Seek(0, SeekOrigin.Begin);
                    byte[] byteArray = new byte[stream.Length];
                    stream.Read(byteArray, 0, (int)stream.Length);
                    byte[] newArray = new byte[Encoding.UTF8.GetBytes("AA").Length + 1];
                    Encoding.UTF8.GetBytes("AA").CopyTo(newArray, 0);
                    Assert.AreEqual(Encoding.Default.GetString(newArray), Encoding.Default.GetString(byteArray));
                }
            }
        }

        [TestMethod]
        public void TestWriteVarBytes()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            Neo.IO.Helper.WriteVarBytes(writer, new byte[] { 0xAA });
            stream.Seek(0, SeekOrigin.Begin);
            byte[] byteArray = new byte[stream.Length];
            stream.Read(byteArray, 0, (int)stream.Length);
            Assert.AreEqual(Encoding.Default.GetString(new byte[] { 0x01, 0xAA }), Encoding.Default.GetString(byteArray));
        }

        [TestMethod]
        public void TestWriteVarInt()
        {
            for (int i = 0; i < 5; i++)
            {
                if (i == 0)
                {
                    MemoryStream stream = new MemoryStream();
                    BinaryWriter writer = new BinaryWriter(stream);
                    Action action = () => Neo.IO.Helper.WriteVarInt(writer, -1);
                    action.Should().Throw<ArgumentOutOfRangeException>();
                }
                else if (i == 1)
                {
                    MemoryStream stream = new MemoryStream();
                    BinaryWriter writer = new BinaryWriter(stream);
                    Neo.IO.Helper.WriteVarInt(writer, 0xFC);
                    stream.Seek(0, SeekOrigin.Begin);
                    byte[] byteArray = new byte[stream.Length];
                    stream.Read(byteArray, 0, (int)stream.Length);
                    Assert.AreEqual(0xFC, byteArray[0]);
                }
                else if (i == 2)
                {
                    MemoryStream stream = new MemoryStream();
                    BinaryWriter writer = new BinaryWriter(stream);
                    Neo.IO.Helper.WriteVarInt(writer, 0xFFFF);
                    stream.Seek(0, SeekOrigin.Begin);
                    byte[] byteArray = new byte[stream.Length];
                    stream.Read(byteArray, 0, (int)stream.Length);
                    Assert.AreEqual(0xFD, byteArray[0]);
                    Assert.AreEqual(Encoding.Default.GetString(new byte[] { 0xFF, 0xFF }), Encoding.Default.GetString(byteArray.Skip(1).Take(byteArray.Length - 1).ToArray()));
                }
                else if (i == 3)
                {
                    MemoryStream stream = new MemoryStream();
                    BinaryWriter writer = new BinaryWriter(stream);
                    Neo.IO.Helper.WriteVarInt(writer, 0xFFFFFFFF);
                    stream.Seek(0, SeekOrigin.Begin);
                    byte[] byteArray = new byte[stream.Length];
                    stream.Read(byteArray, 0, (int)stream.Length);
                    Assert.AreEqual(0xFE, byteArray[0]);
                    Assert.AreEqual(0xFFFFFFFF, BitConverter.ToUInt32(byteArray, 1));
                }
                else
                {
                    MemoryStream stream = new MemoryStream();
                    BinaryWriter writer = new BinaryWriter(stream);
                    Neo.IO.Helper.WriteVarInt(writer, 0xAEFFFFFFFF);
                    stream.Seek(0, SeekOrigin.Begin);
                    byte[] byteArray = new byte[stream.Length];
                    stream.Read(byteArray, 0, (int)stream.Length);
                    Assert.AreEqual(0xFF, byteArray[0]);
                    Assert.AreEqual(Encoding.Default.GetString(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00 }), Encoding.Default.GetString(byteArray.Skip(1).Take(byteArray.Length - 1).ToArray()));
                }
            }
        }

        [TestMethod]
        public void TestWriteVarString()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            Neo.IO.Helper.WriteVarString(writer, "a");
            stream.Seek(0, SeekOrigin.Begin);
            byte[] byteArray = new byte[stream.Length];
            stream.Read(byteArray, 0, (int)stream.Length);
            Assert.AreEqual(0x01, byteArray[0]);
            Assert.AreEqual(0x61, byteArray[1]);
        }
    }
}