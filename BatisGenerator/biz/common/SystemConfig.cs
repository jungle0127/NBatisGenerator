using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Configuration;

namespace BatisGenerator.biz.common
{
    public class SystemConfig : ISystemConfig
    {
        private Hashtable sysConfigMap;

        public Hashtable SysConfigMap
        {
            get { return sysConfigMap; }
        }
        public SystemConfig()
        {
            this.sysConfigMap = new Hashtable();
            this.sysConfigMap.Add("db_type", ConfigurationManager.AppSettings["db_type"].ToString());
            this.sysConfigMap.Add("schema_name", ConfigurationManager.AppSettings["schema_name"].ToString());
            this.sysConfigMap.Add("mybatis_sqlmap_file", ConfigurationManager.AppSettings["mybatis_sqlmap_file"].ToString());
            this.sysConfigMap.Add("entity_namespace", ConfigurationManager.AppSettings["entity_namespace"].ToString());
            this.sysConfigMap.Add("dao_namespace", ConfigurationManager.AppSettings["dao_namespace"].ToString());
            this.sysConfigMap.Add("mapper_namespace_root", ConfigurationManager.AppSettings["mapper_namespace_root"].ToString());
        }
        
    }
}
