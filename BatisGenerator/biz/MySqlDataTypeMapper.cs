using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace BatisGenerator.biz
{
    public class MySqlDataTypeMapper
    {
        private Hashtable dataTypeMapper;
        public Hashtable DataTypeMapper 
        {
            get { return this.dataTypeMapper; }
        }
        public MySqlDataTypeMapper()
        {
            this.dataTypeMapper = new Hashtable();
            this.dataTypeMapper.Add("bigint", "Int64");
            this.dataTypeMapper.Add("int", "Int32");
            this.dataTypeMapper.Add("varchar", "String");
            this.dataTypeMapper.Add("char", "String");
            this.dataTypeMapper.Add("tinyint", "Byte");
            this.dataTypeMapper.Add("nvarchar", "String");
            this.dataTypeMapper.Add("text", "String");
            this.dataTypeMapper.Add("datetime", "DateTime");
        }
    }
}
