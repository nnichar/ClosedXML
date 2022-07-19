﻿using System;
using System.Collections.Generic;
using System.Reflection;
using AnyValue = OneOf.OneOf<ClosedXML.Excel.CalcEngine.Logical, ClosedXML.Excel.CalcEngine.Number1, ClosedXML.Excel.CalcEngine.Text, ClosedXML.Excel.CalcEngine.Error1, ClosedXML.Excel.CalcEngine.Array, ClosedXML.Excel.CalcEngine.Reference>;

namespace ClosedXML.Excel.CalcEngine
{
    internal class FunctionRegistry
    {
        private readonly Dictionary<string, FormulaFunction> _func = new(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<string, FunctionDefinition> _legacyFunc = new(StringComparer.InvariantCultureIgnoreCase);

        public void RegisterFunction(string functionName, CalcEngineFunction fn)
        {
            _func.Add(functionName, new FormulaFunction(fn, 0, int.MaxValue));
        }

        public void RegisterFunction(string functionName, CalcEngineFunction fn, int parmMin, int parmMax)
        {
            _func.Add(functionName, new FormulaFunction(fn, parmMin, parmMax));
        }

        public bool TryGetFunc(string name, out FormulaFunction func)
        {
            return _func.TryGetValue(name, out func);
        }

        public bool TryGetFunc(string name, out FunctionDefinition func)
        {
            return _legacyFunc.TryGetValue(name, out func);
        }


        #region Legacy registration

        public void RegisterFunction(string functionName, int parmCount, LegacyCalcEngineFunction fn)
        {
            RegisterFunction(functionName, parmCount, parmCount, fn);
        }

        public void RegisterFunction(string functionName, int parmMin, int parmMax, LegacyCalcEngineFunction fn)
        {
            _legacyFunc.Add(functionName, new FunctionDefinition(functionName, parmMin, parmMax, fn));
        }

        public bool TryGetFunc(string name, out int paramMin, out int paramMax)
        {
            if (_func.TryGetValue(name, out var funcDef))
            {
                paramMin = funcDef.ParmMin;
                paramMax = funcDef.ParmMax;
                return true;
            }

            if (_legacyFunc.TryGetValue(name, out var func))
            {
                paramMin = func.ParmMin;
                paramMax = func.ParmMax;
                return true;
            }

            paramMin = -1;
            paramMax = -1;
            return false;
        }

        #endregion
    }
}