﻿/* ****************************************************************************
 *
 * Copyright (c) Jeff Hardy 2010-2012.
 *
 * This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the  Apache License, Version 2.0, please send an email to 
 * dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Apache License, Version 2.0.
 *
 * You must not remove this notice, or any other, from this software.
 *
 *
 * ***************************************************************************/

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Community.CsharpSqlite;
using IronPython.Runtime;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;

[assembly: PythonModule("_sqlite3", typeof(IronPython.SQLite.PythonSQLite))]

namespace IronPython.SQLite
{
    public static partial class PythonSQLite
    {
        public static readonly string version = string.Format("{0}.{1}.{2}", 
                                                    IronPython.CurrentVersion.Major, 
                                                    IronPython.CurrentVersion.Minor, 
                                                    IronPython.CurrentVersion.Micro);
        public static readonly string sqlite_version = Sqlite3.sqlite3_version.Replace("(C#)", "");

        public static PythonDictionary converters = new PythonDictionary();
        public static PythonDictionary adapters = new PythonDictionary();

        public static readonly Type OptimizedUnicode = typeof(string);

        internal static Encoding Latin1 = Encoding.GetEncoding("iso-8859-1");

        [SpecialName]
        public static void PerformModuleReload(PythonContext context, PythonDictionary dict)
        {
            InitModuleExceptions(context, dict);
        }

        public static object connect(CodeContext context,
            string database,
            double timeout=0.0,
            int detect_types=0,
            string isolation_level=null,
            bool check_same_thread=true,
            object factory=null,
            int cached_statements=0)
        {
            if(factory == null)
                return new Connection(database, timeout, detect_types, isolation_level, check_same_thread, factory, cached_statements);
            else
                return PythonCalls.Call(context, factory, database, timeout, detect_types, isolation_level, check_same_thread, factory, cached_statements);
        }

        [Documentation(@"register_adapter(type, callable)

Registers an adapter with pysqlite's adapter registry. Non-standard.")]
        public static void register_adapter(CodeContext context, PythonType type, object adapter)
        {
            object proto = DynamicHelpers.GetPythonTypeFromType(typeof(PrepareProtocol));
            object key = new PythonTuple(new object[] { type, proto });
            adapters[key] = adapter;
        }

        [Documentation(@"register_converter(typename, callable)

Registers a converter with pysqlite. Non-standard.")]
        public static void register_converter(CodeContext context, string type, object converter)
        {
            converters[type.ToUpperInvariant()] = converter;
        }

        public class PrepareProtocol { }
    }
}
