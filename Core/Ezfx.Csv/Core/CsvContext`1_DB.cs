﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Ezfx.Csv
{
    public static partial class CsvContext
    {
        public static T[] ReadFile<T>(string path, string tableName, CsvConfig config)
            where T : new()
        {
#if NET20 || NET35 || NET40 || NET45 || NET451 || NET452 || NET46 || NET461 || NET462 || NET47 || NET471 || NET472
            if (path.GetIsOleDb())
            {
                DataTable table = GetDataTable(path, tableName);
                return ReadDataTable<T>(table, config);
            }
            else
            {
                return ReadFile<T>(path, config);
            }
#else
            return ReadFile<T>(path, config);
#endif
        }

        public static T[] ReadDataTable<T>(DataTable table, CsvConfig config)
            where T : new()
        {
            List<T> ts = new List<T>();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                ts.Add(Read<T>(table.Rows[i], config));
            }
            return ts.ToArray();
        }

        private static T Read<T>(DataRow dataRow, CsvConfig config) where T : new()
        {
            if (config == null)
            {
                return ReadByTitle<T>(dataRow);
            }

            switch (config.MappingType)
            {
                case CsvMappingType.Title:
                    return ReadByTitle<T>(dataRow);
                case CsvMappingType.Order:
                    return ReadByOrder<T>(dataRow);
                default:
                    return ReadByTitle<T>(dataRow);
            }
        }

        private static T ReadByOrder<T>(DataRow dataRow) where T : new()
        {
            T result = new T();
            Type t = typeof(T);
            List<PropertyInfo> pis = t.GetProperties().ToList();
            for (int i = 0; i < dataRow.ItemArray.Length; i++)
            {
                PropertyInfo pi = GetPropertyInfo(pis, i);
                if (pi != null)
                {
                    pi.SetValue(result, dataRow[i].ToString(), null);
                }
            }
            return result;
        }


        private static T ReadByTitle<T>(DataRow dataRow) where T : new()
        {
            T result = new T();
            Type t = typeof(T);
            List<PropertyInfo> pis = t.GetProperties().ToList();
            for (int i = 0; i < dataRow.ItemArray.Length; i++)
            {
                PropertyInfo pi = GetPropertyInfo(pis, dataRow.Table.Columns[i].ColumnName);
                if (pi != null)
                {
                    pi.SetValue(result, dataRow[i].ToString(),null);
                }
            }
            return result;
        }
    }
}
