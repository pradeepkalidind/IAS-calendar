using System;
using FluentMigrator;
using FluentMigrator.Builders.Execute;

namespace Calendar.Migration.R1
{
    internal class ViewHelper
    {
        public static void CreateHistoryView(IExecuteExpressionRoot execute, string tableName, string additionalColumn = null)
        {
            if (!string.IsNullOrEmpty(additionalColumn)) additionalColumn = ", " + additionalColumn;
            execute.Sql(
                string.Format(
                                @"CREATE VIEW {0}HistoryView AS 
                                SELECT *
                                FROM (SELECT *, ROW_NUMBER() OVER (PARTITION BY userId, date{1}  ORDER BY timestamp DESC) AS rn FROM {0}) q
                                WHERE q.rn > 1", 
               tableName, additionalColumn));
        }

        public static void DeleteHistoryView(IExecuteExpressionRoot execute, string tableName)
        {
            execute.Sql(string.Format("DROP VIEW {0}HistoryView", tableName));
        } 

        public static void DeleteViews(IExecuteExpressionRoot execute, string tableName)
        {
            execute.Sql(string.Format("DROP VIEW {0}CreationView", tableName));
            execute.Sql(string.Format("DROP VIEW {0}DeletionView", tableName));
        } 

        public static void CreatesViews(IExecuteExpressionRoot execute, string tableName, string columns = null)
        {
            var sql = "IF OBJECT_ID ('{0}', 'V') IS NOT NULL DROP VIEW {0};";
            execute.Sql(string.Format(sql, tableName + "CreationView"));
            execute.Sql(CreateCreationView(tableName, columns));
            execute.Sql(string.Format(sql, tableName + "DeletionView"));
            execute.Sql(CreateDeletionView(tableName,""));
        }



        private static string CreateCreationView(string tableName, string columns = null)
        {
            return GetBaseSql(string.Format("{0}CreationView", tableName), tableName, columns, "q.id, q.enterFrom, q.timestamp") + "(q.timestamp > p.timestamp or p.timestamp is null)";
        }

        private static string CreateDeletionView(string tableName, string columns)
        {
            return GetBaseSql(string.Format("{0}DeletionView", tableName), tableName, columns, "p.id, p.enterFrom, p.timestamp") + "(q.timestamp < p.timestamp)";
        }

        private static string GetBaseSql(string viewName, string tableName, string columns, string timestamp)
        {
            if (!string.IsNullOrEmpty(columns)) columns = "," + columns;

            return string.Format(
                "CREATE VIEW {0} AS " +
                "SELECT q.userId, q.date{1}, {2} " +
                "FROM " +
                "(SELECT  *, ROW_NUMBER() OVER (PARTITION BY userId, date ORDER BY timestamp DESC) AS rn FROM {3}) q " +
                "left outer join " +
                "(SELECT  *, ROW_NUMBER() OVER (PARTITION BY userId, date ORDER BY timestamp DESC) AS rn FROM DeletedElements) p " +
                "on q.userId = p.userId and q.date = p.date " +
                "WHERE q.rn = 1 and (p.rn =1 or p.rn is null) and ",
                viewName, columns, timestamp, tableName);
        }
    }
}