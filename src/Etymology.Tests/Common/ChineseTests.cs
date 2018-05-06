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
            ("FF6C", "ｬ"),
            ("2B8B8", "𫢸")
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
            ChineseCharacters.ForEach(item => Assert.AreEqual(item.HexCodePoint, item.Text.TextToCodePoint()));
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
