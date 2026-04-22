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
using Point = Autodesk.Revit.DB.Point;
using View = Autodesk.Revit.DB.View;

namespace xml.Revit.Toolkit.Extensions
{
    /// <summary>
    /// ElementExtensions
    /// </summary>
    public static class ElementExtensions
    {
        /// <summary>
        /// 原地复制 Element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="el"></param>
        /// <returns></returns>
        public static T CopyElement<T>(this T el)
            where T : Element
        {
            var doc = el.Document;
            var id = ElementTransformUtils
                .CopyElement(el.Document, el.Id, XYZ.Zero)
                .FirstOrDefault();
            return id.ToElement(doc) as T;
        }

        /// <summary>
        /// 获取模型对象的 Solid
        /// <para>用于尺寸标注</para>
        /// </summary>
        /// <param name="el"> 对象</param>
        /// <returns>按体积大小升序排列</returns>
        public static List<Solid> GetSolidBySymbol(this Element el)
        {
            var solids = new List<Solid>();
            if (el == null)
            {
                return solids;
            }
            var opt = new Autodesk.Revit.DB.Options
            {
                ComputeReferences = true,
                IncludeNonVisibleObjects = true,
            };
            foreach (var g in el.get_Geometry(opt))
            {
                if (g is GeometryInstance geometryInstance)
                {
                    foreach (var g_ins in geometryInstance.GetSymbolGeometry())
                    {
                        if (g_ins is Solid solidIns && solidIns.Volume > 0.0001)
                        {
                            solids.Add(solidIns);
                        }
                    }
                }
                else
                {
                    if (g is Solid solid && solid.Volume > 0.0001)
                    {
                        solids.Add(solid);
                    }
                }
            }
            return solids.OrderBy(o => o.Volume).ToList();
        }

        /// <summary>
        /// 判断给定对象 结构 参数是否勾选
        /// </summary>
        /// <param name="el"> 对象</param>
        /// <returns> 是否结构属性</returns>
        public static bool IsAnalyticalModel(this Element el)
        {
            if (el.IsEqualCategory(BuiltInCategory.OST_Walls))
            {
                return el.get_Parameter(BuiltInParameter.WALL_STRUCTURAL_SIGNIFICANT).AsInteger()
                    == 1;
            }
            else
            {
                if (el.IsEqualCategory(BuiltInCategory.OST_Floors))
                {
                    return el.get_Parameter(BuiltInParameter.FLOOR_PARAM_IS_STRUCTURAL).AsInteger()
                        == 1;
                }
                else
                {
                    bool framing = el.IsEqualCategory(BuiltInCategory.OST_StructuralFraming);
                    bool column = el.IsEqualCategory(BuiltInCategory.OST_StructuralColumns);
                    bool level = el.IsEqualCategory(BuiltInCategory.OST_Levels);
                    return framing
                        || column
                        || (
                            level
                            && el.get_Parameter(BuiltInParameter.LEVEL_IS_STRUCTURAL).AsInteger()
                                == 1
                        );
                }
            }
        }

        /// <summary>
        /// 已链接到项目
        /// </summary>
        /// <param name="linkInstance"></param>
        /// <returns></returns>
        [Pure]
        public static bool HasLinked(this RevitLinkInstance linkInstance)
        {
            return linkInstance?.GetLinkDocument() != null;
        }

        /// <summary>
        /// 将元素转换为指定类型
        /// </summary>
        /// <typeparam name="T">Element派生的类型</typeparam>
        /// <exception cref="InvalidCastException">Element 不能被转换</exception>
        [Pure]
        public static T Cast<T>(this Element element)
            where T : Element
        {
            return (T)element;
        }

        /// <summary>
        /// 复制一个元素，并将副本放置在给定变换所指示的位置上
        /// </summary>
        /// <param name="element"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static ElementId Copy(this Element element, XYZ vector)
        {
            return ElementTransformUtils
                .CopyElement(element.Document, element.Id, vector)
                .FirstOrDefault();
        }

        /// <summary>
        /// 镜像实例
        /// </summary>
        /// <param name="element"></param>
        /// <param name="plane"></param>
        /// <returns></returns>
        public static Element Mirror(this Element element, Plane plane)
        {
            ElementTransformUtils.MirrorElement(element.Document, element.Id, plane);
            return element;
        }

        /// <summary>
        /// 镜像实例
        /// </summary>
        /// <param name="element"></param>
        /// <param name="plane"></param>
        /// <param name="mirrorCopies"></param>
        /// <returns></returns>
        public static ElementId Mirror(this Element element, Plane plane, bool mirrorCopies)
        {
            var ids = new List<ElementId> { element.Id };
            return ElementTransformUtils
                .MirrorElements(element.Document, ids, plane, mirrorCopies)
                .FirstOrDefault();
        }

        /// <summary>
        /// 按指定矢量移动元素
        /// </summary>
        /// <param name="element"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Element Move(this Element element, XYZ vector)
        {
            ElementTransformUtils.MoveElement(element.Document, element.Id, vector);
            return element;
        }

        /// <summary>
        /// 围绕给定轴线和角度旋转元素
        /// </summary>
        /// <param name="element"></param>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Element Rotate(this Element element, Line axis, double angle)
        {
            ElementTransformUtils.RotateElement(element.Document, element.Id, axis, angle);
            return element;
        }

        /// <summary>
        /// 是否可以镜像
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        [Pure]
        public static bool CanBeMirrored(this Element element)
        {
            return ElementTransformUtils.CanMirrorElement(element.Document, element.Id);
        }

#if REVIT2022_OR_GREATER
        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="element"></param>
        /// <param name="parameter"></param>
        /// <param name="snoopType"></param>
        /// <returns></returns>
        [Pure]
        public static Parameter GetParameter(
            this Element element,
            ForgeTypeId parameter,
            bool snoopType
        )
        {
            var instanceParameter = element.GetParameter(parameter);
            if (instanceParameter is { HasValue: true } || snoopType == false)
                return instanceParameter;
            var elementTypeId = element.GetTypeId();
            if (elementTypeId == ElementId.InvalidElementId)
                return instanceParameter;
            var elementType = element.Document.GetElement(elementTypeId);
            var symbolParameter = elementType.GetParameter(parameter);
            return symbolParameter ?? instanceParameter;
        }
#endif

        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="element"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [Pure]
        public static Parameter GetParameter(this Element element, BuiltInParameter parameter)
        {
            var instanceParameter = element.get_Parameter(parameter);
            if (instanceParameter is { HasValue: true })
                return instanceParameter;
            var elementTypeId = element.GetTypeId();
            if (elementTypeId == ElementId.InvalidElementId)
                return instanceParameter;
            var elementType = element.Document.GetElement(elementTypeId);
            var symbolParameter = elementType.get_Parameter(parameter);
            return symbolParameter ?? instanceParameter;
        }

        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="element"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [Pure]
        public static Parameter GetParameter(this Element element, string parameter)
        {
            var instanceParameter = element.LookupParameter(parameter);
            if (instanceParameter is { HasValue: true })
                return instanceParameter;
            var elementTypeId = element.GetTypeId();
            if (elementTypeId == ElementId.InvalidElementId)
                return instanceParameter;
            var elementType = element.Document.GetElement(elementTypeId);
            var symbolParameter = elementType.LookupParameter(parameter);
            return symbolParameter ?? instanceParameter;
        }

        /// <summary>
        /// 获取对象参数(实例+类型)且略参数大小写
        /// </summary>
        /// <param name="el"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [Pure]
        public static Parameter LookupParameterByName(this Element el, string name)
        {
            if (el is null)
            {
                return default;
            }

            var parameter =
                el.LookupParameter(name) ?? GetParameterByNameIngoreCase(name, el.Parameters);
            if (parameter == null)
            {
                if (el.Document.GetElement(el.GetTypeId()) is ElementType elementtype)
                {
                    parameter =
                        elementtype.LookupParameter(name)
                        ?? GetParameterByNameIngoreCase(name, elementtype.Parameters);
                }
            }
            return parameter;
        }

        /// <summary>
        /// 获取参数忽略大小写
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parameterSet"></param>
        /// <returns></returns>
        [Pure]
        private static Parameter GetParameterByNameIngoreCase(
            string name,
            ParameterSet parameterSet
        )
        {
            var _name = name.ToLower();
            foreach (Parameter item in parameterSet)
            {
                if (item.Definition.Name.Equals(_name, StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }
            }

            return default;
        }

        /// <summary>
        /// 通过名称更新参数
        /// </summary>
        /// <typeparam name="T"> 类型</typeparam>
        /// <param name="el"> Element</param>
        /// <param name="name"> 参数名称不能是重复字段</param>
        /// <param name="value"> 参数值 带类型</param>
        /// <returns> 是否成功 更新参数</returns>
        public static bool SetParameterByName<T>(this Element el, string name, T value)
        {
            if (el is null)
            {
                return false;
            }
            var parameter = el.LookupParameterByName(name);
            if (parameter != null && !parameter.IsReadOnly)
            {
                if (value is double para_double)
                {
                    return parameter.Set(para_double);
                }
                else if (value is int para_int)
                {
                    return parameter.Set(para_int);
                }
                else if (value is string para_string)
                {
                    return parameter.Set(para_string);
                }
                else if (value is ElementId para_ElementId)
                {
                    return parameter.Set(para_ElementId);
                }
            }
            return false;
        }

        /// <summary>
        /// 通过名称获取参数值
        /// 自动测试参数(全部大写/小写)
        /// </summary>
        /// <param name="el"> 对象</param>
        /// <param name="parameterName"> 参数名称</param>
        /// <returns> string:参数名称对应的参数值.参数名称获取失败则返回:""</returns>
        [Pure]
        public static string GetParameterValueByName(this Element el, string parameterName)
        {
            if (el is null)
            {
                return "";
            }
            var parameter = el.LookupParameterByName(parameterName);
            if (parameter is null)
            {
                return "";
            }
            return parameter.StorageType switch
            {
                StorageType.Double => parameter.AsValueString(),
                StorageType.ElementId => parameter.AsElementId().GetValue().ToString(),
                StorageType.Integer => parameter.AsValueString(),
                StorageType.String => parameter.AsString(),
                StorageType.None => parameter.AsValueString(),
                _ => "",
            };
        }

        /// <summary>
        /// 参数值是否等于给定值
        /// </summary>
        /// <param name="el"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        [Pure]
        public static bool IsEqualsParameterValue(
            this Element el,
            string parameterName,
            string parameterValue
        )
        {
            return parameterValue.Equals(el.GetParameterValueByName(parameterName));
        }

        /// <summary>
        /// 参数值是否等于给定值
        /// </summary>
        /// <param name="el"></param>
        /// <param name="builtInParameter"></param>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        [Pure]
        public static bool IsEqualsParameterValue(
            this Element el,
            BuiltInParameter builtInParameter,
            string parameterValue
        )
        {
            el.ThrowIfNullOrInvalid();
            var p = el.get_Parameter(builtInParameter);
            if (p == null)
            {
                return false;
            }

            if (!p.HasValue)
            {
                return false;
            }

            var value = p.StorageType switch
            {
                StorageType.None => string.Empty,
                StorageType.Integer => p.AsValueString(),
                StorageType.Double => p.AsValueString(),
                StorageType.String => p.AsString(),
                StorageType.ElementId => p.AsElementId().GetValue().ToString(),
                _ => string.Empty,
            };

            return parameterValue.Equals(value);
        }

        /// <summary>
        /// 获取模型的定位点
        /// </summary>
        /// <param name="el"> 对象</param>
        /// <returns> 定位点</returns>
        [Pure]
        public static XYZ GetLocationPoint(this Element el)
        {
            return (el.Location as LocationPoint)?.Point;
        }

        /// <summary>
        /// 获取模型的定位线
        /// </summary>
        /// <param name="el"> 对象</param>
        /// <returns> 定位线</returns>
        [Pure]
        public static Curve GetLocationCurve(this Element el)
        {
            el.ThrowIfNullOrInvalid();
            return (el.Location as LocationCurve)?.Curve;
        }

        /// <summary>
        /// 获取对象的类型
        /// </summary>
        /// <param name="el"> 对象</param>
        /// <returns> 类别BuiltInCategory</returns>
        [Pure]
        public static BuiltInCategory GetBuiltInCategory(this Element el)
        {
            el.ThrowIfNullOrInvalid();
            return el?.Category?.Id != ElementId.InvalidElementId
                ? (BuiltInCategory)el!.Category.Id.GetValue()
                : BuiltInCategory.INVALID;
        }

        /// <summary>
        /// 判断构件是否为指定类别模型
        /// </summary>
        /// <param name="el"> 对象</param>
        /// <param name="builtInCategory"> 类别</param>
        /// <returns> 是否相等</returns>
        [Pure]
        public static bool IsEqualCategory(this Element el, BuiltInCategory builtInCategory)
        {
            el.ThrowIfNullOrInvalid();
            return el?.Category?.Id != ElementId.InvalidElementId
                && (BuiltInCategory)el!.Category.Id.GetValue() == builtInCategory;
        }

        /// <summary>
        /// 判断构件是否为指定类别模型
        /// </summary>
        /// <param name="el"> 对象</param>
        /// <param name="id"> 类别ID</param>
        /// <returns> 是否相等</returns>
        [Pure]
        public static bool IsEqualCategory(this Element el, int id)
        {
            return el?.Category?.Id?.GetValue() == id;
        }

        /// <summary>
        /// 解析图形
        /// </summary>
        /// <param name="element">实例对象</param>
        /// <param name="options">精细程度配置 </param>
        /// <returns></returns>
        public static List<GeometryObject> ResolveGeometry(this Element element, Options options)
        {
            var geometries = element.get_Geometry(options);
            return ResolveGeometry(geometries);
        }

        private static List<GeometryObject> ResolveGeometry(this GeometryElement geometries)
        {
            List<GeometryObject> objects = [];
            foreach (GeometryObject item in geometries)
            {
                switch (item)
                {
                    case GeometryInstance geomInstance:
                        objects.AddRange(ResolveGeometry(geomInstance.GetInstanceGeometry()));
                        break;
                    case GeometryElement geometryElement:
                        objects.AddRange(ResolveGeometry(geometryElement));
                        break;
                    case Curve curve:
                        objects.Add(curve);
                        break;
                    case Edge edge:
                        objects.Add(edge);
                        break;
                    case Face face:
                        objects.Add(face);
                        break;
                    case Mesh mesh:
                        objects.Add(mesh);
                        break;
                    case Point point:
                        objects.Add(point);
                        break;
                    case PolyLine polyLine:
                        objects.Add(polyLine);
                        break;
                    case Profile profile:
                        objects.Add(profile);
                        break;
                    case Solid solid:
                        objects.Add(solid);
                        break;
                }
            }
            return objects;
        }

        /// <summary>
        /// 获取给定所有类型的实例的参数名称可选列表
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        public static List<string> GetParameterNames(this IEnumerable<Element> elements)
        {
            return elements
                .Where(o => o.IsValidObject)
                .SelectMany(o => o.GetOrderedParameters().Select(o => o.Definition.Name))
                .Distinct()
                .OrderBy(o => o)
                .ToList();
        }

        /// <summary>
        /// 获取给定所有类型的实例的参数名称可选列表
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static List<string> GetParameterNames(this Element element)
        {
            return
            [
                .. element
                .GetOrderedParameters()
                .Select(o => o.Definition.Name)
                .Distinct()
                .OrderBy(o => o)
            ];
        }
    }
}
