﻿namespace Etymology.Common
{
    using System;
    using System.Text;

    public static class Chinese
    {
        public static Encoding GB18030 { get; } = Encoding.GetEncoding(nameof(GB18030));

        public static byte[] ConvertToBytes(this string value, Encoding from, Encoding to) =>
            Encoding.Convert(from, to, from.GetBytes(value));

        public static byte[] ConvertFromDefaultToGB18030(this string value) =>
            ConvertToBytes(value, Encoding.Default, GB18030);

        private static bool InRange(this byte[] values, params (byte Min, byte Max)[] ranges)
        {
            if (values.Length != ranges.Length)
            {
                return false;
            }

            for (int index = 0; index < values.Length; index++)
            {
                byte value = values[index];
                (byte Min, byte Max) range = ranges[index];
                if (value < range.Min || value > range.Max)
                {
                    return false;
                }
            }

            return true;
        }

        public static Exception ValidateSingleChineseCharacter(string chinese, string argument)
        {
            if (string.IsNullOrWhiteSpace(chinese))
            {
                return new ArgumentNullException(argument, "Input is null or white space.");
            }

            if (chinese.Length < 1 || chinese.Length > 2)
            {
                return new ArgumentOutOfRangeException(argument, "Input is not a single character.");
            }

            byte[] bytes = ConvertFromDefaultToGB18030(chinese);
            if (bytes.Length < 2 || bytes.Length > 4)
            {
                return new ArgumentOutOfRangeException(argument, "Input is not a single character.");
            }

            return null;
            // http://www.qqxiuzi.cn/zh/hanzi-gb18030-bianma.php
            // if (bytes.InRange((0xB0, 0xF7), (0xA1, 0xFE))
            //    || bytes.InRange((0x31, 0x31), (0xC0, 0xEF))
            //    || bytes.InRange((0x81, 0xA0), (0x40, 0xFE))
            //    || bytes.InRange((0xAA, 0xFE), (0x40, 0xA0))
            //    || bytes.InRange((0xF9, 0xFA), (0x00, 0xFF))
            //    || bytes.InRange((0x81, 0x82), (0x30, 0x39), (0x81, 0xFE), (0x30, 0x39))
            //    || bytes.InRange((0x95, 0x98), (0x30, 0x39), (0x81, 0xFE), (0x30, 0x39)))
            // {
            //    return null;
            // }
        }
    }
}