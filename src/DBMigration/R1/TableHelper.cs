using System;
using System.Collections.Generic;
using FluentMigrator.Builders.Create;
using FluentMigrator.Builders.Execute;

namespace Calendar.Migration.R1
{
    public static class TableHelper
    {
        public static void CreatePrimaryKey(IExecuteExpressionRoot execute, string tableName)
        {
            execute.Sql(string.Format("ALTER TABLE [{0}] ADD Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY NONCLUSTERED", tableName));
        }
        
        public static void DropIndex(IExecuteExpressionRoot execute, string tableName,string indexName)
        {
            execute.Sql(string.Format("Drop Index [{0}] On[{1}]",indexName, tableName));
        }
        
        public static void CreatePrimaryKeyAutoIncrement(IExecuteExpressionRoot execute, string tableName,string pkName)
        {
            execute.Sql(string.Format("ALTER TABLE [{0}] ADD Id BIGINT IDENTITY CONSTRAINT {1} PRIMARY KEY NONCLUSTERED", tableName, pkName));
        }

        public static void Create(ICreateExpressionRoot create, string tableName)
        {
            create.Index("UserId_Date_Timestamp").OnTable(tableName)
                .OnColumn("UserId").Ascending()
                .OnColumn("Date").Ascending()
                .OnColumn("Timestamp").Descending()
                .WithOptions().Clustered();
        }

        public static void CreateClusterIndex(ICreateExpressionRoot create, string tableName, string indexName, Dictionary<string,string> columns)
        {
            var index = create.Index(indexName).OnTable(tableName);
            foreach (var key in columns.Keys)
            {
                if("Ascending".Equals(columns[key]))
                {
                    index.OnColumn(key).Ascending();
                    continue;
                }
                index.OnColumn(key).Descending();
            }
            index.WithOptions().Clustered();
        }
    }

}
