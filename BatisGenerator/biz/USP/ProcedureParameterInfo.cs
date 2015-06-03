using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BatisGenerator.biz.USP
{
    /// <summary>
    /// Procedure parameter POCO
    /// </summary>
    public class ProcedureParameterInfo
    {
        private ProcedureParameterType paramType;

        public ProcedureParameterType ParamType
        {
            get { return paramType; }
            set { paramType = value; }
        }
        
        private string prameterName;

        public string PrameterName
        {
            get { return prameterName; }
            set { prameterName = value; }
        }
        private string parameterDataType;

        public string ParameterDataType
        {
            get { return parameterDataType; }
            set { parameterDataType = value; }
        }
    }
    public enum ProcedureParameterType
    {
        IN,
        OUT,
        INOUT
    }
}
