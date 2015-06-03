using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BatisGenerator.biz.common;

namespace BatisGenerator.biz.USP
{
    public class USPEntityGenerator
    {
        private string usingNameSpace;
        private string procName;
        private MySqlDataTypeMapper dataTypeMapper;
        private IList<ProcedureParameterInfo> paramList;
        public USPEntityGenerator(string procName,IList<ProcedureParameterInfo> paramList)
        {
            ISystemConfig systemConfig = new SystemConfig();
            this.usingNameSpace = "using System;\nusing System.Collections.Generic; \nusing System.Text;\n";
            this.procName = procName;
            this.paramList = paramList;
            this.dataTypeMapper = new MySqlDataTypeMapper();
        }

        public string getEntity()
        {
            StringBuilder entityBuilder = new StringBuilder();
            entityBuilder.Append(this.usingNameSpace);
            entityBuilder.Append("\n\n");
            entityBuilder.Append("namespace Trans.DAL.Entity.USP\n");
            entityBuilder.Append("{\n");

            entityBuilder.Append("    [Serializable]\n");
            entityBuilder.Append("    public class ");
            entityBuilder.Append(CommonUtil.formateString(this.procName));
            entityBuilder.Append("\n");
            entityBuilder.Append("    {\n");

            foreach (ProcedureParameterInfo ppi in this.paramList)
            {
                string dataType = this.dataTypeMapper.DataTypeMapper[this.getDatatype(ppi.ParameterDataType)].ToString();
                entityBuilder.Append("        private ");
                entityBuilder.Append(dataType);
                entityBuilder.Append(" _");
                entityBuilder.Append(ppi.PrameterName);
                entityBuilder.Append(";\n\n");

                entityBuilder.Append("        public ");
                entityBuilder.Append(dataType);
                entityBuilder.Append(" ");
                entityBuilder.Append(CommonUtil.formateString(ppi.PrameterName));
                entityBuilder.Append("\n");
                entityBuilder.Append("        {\n");
                entityBuilder.Append("            get { return ");
                entityBuilder.Append("_" + ppi.PrameterName);
                entityBuilder.Append("; }\n");
                entityBuilder.Append("            set { ");
                entityBuilder.Append("_" + ppi.PrameterName);
                entityBuilder.Append(" = value; }\n");

                entityBuilder.Append("        }\n");
            }

            entityBuilder.Append("    }\n");
            entityBuilder.Append("}");
            return entityBuilder.ToString();
        }

        private string getDatatype(string dbType)
        {
            return dbType.Split('(')[0];
        }
    }
}
