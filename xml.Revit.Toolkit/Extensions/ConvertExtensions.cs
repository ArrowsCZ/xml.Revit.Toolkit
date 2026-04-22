/* 作    者: xml
** 创建时间: 2024/2/16 20:26:06
**
** Copyright 2024 by zedmoster
** Permission to use, copy, modify, and distribute this software in
** object code form for any purpose and without fee is hereby granted,
** provided that the above copyright notice appears in all copies and
** that both that copyright notice and the limited warranty and
** restricted rights notice below appear in all supporting
** documentation.
*/

namespace xml.Revit.Toolkit.Extensions;

/// <summary>
/// Convert Extensions
/// </summary>
public static class ConvertExtensions
{
    /// <summary>
    /// 长
    /// </summary>
    public const uint MaxLength = 30000;

    /// <summary>
    /// 单位转换 mm,feet
    /// </summary>
    public const double KUnit = 304.8;

    /// <summary>
    /// π 值
    /// </summary>
    public const double KPi = 3.14159265358979323846;

    /// <summary>
    /// 最小长度单位:英尺
    /// </summary>
    public const double ShortCurveTolerance = 0.00256026455729167;

    /// <summary>
    /// 误差
    /// </summary>
    public const double Tolerance = 1e-6;

    /// <summary>
    /// 两个值在误差范围内是否相等
    /// </summary>
    /// <param name="value1"> value1</param>
    /// <param name="value2"> value2</param>
    /// <param name="tolerance"> 误差</param>
    /// <returns> 是否相等</returns>
    public static bool IsEquals(this int value1, double value2, double tolerance = Tolerance)
    {
        return Math.Abs(value1 - value2) <= tolerance;
    }

    /// <param name="value1"> value1</param>
    extension(double value1)
    {
        /// <summary>
        /// 两个值在误差范围内是否相等
        /// </summary>
        /// <param name="value2"> value2</param>
        /// <param name="tolerance"> 误差</param>
        /// <returns> 是否相等</returns>
        public bool IsEquals(double value2, double tolerance = Tolerance) => Math.Abs(value1 - value2) <= tolerance;

        /// <summary>
        /// 获取比例值
        /// </summary>
        /// <param name="view">视图</param>
        /// <returns> 视图比例换算</returns>
        public double Scale(View view) => value1 * view.Scale / 100;

        /// <summary>
        /// 毫米 --> 英尺
        /// </summary>
        /// <returns> 英尺</returns>
        public double MmToFeet() => value1 / KUnit;
    }

    /// <summary>
    /// 毫米 --> 英尺
    /// </summary>
    /// <param name="value"> 毫米</param>
    /// <returns> 英尺</returns>
    public static double MmToFeet(this int value)
    {
        return value / KUnit;
    }

    /// <param name="value"> 英尺</param>
    extension(double value)
    {
        /// <summary>
        /// 英尺 --> 毫米
        /// </summary>
        /// <returns> 毫米</returns>
        public double FeetToMm() => value * KUnit;

        /// <summary>
        /// 角度 --> 弧度
        /// </summary>
        /// <returns> 弧度</returns>
        public double ToRadians()
        {
#if REVIT2021_OR_GREATER
            return UnitUtils.ConvertToInternalUnits(degrees, UnitTypeId.Degrees);
#else
            return UnitUtils.ConvertToInternalUnits(value, DisplayUnitType.DUT_DECIMAL_DEGREES);
#endif
        }

        /// <summary>
        /// 弧度 --> 角度
        /// </summary>
        /// <returns> 角度</returns>
        public double ToDegrees()
        {
#if REVIT2021_OR_GREATER
            return UnitUtils.ConvertFromInternalUnits(radians, UnitTypeId.Degrees);
#else
            return UnitUtils.ConvertFromInternalUnits(value, DisplayUnitType.DUT_DECIMAL_DEGREES);
#endif
        }

        /// <summary>
        /// 将小数四舍五入到指定的小数位数。
        /// </summary>
        /// <param name="decimalPlaces">要保留的小数位数</param>
        /// <returns>四舍五入后的小数</returns>
        public double RoundToDouble(int decimalPlaces = 0) => Math.Round(value, decimalPlaces);

        /// <summary>
        /// 将小数四舍五入到最接近的整数，可指定一个比例。
        /// </summary>
        /// <param name="scale">比例（默认为1，不进行缩放）</param>
        /// <returns>四舍五入后的整数</returns>
        public int RoundToInt(double scale = 1) => (int)Math.Round(value / scale);

        /// <summary>
        /// 将给定的小数四舍五入到最接近的整数，考虑模数，并返回整数结果。
        /// </summary>
        /// <param name="modulus">模数，必须为正整数</param>
        /// <returns>整数</returns>
        public int RoundToNearestIntWithModulus(int modulus)
        {
            if (modulus <= 0)
            {
                throw new ArgumentException("模数必须为正整数。", nameof(modulus));
            }
            var quotient = value / modulus;
            var divide = Math.Truncate(value / modulus);
            var increment = Math.Round(quotient - divide, 2) >= 0.5 ? 1 : 0;
            return (int)((divide + increment) * modulus);
        }
    }

    /// <param name="id"> Int ID</param>
    extension(string id)
    {
        /// <summary>
        /// Int 转 ElementId
        /// </summary>
        /// <returns> ElementId</returns>
        public ElementId ToElementId()
        {
#if REVIT2024_OR_GREATER
            return ElementId.Parse(id);
#else
            return new ElementId(id.ToInteger());
#endif
        }

        /// <summary>
        /// 将字符串转换为 double 值。
        /// </summary>
        /// <returns>转换后的 double 值，如果转换失败则返回 0.0</returns>
        public double ToDouble()
        {
            return double.TryParse(id, out double result) ? result : 0.0;
        }

        /// <summary>
        /// 将字符串转换为整数值。
        /// </summary>
        /// <returns>转换后的整数值，如果转换失败则返回 0</returns>
        public int ToInteger()
        {
            return int.TryParse(id, out int result) ? result : 0;
        }

        /// <summary>
        /// 将字符串转换为布尔值。
        /// </summary>
        /// <returns>转换后的布尔值，如果转换失败则返回 false</returns>
        public bool ToBool()
        {
            string upperValue = id.ToUpper();
            return upperValue is "Y" or "OK" or "YES" or "TRUE" or "1" or "是";
        }
    }
}
