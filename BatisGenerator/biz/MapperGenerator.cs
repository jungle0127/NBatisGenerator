using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BatisGenerator.dal;
using System.Data;
using System.Collections;
using BatisGenerator.biz.common;

namespace BatisGenerator.biz
{
    public class MapperGenerator
    {
        private string nameSpace;
        private string xmlHeader;
        private string sqlMapHeader;
        private string tableName;
        private string formatedTableName;
        private string schemaName;
        private Hashtable columnInfoMap;
        private Hashtable primaryKeyMap;
        private MySqlConnector dbManager;
        private MySqlDataTypeMapper dataTypeMapper;
        public MapperGenerator(string schemaName,string tableName)
        {
            this.schemaName = schemaName;
            this.tableName = tableName;
            this.formatedTableName = CommonUtil.formateString(tableName);
            this.xmlHeader = "<?xml version=\"1.0\" encoding=\"utf-8\" ?> \n";
            this.dataTypeMapper = new MySqlDataTypeMapper();
            this.dbManager = new MySqlConnector();
            this.columnInfoMap = this.dbManager.getColumnsPairInfo(this.schemaName, this.tableName);
            this.nameSpace = "Trans.DAL.Entity." + this.formatedTableName;
            this.sqlMapHeader = "<sqlMap namespace=\"" + this.formatedTableName + "\" xmlns=\"http://ibatis.apache.org/mapping\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\n";
            this.primaryKeyMap = this.dbManager.getPrimaryKeyPairInfo(schemaName);
        }
        public string getMapper()
        {
            StringBuilder mapperBuilder = new StringBuilder();
            mapperBuilder.Append(this.xmlHeader);
            mapperBuilder.Append(this.sqlMapHeader);
            mapperBuilder.Append(this.getAliasSection());
            mapperBuilder.Append(this.getResultMapsSection());
            mapperBuilder.Append(this.getStatementsSection());
            mapperBuilder.Append("</sqlMap>\n");
            return mapperBuilder.ToString();
        }
        private string getAliasSection()
        {
            StringBuilder aliasBuilder = new StringBuilder();
            aliasBuilder.Append("	<alias>\n");
            
            aliasBuilder.Append("		<typeAlias alias=\"");
            aliasBuilder.Append(this.formatedTableName);
            aliasBuilder.Append("\" type=\"");
            aliasBuilder.Append(this.nameSpace);
            aliasBuilder.Append("\" />\n");
            //Add pagination alias.
            aliasBuilder.Append("		<typeAlias alias=\"");
            aliasBuilder.Append(this.formatedTableName);
            aliasBuilder.Append("Pagination\" type=\"");
            aliasBuilder.Append(this.nameSpace);
            aliasBuilder.Append("Pagination\" />\n");

            aliasBuilder.Append("	</alias>\n");
            return aliasBuilder.ToString();
        }

        private string getResultMapsSection()
        {
            StringBuilder resultMapsBuilder = new StringBuilder();
            resultMapsBuilder.Append("	<resultMaps>\n");
            resultMapsBuilder.Append("		<resultMap id=\"FullResultMap\" class=\"" + this.formatedTableName + "\">\n");

            foreach (string columnName in this.columnInfoMap.Keys)
            {
                resultMapsBuilder.Append("			<result property=\"");
                resultMapsBuilder.Append(CommonUtil.getFirstLetterUpperString(columnName));
                resultMapsBuilder.Append("\" ");
                resultMapsBuilder.Append("column=\"");
                resultMapsBuilder.Append(columnName);
                resultMapsBuilder.Append("\" ");
                resultMapsBuilder.Append("dbType=\"");
                resultMapsBuilder.Append(this.columnInfoMap[columnName].ToString());
                resultMapsBuilder.Append("\"/>\n");
            }
            resultMapsBuilder.Append("		</resultMap>\n");
            resultMapsBuilder.Append("	</resultMaps>\n");
            return resultMapsBuilder.ToString();
        }

        private string getStatementsSection()
        {
            StringBuilder statementBuilder = new StringBuilder();
            statementBuilder.Append("	<statements>\n");
            statementBuilder.Append(this.getSelectStatements());
            if (this.tableName.ToCharArray()[0] != 'v')
            {
                statementBuilder.Append(this.getInsertStatements());
                statementBuilder.Append(this.getUpdateStatements());
                statementBuilder.Append(this.getDeleteStatements());
            }
            statementBuilder.Append("	</statements>\n");
            return statementBuilder.ToString();
        }
        
        private string getSelectStatements()
        {
            StringBuilder selectBuilder = new StringBuilder();
            // count
            selectBuilder.Append("		<select id=\"" + this.formatedTableName + ".GetCount\" resultClass=\"System.Int32\">\n");
            selectBuilder.Append("			SELECT count(*) FROM " + this.tableName + " \n");
            selectBuilder.Append("		</select>\n");
            // findall
            selectBuilder.Append("		<select id=\"" + this.formatedTableName + ".FindAll\" resultMap=\"FullResultMap\">\n");
            selectBuilder.Append("			SELECT * FROM " + this.tableName + "\n");
            selectBuilder.Append("		</select>\n");

            //findall pagination
            selectBuilder.Append("		<select id=\"" + this.formatedTableName + ".FindAllPagination\" parameterClass=\"" + this.formatedTableName + "Pagination\" resultMap=\"FullResultMap\">\n");
            selectBuilder.Append("			SELECT * FROM " + this.tableName + " limit #Limit# offset #Offset# \n");
            selectBuilder.Append("		</select>\n");

            //FindBy
            foreach (string columnName in this.columnInfoMap.Keys)
            {
                string csharpDatatype = this.dataTypeMapper.DataTypeMapper[columnInfoMap[columnName].ToString()].ToString();
                if (this.primaryKeyMap.ContainsKey(this.tableName) && columnName == this.primaryKeyMap[this.tableName].ToString())
                {
                    //<select id="RoleHasRights.Find" parameterClass="RoleHasRights" resultMap="FullResultMap" extends="RoleHasRights.FindAll">

                    selectBuilder.Append(this.getSelectFindStatementItem(columnName, CommonUtil.formateString(columnName), this.formatedTableName, this.formatedTableName));
                    //selectBuilder.Append(this.getSelectFindStatementItem(columnName, CommonUtil.formateString(columnName), this.formatedTableName + "Pagination", this.formatedTableName + "Pagination", true));
                }
                else
                {
                    //<select id="RoleHasRights.FindByRoleid" parameterClass="Int64" resultMap="FullResultMap" extends="RoleHasRights.FindAll">
                    selectBuilder.Append(this.getSelectFindByStatementItem(columnName,csharpDatatype,this.formatedTableName,this.formatedTableName));
                    selectBuilder.Append(this.getSelectFindByStatementItem(columnName, csharpDatatype, this.formatedTableName + "Pagination", this.formatedTableName + "Pagination", true));
                }

            }

            return selectBuilder.ToString();
        }

        private string getSelectFindStatementItem(string columnName, string valueName, string formatedClass, string statementId,bool isPagination = false)
        {
            StringBuilder selectStatementItemBuilder = new StringBuilder();
            selectStatementItemBuilder.Append("		<select id=\"");
            selectStatementItemBuilder.Append(statementId);
            selectStatementItemBuilder.Append(".Find\" parameterClass=\"");
            selectStatementItemBuilder.Append(formatedClass);
            selectStatementItemBuilder.Append("\" resultMap=\"FullResultMap\" extends=\"");
            selectStatementItemBuilder.Append(this.formatedTableName);
            selectStatementItemBuilder.Append(".FindAll\">\n");

            selectStatementItemBuilder.Append("			WHERE ");
            selectStatementItemBuilder.Append(this.tableName);
            selectStatementItemBuilder.Append(".");
            selectStatementItemBuilder.Append(columnName);
            selectStatementItemBuilder.Append("=#");
            selectStatementItemBuilder.Append(valueName);
            selectStatementItemBuilder.Append("#");

            if (isPagination)
            {
                selectStatementItemBuilder.Append(" limit #Limit# offset #Offset# ");
            }

            selectStatementItemBuilder.Append("\n");

            selectStatementItemBuilder.Append("		</select>\n");
            return selectStatementItemBuilder.ToString();
        }

        private string getSelectFindByStatementItem(string columnName, string csharpDatatype,
            string formatedClass,string statementId,bool isPagination=false)
        {
            StringBuilder selectStatementItemBuilder = new StringBuilder();
            selectStatementItemBuilder.Append("		<select id=\"");
            selectStatementItemBuilder.Append(statementId);
            selectStatementItemBuilder.Append(".FindBy");
            selectStatementItemBuilder.Append(CommonUtil.formateString(columnName));
            selectStatementItemBuilder.Append("\"");
            selectStatementItemBuilder.Append(" parameterClass=\"");
            if (isPagination)
            {
                selectStatementItemBuilder.Append(formatedClass);
            }
            else
            {
                selectStatementItemBuilder.Append(csharpDatatype);
            }
            selectStatementItemBuilder.Append("\" resultMap=\"FullResultMap\" extends=\"");
            selectStatementItemBuilder.Append(this.formatedTableName);
            selectStatementItemBuilder.Append(".FindAll\">\n");

            selectStatementItemBuilder.Append("			WHERE ");
            selectStatementItemBuilder.Append(this.tableName);
            selectStatementItemBuilder.Append(".");
            selectStatementItemBuilder.Append(columnName);
            if (isPagination)
            {
                selectStatementItemBuilder.Append(" = #" + CommonUtil.formateString(columnName) + "#");
                selectStatementItemBuilder.Append(" limit #Limit# offset #Offset# ");
            }
            else
            {
                selectStatementItemBuilder.Append(" = #value#");
            }
            selectStatementItemBuilder.Append("\n");

            selectStatementItemBuilder.Append("		</select>\n");
            return selectStatementItemBuilder.ToString();
        }

        private string getInsertStatements()
        {
            StringBuilder insertBuilder = new StringBuilder();
            StringBuilder valuesBuilder = new StringBuilder();
            insertBuilder.Append("		<insert id=\"" + this.formatedTableName + ".Insert\" parameterClass=\"" + this.formatedTableName + "\">\n");

            insertBuilder.Append("			INSERT INTO " + this.tableName + " (");
            foreach (string columnName in this.columnInfoMap.Keys)
            {
                insertBuilder.Append(columnName);
                insertBuilder.Append(",");
                valuesBuilder.Append("#");
                valuesBuilder.Append(CommonUtil.getFirstLetterUpperString(columnName));
                valuesBuilder.Append("#");
                valuesBuilder.Append(",");
            }
            insertBuilder.Remove(insertBuilder.Length - 1, 1);
            valuesBuilder.Remove(valuesBuilder.Length - 1, 1);

            insertBuilder.Append(") VALUES (");
            insertBuilder.Append(valuesBuilder.ToString());
            insertBuilder.Append(")\n");
            insertBuilder.Append("		</insert>\n");
            return insertBuilder.ToString();
        }

        private string getUpdateStatements()
        {
            StringBuilder updateBuilder = new StringBuilder();
            updateBuilder.Append("		<update id=\"");
            updateBuilder.Append(this.formatedTableName);
            updateBuilder.Append(".Update\" parameterClass=\"");
            updateBuilder.Append(this.formatedTableName);
            updateBuilder.Append("\">\n");

            updateBuilder.Append("			UPDATE " + this.tableName + " SET ");
            
            foreach (string columnName in this.columnInfoMap.Keys)
            {
                if (columnName == this.primaryKeyMap[this.tableName].ToString())
                    continue;
                updateBuilder.Append(columnName);
                updateBuilder.Append("=#");
                updateBuilder.Append(CommonUtil.getFirstLetterUpperString(columnName));
                updateBuilder.Append("#");
                updateBuilder.Append(",");
            }
            updateBuilder.Remove(updateBuilder.Length - 1, 1);
            updateBuilder.Append(" WHERE ");
            updateBuilder.Append(this.primaryKeyMap[this.tableName].ToString());
            updateBuilder.Append("=#");
            updateBuilder.Append(CommonUtil.getFirstLetterUpperString(this.primaryKeyMap[this.tableName].ToString()));
            updateBuilder.Append("#\n");
            updateBuilder.Append("		</update>\n");
            return updateBuilder.ToString();
        }

        private string getDeleteStatements()
        {
            StringBuilder deleteBuilder = new StringBuilder();

            foreach (string columnName in this.columnInfoMap.Keys)
            {
                string csharpDatatype = this.dataTypeMapper.DataTypeMapper[columnInfoMap[columnName].ToString()].ToString();
                if (columnName == this.primaryKeyMap[this.tableName].ToString())
                {
                    deleteBuilder.Append("		<delete id=\"");
                    deleteBuilder.Append(this.formatedTableName);
                    deleteBuilder.Append(".Delete\" parameterClass=\"");
                    deleteBuilder.Append(this.formatedTableName);
                    deleteBuilder.Append("\">\n");

                    deleteBuilder.Append("			DELETE FROM ");
                    deleteBuilder.Append(this.tableName);
                    deleteBuilder.Append(" WHERE ");
                    deleteBuilder.Append(columnName);
                    deleteBuilder.Append("=#");
                    deleteBuilder.Append(CommonUtil.formateString(columnName));
                    deleteBuilder.Append("#\n");

                    deleteBuilder.Append("		</delete>\n");
                }
                else 
                {
                    deleteBuilder.Append("		<delete id=\"");
                    deleteBuilder.Append(this.formatedTableName);
                    deleteBuilder.Append(".DeleteBy" + CommonUtil.formateString(columnName) + "\" parameterClass=\"");
                    deleteBuilder.Append(csharpDatatype);
                    deleteBuilder.Append("\">\n");

                    deleteBuilder.Append("			DELETE FROM ");
                    deleteBuilder.Append(this.tableName);
                    deleteBuilder.Append(" WHERE ");
                    deleteBuilder.Append(columnName);
                    deleteBuilder.Append("=#value# \n");

                    deleteBuilder.Append("		</delete>\n");

                }
            }

            return deleteBuilder.ToString();
        }
    }
}
