using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using BatisGenerator.dal;
using BatisGenerator.biz.common;

namespace BatisGenerator.biz
{
    public class DaoInterfaceGenerator
    {
        private string tableName;
        private string formatedTableName;
        private string schemaName;
        private string usingNameSpace;
        private Hashtable columnInfoMap;
        private Hashtable primaryKeyMap;
        private Hashtable tableTypeMap;
        private MySqlConnector dbManager;
        private MySqlDataTypeMapper dataTypeMapper;
        public DaoInterfaceGenerator(string schemaName,string tableName)
        {
            this.usingNameSpace = "using System; \nusing System.Collections.Generic; \nusing System.Text; \nusing Trans.DAL.Entity;\n\n";
            this.tableName = tableName;
            this.schemaName = schemaName;
            this.formatedTableName = CommonUtil.formateString(tableName);
            this.dataTypeMapper = new MySqlDataTypeMapper();
            this.dbManager = new MySqlConnector();
            this.columnInfoMap = this.dbManager.getColumnsPairInfo(this.schemaName, this.tableName);
            this.primaryKeyMap = this.dbManager.getPrimaryKeyPairInfo(schemaName);
            this.tableTypeMap = this.dbManager.getTableTypeMap(schemaName);
        }

        public string getDaoInterface()
        {
            StringBuilder daoInterfaceBuilder = new StringBuilder();

            daoInterfaceBuilder.Append(this.usingNameSpace);

            daoInterfaceBuilder.Append("namespace Trans.DAL.Dao \n");
            daoInterfaceBuilder.Append("{\n");
            daoInterfaceBuilder.Append("    public partial interface I");
            daoInterfaceBuilder.Append(this.formatedTableName);
            daoInterfaceBuilder.Append("Dao\n");
            daoInterfaceBuilder.Append("    {\n");



            daoInterfaceBuilder.Append(this.getSelectInterface());
            if (this.tableName.ToCharArray()[0] != 'v')
            {
                daoInterfaceBuilder.Append(this.getInsertInterface());
                daoInterfaceBuilder.Append(this.getUpdateInterface());
                daoInterfaceBuilder.Append(this.getDeleteInterface());
            }
            daoInterfaceBuilder.Append("		void Reload(" + this.formatedTableName + " obj);\n");

            daoInterfaceBuilder.Append("	}\n");
            daoInterfaceBuilder.Append("}\n");
            return daoInterfaceBuilder.ToString();
        }

        private string getSelectInterface()
        {
            Hashtable primaryKeyPainMap = this.dbManager.getPrimaryKeyPairInfo(this.schemaName);
            String primaryKey = null;
            String primaryKeyDBtype = null;
            String primaryKeyDatatype = null;
            if (primaryKeyPainMap.ContainsKey(this.tableName))
            {
                primaryKey = primaryKeyPainMap[this.tableName].ToString();
                primaryKeyDBtype = this.columnInfoMap[primaryKey].ToString();
                primaryKeyDatatype = this.dataTypeMapper.DataTypeMapper[primaryKeyDBtype].ToString();
            }

            StringBuilder selectInterfaceDefinationBuilder = new StringBuilder();

            selectInterfaceDefinationBuilder.Append("		int GetCount();\n\n");

            //Find
            if (null != primaryKey)
            {
                selectInterfaceDefinationBuilder.Append("		");
                selectInterfaceDefinationBuilder.Append(this.formatedTableName);
                selectInterfaceDefinationBuilder.Append(" Find(");
                selectInterfaceDefinationBuilder.Append(primaryKeyDatatype);
                selectInterfaceDefinationBuilder.Append(" ");
                selectInterfaceDefinationBuilder.Append(primaryKey);
                selectInterfaceDefinationBuilder.Append(");\n\n");

                //Find Count

                selectInterfaceDefinationBuilder.Append("		");
                selectInterfaceDefinationBuilder.Append("int GetFindCount(");
                selectInterfaceDefinationBuilder.Append(primaryKeyDatatype);
                selectInterfaceDefinationBuilder.Append(" ");
                selectInterfaceDefinationBuilder.Append(primaryKey);
                selectInterfaceDefinationBuilder.Append(");\n\n");
            }
            
            


            //FindAll
            selectInterfaceDefinationBuilder.Append("		IList<");
            selectInterfaceDefinationBuilder.Append(this.formatedTableName);
            selectInterfaceDefinationBuilder.Append("> FindAll();\n\n");

            //FindAllPagination
            selectInterfaceDefinationBuilder.Append("		IList<");
            selectInterfaceDefinationBuilder.Append(this.formatedTableName);
            selectInterfaceDefinationBuilder.Append("> PaginationFindAll(" + this.formatedTableName + "Pagination obj);\n\n");

            //QuickFindAll
            selectInterfaceDefinationBuilder.Append("		IList<");
            selectInterfaceDefinationBuilder.Append(this.formatedTableName);
            selectInterfaceDefinationBuilder.Append("> QuickFindAll();\n\n");

            //FindBy
            foreach (string columnName in columnInfoMap.Keys)
            {
                if (this.tableTypeMap[this.tableName].ToString() == "VIEW" || this.primaryKeyMap.ContainsKey(this.tableName) && columnName != this.primaryKeyMap[this.tableName].ToString())
                {
                    selectInterfaceDefinationBuilder.Append(this.getSelectInterfaceFindByItem(columnName));
                    selectInterfaceDefinationBuilder.Append(this.getSelectInterfaceFindByItem(columnName, true));
                    selectInterfaceDefinationBuilder.Append(this.getFindByCountItem(columnName));
                }
            }
            
            return selectInterfaceDefinationBuilder.ToString();
        }

        private string getFindByCountItem(string columnName)
        {
            StringBuilder itemBuilder = new StringBuilder();
            itemBuilder.Append("		");
            itemBuilder.Append("int FindCountBy");
            itemBuilder.Append(CommonUtil.formateString(columnName));
            itemBuilder.Append("(");
            itemBuilder.Append(this.formatedTableName);
            itemBuilder.Append(" obj);\n\n");
            return itemBuilder.ToString();
        }

        private string getSelectInterfaceFindByItem(string columnName,bool isPagination = false)
        {
            StringBuilder itemBuilder = new StringBuilder();
            itemBuilder.Append("		IList<");
            itemBuilder.Append(formatedTableName);
            if (isPagination)
            {
                itemBuilder.Append("> PaginationFindBy");
            }
            else
            {
                itemBuilder.Append("> FindBy");
            }
            itemBuilder.Append(CommonUtil.formateString(columnName));
            itemBuilder.Append("(");
            if (isPagination)
            {
                itemBuilder.Append(this.formatedTableName);
                itemBuilder.Append("Pagination obj");
            }
            else
            {
                itemBuilder.Append(this.dataTypeMapper.DataTypeMapper[columnInfoMap[columnName].ToString()].ToString());
                itemBuilder.Append(" ");
                itemBuilder.Append(columnName);
            }
            itemBuilder.Append(");\n\n");
            return itemBuilder.ToString();
        }

        private string getInsertInterface()
        {
            return "		void Insert(" + this.formatedTableName + " obj);\n\n";
        }

        private string getUpdateInterface()
        {
            return "		void Update(" + this.formatedTableName + " obj);\n\n";
        }

        private string getDeleteInterface()
        {
            StringBuilder deleteBuilder = new StringBuilder();
            //Delete
            deleteBuilder.Append("		void Delete(");
            deleteBuilder.Append(this.formatedTableName);
            deleteBuilder.Append(" obj);\n\n");

            //DeleteBy
            foreach (string columnName in columnInfoMap.Keys)
            {
                if (columnName != this.primaryKeyMap[this.tableName].ToString())
                {
                    deleteBuilder.Append("		int DeleteBy");
                    deleteBuilder.Append(CommonUtil.formateString(columnName));
                    deleteBuilder.Append("(");
                    deleteBuilder.Append(this.dataTypeMapper.DataTypeMapper[columnInfoMap[columnName].ToString()].ToString());
                    deleteBuilder.Append(" ");
                    deleteBuilder.Append(columnName);
                    deleteBuilder.Append(");\n\n");
                }
            }

            return deleteBuilder.ToString();
        }
    }
}

