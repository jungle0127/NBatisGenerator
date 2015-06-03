using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Data;
using System.Collections;
using BatisGenerator.biz.USP;

namespace BatisGenerator.dal
{
    public class MySqlConnector
    {
        private string connectionString;
        private MySqlConnection mySqlConn;
        /// <summary>
        ///  Init SQL connection.
        /// </summary>
        public MySqlConnector()
        {
            this.connectionString = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;
            this.initConnection();
        }
        /// <summary>
        /// Init and open connection.
        /// </summary>
        private void initConnection()
        {
            this.mySqlConn = new MySqlConnection(this.connectionString);
            this.mySqlConn.Open();
        }
        /// <summary>
        /// close SQL connection
        /// </summary>
        public void closeConnection()
        {
            this.mySqlConn.Close();
            this.mySqlConn.Dispose();
        }
        /// <summary>
        /// Get table information according to SQL clause.
        /// </summary>
        /// <param name="sqlStr">SQL clause</param>
        /// <returns>Datatable instance</returns>
        public DataTable getTable(string sqlStr)
        {
            MySqlCommand cmd = new MySqlCommand(sqlStr, this.mySqlConn);
            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            dataAdapter.Fill(dt);
            return dt;
        }
        /// <summary>
        /// Get table name list according to database names.
        /// </summary>
        /// <param name="schemaName">database name</param>
        /// <returns></returns>
        public IList<String> getTableNames(string schemaName)
        {
            DataTable dt = this.getTable("select table_name from information_schema.tables where table_schema='" + schemaName + "'");
            IList<String> tableNameList = new List<String>();
            foreach (DataRow dr in dt.Rows)
            {
                tableNameList.Add(dr[0].ToString());
            }
            return tableNameList;
        }
        /// <summary>
        /// Get column information according to database name and table name.
        /// </summary>
        /// <param name="schemaName">Database name</param>
        /// <param name="tableName">table name.</param>
        /// <returns>data table instance.</returns>
        public DataTable getColumnsInfo(string schemaName, string tableName)
        {
            return this.getTable("select column_name,is_nullable,data_type,character_maximum_length from information_schema.columns where table_schema='" + schemaName + "' and table_name='" + tableName + "'");
        }
        /// <summary>
        /// K: column name
        /// V: data type
        /// </summary>
        /// <param name="schemaName">data base name</param>
        /// <param name="tableName">data table name</param>
        /// <returns>column data type Map</returns>
        public Hashtable getColumnsPairInfo(string schemaName, string tableName)
        {
            Hashtable columnPaireTable = new Hashtable();
            DataTable columnInfoTable = this.getColumnsInfo(schemaName, tableName);
            foreach (DataRow dr in columnInfoTable.Rows)
            {
                columnPaireTable.Add(dr[0].ToString(), dr[2].ToString());
            }
            return columnPaireTable;
        }
        /// <summary>
        /// Get the table name and primary key Map
        /// </summary>
        /// <param name="schemaName">data base name.</param>
        /// <returns>table name and primary key Map.</returns>
        public Hashtable getPrimaryKeyPairInfo(string schemaName)
        {
            Hashtable primaryKeyPairMap = new Hashtable();
            DataTable primaryKeyTable = this.getTable("select table_name,column_name from information_schema.columns where table_schema='" + schemaName + "' and column_key='PRI'");
            foreach (DataRow dr in primaryKeyTable.Rows)
            {
                primaryKeyPairMap.Add(dr[0].ToString(), dr[1].ToString());
            }
            return primaryKeyPairMap;
        }
        /// <summary>
        /// Get table type information. Table type, table or view.
        /// </summary>
        /// <param name="schemaName">data base name.</param>
        /// <returns>Table type Map.</returns>
        public Hashtable getTableTypeMap(string schemaName)
        {
            Hashtable tableTypeMap = new Hashtable();

            DataTable tableTypeTable = this.getTable(" select table_name,table_type from information_schema.tables where table_schema = '" + schemaName + "';");
            foreach (DataRow dr in tableTypeTable.Rows)
            {
                tableTypeMap.Add(dr[0].ToString(), dr[1].ToString());
            }

            return tableTypeMap;
        }
        /// <summary>
        /// Get procedure parameter list according to database name.
        /// </summary>
        /// <param name="schemaName">database name.</param>
        /// <returns>procedure parameter Map.</returns>
        public Hashtable procedureParamlistMap(string schemaName)
        {
            Hashtable paramlistMap = new Hashtable();
            DataTable paramTable = this.getTable("SELECT name,param_list FROM mysql.proc WHERE db='" + schemaName + "'");
            foreach (DataRow dr in paramTable.Rows)
            {
                string paramString = Encoding.Default.GetString((Byte[]) dr["param_list"]);
                string[] paramList = paramString.Split(',');
                List<ProcedureParameterInfo> ppiList = new List<ProcedureParameterInfo>();
                foreach (string parameter in paramList)
                {
                    string[] paramInfo = parameter.Split(' ');
                    ProcedureParameterInfo ppi = new ProcedureParameterInfo();
                    switch (paramInfo[0])
                    {
                        case "IN":
                            ppi.ParamType = ProcedureParameterType.IN;
                            break;
                        case "OUT":
                            ppi.ParamType = ProcedureParameterType.OUT;
                            break;
                        default:
                            ppi.ParamType = ProcedureParameterType.INOUT;
                            break;
                    }
                    ppi.PrameterName = paramInfo[1];
                    ppi.ParameterDataType = paramInfo[2];
                    ppiList.Add(ppi);
                }
                paramlistMap.Add(dr["name"].ToString(), ppiList);
            }
            return paramlistMap;
        }
    }
}
