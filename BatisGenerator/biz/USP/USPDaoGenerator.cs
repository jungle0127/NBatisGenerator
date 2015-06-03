using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BatisGenerator.biz.common;

namespace BatisGenerator.biz.USP
{
    public class USPDaoGenerator
    {
        private string usingNameSpace;
        private string procName;
        public USPDaoGenerator(string procName)
        {
            this.usingNameSpace = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Trans.DAL.IDao.USP;
using Trans.DAL.Entity.USP;
using IBatisNet.DataMapper;
using IBatisNet.DataMapper.Configuration;
";
            this.procName = procName;

        }
        public string getUSPDao()
        {
            StringBuilder daoBuilder = new StringBuilder();
            daoBuilder.Append(this.usingNameSpace);
            daoBuilder.Append("namespace Trans.DAL.Dao.USP\n");
            daoBuilder.Append("{\n");
            daoBuilder.Append("public class ");
            daoBuilder.Append(CommonUtil.formateString(this.procName));
            daoBuilder.Append("Dao : I");
            daoBuilder.Append(CommonUtil.formateString(this.procName));
            daoBuilder.Append("Dao\n");

            daoBuilder.Append("    {\n");

            daoBuilder.Append("        private ISqlMapper sqlMapper = null;\n");
            daoBuilder.Append("        public ");
            daoBuilder.Append(CommonUtil.formateString(this.procName));
            daoBuilder.Append("Dao()\n");
            daoBuilder.Append("        {\n");
            daoBuilder.Append("            DomSqlMapBuilder sqlMapBuilder = new DomSqlMapBuilder();\n");
            daoBuilder.Append("            this.sqlMapper = sqlMapBuilder.Configure(\"etc/mybatis.sqlmap.cfg.xml\");\n");
            daoBuilder.Append("        }\n");


            daoBuilder.Append("        public void RunProc(");
            daoBuilder.Append(CommonUtil.formateString(this.procName));
            daoBuilder.Append(" obj)\n");
            daoBuilder.Append("        {\n");
            daoBuilder.Append("            String stmtId = \"");
            daoBuilder.Append(this.procName);
            daoBuilder.Append("id\";\n");
            daoBuilder.Append("            this.sqlMapper.QueryForObject(stmtId, obj);\n");
            daoBuilder.Append("        }\n");
            daoBuilder.Append("    }\n");

            daoBuilder.Append("}\n");
            return daoBuilder.ToString();
        }
    }
}
