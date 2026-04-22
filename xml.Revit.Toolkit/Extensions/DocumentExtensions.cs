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

using System.Diagnostics.Contracts;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using View = Autodesk.Revit.DB.View;

namespace xml.Revit.Toolkit.Extensions
{
    /// <summary>
    /// DocumentExtensions
    /// </summary>
    public static class DocumentExtensions
    {
        /// <summary>
        /// 获取实例
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IList<Element> OfClass(this Document doc, Type type)
        {
            doc.ThrowIfNullOrInvalid();

            return new FilteredElementCollector(doc)
                .OfClass(type)
                .WhereElementIsNotElementType()
                .ToElements();
        }

        /// <summary>
        /// 获取实例
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="type"></param>
        /// <param name="builtInCategory"></param>
        /// <returns></returns>
        public static IList<Element> OfClass(
            this Document doc,
            Type type,
            BuiltInCategory builtInCategory
        )
        {
            doc.ThrowIfNullOrInvalid();

            return new FilteredElementCollector(doc)
                .OfClass(type)
                .OfCategory(builtInCategory)
                .WhereElementIsNotElementType()
                .ToElements();
        }

        /// <summary>
        /// 获取实例
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="builtInCategory"></param>
        /// <returns></returns>
        public static IList<Element> OfCategory(this Document doc, BuiltInCategory builtInCategory)
        {
            doc.ThrowIfNullOrInvalid();

            return new FilteredElementCollector(doc)
                .OfCategory(builtInCategory)
                .WhereElementIsNotElementType()
                .ToElements();
        }

        /// <summary>
        /// 获取实例或类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static IList<T> OfClass<T>(this Document doc)
            where T : Element
        {
            doc.ThrowIfNullOrInvalid();

            return new FilteredElementCollector(doc).OfClass(typeof(T)).Cast<T>().ToList();
        }

        /// <summary>
        /// 获取实例或类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="doc"></param>
        /// <param name="builtInCategory"></param>
        /// <returns></returns>
        public static IList<T> OfClass<T>(this Document doc, BuiltInCategory builtInCategory)
            where T : Element
        {
            doc.ThrowIfNullOrInvalid();

            return new FilteredElementCollector(doc)
                .OfClass(typeof(T))
                .OfCategory(builtInCategory)
                .Cast<T>()
                .ToList();
        }

        /// <summary>
        /// 获取实例或类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="doc"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        public static IList<T> OfClass<T>(this Document doc, View view)
            where T : Element
        {
            doc.ThrowIfNullOrInvalid();
            view.ThrowIfNullOrInvalid();

            return new FilteredElementCollector(doc, view.Id).OfClass(typeof(T)).Cast<T>().ToList();
        }

        /// <summary>
        /// 获取实例或类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="doc"></param>
        /// <param name="view"></param>
        /// <param name="builtInCategory"></param>
        /// <returns></returns>
        public static IList<T> OfClass<T>(
            this Document doc,
            View view,
            BuiltInCategory builtInCategory
        )
            where T : Element
        {
            doc.ThrowIfNullOrInvalid();
            view.ThrowIfNullOrInvalid();

            return new FilteredElementCollector(doc, view.Id)
                .OfClass(typeof(T))
                .OfCategory(builtInCategory)
                .Cast<T>()
                .ToList();
        }

        /// <summary>
        /// 获取所有实例
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        [Pure]
        public static IList<Element> GetAllElements(this Document doc)
        {
            doc.ThrowIfNullOrInvalid();
            return new FilteredElementCollector(doc).WhereElementIsNotElementType().ToElements();
        }

        /// <summary>
        /// 获取所有视图可见的实例
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        [Pure]
        public static IList<Element> GetAllElements(this Document doc, View view)
        {
            doc.ThrowIfNullOrInvalid();
            view.ThrowIfNullOrInvalid();
            return new FilteredElementCollector(doc, view.Id)
                .WhereElementIsNotElementType()
                .ToElements();
        }

        /// <summary>
        /// 获取所有类型
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        [Pure]
        public static IList<Element> GetAllElementTypes(this Document doc)
        {
            doc.ThrowIfNullOrInvalid();
            return new FilteredElementCollector(doc).WhereElementIsElementType().ToElements();
        }

        /// <summary>
        /// 获取所有视图可见的类型
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        [Pure]
        public static IList<Element> GetAllElementTypes(this Document doc, View view)
        {
            doc.ThrowIfNullOrInvalid();
            view.ThrowIfNullOrInvalid();
            return new FilteredElementCollector(doc, view.Id)
                .WhereElementIsElementType()
                .ToElements();
        }

        /// <summary>
        /// 设置自动连接
        /// </summary>
        /// <param name="doc">doc</param>
        /// <param name="el1">主要的实体</param>
        /// <param name="el2">次要的实例</param>
        public static void JoinElements(this Document doc, Element el1, Element el2)
        {
            doc.ThrowIfNullOrInvalid();

            if (JoinGeometryUtils.AreElementsJoined(doc, el1, el2))
            {
                if (!JoinGeometryUtils.IsCuttingElementInJoin(doc, el1, el2))
                {
                    JoinGeometryUtils.SwitchJoinOrder(doc, el1, el2);
                }
            }
            else
            {
                JoinGeometryUtils.JoinGeometry(doc, el1, el2);
            }
        }

        /// <summary>
        /// 获取当前窗口激活的视图集合
        /// </summary>
        /// <param name="doc"> 文档</param>
        /// <returns> 视图列表</returns>
        public static IEnumerable<View> GetViews(this Document doc)
        {
            if (doc == null)
            {
                yield break;
            }
            if (doc.ActiveView is ViewSheet sheet)
            {
                foreach (var id in sheet.GetAllPlacedViews())
                {
                    var view = id.ToElement(doc) as View;
                    yield return view;
                }
            }
            else
            {
                yield return doc.ActiveView;
            }
        }

        /// <summary>
        /// 创建文字
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="origin"></param>
        /// <param name="text"></param>
        /// <param name="view"></param>
        /// <param name="textNoteTypeId"></param>
        /// <returns></returns>
        public static TextNote CreateTextNote(
            this Document doc,
            XYZ origin,
            string text,
            View view = null,
            ElementId textNoteTypeId = null
        )
        {
            view ??= doc.ActiveView;
            if (view is View3D)
            {
                return default;
            }
            textNoteTypeId =
                textNoteTypeId ?? doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType);
            var textNote = TextNote.Create(doc, view.Id, origin, text, textNoteTypeId);
            return textNote;
        }

        /// <summary>
        /// 创建详图线
        /// </summary>
        /// <param name="doc"> doc</param>
        /// <param name="curve"> 定位线</param>
        /// <param name="view"> 视图</param>
        /// <returns> 详图线</returns>
        public static DetailCurve CreateDetailCurve(
            this Document doc,
            Curve curve,
            View view = null
        )
        {
            doc.ThrowIfNullOrInvalid();

            if (curve is null)
            {
                return default;
            }
            if (curve.Length < 1e-6)
            {
                return default;
            }
            @view ??= doc.ActiveView;
            if (view is View3D)
            {
                return default;
            }
            return doc.Create.NewDetailCurve(view, curve);
        }

        /// <summary>
        /// 创建详图线
        /// </summary>
        /// <param name="doc"> doc</param>
        /// <param name="curve"> 定位线</param>
        /// <param name="graphicsStyle"> 线样式</param>
        /// <param name="view"> 视图</param>
        /// <returns> 详图线</returns>
        public static DetailCurve CreateDetailCurve(
            this Document doc,
            Curve curve,
            GraphicsStyle graphicsStyle,
            View view = null
        )
        {
            var detailCurve = CreateDetailCurve(doc, curve, view);
            if (detailCurve != null)
            {
                detailCurve.LineStyle = graphicsStyle;
            }
            return detailCurve;
        }

        /// <summary>
        /// 创建圆形详图线注释
        /// </summary>
        /// <param name="doc"> doc</param>
        /// <param name="center"> 中心点</param>
        /// <param name="radius"> 半径</param>
        /// <param name="view"> 视图</param>
        /// <param name="graphicsStyle"></param>
        /// <returns> 详图线</returns>
        public static DetailArc CreateDetailArc(
            this Document doc,
            XYZ center,
            double radius = 1,
            View view = null,
            GraphicsStyle graphicsStyle = default
        )
        {
            if (center is null)
            {
                return default;
            }
            @view ??= doc.ActiveView;
            if (view is View3D)
            {
                return default;
            }
            var arc = Arc.Create(
                center,
                radius,
                0,
                Math.PI * 2,
                view.RightDirection,
                view.UpDirection
            );
            if (arc.Length < 1e-6)
            {
                return default;
            }
            var detailArc = doc.Create.NewDetailCurve(view, arc) as DetailArc;
            detailArc?.get_Parameter(BuiltInParameter.ARC_WALL_CNTR_MRK_VISIBLE).Set(1);
            if (detailArc != null && graphicsStyle != null)
            {
                detailArc.LineStyle = graphicsStyle;
            }
            return detailArc;
        }

        /// <summary>
        /// 创建模型线
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="curve"></param>
        /// <param name="normal"></param>
        /// <param name="graphicsStyle"></param>
        /// <returns></returns>
        public static ModelCurve CreateModelCurve(
            this Document doc,
            Curve curve,
            XYZ normal = null,
            GraphicsStyle graphicsStyle = default
        )
        {
            if (curve is null)
            {
                return default;
            }
            normal = normal ?? XYZ.BasisZ;
            var plane = Plane.CreateByNormalAndOrigin(normal, curve.GetEndPoint(0));
            var sketch = SketchPlane.Create(doc, plane);
            var modelCurve = doc.Create.NewModelCurve(curve, sketch);
            if (modelCurve != null && graphicsStyle != null)
            {
                modelCurve.LineStyle = graphicsStyle;
            }
            return modelCurve;
        }

        /// <summary>
        /// 创建的几何形状
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="geos"></param>
        /// <returns></returns>
        public static DirectShape CreateDirectShape(this Document doc, params GeometryObject[] geos)
        {
            var ds = DirectShape.CreateElement(
                doc,
                new ElementId(BuiltInCategory.OST_GenericModel)
            );
            ds.SetName("xml.Revit");
            ds.ApplicationId = "52A3FA43-C16F-4E21-9A43-6BE13544F87D";
            ds.ApplicationDataId = Guid.NewGuid().ToString();
            ds.SetShape(geos);
            return ds;
        }

        /// <summary>
        /// 创建LineStyle
        /// </summary>
        /// <param name="doc">文档</param>
        /// <param name="lineStyleName">名称</param>
        /// <param name="color">颜色</param>
        /// <param name="width">宽度</param>
        /// <param name="linePatternName">线样式</param>
        public static GraphicsStyle CreateLineStyle(
            this Document doc,
            string lineStyleName,
            Autodesk.Revit.DB.Color color,
            int width = 1,
            string linePatternName = "实线"
        )
        {
            var g = doc.GetGraphicsStyles().FirstOrDefault(o => o.Name == lineStyleName);
            if (g != null)
            {
                return g;
            }
            var categories = doc.Settings.Categories;
            var category = categories.NewSubcategory(
                categories.get_Item(BuiltInCategory.OST_Lines),
                lineStyleName
            );
            category.LineColor = color;
            category.SetLineWeight(width, GraphicsStyleType.Projection);
            var linePattern = LinePatternElement.GetLinePatternElementByName(doc, linePatternName);
            if (linePattern != null)
            {
                category.SetLinePatternId(linePattern.Id, GraphicsStyleType.Projection);
            }
            return category.GetGraphicsStyle(GraphicsStyleType.Projection);
        }

        /// <summary>
        /// 获取详图线类型Id
        /// </summary>
        /// <param name="doc"> 文档</param>
        /// <returns> 获取对象列表</returns>
        public static IEnumerable<GraphicsStyle> GetGraphicsStyles(this Document doc)
        {
            var map = Category.GetCategory(doc, BuiltInCategory.OST_Lines).SubCategories;
            return map.Cast<Category>()
                .Select(o => o.GetGraphicsStyle(GraphicsStyleType.Projection));
        }

        /// <summary>
        /// 过滤项目中指定类别图元
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="outline"></param>
        /// <param name="builtInCategory"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static List<T> TBoundingBoxIntersectsFilter<T>(
            this Document doc,
            Outline outline,
            BuiltInCategory builtInCategory,
            double tolerance = 1e-6
        )
            where T : Element
        {
            var invertFilter = new BoundingBoxIntersectsFilter(outline, tolerance);
            return new FilteredElementCollector(doc)
                .OfCategory(builtInCategory)
                .WherePasses(invertFilter)
                .Cast<T>()
                .ToList();
        }

        /// <summary>
        /// 实体碰撞过滤器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="doc"></param>
        /// <param name="solid"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public static List<T> TSolidIntersectsFilter<T>(
            this Document doc,
            Solid solid,
            BuiltInCategory category = BuiltInCategory.INVALID
        )
            where T : Element
        {
            var filter = new ElementIntersectsSolidFilter(solid);
            var col = new FilteredElementCollector(doc).OfClass(typeof(T));
            var fc = col.OfClass(typeof(T));
            if (category != BuiltInCategory.INVALID)
            {
                fc = fc.OfCategory(category);
            }
            return fc.WherePasses(filter).Cast<T>().ToList();
        }

        /// <summary>
        /// 设置
        /// </summary>
        private static readonly Options opt =
            new()
            {
                ComputeReferences = false,
                IncludeNonVisibleObjects = false,
                DetailLevel = ViewDetailLevel.Fine,
            };

        /// <summary>
        /// 获取当前标高所有的房间
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="level"></param>
        /// <param name="hasLinkdoc">链接房间所在标高名称等于参数<paramref name="level"/>名称</param>
        /// <returns></returns>
        public static List<Room> GetLevelRooms(this Document doc, Level level, bool hasLinkdoc)
        {
            List<Room> rooms = new();
            var docRooms = doc.OfClass<SpatialElement>(BuiltInCategory.OST_Rooms)
                .Where(o =>
                    o.Area != 0
                    && (
                        o.Level.Name.Equals(level.Name)
                        || o.Level.ProjectElevation.Equals(level.ProjectElevation)
                    )
                )
                .Cast<Room>();
            if (docRooms.Any())
            {
                rooms.AddRange(docRooms);
            }
            if (hasLinkdoc)
            {
                var linkInstances = doc.OfClass<RevitLinkInstance>().Where(o => o.HasLinked());
                foreach (var item in linkInstances)
                {
                    var linkdoc = item.GetLinkDocument();
                    var linkRooms = linkdoc
                        .OfClass<SpatialElement>(BuiltInCategory.OST_Rooms)
                        .Where(o =>
                            o.Area != 0
                            && (
                                o.Level.Name.Equals(level.Name)
                                || o.Level.ProjectElevation.Equals(level.ProjectElevation)
                            )
                        )
                        .Cast<Room>();
                    if (linkRooms.Any())
                    {
                        rooms.AddRange(linkRooms);
                    }
                }
            }

            return rooms;
        }

        /// <summary>
        /// 获取或创建工作集
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="worksetName"></param>
        /// <param name="worksetKind"></param>
        /// <returns></returns>
        public static Workset NewWorkset(
            this Document doc,
            string worksetName,
            WorksetKind worksetKind = WorksetKind.UserWorkset
        )
        {
            using FilteredWorksetCollector elementCollector = new(doc);
            var worksets = elementCollector.OfKind(worksetKind).ToWorksets();
            var workset = worksets.FirstOrDefault(o => o.Name == worksetName);
            if (workset == null)
            {
                if (doc.IsWorkshared)
                {
                    if (WorksetTable.IsWorksetNameUnique(doc, worksetName))
                    {
                        workset = Workset.Create(doc, worksetName);
                    }
                    else
                    {
                        throw new Exception("工作集名称无法创建:" + worksetName);
                    }
                }
            }
            return workset;
        }

        /// <summary>
        /// 获取风管尺寸宽度高度可行列表值
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="widths"></param>
        /// <param name="heights"></param>
        public static void GetDuctSizes(
            this Document doc,
            out List<double> widths,
            out List<double> heights
        )
        {
            var mepSizes = DuctSizeSettings
                .GetDuctSizeSettings(doc)
                .FirstOrDefault(o => o.Key == DuctShape.Rectangular)
                .Value;
            widths = new List<double>();
            heights = new List<double>();
            foreach (var size in mepSizes)
            {
                var d = size.NominalDiameter.FeetToMM().RoundToDouble();
                if (size.UsedInSizeLists)
                {
                    widths.Add(d);
                }
                if (size.UsedInSizing)
                {
                    heights.Add(d);
                }
            }
        }

        /// <summary>
        /// 创建三维视图
        /// 设置 名称及显示样式(着色)
        /// </summary>
        /// <param name="doc"> 文档</param>
        /// <param name="name"> 视图名称</param>
        /// <param name="setStyle"> 是否设置显示样式</param>
        /// <returns> View3D</returns>
        public static View3D Create3DView(this Document doc, string name, bool setStyle = true)
        {
            var view3D = doc.OfClass<View3D>().FirstOrDefault(o => o.Name == name);
            if (view3D == null)
            {
                var view3Dtype = doc.OfClass<ViewFamilyType>()
                    .FirstOrDefault(o => ViewFamily.ThreeDimensional == o.ViewFamily);
                if (view3Dtype != null)
                {
                    view3D = View3D.CreateIsometric(doc, view3Dtype.Id);
                    view3D.Name = name;
                    if (setStyle)
                    {
                        view3D.DisplayStyle = DisplayStyle.ShadingWithEdges;
                        view3D.DetailLevel = ViewDetailLevel.Fine;
                    }
                }
            }
            return view3D;
        }
    }
}
