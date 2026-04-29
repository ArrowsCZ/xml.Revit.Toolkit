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

using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Color = Autodesk.Revit.DB.Color;

namespace xml.Revit.Toolkit.Extensions;

/// <summary>
/// DocumentExtensions
/// </summary>
public static class DocumentExtensions
{
    /// <param name="doc"></param>
    extension(Document doc)
    {
        /// <summary>
        /// 获取实例
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IList<Element> OfClass(Type type)
        {
            doc.ThrowIfNullOrInvalid();

            return new FilteredElementCollector(doc).OfClass(type).WhereElementIsNotElementType().ToElements();
        }

        /// <summary>
        /// 获取实例
        /// </summary>
        /// <param name="type"></param>
        /// <param name="builtInCategory"></param>
        /// <returns></returns>
        public IList<Element> OfClass(Type type, BuiltInCategory builtInCategory)
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
        /// <param name="builtInCategory"></param>
        /// <returns></returns>
        public IList<Element> OfCategory(BuiltInCategory builtInCategory)
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
        /// <returns></returns>
        public IList<T> OfClass<T>()
            where T : Element
        {
            doc.ThrowIfNullOrInvalid();

            return new FilteredElementCollector(doc).OfClass(typeof(T)).Cast<T>().ToList();
        }

        /// <summary>
        /// 获取实例或类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builtInCategory"></param>
        /// <returns></returns>
        public IList<T> OfClass<T>(BuiltInCategory builtInCategory)
            where T : Element
        {
            doc.ThrowIfNullOrInvalid();

            return new FilteredElementCollector(doc).OfClass(typeof(T)).OfCategory(builtInCategory).Cast<T>().ToList();
        }

        /// <summary>
        /// 获取实例或类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="view"></param>
        /// <returns></returns>
        public IList<T> OfClass<T>(View view)
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
        /// <param name="view"></param>
        /// <param name="builtInCategory"></param>
        /// <returns></returns>
        public IList<T> OfClass<T>(View view, BuiltInCategory builtInCategory)
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
        /// <returns></returns>
        [Pure]
        public IList<Element> GetAllElements()
        {
            doc.ThrowIfNullOrInvalid();
            return new FilteredElementCollector(doc).WhereElementIsNotElementType().ToElements();
        }

        /// <summary>
        /// 获取所有视图可见的实例
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [Pure]
        public IList<Element> GetAllElements(View view)
        {
            doc.ThrowIfNullOrInvalid();
            view.ThrowIfNullOrInvalid();
            return new FilteredElementCollector(doc, view.Id).WhereElementIsNotElementType().ToElements();
        }

        /// <summary>
        /// 获取所有类型
        /// </summary>
        /// <returns></returns>
        [Pure]
        public IList<Element> GetAllElementTypes()
        {
            doc.ThrowIfNullOrInvalid();
            return new FilteredElementCollector(doc).WhereElementIsElementType().ToElements();
        }

        /// <summary>
        /// 获取所有视图可见的类型
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [Pure]
        public IList<Element> GetAllElementTypes(View view)
        {
            doc.ThrowIfNullOrInvalid();
            view.ThrowIfNullOrInvalid();
            return new FilteredElementCollector(doc, view.Id).WhereElementIsElementType().ToElements();
        }

        /// <summary>
        /// 设置自动连接
        /// </summary>
        /// <param name="el1">主要的实体</param>
        /// <param name="el2">次要的实例</param>
        public void JoinElements(Element el1, Element el2)
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
        /// <returns> 视图列表</returns>
        public IEnumerable<View> GetViews()
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
        /// <param name="origin"></param>
        /// <param name="text"></param>
        /// <param name="view"></param>
        /// <param name="textNoteTypeId"></param>
        /// <returns></returns>
        public TextNote CreateTextNote(XYZ origin, string text, View view = null, ElementId textNoteTypeId = null)
        {
            view ??= doc.ActiveView;
            if (view is View3D)
            {
                return null;
            }
            textNoteTypeId ??= doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType);
            var textNote = TextNote.Create(doc, view.Id, origin, text, textNoteTypeId);
            return textNote;
        }

        /// <summary>
        /// 创建详图线
        /// </summary>
        /// <param name="curve"> 定位线</param>
        /// <param name="view"> 视图</param>
        /// <returns> 详图线</returns>
        public DetailCurve CreateDetailCurve(Curve curve, View view = null)
        {
            doc.ThrowIfNullOrInvalid();

            if (curve is null)
            {
                return null;
            }
            if (curve.Length < 1e-6)
            {
                return null;
            }
            view ??= doc.ActiveView;
            if (view is View3D)
            {
                return null;
            }
            return doc.Create.NewDetailCurve(view, curve);
        }

        /// <summary>
        /// 创建详图线
        /// </summary>
        /// <param name="curve"> 定位线</param>
        /// <param name="graphicsStyle"> 线样式</param>
        /// <param name="view"> 视图</param>
        /// <returns> 详图线</returns>
        public DetailCurve CreateDetailCurve(Curve curve, GraphicsStyle graphicsStyle, View view = null)
        {
            var detailCurve = doc.CreateDetailCurve(curve, view);
            detailCurve?.LineStyle = graphicsStyle;
            return detailCurve;
        }

        /// <summary>
        /// 创建圆形详图线注释
        /// </summary>
        /// <param name="center"> 中心点</param>
        /// <param name="radius"> 半径</param>
        /// <param name="view"> 视图</param>
        /// <param name="graphicsStyle"></param>
        /// <returns> 详图线</returns>
        public DetailArc CreateDetailArc(
            XYZ center,
            double radius = 1,
            View view = null,
            GraphicsStyle graphicsStyle = null
        )
        {
            if (center is null)
            {
                return null;
            }
            view ??= doc.ActiveView;
            if (view is View3D)
            {
                return null;
            }
            var arc = Arc.Create(center, radius, 0, Math.PI * 2, view.RightDirection, view.UpDirection);
            if (arc.Length < 1e-6)
            {
                return null;
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
        /// <param name="curve"></param>
        /// <param name="normal"></param>
        /// <param name="graphicsStyle"></param>
        /// <returns></returns>
        public ModelCurve CreateModelCurve(Curve curve, XYZ normal = null, GraphicsStyle graphicsStyle = null)
        {
            if (curve is null)
            {
                return null;
            }
            normal ??= XYZ.BasisZ;
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
        /// <param name="geos"></param>
        /// <returns></returns>
        public DirectShape CreateDirectShape(params GeometryObject[] geos)
        {
            var ds = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_GenericModel));
            ds.SetName("xml.Revit");
            ds.ApplicationId = "52A3FA43-C16F-4E21-9A43-6BE13544F87D";
            ds.ApplicationDataId = Guid.NewGuid().ToString();
            ds.SetShape(geos);
            return ds;
        }

        /// <summary>
        /// 创建LineStyle
        /// </summary>
        /// <param name="lineStyleName">名称</param>
        /// <param name="color">颜色</param>
        /// <param name="width">宽度</param>
        /// <param name="linePatternName">线样式</param>
        public GraphicsStyle CreateLineStyle(
            string lineStyleName,
            Color color,
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
            var category = categories.NewSubcategory(categories.get_Item(BuiltInCategory.OST_Lines), lineStyleName);
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
        /// <returns> 获取对象列表</returns>
        public IEnumerable<GraphicsStyle> GetGraphicsStyles()
        {
            var map = Category.GetCategory(doc, BuiltInCategory.OST_Lines).SubCategories;
            return map.Cast<Category>().Select(o => o.GetGraphicsStyle(GraphicsStyleType.Projection));
        }

        /// <summary>
        /// 过滤项目中指定类别图元
        /// </summary>
        /// <param name="outline"></param>
        /// <param name="builtInCategory"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public List<T> BoundingBoxIntersectsFilter<T>(
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
        /// <param name="solid"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public List<T> SolidIntersectsFilter<T>(Solid solid, BuiltInCategory category = BuiltInCategory.INVALID)
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
    }

    /// <summary>
    /// 设置
    /// </summary>
    private static readonly Options Opt = new()
    {
        ComputeReferences = false,
        IncludeNonVisibleObjects = false,
        DetailLevel = ViewDetailLevel.Fine,
    };

    /// <param name="doc"></param>
    extension(Document doc)
    {
        /// <summary>
        /// 获取当前标高所有的房间
        /// </summary>
        /// <param name="level"></param>
        /// <param name="hasLinkDoc">链接房间所在标高名称等于参数<paramref name="level"/>名称</param>
        /// <returns></returns>
        public List<Room> GetLevelRooms(Level level, bool hasLinkDoc)
        {
            List<Room> rooms = [];
            var docRooms = doc.OfClass<SpatialElement>(BuiltInCategory.OST_Rooms)
                .Where(o =>
                    o.Area != 0
                    && (o.Level.Name.Equals(level.Name) || o.Level.ProjectElevation.Equals(level.ProjectElevation))
                )
                .Cast<Room>()
                .ToList();
            if (docRooms.Any())
                rooms.AddRange(docRooms);

            if (hasLinkDoc)
            {
                var linkInstances = doc.OfClass<RevitLinkInstance>().Where(o => o.HasLinked());
                foreach (var item in linkInstances)
                {
                    var linkDocument = item.GetLinkDocument();
                    var linkRooms = linkDocument
                        .OfClass<SpatialElement>(BuiltInCategory.OST_Rooms)
                        .Where(o =>
                            o.Area != 0
                            && (
                                o.Level.Name.Equals(level.Name)
                                || o.Level.ProjectElevation.Equals(level.ProjectElevation)
                            )
                        )
                        .Cast<Room>()
                        .ToList();
                    if (linkRooms.Any())
                        rooms.AddRange(linkRooms);
                }
            }

            return rooms;
        }

        /// <summary>
        /// 获取或创建工作集
        /// </summary>
        /// <param name="workSetName"></param>
        /// <param name="workSetKind"></param>
        /// <returns></returns>
        public Workset NewWorkSet(string workSetName, WorksetKind workSetKind = WorksetKind.UserWorkset)
        {
            using FilteredWorksetCollector elementCollector = new(doc);
            var workSets = elementCollector.OfKind(workSetKind).ToWorksets();
            var workSet = workSets.FirstOrDefault(o => o.Name == workSetName);
            if (workSet == null)
            {
                if (doc.IsWorkshared)
                {
                    if (WorksetTable.IsWorksetNameUnique(doc, workSetName))
                        workSet = Workset.Create(doc, workSetName);
                    else
                        throw new Exception("工作集名称无法创建:" + workSetName);
                }
            }
            return workSet;
        }

        /// <summary>
        /// 获取风管尺寸宽度高度可行列表值
        /// </summary>
        /// <param name="widths"></param>
        /// <param name="heights"></param>
        public void GetDuctSizes(out List<double> widths, out List<double> heights)
        {
            var mepSizes = DuctSizeSettings
                .GetDuctSizeSettings(doc)
                .FirstOrDefault(o => o.Key == DuctShape.Rectangular)
                .Value;
            widths = [];
            heights = [];
            foreach (var size in mepSizes)
            {
                var d = size.NominalDiameter.FeetToMm().RoundToDouble();
                if (size.UsedInSizeLists)
                    widths.Add(d);
                if (size.UsedInSizing)
                    heights.Add(d);
            }
        }

        /// <summary>
        /// 创建三维视图
        /// 设置 名称及显示样式(着色)
        /// </summary>
        /// <param name="name"> 视图名称</param>
        /// <param name="setStyle"> 是否设置显示样式</param>
        /// <returns> View3D</returns>
        public View3D Create3DView(string name, bool setStyle = true)
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
