using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using BatisGenerator.dal;
using BatisGenerator.biz.common;

namespace BatisGenerator.biz
{
    public class EntityGenerator
    {
        private string usingNameSpace;
        private string tableName;
        private string formatedTableName;
        private string schemaName;
        private Hashtable columnInfoMap;
        private MySqlConnector dbManager;
        private MySqlDataTypeMapper dataTypeMapper;

        public EntityGenerator(string schemaName,string tableName)
        {
            this.usingNameSpace = "using System;\nusing System.Collections.Generic; \nusing System.Text;\n";
            this.schemaName = schemaName;
            this.tableName = tableName;
            this.dataTypeMapper = new MySqlDataTypeMapper();
            this.dbManager = new MySqlConnector();
            this.columnInfoMap = this.dbManager.getColumnsPairInfo(this.schemaName, this.tableName);
            this.formatedTableName = CommonUtil.formateString(tableName);
        }


        public string getEnity()
        {
            StringBuilder entityBuilder = new StringBuilder();
            entityBuilder.Append(this.usingNameSpace);
            entityBuilder.Append("namespace Trans.DAL.Entity \n");
            entityBuilder.Append("{\n");
            entityBuilder.Append("	[Serializable]\n");
            entityBuilder.Append("	public class ");
            entityBuilder.Append(this.formatedTableName);
            entityBuilder.Append("\n");
            entityBuilder.Append("	{\n");

            foreach (string columnName in this.columnInfoMap.Keys)
            {
                entityBuilder.Append(this.getProperty(columnName, this.columnInfoMap[columnName].ToString()));
            }

            entityBuilder.Append("	}\n");
            entityBuilder.Append("}\n");
            return entityBuilder.ToString();
        }

        public string getPaginationEntity()
        {
            StringBuilder entityBuilder = new StringBuilder();
            entityBuilder.Append(this.usingNameSpace);
            entityBuilder.Append("namespace Trans.DAL.Entity \n");
            entityBuilder.Append("{\n");
            entityBuilder.Append("	[Serializable]\n");
            entityBuilder.Append("	public class ");
            entityBuilder.Append(this.formatedTableName + "Pagination");
            entityBuilder.Append("\n");
            entityBuilder.Append("	{\n");

            foreach (string columnName in this.columnInfoMap.Keys)
            {
                entityBuilder.Append(this.getProperty(columnName, this.columnInfoMap[columnName].ToString()));
            }
            entityBuilder.Append(this.getProperty("limit", "int"));
            entityBuilder.Append(this.getProperty("offset", "int"));
            entityBuilder.Append("	}\n");
            entityBuilder.Append("}\n");
            return entityBuilder.ToString();
        }

        private string getProperty(string columnName,string dataType)
        {
            StringBuilder propertyBuilder = new StringBuilder();
            //private Int64 m_id;
            propertyBuilder.Append("		private ");
            
            propertyBuilder.Append(this.dataTypeMapper.DataTypeMapper[dataType].ToString() + " ");
            propertyBuilder.Append("m_" + columnName + ";\n");
            //public Int64 Id {
            propertyBuilder.Append("		public ");
            propertyBuilder.Append(this.dataTypeMapper.DataTypeMapper[dataType].ToString());
            propertyBuilder.Append(" " + CommonUtil.getFirstLetterUpperString(columnName));
            propertyBuilder.Append("\n		{\n");
            propertyBuilder.Append("			get { return m_" + columnName + "; }\n");
            propertyBuilder.Append("			set { m_" + columnName + " = value;}\n");
            propertyBuilder.Append("		}\n");
            return propertyBuilder.ToString();
        }
    }
}
