using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
//add System.Web.Extensions.dll
using Newtonsoft.Json;

namespace FirstOhm
{
    public sealed class DynamicJsonConverter : JavaScriptConverter
    {
        //叫用此物件的方式
        //string json = ...;
        //var serializer = new JavaScriptSerializer();
        //serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
        //dynamic obj = serializer.Deserialize(json, typeof(object));

        // Json 範例
        //[{\"colname\":\"B\",\"condition\":\"contain\", \"compareValue\":\"年\", \"action\":\"keep\"}," +
        //"{\"colname\":\"D\",\"condition\":\"contain\", \"compareValue\":\"序號\", \"action\":\"keep\"}]"
        //  取值 ==> obj[0].condition
        //  or 取值 ==> obj[0]["condition"]
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            return type == typeof(object) ? new DynamicJsonObject(dictionary) : null;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get { return new ReadOnlyCollection<Type>(new List<Type>(new[] { typeof(object) })); }
        }

        #region json convert
        //json to known object
        //var myObject = JsonConvert.DeserializeObject<MyType>(jsonString);

        // object to json
        public static string objToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        public static Dictionary<string, string> jsonToDictionary(string jsonStr)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonStr);
        }


        public static string objectToJson(Object obj)
        {
            string json = new JavaScriptSerializer().Serialize(obj);
            return json;
        }


        //recordNull : false--Data 為 null 時, 不產生 JsonElement
        public static string DtToJson(DataTable dt, bool recordNull = false)
        {
            string colName = null;
            Dictionary<string, object> item = new Dictionary<string, object>();
            List<Dictionary<string, object>> query = new List<Dictionary<string, object>>();
            //Creating and opening a data connection to the Excel sheet 
            foreach (DataRow myRow in dt.Rows)
            {
                item.Clear();
                foreach (DataColumn myColumn in dt.Columns)
                {
                    colName = myColumn.ColumnName;
                    if (myRow[colName] == DBNull.Value && !recordNull)
                        continue;
                    else
                        item.Add(colName, myRow[colName]);
                }
                if (item.Count > 0)
                    query.Add(item);
            }
            var json = JsonConvert.SerializeObject(query);
            return json;
        }

        public static List<object> dynamicArrObjToListObj(dynamic dynamicList)
        {
            if (dynamicList is IEnumerable) //handles null as well
                return Enumerable.ToList(dynamicList);
            else
                return null;
            //throw; //better meaning here.
        }

        public static dynamic jsonToObj(string jsonStr)
        {
            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
            dynamic obj = serializer.Deserialize(jsonStr, typeof(object));
            return obj;
        }

        public static List<object> dtToListObj(DataTable dt)
        {
            object objectArr = jsonToObj(DtToJson(dt));
            return dynamicArrObjToListObj(objectArr);
        }

        #endregion

        #region Nested type: DynamicJsonObject

        private class DynamicJsonObject : DynamicObject
        {
            private readonly IDictionary<string, object> _dictionary;

            public DynamicJsonObject(IDictionary<string, object> dictionary)
            {
                if (dictionary == null)
                    throw new ArgumentNullException("dictionary");
                _dictionary = dictionary;
            }

            public override string ToString()
            {
                var sb = new StringBuilder("{");
                ToString(sb);
                return sb.ToString();
            }

            private void ToString(StringBuilder sb)
            {
                var firstInDictionary = true;
                foreach (var pair in _dictionary)
                {
                    if (!firstInDictionary)
                        sb.Append(",");
                    firstInDictionary = false;
                    var value = pair.Value;
                    var name = pair.Key;
                    if (value is string)
                    {
                        sb.AppendFormat("{0}:\"{1}\"", name, value);
                    }
                    else if (value is IDictionary<string, object>)
                    {
                        new DynamicJsonObject((IDictionary<string, object>)value).ToString(sb);
                    }
                    else if (value is ArrayList)
                    {
                        sb.Append(name + ":[");
                        var firstInArray = true;
                        foreach (var arrayValue in (ArrayList)value)
                        {
                            if (!firstInArray)
                                sb.Append(",");
                            firstInArray = false;
                            if (arrayValue is IDictionary<string, object>)
                                new DynamicJsonObject((IDictionary<string, object>)arrayValue).ToString(sb);
                            else if (arrayValue is string)
                                sb.AppendFormat("\"{0}\"", arrayValue);
                            else
                                sb.AppendFormat("{0}", arrayValue);

                        }
                        sb.Append("]");
                    }
                    else
                    {
                        sb.AppendFormat("{0}:{1}", name, value);
                    }
                }
                sb.Append("}");
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                if (!_dictionary.TryGetValue(binder.Name, out result))
                {
                    // return null to avoid exception.  caller can check for null this way...
                    result = null;
                    return true;
                }

                result = WrapResultObject(result);
                return true;
            }

            public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
            {
                if (indexes.Length == 1 && indexes[0] != null)
                {
                    if (!_dictionary.TryGetValue(indexes[0].ToString(), out result))
                    {
                        // return null to avoid exception.  caller can check for null this way...
                        result = null;
                        return true;
                    }

                    result = WrapResultObject(result);
                    return true;
                }
                return base.TryGetIndex(binder, indexes, out result);
            }

            private static object WrapResultObject(object result)
            {
                var dictionary = result as IDictionary<string, object>;
                if (dictionary != null)
                    return new DynamicJsonObject(dictionary);

                var arrayList = result as ArrayList;
                if (arrayList != null && arrayList.Count > 0)
                {
                    return arrayList[0] is IDictionary<string, object>
                        ? new List<object>(arrayList.Cast<IDictionary<string, object>>().Select(x => new DynamicJsonObject(x)))
                        : new List<object>(arrayList.Cast<object>());
                }
                return result;
            }
        }

        #endregion
    }
}
