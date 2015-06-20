using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using BatisGenerator.dal;
using BatisGenerator.biz.common;

namespace BatisGenerator.biz
{
    public class DaoGenerator
    {
        private string tableName;
        private string formatedTableName;
        private string schemaName;
        private string usingNameSpace;
        private string mybatisSqlmapFile;
        private Hashtable columnInfoMap;
        private Hashtable primaryKeyMap;
        private Hashtable tableTypeMap;
        private MySqlConnector dbManager;
        private MySqlDataTypeMapper dataTypeMapper;
        public DaoGenerator(string schemaName,string tableName)
        {
            this.usingNameSpace = "using System;\nusing System.Collections.Generic;\nusing System.Text;\nusing IBatisNet.DataMapper;\nusing IBatisNet.DataMapper.Configuration;\nusing Trans.DAL.Entity;\n\n";
            this.tableName = tableName;
            this.schemaName = schemaName;
            this.mybatisSqlmapFile = "etc/mybatis.sqlmap.cfg.xml";
            this.formatedTableName = CommonUtil.formateString(tableName);
            this.dataTypeMapper = new MySqlDataTypeMapper();
            this.dbManager = new MySqlConnector();
            this.columnInfoMap = this.dbManager.getColumnsPairInfo(this.schemaName, this.tableName);
            this.primaryKeyMap = this.dbManager.getPrimaryKeyPairInfo(schemaName);
            this.tableTypeMap = this.dbManager.getTableTypeMap(schemaName);
        }

        public string getDao()
        {
            StringBuilder daoBuilder = new StringBuilder();
            daoBuilder.Append(this.usingNameSpace);
            daoBuilder.Append("namespace Trans.DAL.Dao \n");
            daoBuilder.Append("{\n\n");

            daoBuilder.Append("    public partial class " + this.formatedTableName + "Dao : I" + this.formatedTableName + "Dao\n");
            daoBuilder.Append("    {\n");

            daoBuilder.Append("        private ISqlMapper sqlMapper = null;\n");
            //Construct method
            daoBuilder.Append("        public " + this.formatedTableName + "Dao()\n");
            daoBuilder.Append("        {\n");
            daoBuilder.Append("            DomSqlMapBuilder sqlMapBuilder = new DomSqlMapBuilder();\n");
            daoBuilder.Append("            this.sqlMapper = sqlMapBuilder.Configure(\"" + this.mybatisSqlmapFile + "\");\n");
            daoBuilder.Append("        }\n");

            //select
            daoBuilder.Append(this.getSelectMethods());
            if (this.tableName.ToCharArray()[0] != 'v')
            {
                //update
                daoBuilder.Append(this.getUpdateMethods());
                //insert
                daoBuilder.Append(this.getInsertMethods());
                //delete
                daoBuilder.Append(this.getDeleteMethods());
            }
            //reload
            daoBuilder.Append(this.getReloadMethod());
            daoBuilder.Append("    }\n");
            daoBuilder.Append("}\n");
            return daoBuilder.ToString();
        }

        public string getSelectMethods()
        {
            StringBuilder selectBuilder = new StringBuilder();
            //GetCount
            selectBuilder.Append("		public int GetCount() {\n");
            selectBuilder.Append("			String stmtId = \"" + this.formatedTableName + ".GetCount\";\n");
            selectBuilder.Append("			int result = this.sqlMapper.QueryForObject<int>(stmtId, null);\n");
            selectBuilder.Append("			return result;\n");
            selectBuilder.Append("		}\n");
            //Find
            selectBuilder.Append("		public " + this.formatedTableName + " Find(Int64 id) \n");
            selectBuilder.Append("        {\n");
            selectBuilder.Append("			String stmtId = \"" + this.formatedTableName + ".Find\";\n");
            selectBuilder.Append("			" + this.formatedTableName + " result = this.sqlMapper.QueryForObject<" + this.formatedTableName + ">(stmtId, id);\n");
            selectBuilder.Append("			return result;\n");
            selectBuilder.Append("        }\n");
            //Find Count
            selectBuilder.Append("		public int GetFindCount(Int64 id) {\n");
            selectBuilder.Append("			String stmtId = \"" + this.formatedTableName + ".GetFindCount\";\n");
            selectBuilder.Append("			int result = this.sqlMapper.QueryForObject<int>(stmtId, id);\n");
            selectBuilder.Append("			return result;\n");
            selectBuilder.Append("		}\n");
            //FindAll
            selectBuilder.Append("		public IList<" + this.formatedTableName + "> FindAll() {\n");
            selectBuilder.Append("			String stmtId = \"" + this.formatedTableName + ".FindAll\";\n");
            selectBuilder.Append("			IList<" + this.formatedTableName + "> result = this.sqlMapper.QueryForList<" + this.formatedTableName + ">(stmtId, null);\n");
            selectBuilder.Append("			return result;\n");
            selectBuilder.Append("        }\n");

            //PaginationFindAll
            selectBuilder.Append("		public IList<" + this.formatedTableName + "> PaginationFindAll("+this.formatedTableName+"Pagination obj) {\n");
            selectBuilder.Append("			String stmtId = \"" + this.formatedTableName + ".FindAllPagination\";\n");
            selectBuilder.Append("			IList<" + this.formatedTableName + "> result = this.sqlMapper.QueryForList<" + this.formatedTableName + ">(stmtId, obj);\n");
            selectBuilder.Append("			return result;\n");
            selectBuilder.Append("        }\n");

            //QuickFindAll
            selectBuilder.Append("		public IList<" + this.formatedTableName + "> QuickFindAll() {\n");
            selectBuilder.Append("			String stmtId = \"" + this.formatedTableName + ".QuickFindAll\";\n");
            selectBuilder.Append("			IList<" + this.formatedTableName + "> result = this.sqlMapper.QueryForList<" + this.formatedTableName + ">(stmtId, null);\n");
            selectBuilder.Append("			return result;\n");
            selectBuilder.Append("        }\n");
            //FindBy
            foreach (string columnName in this.columnInfoMap.Keys)
            {
                string csharpDatatype = this.dataTypeMapper.DataTypeMapper[columnInfoMap[columnName].ToString()].ToString();
                if (this.tableTypeMap[this.tableName].ToString() == "VIEW" || this.primaryKeyMap.ContainsKey(this.tableName) && columnName != this.primaryKeyMap[this.tableName].ToString())
                {
                    selectBuilder.Append("		public IList<" + this.formatedTableName + "> FindBy" + CommonUtil.formateString(columnName) + "(" + csharpDatatype + " " + columnName + ") {\n");
                    selectBuilder.Append("			String stmtId = \"" + this.formatedTableName + ".FindBy" + CommonUtil.formateString(columnName) + "\";\n");
                    selectBuilder.Append("			IList<" + this.formatedTableName + "> result = this.sqlMapper.QueryForList<" + this.formatedTableName + ">(stmtId, " + columnName + ");\n");
                    selectBuilder.Append("			return result;\n");
                    selectBuilder.Append("        }\n");
                    //Pagination
                    selectBuilder.Append("		public IList<" + this.formatedTableName + "> PaginationFindBy" + CommonUtil.formateString(columnName) + "(" + this.formatedTableName + "Pagination obj) {\n");
                    selectBuilder.Append("			String stmtId = \"" + this.formatedTableName + "Pagination.FindBy" + CommonUtil.formateString(columnName) + "\";\n");
                    selectBuilder.Append("			IList<" + this.formatedTableName + "> result = this.sqlMapper.QueryForList<" + this.formatedTableName + ">(stmtId, obj);\n");
                    selectBuilder.Append("			return result;\n");
                    selectBuilder.Append("        }\n");
                    //FindBy Count
                    selectBuilder.Append("		public int FindCountBy");
                    selectBuilder.Append(CommonUtil.formateString(columnName));
                    selectBuilder.Append("(");
                    selectBuilder.Append(this.formatedTableName);
                    selectBuilder.Append(" obj){\n");
                    selectBuilder.Append("			String stmtId = \"" + this.formatedTableName + ".GetFindBy");
                    selectBuilder.Append(CommonUtil.formateString(columnName));
                    selectBuilder.Append("Count\";\n");
                    selectBuilder.Append("			int result = this.sqlMapper.QueryForObject<int>(stmtId, obj);\n");
                    selectBuilder.Append("			return result;\n");
                    selectBuilder.Append("		}\n");
                }
            }


            return selectBuilder.ToString();
        }

        public string getInsertMethods()
        {
            StringBuilder insertBuilder = new StringBuilder();

            insertBuilder.Append("		public void Insert(" + this.formatedTableName + " obj) {\n");
            insertBuilder.Append("			if (obj == null) throw new ArgumentNullException(\"obj\");\n");
            insertBuilder.Append("			String stmtId = \"" + this.formatedTableName + ".Insert\";\n");
            insertBuilder.Append("			this.sqlMapper.Insert(stmtId, obj);\n");
            insertBuilder.Append("		}\n");

            return insertBuilder.ToString();
        }

        public string getUpdateMethods()
        {
            StringBuilder updateBuilder = new StringBuilder();
            updateBuilder.Append("		public void Update("+this.formatedTableName+" obj) {\n");
            updateBuilder.Append("			String stmtId = \"" + this.formatedTableName + ".Update\";\n");
            updateBuilder.Append("			this.sqlMapper.Update(stmtId, obj);\n");
            updateBuilder.Append("		}\n");
            return updateBuilder.ToString();
        }
        
        public string getDeleteMethods()
        {
            StringBuilder deleteBuilder = new StringBuilder();
            //Delete
            deleteBuilder.Append("		public void Delete("+this.formatedTableName+" obj) {\n");
            deleteBuilder.Append("			if (obj == null) throw new ArgumentNullException(\"obj\");\n");
            deleteBuilder.Append("			String stmtId = \"" + this.formatedTableName + ".Delete\";\n");
            deleteBuilder.Append("			this.sqlMapper.Delete(stmtId, obj);\n");
            deleteBuilder.Append("		}\n");
            //DeleteBy
            foreach (string columnName in this.columnInfoMap.Keys)
            {
                string csharpDatatype = this.dataTypeMapper.DataTypeMapper[columnInfoMap[columnName].ToString()].ToString();
                if (columnName != this.primaryKeyMap[this.tableName].ToString())
                {
                    deleteBuilder.Append("		public int DeleteBy" + CommonUtil.formateString(columnName) + "(" + csharpDatatype + " " + columnName + ") {\n");
                    deleteBuilder.Append("			String stmtId = \"" + this.formatedTableName + ".DeleteBy" + CommonUtil.formateString(columnName) + "\";\n");
                    deleteBuilder.Append("			int result = this.sqlMapper.Delete(stmtId, " + columnName + ");\n");
                    deleteBuilder.Append("			return result;\n");
                    deleteBuilder.Append("        }\n");
                }
            }
            return deleteBuilder.ToString();
        }

        public string getReloadMethod()
        {
            StringBuilder reloadBuilder = new StringBuilder();
            reloadBuilder.Append("		public void Reload(" + this.formatedTableName + " obj) {\n");
            reloadBuilder.Append("			if (obj == null) throw new ArgumentNullException(\"obj\");\n");
            reloadBuilder.Append("			String stmtId = \""+this.formatedTableName+".Find\";\n");
            reloadBuilder.Append("			this.sqlMapper.QueryForObject<" + this.formatedTableName + ">(stmtId, obj, obj);\n");
            reloadBuilder.Append("		}\n");
            return reloadBuilder.ToString();
        }
    }
}
