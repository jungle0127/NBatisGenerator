using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BatisGenerator.biz.common;

namespace BatisGenerator.biz.USP
{
    public class USPMapperGenerator
    {
        private string procName;
        private IList<ProcedureParameterInfo> paramList;
        private ISystemConfig systemConfig;
        private string nameSpace;
        private string xmlHeader;
        private string formatedProceName;
        private string sqlMapHeader;
        public USPMapperGenerator(string procName,IList<ProcedureParameterInfo> paramList)
        {
            this.procName = procName;
            this.paramList = paramList;
            this.systemConfig = new SystemConfig();
            this.xmlHeader = "<?xml version=\"1.0\" encoding=\"utf-8\" ?> \n";
            this.formatedProceName = CommonUtil.formateString(procName);
            this.nameSpace = "Trans.DAL.Entity.USP." + this.formatedProceName;
            this.sqlMapHeader = "<sqlMap namespace=\"" + this.formatedProceName + "\" xmlns=\"http://ibatis.apache.org/mapping\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\n";
        }

        public string getMapper()
        {
            StringBuilder mapperBuilder = new StringBuilder();
            mapperBuilder.Append(this.xmlHeader);
            mapperBuilder.Append(this.sqlMapHeader);
            mapperBuilder.Append(this.getAliasSetion());
            mapperBuilder.Append(this.getParameterMapsSection());
            mapperBuilder.Append(this.getStatementsSection());
            mapperBuilder.Append("</sqlMap>\n");
            return mapperBuilder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string getAliasSetion()
        {
            StringBuilder aliasBuilder = new StringBuilder();
            aliasBuilder.Append("  <alias>\n");
            aliasBuilder.Append("    <typeAlias alias=\"");
            aliasBuilder.Append(this.formatedProceName);
            aliasBuilder.Append("\" type=\"");
            aliasBuilder.Append(this.nameSpace);
            aliasBuilder.Append("\" />\n");
            aliasBuilder.Append("  </alias>\n");
            return aliasBuilder.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string getParameterMapsSection()
        {
            StringBuilder parameterMapsBuilder = new StringBuilder();
            parameterMapsBuilder.Append("  <parameterMaps>\n");
            parameterMapsBuilder.Append("  <parameterMap id=\"");
            parameterMapsBuilder.Append(this.procName);
            parameterMapsBuilder.Append("_data\"");
            parameterMapsBuilder.Append(" class=\"");
            parameterMapsBuilder.Append(this.formatedProceName);
            parameterMapsBuilder.Append("\">\n");

            foreach (ProcedureParameterInfo paramInfo in this.paramList)
            {
                parameterMapsBuilder.Append("      <parameter property=\"");
                parameterMapsBuilder.Append(CommonUtil.getFirstLetterUpperString(paramInfo.PrameterName));
                parameterMapsBuilder.Append("\" dbType=\"");
                parameterMapsBuilder.Append(paramInfo.ParameterDataType);
                parameterMapsBuilder.Append("\" direction=\"");
                switch (paramInfo.ParamType)
                {
                    case ProcedureParameterType.IN:
                        parameterMapsBuilder.Append("Input");
                        break;
                    case ProcedureParameterType.OUT:
                        parameterMapsBuilder.Append("Output");
                        break;
                    case ProcedureParameterType.INOUT:
                        parameterMapsBuilder.Append("InputOutput");
                        break;
                }
                parameterMapsBuilder.Append("\"></parameter>\n");
            }

            parameterMapsBuilder.Append("    </parameterMap>\n");
            parameterMapsBuilder.Append("  </parameterMaps>\n");
            return parameterMapsBuilder.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string getStatementsSection()
        {
            StringBuilder statementBuilder = new StringBuilder();

            statementBuilder.Append("  <statements>\n");
            statementBuilder.Append("    <procedure id=\"");
            statementBuilder.Append(this.procName + "id");
            statementBuilder.Append("\" parameterMap=\"");
            statementBuilder.Append(this.procName);
            statementBuilder.Append("_data\">\n");
            statementBuilder.Append(this.procName + "\n");
            statementBuilder.Append("    </procedure>\n");
            statementBuilder.Append("  </statements>\n");

            return statementBuilder.ToString();
        }

    }
}