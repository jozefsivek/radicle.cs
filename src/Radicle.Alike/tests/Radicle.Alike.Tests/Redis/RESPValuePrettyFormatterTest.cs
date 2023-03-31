namespace Radicle.Alike.Redis;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Radicle.Alike.Redis.Models;
using Xunit;

public class RESPValuePrettyFormatterTest
{
    [Theory]
    [InlineData(0, new string[] { "(big integer) 0" })]
    [InlineData(long.MaxValue, new string[] { "(big integer) 9223372036854775807" })]
    [InlineData(long.MinValue, new string[] { "(big integer) -9223372036854775808" })]
    public void BigNumber_Plain_Returns(
            long number,
            string[] expected)
    {
        Assert.Equal(
                expected,
                new RESPBigNumber(number).Accept(new RESPValuePrettyFormatter()));
    }

    [Fact]
    public void BigNumber_Large_Returns()
    {
        Assert.Equal(
                new string[] { "(big integer) 18446744073709551614" },
                new RESPBigNumber(new BigInteger(long.MaxValue) + new BigInteger(long.MaxValue))
                    .Accept(new RESPValuePrettyFormatter()));
    }

    [Theory]
    [InlineData(0, new string[] { "(big integer) 0*" })]
    [InlineData(long.MaxValue, new string[] { "(big integer) 9223372036854775807*" })]
    [InlineData(long.MinValue, new string[] { "(big integer) -9223372036854775808*" })]
    public void BigNumber_WithAttribute_Returns(
            long number,
            string[] expected)
    {
        Assert.Equal(
                expected,
                new RESPBigNumber(number)
                {
                    Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
                    {
                        { new RESPSimpleString("foo"), new RESPSimpleString("Bar") },
                    }),
                }.Accept(new RESPValuePrettyFormatter()));
    }

    [Theory]
    [InlineData("Foo", new string[] { "\"Foo\"" })]
    [InlineData("foo\nbar", new string[] { "\"foo\\nbar\"" })]
    [InlineData("foo\r\nbar", new string[] { "\"foo\\r\\nbar\"" })]
    [InlineData("foo\tbar", new string[] { "\"foo\\tbar\"" })]
    [InlineData("foo\bbar", new string[] { "\"foo\\bbar\"" })]
    public void BlobError_String_Returns(
            string str,
            string[] expected)
    {
        Assert.Equal(
                expected,
                new RESPBlobError(str).Accept(new RESPValuePrettyFormatter()));
    }

    [Theory]
    [InlineData(new byte[] { 0 }, new string[] { "\"\\x00\"" })]
    [InlineData(new byte[] { 2 }, new string[] { "\"\\x02\"" })]
    public void BlobError_Binary_Returns(
            byte[] value,
            string[] expected)
    {
        Assert.Equal(
                expected,
                new RESPBlobError(value).Accept(new RESPValuePrettyFormatter()));
    }

    [Theory]
    [InlineData("Foo", new string[] { "\"Foo\"", "[*]" })]
    [InlineData("foo\nbar", new string[] { "\"foo\\nbar\"", "[*]" })]
    [InlineData("foo\r\nbar", new string[] { "\"foo\\r\\nbar\"", "[*]" })]
    [InlineData("foo\tbar", new string[] { "\"foo\\tbar\"", "[*]" })]
    [InlineData("foo\bbar", new string[] { "\"foo\\bbar\"", "[*]" })]
    public void BlobError_StringWithAttribute_Returns(
            string str,
            string[] expected)
    {
        Assert.Equal(
                expected,
                new RESPBlobError(str)
                {
                    Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
                    {
                        { new RESPSimpleString("foo"), new RESPSimpleString("Bar") },
                    }),
                }.Accept(new RESPValuePrettyFormatter()));
    }

    [Theory]
    [InlineData("Foo", new string[] { "\"Foo\"" })]
    [InlineData("foo\nbar", new string[] { "\"foo\\nbar\"" })]
    [InlineData("foo\r\nbar", new string[] { "\"foo\\r\\nbar\"" })]
    [InlineData("foo\tbar", new string[] { "\"foo\\tbar\"" })]
    [InlineData("foo\bbar", new string[] { "\"foo\\bbar\"" })]
    public void BlobString_String_Returns(
            string str,
            string[] expected)
    {
        Assert.Equal(
                expected,
                new RESPBlobString(str).Accept(new RESPValuePrettyFormatter()));
    }

    [Theory]
    [InlineData(new byte[] { 0 }, new string[] { "\"\\x00\"" })]
    [InlineData(new byte[] { 2 }, new string[] { "\"\\x02\"" })]
    public void BlobString_Binary_Returns(
            byte[] value,
            string[] expected)
    {
        Assert.Equal(
                expected,
                new RESPBlobString(value).Accept(new RESPValuePrettyFormatter()));
    }

    [Theory]
    [InlineData("Foo", new string[] { "\"Foo\"", "[*]" })]
    [InlineData("foo\nbar", new string[] { "\"foo\\nbar\"", "[*]" })]
    [InlineData("foo\r\nbar", new string[] { "\"foo\\r\\nbar\"", "[*]" })]
    [InlineData("foo\tbar", new string[] { "\"foo\\tbar\"", "[*]" })]
    [InlineData("foo\bbar", new string[] { "\"foo\\bbar\"", "[*]" })]
    public void BlobString_StringWithAttribute_Returns(
            string str,
            string[] expected)
    {
        Assert.Equal(
                expected,
                new RESPBlobString(str)
                {
                    Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
                    {
                        { new RESPSimpleString("foo"), new RESPSimpleString("Bar") },
                    }),
                }.Accept(new RESPValuePrettyFormatter()));
    }

    [Theory]
    [InlineData(true, new string[] { "(true)" })]
    [InlineData(false, new string[] { "(false)" })]
    public void Boolean_Plain_Returns(
            bool value,
            string[] expected)
    {
        Assert.Equal(
                expected,
                new RESPBoolean(value).Accept(new RESPValuePrettyFormatter()));
    }

    [Theory]
    [InlineData(true, new string[] { "(true)*" })]
    [InlineData(false, new string[] { "(false)*" })]
    public void Boolean_WithAttribute_Returns(
            bool value,
            string[] expected)
    {
        Assert.Equal(
                expected,
                new RESPBoolean(value)
                {
                    Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
                    {
                        { new RESPSimpleString("foo"), new RESPSimpleString("Bar") },
                    }),
                }.Accept(new RESPValuePrettyFormatter()));
    }

    [Theory]
    [InlineData(0.0, new string[] { "(double) 0" })]
    [InlineData(double.MaxValue, new string[] { "(double) 1.7976931348623157e+308" })]
    [InlineData(double.MinValue, new string[] { "(double) -1.7976931348623157e+308" })]
    [InlineData(double.NaN, new string[] { "(double) nan" })]
    [InlineData(double.NegativeInfinity, new string[] { "(double) -inf" })]
    [InlineData(double.PositiveInfinity, new string[] { "(double) inf" })]
    public void Double_Plain_Returns(
            double number,
            string[] expected)
    {
        Assert.Equal(
                expected,
                new RESPDouble(number).Accept(new RESPValuePrettyFormatter()));
    }

    [Theory]
    [InlineData(0.0, new string[] { "(double) 0*" })]
    [InlineData(3.14159, new string[] { "(double) 3.14159*" })]
    public void Double_WithAttribute_Returns(
            double number,
            string[] expected)
    {
        Assert.Equal(
                expected,
                new RESPDouble(number)
                {
                    Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
                    {
                        { new RESPSimpleString("foo"), new RESPSimpleString("Bar") },
                    }),
                }.Accept(new RESPValuePrettyFormatter()));
    }

    [Fact]
    public void Null_Plain_Returns()
    {
        Assert.Equal(
                new[] { "(null)" },
                RESPNull.Instance.Accept(new RESPValuePrettyFormatter()));
    }

    [Fact]
    public void Null_WithAttribute_Returns()
    {
        Assert.Equal(
                new[] { "(null)*" },
                new RESPNull()
                {
                    Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
                    {
                        { new RESPSimpleString("foo"), new RESPSimpleString("Bar") },
                    }),
                }.Accept(new RESPValuePrettyFormatter()));
    }

    [Theory]
    [InlineData(0, new string[] { "(integer) 0" })]
    [InlineData(long.MaxValue, new string[] { "(integer) 9223372036854775807" })]
    [InlineData(long.MinValue, new string[] { "(integer) -9223372036854775808" })]
    public void Number_Plain_Returns(
            long number,
            string[] expected)
    {
        Assert.Equal(
                expected,
                new RESPNumber(number).Accept(new RESPValuePrettyFormatter()));
    }

    [Theory]
    [InlineData(0, new string[] { "(integer) 0*" })]
    [InlineData(long.MaxValue, new string[] { "(integer) 9223372036854775807*" })]
    [InlineData(long.MinValue, new string[] { "(integer) -9223372036854775808*" })]
    public void Number_WithAttribute_Returns(
            long number,
            string[] expected)
    {
        Assert.Equal(
                expected,
                new RESPNumber(number)
                {
                    Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
                    {
                        { new RESPSimpleString("foo"), new RESPSimpleString("Bar") },
                    }),
                }.Accept(new RESPValuePrettyFormatter()));
    }

    [Theory]
    [InlineData("Foo", new string[] { "\"Foo\"" })]
    [InlineData("foo\bbar", new string[] { "\"foo\\bbar\"" })]
    [InlineData("foo\tbar", new string[] { "\"foo\\tbar\"" })]
    public void SimpleError_String_Returns(
            string str,
            string[] expected)
    {
        Assert.Equal(
                expected,
                new RESPSimpleError(str).Accept(new RESPValuePrettyFormatter()));
    }

    [Theory]
    [InlineData(new byte[] { 0 }, new string[] { "\"\\x00\"" })]
    [InlineData(new byte[] { 2 }, new string[] { "\"\\x02\"" })]
    public void SimpleError_Binary_Returns(
            byte[] value,
            string[] expected)
    {
        Assert.Equal(
                expected,
                new RESPSimpleError(value).Accept(new RESPValuePrettyFormatter()));
    }

    [Theory]
    [InlineData("Foo", new string[] { "\"Foo\"*" })]
    [InlineData("foo\bbar", new string[] { "\"foo\\bbar\"*" })]
    [InlineData("foo\tbar", new string[] { "\"foo\\tbar\"*" })]
    public void SimpleError_StringWithAttribute_Returns(
            string str,
            string[] expected)
    {
        Assert.Equal(
                expected,
                new RESPSimpleError(str)
                {
                    Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
                    {
                        { new RESPSimpleString("foo"), new RESPSimpleString("Bar") },
                    }),
                }.Accept(new RESPValuePrettyFormatter()));
    }

    [Theory]
    [InlineData("Foo", new string[] { "\"Foo\"" })]
    [InlineData("foo\bbar", new string[] { "\"foo\\bbar\"" })]
    [InlineData("foo\tbar", new string[] { "\"foo\\tbar\"" })]
    public void SimpleString_String_Returns(
            string str,
            string[] expected)
    {
        Assert.Equal(
                expected,
                new RESPSimpleString(str).Accept(new RESPValuePrettyFormatter()));
    }

    [Theory]
    [InlineData(new byte[] { 0 }, new string[] { "\"\\x00\"" })]
    [InlineData(new byte[] { 2 }, new string[] { "\"\\x02\"" })]
    public void SimpleString_Binary_Returns(
            byte[] value,
            string[] expected)
    {
        Assert.Equal(
                expected,
                new RESPSimpleString(value).Accept(new RESPValuePrettyFormatter()));
    }

    [Theory]
    [InlineData("Foo", new string[] { "\"Foo\"*" })]
    [InlineData("foo\bbar", new string[] { "\"foo\\bbar\"*" })]
    [InlineData("foo\tbar", new string[] { "\"foo\\tbar\"*" })]
    public void SimpleString_StringWithAttribute_Returns(
            string str,
            string[] expected)
    {
        Assert.Equal(
                expected,
                new RESPSimpleString(str)
                {
                    Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
                    {
                        { new RESPSimpleString("foo"), new RESPSimpleString("Bar") },
                    }),
                }.Accept(new RESPValuePrettyFormatter()));
    }

    [Theory]
    [InlineData("Foo", new string[] { "(txt verbatim string)", "Foo" })]
    [InlineData("foo\nbar", new string[] { "(txt verbatim string)", "foo", "bar" })]
    [InlineData("foo\r\nbar", new string[] { "(txt verbatim string)", "foo", "bar" })]
    [InlineData("foo\tbar", new string[] { "(txt verbatim string)", "foo\tbar" })]
    [InlineData("foo\bbar", new string[] { "(txt verbatim string)", "foo\bbar" })]
    public void VerbatimString_String_Returns(
            string str,
            string[] expected)
    {
        Assert.Equal(
                expected,
                new RESPVerbatimString(VerbatimStringType.Text, str).Accept(new RESPValuePrettyFormatter()));
    }

    [Theory]
    [InlineData(new byte[] { 0 }, new string[] { "(txt verbatim string)", "\0" })]
    /* see https://www.cl.cam.ac.uk/~mgk25/ucs/examples/UTF-8-test.txt */
    [InlineData(new byte[] { 0xfe }, new string[] { "(txt verbatim string)", "\\xfe" })]
    public void VerbatimString_Binary_Returns(
            byte[] value,
            string[] expected)
    {
        Assert.Equal(
                expected,
                new RESPVerbatimString(VerbatimStringType.Text, value).Accept(new RESPValuePrettyFormatter()));
    }

    [Theory]
    [InlineData("Foo", new string[] { "(mkd verbatim string)", "Foo", "[*]" })]
    [InlineData("foo\nbar", new string[] { "(mkd verbatim string)", "foo", "bar", "[*]" })]
    [InlineData("foo\r\nbar", new string[] { "(mkd verbatim string)", "foo", "bar", "[*]" })]
    [InlineData("foo\tbar", new string[] { "(mkd verbatim string)", "foo\tbar", "[*]" })]
    [InlineData("foo\bbar", new string[] { "(mkd verbatim string)", "foo\bbar", "[*]" })]
    public void VerbatimString_StringWithAttribute_Returns(
            string str,
            string[] expected)
    {
        Assert.Equal(
                expected,
                new RESPVerbatimString(VerbatimStringType.Markdown, str)
                {
                    Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
                    {
                        { new RESPSimpleString("foo"), new RESPSimpleString("Bar") },
                    }),
                }.Accept(new RESPValuePrettyFormatter()));
    }

    /* ------------------- Aggregate types ------------------- */

    [Fact]
    public void Array_Empty_Returns()
    {
        Assert.Equal(
                new[] { "(empty array)" },
                RESPArray.Empty.Accept(new RESPValuePrettyFormatter()));
    }

    [Fact]
    public void Array_Plain_Returns()
    {
        Assert.Equal(
                new[] { "1) (integer) 0" },
                new RESPArray(new RESPValue[]
                {
                    new RESPNumber(0),
                }).Accept(new RESPValuePrettyFormatter()));
    }

    [Fact]
    public void Array_Long_Returns()
    {
        Assert.Equal(
                new[] // see https://redis.io/commands/xautoclaim/ example
                {
                    "1) \"0-0\"",
                    "2) 1) 1) \"1609338752495-0\"*",
                    "      2) 1) \"field\"",
                    "         2) \"value\"",
                    "         [*]",
                    "3) (empty array)",
                },
                new RESPArray(new RESPValue[]
                {
                    new RESPSimpleString("0-0"),
                    new RESPArray(new RESPValue[]
                    {
                        new RESPArray(new RESPValue[]
                        {
                            new RESPSimpleString("1609338752495-0")
                            {
                                Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
                                {
                                    { new RESPSimpleString("foo"), new RESPSimpleString("Bar") },
                                }),
                            },
                            new RESPArray(new RESPValue[]
                            {
                                new RESPSimpleString("field"),
                                new RESPSimpleString("value"),
                            })
                            {
                                Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
                                {
                                    { new RESPSimpleString("foo"), new RESPSimpleString("Bar") },
                                }),
                            },
                        }),
                    }),
                    RESPArray.Empty,
                }).Accept(new RESPValuePrettyFormatter()));
    }

    [Fact]
    public void Array_EmptyWithAttribute_Returns()
    {
        Assert.Equal(
                new[] { "(empty array)*" },
                new RESPArray(Array.Empty<RESPValue>())
                {
                    Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
                    {
                        { new RESPSimpleString("foo"), new RESPSimpleString("Bar") },
                    }),
                }.Accept(new RESPValuePrettyFormatter()));
    }

    [Fact]
    public void Array_WithAttribute_Returns()
    {
        Assert.Equal(
                new[]
                {
                    "1) (integer) 0",
                    "[*]",
                },
                new RESPArray(new RESPValue[]
                {
                    new RESPNumber(0),
                })
                {
                    Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
                    {
                        { new RESPSimpleString("foo"), new RESPSimpleString("Bar") },
                    }),
                }.Accept(new RESPValuePrettyFormatter()));
    }

    [Fact]
    public void AttributeValue_Empty_Returns()
    {
        Assert.Equal(
                new[] { "(empty attribute map)" },
                RESPAttributeValue.Empty.Accept(new RESPValuePrettyFormatter()));
    }

    [Fact]
    public void AttributeValue_Plain_Returns()
    {
        Assert.Equal(
                new[] { "| \"foo\": (integer) 0" },
                new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
                {
                    { new RESPSimpleString("foo"), new RESPNumber(0) },
                }).Accept(new RESPValuePrettyFormatter()));
    }

    [Fact]
    public void AttributeValue_Long_Returns()
    {
        HashSet<string> expected = new[]
        {
            "| \"foo\": (integer) 0",
            "| \"bar\": (integer) 0*",
        }.ToHashSet();

        HashSet<string> actual = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
        {
            { new RESPSimpleString("foo"), new RESPNumber(0) },
            {
                new RESPSimpleString("bar"),
                new RESPNumber(0)
                {
                    Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
                    {
                        { new RESPSimpleString("foo"), new RESPSimpleString("Bar") },
                    }),
                }
            },
        }).Accept(new RESPValuePrettyFormatter()).ToHashSet();

        Assert.Subset(
                expected,
                actual);
    }

    [Fact]
    public void AttributeValue_EmptyWithAttribute_Returns()
    {
        Assert.Equal(
                new[] { "(empty attribute map)*" },
                new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>())
                {
                    Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
                    {
                        { new RESPSimpleString("foo"), new RESPSimpleString("Bar") },
                    }),
                }.Accept(new RESPValuePrettyFormatter()));
    }

    [Fact]
    public void AttributeValue_WithAttribute_Returns()
    {
        HashSet<string> expected = new[]
        {
            "| (integer) 0: (integer) 0",
            "[*]",
        }.ToHashSet();

        HashSet<string> actual = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
        {
            { new RESPNumber(0), new RESPNumber(0) },
        })
        {
            Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
            {
                { new RESPSimpleString("foo"), new RESPSimpleString("Bar") },
            }),
        }.Accept(new RESPValuePrettyFormatter()).ToHashSet();

        Assert.Subset(
                expected,
                actual);
    }

    [Fact]
    public void Map_Empty_Returns()
    {
        Assert.Equal(
                new[] { "(empty map)" },
                RESPMap.Empty.Accept(new RESPValuePrettyFormatter()));
    }

    [Fact]
    public void Map_Plain_Returns()
    {
        Assert.Equal(
                new[] { "* \"foo\": (integer) 0" },
                new RESPMap(new Dictionary<RESPValue, RESPValue>()
                {
                    { new RESPSimpleString("foo"), new RESPNumber(0) },
                }).Accept(new RESPValuePrettyFormatter()));
    }

    [Fact]
    public void Map_Long_Returns()
    {
        HashSet<string> expected = new[]
        {
            "* \"foo\": (integer) 0",
            "* \"bar\": (integer) 0*",
        }.ToHashSet();

        HashSet<string> actual = new RESPMap(new Dictionary<RESPValue, RESPValue>()
        {
            { new RESPSimpleString("foo"), new RESPNumber(0) },
            {
                new RESPSimpleString("bar"),
                new RESPNumber(0)
                {
                    Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
                    {
                        { new RESPSimpleString("foo"), new RESPSimpleString("Bar") },
                    }),
                }
            },
        }).Accept(new RESPValuePrettyFormatter()).ToHashSet();

        Assert.Subset(
                expected,
                actual);
    }

    [Fact]
    public void Map_EmptyWithAttribute_Returns()
    {
        Assert.Equal(
                new[] { "(empty map)*" },
                new RESPMap(new Dictionary<RESPValue, RESPValue>())
                {
                    Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
                    {
                        { new RESPSimpleString("foo"), new RESPSimpleString("Bar") },
                    }),
                }.Accept(new RESPValuePrettyFormatter()));
    }

    [Fact]
    public void Map_WithAttribute_Returns()
    {
        HashSet<string> expected = new[]
        {
            "* (integer) 0: (integer) 0",
            "[*]",
        }.ToHashSet();

        HashSet<string> actual = new RESPMap(new Dictionary<RESPValue, RESPValue>()
        {
            { new RESPNumber(0), new RESPNumber(0) },
        })
        {
            Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
            {
                { new RESPSimpleString("foo"), new RESPSimpleString("Bar") },
            }),
        }.Accept(new RESPValuePrettyFormatter()).ToHashSet();

        Assert.Subset(
                expected,
                actual);
    }

    [Fact]
    public void Push_AlmoustEmpty_Returns()
    {
        Assert.Equal(
                new[]
                {
                    "1} \"pubsub\"",
                },
                new RESPPush(new RESPValue[]
                {
                    new RESPSimpleString("pubsub"),
                }).Accept(new RESPValuePrettyFormatter()));
    }

    [Fact]
    public void Push_Plain_Returns()
    {
        Assert.Equal(
                new[]
                {
                    "1} \"monitor\"",
                    "2} (integer) 0",
                },
                new RESPPush(new RESPValue[]
                {
                    new RESPSimpleString("monitor"),
                    new RESPNumber(0),
                }).Accept(new RESPValuePrettyFormatter()));
    }

    [Fact]
    public void Push_Long_Returns()
    {
        Assert.Equal(
                new[] // see https://redis.io/commands/xautoclaim/ example
                {
                    "1} \"monitor\"",
                    "2} 1) 1) \"1609338752495-0\"*",
                    "      2) 1) \"field\"",
                    "         2) \"value\"",
                    "         [*]",
                    "3} (empty array)",
                },
                new RESPPush(new RESPValue[]
                {
                    new RESPSimpleString("monitor"),
                    new RESPArray(new RESPValue[]
                    {
                        new RESPArray(new RESPValue[]
                        {
                            new RESPSimpleString("1609338752495-0")
                            {
                                Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
                                {
                                    { new RESPSimpleString("foo"), new RESPSimpleString("Bar") },
                                }),
                            },
                            new RESPArray(new RESPValue[]
                            {
                                new RESPSimpleString("field"),
                                new RESPSimpleString("value"),
                            })
                            {
                                Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
                                {
                                    { new RESPSimpleString("foo"), new RESPSimpleString("Bar") },
                                }),
                            },
                        }),
                    }),
                    RESPArray.Empty,
                }).Accept(new RESPValuePrettyFormatter()));
    }

    [Fact]
    public void Push_EmptyWithAttribute_Returns()
    {
        Assert.Equal(
                new[]
                {
                    "1} \"monitor\"",
                    "[*]",
                },
                new RESPPush(new RESPValue[]
                {
                    new RESPSimpleString("monitor"),
                })
                {
                    Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
                    {
                        { new RESPSimpleString("foo"), new RESPSimpleString("Bar") },
                    }),
                }.Accept(new RESPValuePrettyFormatter()));
    }

    [Fact]
    public void Push_WithAttribute_Returns()
    {
        string[] expected = new[]
        {
            "1} \"monitor\"",
            "2} \"String spaning\\nmultiple lines\"",
            "3} (txt verbatim string)",
            "   Verbatim string spaning",
            "   multiple lines",
            "   [*]",
            "[*]",
        };
        string[] actual = new RESPPush(new RESPValue[]
        {
            new RESPSimpleString("monitor"),
            new RESPBlobString("String spaning\nmultiple lines"),
            new RESPVerbatimString(VerbatimStringType.Text, "Verbatim string spaning\nmultiple lines")
            {
                Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
                {
                    { new RESPSimpleString("foo"), new RESPSimpleString("Bar") },
                }),
            },
        })
        {
            Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
            {
                { new RESPSimpleString("foo"), new RESPSimpleString("Bar") },
            }),
        }.Accept(new RESPValuePrettyFormatter()).ToArray();

        Assert.Equal(
                expected,
                actual);
    }

    [Fact]
    public void Set_Empty_Returns()
    {
        Assert.Equal(
                new[] { "(empty set)" },
                RESPSet.Empty.Accept(new RESPValuePrettyFormatter()));
    }

    [Fact]
    public void Set_Plain_Returns()
    {
        Assert.Equal(
                new[] { "~ \"foo\"" },
                new RESPSet(new RESPValue[]
                {
                    new RESPSimpleString("foo"),
                }).Accept(new RESPValuePrettyFormatter()));
    }

    [Fact]
    public void Set_Long_Returns()
    {
        HashSet<string> expected = new[]
        {
            "~ \"foo\"",
            "~ (integer) 0*",
        }.ToHashSet();

        HashSet<string> actual = new RESPSet(new RESPValue[]
        {
            new RESPSimpleString("foo"),
            new RESPNumber(0)
            {
                Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
                {
                    { new RESPSimpleString("foo"), new RESPSimpleString("Bar") },
                }),
            },
        }).Accept(new RESPValuePrettyFormatter()).ToHashSet();

        Assert.Subset(
                expected,
                actual);
    }

    [Fact]
    public void Set_EmptyWithAttribute_Returns()
    {
        Assert.Equal(
                new[] { "(empty set)*" },
                new RESPSet(Array.Empty<RESPValue>())
                {
                    Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
                    {
                        { new RESPSimpleString("foo"), new RESPSimpleString("Bar") },
                    }),
                }.Accept(new RESPValuePrettyFormatter()));
    }

    [Fact]
    public void Set_WithAttribute_Returns()
    {
        HashSet<string> expected = new[]
        {
            "~ (integer) 0",
            "[*]",
        }.ToHashSet();

        HashSet<string> actual = new RESPSet(new RESPValue[]
        {
            new RESPNumber(0),
        })
        {
            Attribs = new RESPAttributeValue(new Dictionary<RESPValue, RESPValue>()
            {
                { new RESPSimpleString("foo"), new RESPSimpleString("Bar") },
            }),
        }.Accept(new RESPValuePrettyFormatter()).ToHashSet();

        Assert.Subset(
                expected,
                actual);
    }
}
