namespace Etymology.Tests.Common
{
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using static Etymology.Common.Chinese;

    [TestClass]
    public class ChineseTests
    {
        static ChineseTests()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        [TestMethod]
        public void ValidateSingleChineseCharacterTest()
        {
            Assert.IsNull(ValidateSingleChineseCharacter("一"));
            Assert.IsNull(ValidateSingleChineseCharacter("㐦"));
            Assert.IsNull(ValidateSingleChineseCharacter("𠀀"));
            Assert.IsNull(ValidateSingleChineseCharacter("𪜀"));
            Assert.IsNull(ValidateSingleChineseCharacter("𫝀"));
            Assert.IsNull(ValidateSingleChineseCharacter("丽"));
            Assert.IsNull(ValidateSingleChineseCharacter("⺀"));
            Assert.IsNull(ValidateSingleChineseCharacter("⼀"));
            Assert.IsNull(ValidateSingleChineseCharacter("⿰"));
            Assert.IsNull(ValidateSingleChineseCharacter("〥"));
            Assert.IsNull(ValidateSingleChineseCharacter("い"));
            Assert.IsNull(ValidateSingleChineseCharacter("ア"));
            Assert.IsNull(ValidateSingleChineseCharacter("ㄆ"));
            Assert.IsNull(ValidateSingleChineseCharacter("ㆡ"));
            Assert.IsNull(ValidateSingleChineseCharacter("㇏"));
            Assert.IsNull(ValidateSingleChineseCharacter("ㇰ"));
            Assert.IsNull(ValidateSingleChineseCharacter("豈"));
            Assert.IsNull(ValidateSingleChineseCharacter("︽"));
            Assert.IsNull(ValidateSingleChineseCharacter("ｬ"));

            Assert.IsNotNull(ValidateSingleChineseCharacter(string.Empty));
            Assert.IsNotNull(ValidateSingleChineseCharacter(null));
            Assert.IsNotNull(ValidateSingleChineseCharacter(" "));
            Assert.IsNotNull(ValidateSingleChineseCharacter("  "));
            Assert.IsNotNull(ValidateSingleChineseCharacter("."));
            Assert.IsNotNull(ValidateSingleChineseCharacter("a"));
            Assert.IsNotNull(ValidateSingleChineseCharacter("1"));
            Assert.IsNotNull(ValidateSingleChineseCharacter("𠀀".Substring(0, 1)));
        }

        [TestMethod]
        public void TextToCodeTest()
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
        public void CodeToTextTest()
        {
            Assert.AreEqual("4E00".CodePointToText(), "一");
            Assert.AreEqual("3400".CodePointToText(), "㐀");
            Assert.AreEqual("3426".CodePointToText(), "㐦");
            Assert.AreEqual("20000".CodePointToText(), "𠀀");
            Assert.AreEqual("2A700".CodePointToText(), "𪜀");
            Assert.AreEqual("2B740".CodePointToText(), "𫝀");
            Assert.AreEqual("2F800".CodePointToText(), "丽");
            Assert.AreEqual("2E80".CodePointToText(), "⺀");
            Assert.AreEqual("2F00".CodePointToText(), "⼀");
            Assert.AreEqual("2FF0".CodePointToText(), "⿰");
            Assert.AreEqual("3025".CodePointToText(), "〥");
            Assert.AreEqual("3044".CodePointToText(), "い");
            Assert.AreEqual("30A2".CodePointToText(), "ア");
            Assert.AreEqual("3106".CodePointToText(), "ㄆ");
            Assert.AreEqual("31A1".CodePointToText(), "ㆡ");
            Assert.AreEqual("31CF".CodePointToText(), "㇏");
            Assert.AreEqual("31F0".CodePointToText(), "ㇰ");
            Assert.AreEqual("F900".CodePointToText(), "豈");
            Assert.AreEqual("FE3D".CodePointToText(), "︽");
            Assert.AreEqual("FF6C".CodePointToText(), "ｬ");
        }
    }
}
