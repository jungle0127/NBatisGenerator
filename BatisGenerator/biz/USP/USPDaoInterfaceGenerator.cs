using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BatisGenerator.biz.common;

namespace BatisGenerator.biz.USP
{
    public class USPDaoInterfaceGenerator
    {
        private string procName;
        private string usingNameSpace;
        public USPDaoInterfaceGenerator(string procName)
        {
            this.procName = procName;
            this.usingNameSpace = "using System; \nusing System.Collections.Generic; \nusing System.Text; \nusing Trans.DAL.Entity.USP;\n\n";
        }

        public string getDaoInterface()
        {
            StringBuilder interfaceBuilder = new StringBuilder();
            interfaceBuilder.Append(this.usingNameSpace);
            interfaceBuilder.Append("namespace Trans.DAL.IDao.USP\n");
            interfaceBuilder.Append("{\n");
            interfaceBuilder.Append("    public interface I");
            interfaceBuilder.Append(CommonUtil.formateString(this.procName));
            interfaceBuilder.Append("Dao\n");
            interfaceBuilder.Append("    {\n");
            interfaceBuilder.Append("        void RunProc(");
            interfaceBuilder.Append(CommonUtil.formateString(this.procName));
            interfaceBuilder.Append(" obj);\n");
            interfaceBuilder.Append("    }\n");
            interfaceBuilder.Append("}");
            return interfaceBuilder.ToString();
        }
    }
}
