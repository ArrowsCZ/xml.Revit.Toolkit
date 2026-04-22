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

using xml.Revit.Toolkit.Utils;
using View = Autodesk.Revit.DB.View;

namespace xml.Revit.Toolkit.Extensions
{
    /// <summary>
    /// XYZ Extensions
    /// </summary>
    public static class XYZExtensions
    {
        /// <summary>
        /// 重置点的 Z 坐标值
        /// </summary>
        /// <param name="point">要重置 Z 坐标的点</param>
        /// <param name="z">新的 Z 坐标值</param>
        /// <returns>具有重置 Z 坐标的新点</returns>
        public static XYZ Flatten(this XYZ point, double z = 0)
        {
            if (point is null)
            {
                throw new ArgumentNullException(nameof(point));
            }

            return new XYZ(point.X, point.Y, z);
        }

        /// <summary>
        /// 误差范围内两个点是否相等
        /// </summary>
        /// <param name="p0">点1</param>
        /// <param name="p1">点2</param>
        /// <param name="tolerance">误差</param>
        /// <returns>是否相等</returns>
        public static bool IsEquals(this XYZ p0, XYZ p1, double tolerance = 1e-5)
        {
            if (p0 is null)
            {
                throw new ArgumentNullException(nameof(p0));
            }

            if (p1 is null)
            {
                throw new ArgumentNullException(nameof(p1));
            }

            return p0.IsAlmostEqualTo(p1, tolerance);
        }

        /// <summary>
        /// 判断两个向量是否垂直
        /// </summary>
        /// <param name="dir1">向量1</param>
        /// <param name="dir2">向量2</param>
        /// <param name="tolerance">误差</param>
        /// <returns>是否垂直</returns>
        public static bool IsVertical(this XYZ dir1, XYZ dir2, double tolerance = 1e-5)
        {
            if (dir1 is null)
            {
                throw new ArgumentNullException(nameof(dir1));
            }

            if (dir2 is null)
            {
                throw new ArgumentNullException(nameof(dir2));
            }

            return dir1.DotProduct(dir2).IsEquals(0, tolerance);
        }

        /// <summary>
        /// 判断两个向量是否平行。
        /// </summary>
        /// <param name="vector1">第一个向量</param>
        /// <param name="vector2">第二个向量</param>
        /// <param name="tolerance">误差范围</param>
        /// <returns>如果两个向量在指定误差范围内平行，返回 true；否则返回 false。</returns>
        public static bool IsParallel(this XYZ vector1, XYZ vector2, double tolerance = 1e-5)
        {
            if (vector1 is null)
            {
                throw new ArgumentNullException(nameof(vector1));
            }
            if (vector2 is null)
            {
                throw new ArgumentNullException(nameof(vector2));
            }
            var b1 = vector1.IsAlmostEqualTo(vector2, tolerance);
            var b2 = vector1.IsAlmostEqualTo(vector2.Negate(), tolerance);
            return b1 || b2;
        }

        /// <summary>
        /// 单位向量转换
        /// vector 以当前视图方向为轴 逆时针 旋转 angle 弧度
        /// </summary>
        /// <param name="xyz"> 定位点</param>
        /// <param name="doc"> doc</param>
        /// <param name="angle"> 角度</param>
        /// <returns> XYZ</returns>
        public static XYZ RotationAxisViewDirection(
            this XYZ xyz,
            Document doc,
            double angle = Math.PI * 0.5
        )
        {
            var axis = doc.ActiveView.ViewDirection;
            Transform transform = Transform.CreateRotationAtPoint(axis, -angle, XYZ.Zero);
            return transform.OfPoint(xyz);
        }

        /// <summary>
        /// 将 XYZ 拍到视图平面上
        /// </summary>
        /// <param name="view"> 视图</param>
        /// <param name="p"> 定位点</param>
        /// <returns> new XYZ</returns>
        public static XYZ FlattenToPlane(this XYZ p, View view)
        {
            var plane = Plane.CreateByOriginAndBasis(
                view.Origin,
                view.RightDirection,
                view.UpDirection
            );
            var v = p - plane.Origin;
            var d = plane.Normal.DotProduct(v);
            return p - (d * plane.Normal);
        }

        /// <summary>
        /// 计算两个 XYZ 点的中点。
        /// </summary>
        /// <param name="point1">第一个点</param>
        /// <param name="point2">第二个点</param>
        /// <returns>中点坐标</returns>
        public static XYZ Midpoint(this XYZ point1, XYZ point2)
        {
            double x = (point1.X + point2.X) / 2.0;
            double y = (point1.Y + point2.Y) / 2.0;
            double z = (point1.Z + point2.Z) / 2.0;

            return new XYZ(x, y, z);
        }

        /// <summary>
        /// 点再平面内
        /// </summary>
        /// <param name="pnt"></param>
        /// <param name="face"></param>
        /// <returns></returns>
        public static bool IsInside(this XYZ pnt, Face face)
        {
            return face != null
                && face.Intersect(Line.CreateUnbound(pnt, XYZ.BasisZ))
                    == SetComparisonResult.Overlap;
        }

        /// <summary>
        /// 点xyz 是否在视图裁剪区域内
        /// </summary>
        /// <param name="point"> 点</param>
        /// <param name="view"> 视图裁剪</param>
        /// <param name="compareZ"> 是否计算Z坐标</param>
        /// <returns> 是否在范围内</returns>
        public static bool IsInCropBox(this XYZ point, View view, bool compareZ = false)
        {
            try
            {
                return !view.CropBoxActive || IsInsideBoundingBox(point, view.CropBox, compareZ);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 判断点是否在 BoundingBoxXYZ 的区域内。
        /// </summary>
        /// <param name="point">要检查的点</param>
        /// <param name="box">要检查的包围盒</param>
        /// <param name="compareZ">是否考虑 Z 坐标</param>
        /// <returns>如果点在包围盒内，返回 true；否则返回 false</returns>
        public static bool IsInsideBoundingBox(
            this XYZ point,
            BoundingBoxXYZ box,
            bool compareZ = false
        )
        {
            XYZ min = box.Min;
            XYZ max = box.Max;

            bool x = point.X >= min.X && point.X <= max.X;
            bool y = point.Y >= min.Y && point.Y <= max.Y;

            if (compareZ)
            {
                bool z = point.Z >= min.Z && point.Z <= max.Z;
                return x && y && z;
            }

            return x && y;
        }

        /// <summary>
        /// 获取给定方向含全部点位的包围盒
        /// </summary>
        /// <param name="points"></param>
        /// <param name="xAxis"></param>
        /// <param name="yAxis"></param>
        /// <returns></returns>
        public static BoundingBoxXYZ GetMaxBoundingBox(
            this IEnumerable<XYZ> points,
            XYZ xAxis,
            XYZ yAxis
        )
        {
            var minX = double.PositiveInfinity;
            var minY = double.PositiveInfinity;
            var maxX = double.NegativeInfinity;
            var maxY = double.NegativeInfinity;

            foreach (var point in points)
            {
                double xValue = point.DotProduct(xAxis);
                double yValue = point.DotProduct(yAxis);

                // 更新最小和最大坐标值
                minX = Math.Min(minX, xValue);
                minY = Math.Min(minY, yValue);
                maxX = Math.Max(maxX, xValue);
                maxY = Math.Max(maxY, yValue);
            }

            return new BoundingBoxXYZ
            {
                Min = xAxis * minX + yAxis * minY,
                Max = xAxis * maxX + yAxis * maxY,
            };
        }

        /// <summary>
        /// 基于定位点及方向创建旋转轴
        /// </summary>
        /// <param name="pnt"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static Line GetAxis(this XYZ pnt, XYZ direction)
        {
            return Line.CreateUnbound(pnt, direction);
        }

        /// <summary>
        /// 创建直线,如果线段太短返回null <see cref="Autodesk.Revit.ApplicationServices.Application.ShortCurveTolerance"/>
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static Line CreateBound(this XYZ origin, XYZ end)
        {
            if (origin.DistanceTo(end) < XmlDoc.UIapp.Application.ShortCurveTolerance)
            {
                return default;
            }
            return Line.CreateBound(origin, end);
        }

        /// <summary>
        /// 创建直线,如果线段太短返回null <see cref="Autodesk.Revit.ApplicationServices.Application.ShortCurveTolerance"/>
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="direction"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static Line CreateBound(this XYZ origin, XYZ direction, double length)
        {
            var end = direction * length;
            return CreateBound(origin, end);
        }

        /// <summary>
        /// 获取包围盒
        /// </summary>
        /// <param name="points"></param>
        /// <param name="xAxis"></param>
        /// <param name="yAxis"></param>
        /// <returns></returns>
        public static BoundingBoxXYZ GetMaxBoundingBox(this List<XYZ> points, XYZ xAxis, XYZ yAxis)
        {
            // 初始化最小和最大坐标值
            double minX = double.PositiveInfinity;
            double minY = double.PositiveInfinity;
            double maxX = double.NegativeInfinity;
            double maxY = double.NegativeInfinity;

            foreach (var point in points)
            {
                double xValue = point.DotProduct(xAxis);
                double yValue = point.DotProduct(yAxis);

                // 更新最小和最大坐标值
                minX = Math.Min(minX, xValue);
                minY = Math.Min(minY, yValue);
                maxX = Math.Max(maxX, xValue);
                maxY = Math.Max(maxY, yValue);
            }
            // 限定Revit内部范围
            if (double.IsInfinity(minX))
            {
                minX = -100;
            }
            if (double.IsInfinity(minY))
            {
                minX = -100;
            }
            if (double.IsInfinity(maxX))
            {
                minX = 100;
            }
            if (double.IsInfinity(maxY))
            {
                minX = 100;
            }
            return new BoundingBoxXYZ
            {
                Min = (xAxis * minX) + (yAxis * minY),
                Max = (xAxis * maxX) + (yAxis * maxY),
            };
        }

        /// <summary>
        /// 查找给定实例中最近的实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="point"></param>
        /// <param name="elements"></param>
        /// <param name="inPlane"></param>
        /// <returns></returns>
        public static T FindClosestElement<T>(
            this XYZ point,
            IEnumerable<T> elements,
            bool inPlane = true
        )
            where T : Element
        {
            var closestElement = default(T);
            var minDistance = double.MaxValue;
            foreach (var element in elements)
            {
                var locationCurve = element.GetLocationCurve();
                var locationPoint = element.GetLocationPoint();
                if (locationCurve != null)
                {
                    var pnt = locationCurve.Project(point).XYZPoint;
                    if (inPlane)
                    {
                        pnt = pnt.Flatten();
                    }
                    var distance = pnt.DistanceTo(point);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestElement = element;
                    }
                }
                else if (locationPoint != null)
                {
                    if (inPlane)
                    {
                        locationPoint = locationPoint.Flatten();
                    }
                    var distance = locationPoint.DistanceTo(point);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestElement = element;
                    }
                }
                else
                {
                    var box = element.get_BoundingBox(null);
                    var center = box.Min.Midpoint(box.Max);
                    if (center != null)
                    {
                        if (inPlane)
                        {
                            center = center.Flatten();
                        }
                        var distance = center.DistanceTo(point);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            closestElement = element;
                        }
                    }
                }
            }

            return closestElement;
        }

        /// <summary>
        /// 查找给定实例中最远的实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="point"></param>
        /// <param name="elements"></param>
        /// <param name="inPlane"></param>
        /// <returns></returns>
        public static T FindFarthestElement<T>(
            this XYZ point,
            IEnumerable<T> elements,
            bool inPlane = true
        )
            where T : Element
        {
            var farthestElement = default(T);
            var maxDistance = double.MinValue;

            foreach (var element in elements)
            {
                var locationCurve = element.GetLocationCurve();
                var locationPoint = element.GetLocationPoint();

                if (locationCurve != null)
                {
                    var pnt = locationCurve.Project(point).XYZPoint;
                    if (inPlane)
                    {
                        pnt = pnt.Flatten();
                    }
                    var distance = pnt.DistanceTo(point);
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        farthestElement = element;
                    }
                }
                else if (locationPoint != null)
                {
                    if (inPlane)
                    {
                        locationPoint = locationPoint.Flatten();
                    }
                    var distance = locationPoint.DistanceTo(point);
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        farthestElement = element;
                    }
                }
                else
                {
                    var box = element.get_BoundingBox(null);
                    var center = box.Min.Midpoint(box.Max);
                    if (center != null)
                    {
                        if (inPlane)
                        {
                            center = center.Flatten();
                        }
                        var distance = center.DistanceTo(point);
                        if (distance > maxDistance)
                        {
                            maxDistance = distance;
                            farthestElement = element;
                        }
                    }
                }
            }

            return farthestElement;
        }

        /// <summary>
        /// 计算两个 XYZ 向量的点积
        /// </summary>
        /// <param name="vector1">第一个 XYZ 向量</param>
        /// <param name="vector2">第二个 XYZ 向量</param>
        /// <param name="modulus">最小单位</param>
        /// <returns>以毫米为单位的点积结果</returns>
        public static double DotProductMM(this XYZ vector1, XYZ vector2, int modulus = 5)
        {
            if (vector1 == null || vector2 == null)
                throw new ArgumentNullException("向量不能为空。");

            return vector1
                .DotProduct(vector2)
                .FeetToMM()
                .RoundToDouble()
                .RoundToNearestIntWithModulus(modulus);
        }

        /// <summary>
        /// 基于中心点创建矩形范围框
        /// </summary>
        /// <param name="midPoint"> 中心点</param>
        /// <param name="y"> y 方向长度</param>
        /// <param name="x"> x 方向长度</param>
        /// <param name="yVector"> y 方向</param>
        /// <param name="xVector"> x 方向</param>
        /// <returns> 矩形范围框</returns>
        public static CurveLoop NewCurveLoop(
            this XYZ midPoint,
            double y,
            double x,
            XYZ yVector,
            XYZ xVector
        )
        {
            var p0 = midPoint - (xVector * x * 0.5) - (yVector * y * 0.5);
            var p1 = midPoint - (xVector * x * 0.5) + (yVector * y * 0.5);
            var p2 = midPoint + (xVector * x * 0.5) + (yVector * y * 0.5);
            var p3 = midPoint + (xVector * x * 0.5) - (yVector * y * 0.5);
            var curves = new CurveLoop();
            curves.Append(Line.CreateBound(p0, p1));
            curves.Append(Line.CreateBound(p1, p2));
            curves.Append(Line.CreateBound(p2, p3));
            curves.Append(Line.CreateBound(p3, p0));
            return curves;
        }

        /// <summary>
        /// 定位点保留小数位
        /// </summary>
        /// <param name="xyz"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public static XYZ Round(this XYZ xyz, int digits = 5)
        {
            return new XYZ(
                xyz.X.RoundToDouble(digits),
                xyz.Y.RoundToDouble(digits),
                xyz.Z.RoundToDouble(digits)
            );
        }

        /// <summary>
        /// 点在多边形内部
        /// </summary>
        /// <param name="point"></param>
        /// <param name="polygon"></param>
        /// <returns></returns>
        public static bool IsInside(this XYZ point, List<XYZ> polygon)
        {
            var count = polygon.Count;
            if (count < 3)
            {
                return false;
            }

            var result = false;
            for (int i = 0, j = count - 1; i < count; j = i++)
            {
                if (
                    (polygon[i].Y > point.Y) != (polygon[j].Y > point.Y)
                    && (
                        point.X
                        < (
                            (polygon[j].X - polygon[i].X)
                            * (point.Y - polygon[i].Y)
                            / (polygon[j].Y - polygon[i].Y)
                        ) + polygon[i].X
                    )
                )
                {
                    result = !result;
                }
            }

            return result;
        }

        /// <summary>
        /// 计算两点之间的曼哈顿距离
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static double ManhattanDistance(this XYZ point1, XYZ point2)
        {
            var distanceX = Math.Abs(point1.X - point2.X);
            var distanceY = Math.Abs(point1.Y - point2.Y);
            var distanceZ = Math.Abs(point1.Z - point2.Z);
            return distanceX + distanceY + distanceZ;
        }

        /// <summary>
        /// 点xyz是否在 BoundingBoxXYZ 的区域内
        /// </summary>
        /// <param name="point"> 点</param>
        /// <param name="box"> 包围盒</param>
        /// <param name="compareZ"> 是否计算Z坐标</param>
        /// <returns> 是否在范围内</returns>
        public static bool IsInBoundingBox(
            this XYZ point,
            BoundingBoxXYZ box,
            bool compareZ = false
        )
        {
            var _min = new XYZ(
                box.Min.X <= box.Max.X ? box.Min.X : box.Max.X,
                box.Min.Y <= box.Max.Y ? box.Min.Y : box.Max.Y,
                box.Min.Z <= box.Max.Z ? box.Min.Z : box.Max.Z
            );
            var _max = new XYZ(
                box.Min.X > box.Max.X ? box.Min.X : box.Max.X,
                box.Min.Y > box.Max.Y ? box.Min.Y : box.Max.Y,
                box.Min.Z > box.Max.Z ? box.Min.Z : box.Max.Z
            );
            var x = point.X >= _min.X && point.X <= _max.X;
            var y = point.Y >= _min.Y && point.Y <= _max.Y;
            var z = point.Z >= _min.Z && point.Z <= _max.Z;
            return compareZ ? x && y && z : x && y;
        }

        /// <summary>
        /// 基于中心点及方向创建线段
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="dir"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static Line CreateLine(this XYZ origin, XYZ dir, double len = 1)
        {
            var vector = dir.Multiply(len);
            return Line.CreateBound(origin.Subtract(vector), origin.Add(vector));
        }

        /// <summary>
        /// 获取轴线
        /// </summary>
        /// <param name="origin"> 基点</param>
        /// <param name="direction"> 默认BasisZ</param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static Line GetAxis(this XYZ origin, XYZ direction = default, double length = 1)
        {
            var dir = direction ?? XYZ.BasisZ;
            return Line.CreateBound(origin, origin + (dir * length));
        }
    }
}
