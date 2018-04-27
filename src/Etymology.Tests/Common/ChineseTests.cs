namespace Etymology.Tests.Common
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using static Etymology.Common.Chinese;

    [TestClass]
    public class ChineseTests
    {
        internal static readonly List<(string HexCodePoint, string Text)> ChineseCharacters = new List<(string HexCodePoint, string Text)>()
        {
            ("4E00", "一"),
            ("3400", "㐀"),
            ("3426", "㐦"),
            ("20000", "𠀀"),
            ("2A700", "𪜀"),
            ("2B740", "𫝀"),
            ("2F800", "丽"),
            ("2E80", "⺀"),
            ("2F00", "⼀"),
            ("2FF0", "⿰"),
            ("3025", "〥"),
            ("3044", "い"),
            ("30A2", "ア"),
            ("3106", "ㄆ"),
            ("31A1", "ㆡ"),
            ("31CF", "㇏"),
            ("31F0", "ㇰ"),
            ("F900", "豈"),
            ("FE3D", "︽"),
            ("FF6C", "ｬ")
        };

        internal static readonly List<string> OtherCharacters = new List<string>()
        {
            null, string.Empty, " ", "  ", ".", "@", "a", "A", "1", "𠀀".Substring(0, 1)
        };

        static ChineseTests()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        [TestMethod]
        public void ValidateSingleChineseCharacterTest()
        {
            ChineseCharacters.ForEach(item => Assert.IsNull(ValidateSingleChineseCharacter(item.Text).Exception));

            OtherCharacters.ForEach(item => Assert.IsNotNull(ValidateSingleChineseCharacter(item).Exception));
        }

        [TestMethod]
        public void TextToCodePointTest()
        {
            Assert.AreEqual("4E00", "一".TextToCodePoint());
            Assert.AreEqual("3400", "㐀".TextToCodePoint());
            Assert.AreEqual("3426", "㐦".TextToCodePoint());
            Assert.AreEqual("20000", "𠀀".TextToCodePoint());
            Assert.AreEqual("2A700", "𪜀".TextToCodePoint());
            Assert.AreEqual("2B740", "𫝀".TextToCodePoint());
            Assert.AreEqual("2F800", "丽".TextToCodePoint());
            Assert.AreEqual("2E80", "⺀".TextToCodePoint());
            Assert.AreEqual("2F00", "⼀".TextToCodePoint());
            Assert.AreEqual("2FF0", "⿰".TextToCodePoint());
            Assert.AreEqual("3025", "〥".TextToCodePoint());
            Assert.AreEqual("3044", "い".TextToCodePoint());
            Assert.AreEqual("30A2", "ア".TextToCodePoint());
            Assert.AreEqual("3106", "ㄆ".TextToCodePoint());
            Assert.AreEqual("31A1", "ㆡ".TextToCodePoint());
            Assert.AreEqual("31CF", "㇏".TextToCodePoint());
            Assert.AreEqual("31F0", "ㇰ".TextToCodePoint());
            Assert.AreEqual("F900", "豈".TextToCodePoint());
            Assert.AreEqual("FE3D", "︽".TextToCodePoint());
            Assert.AreEqual("FF6C", "ｬ".TextToCodePoint());
        }

        [TestMethod]
        public void CodePointToTextTest()
        {
            ChineseCharacters.ForEach(item => Assert.AreEqual(item.Text, item.HexCodePoint.CodePointToText()));
        }

        [TestMethod]
        public void CodePointToBytesTest()
        {
            ChineseCharacters.ForEach(item => Assert.IsTrue(Encoding.Unicode.GetBytes(item.Text).SequenceEqual(item.HexCodePoint.CodePointToBytes())));
        }

        [TestMethod]
        public void BytesToCodePointTest()
        {
            ChineseCharacters.ForEach(item => Assert.AreEqual(item.HexCodePoint, Encoding.Unicode.GetBytes(item.Text).BytesToCodePoint()));
        }
    }
}
