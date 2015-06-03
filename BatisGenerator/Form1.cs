using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BatisGenerator.dal;
using BatisGenerator.biz;
using System.IO;
using System.Collections;
using BatisGenerator.biz.USP;

namespace BatisGenerator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            StringBuilder sqlMapCfgBuilder = new StringBuilder();
            MySqlConnector conn = new MySqlConnector();
            DataTable tables = conn.getTable("select table_name from information_schema.tables where table_schema='transinfo'");
            //DataTable columnInfo = conn.getTable("select column_name,is_nullable,data_type,character_maximum_length from information_schema.columns where table_schema='transinfo' and table_name='city'");
            //this.dataGridView1.DataSource = columnInfo;
            //DataTable usps = conn.getTable("select name,param_list,returns from mysql.proc where db='transinfo' and type='procedure'");

            foreach (DataRow dr in tables.Rows)
            {
                string tableName = dr[0].ToString();
                EntityGenerator entityGenerator = new EntityGenerator("transinfo",tableName);
                MapperGenerator mapperGenerator = new MapperGenerator("transinfo", tableName);
                DaoInterfaceGenerator daoInterfaceGenerator = new DaoInterfaceGenerator("transinfo", tableName);
                DaoGenerator daoGenerator = new DaoGenerator("transinfo", tableName);
                tableName = tableName.Replace("_", "");
                string entity = entityGenerator.getEnity();
                string paginationEntity = entityGenerator.getPaginationEntity();
                this.writeFile(entity, "Entity\\" + this.getFirstUpper(tableName) + ".cs");
                this.writeFile(paginationEntity, "Entity\\Pagination\\" + this.getFirstUpper(tableName) + "Pagination.cs");
                string mapperContent = mapperGenerator.getMapper();
                this.writeFile(mapperContent, "Mapper\\"+this.getFirstUpper(tableName) + ".mapper.xml");
                sqlMapCfgBuilder.Append("    <sqlMap resource = \"App_Code/DAL/Mapper/" + this.getFirstUpper(tableName) + ".mapper.xml\"/>\n");
                this.writeFile(daoInterfaceGenerator.getDaoInterface(), "IDao\\I" + this.getFirstUpper(tableName) + "Dao.cs");
                this.writeFile(daoGenerator.getDao(), "Dao\\" + this.getFirstUpper(tableName) + "Dao.cs");
            }

            Hashtable procInfoMap = conn.procedureParamlistMap("transinfo");
            foreach (DictionaryEntry de in procInfoMap)
            {
                USPMapperGenerator uspMapperGenerator = new USPMapperGenerator(de.Key.ToString(), de.Value as IList<ProcedureParameterInfo>);
                this.writeFile(uspMapperGenerator.getMapper(), "Mapper\\USP\\" + this.getFirstUpper(de.Key.ToString()) + ".mapper.xml");
                sqlMapCfgBuilder.Append("    <sqlMap resource = \"App_Code/DAL/Mapper/usp/" + this.getFirstUpper(de.Key.ToString()) + ".mapper.xml\"/>\n");

                USPEntityGenerator uspEntityGenerator = new USPEntityGenerator(de.Key.ToString(), de.Value as IList<ProcedureParameterInfo>);
                this.writeFile(uspEntityGenerator.getEntity(), "Entity\\USP\\" + this.getFirstUpper(de.Key.ToString()) + ".cs");

                USPDaoInterfaceGenerator uspDaoInterfaceGenerator = new USPDaoInterfaceGenerator(de.Key.ToString());
                this.writeFile(uspDaoInterfaceGenerator.getDaoInterface(), "IDao\\USP\\I" + this.getFirstUpper(de.Key.ToString()) + "Dao.cs");

                USPDaoGenerator uspDaoGenerator = new USPDaoGenerator(de.Key.ToString());
                this.writeFile(uspDaoGenerator.getUSPDao(), "Dao\\USP\\" + this.getFirstUpper(de.Key.ToString()) + "Dao.cs");
            }
            this.writeFile(sqlMapCfgBuilder.ToString(),"sqlmapcfg.txt");
        }
        private void writeFile(string content, string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            sw.Write(content);
            sw.Close();
            fs.Close();
        }

        private String getFirstUpper(string str)
        {
            char[] charArray = str.ToCharArray();
            charArray[0] = charArray[0].ToString().ToUpper().ToCharArray()[0];
            return new String(charArray);
        }
    }
}
