using System.Data.SqlClient;
using Calendar.Service.Configuration;
using PWC.IAS.Calendar.Client;

namespace Calendar.API.Test
{
    public class ApiFactBase
    {
        private readonly IasCalendarResource iasCalendarClient;
        private readonly IasCalendarResource iasCalendarImpersonateClient;
        private readonly IasCalendarSettingsResource iasCalendarSettingsClient;

        protected ApiFactBase()
        {
            var client = new TestApiApplication().CreateClient();
            iasCalendarClient = new IasCalendarResource(client, "mytaxes", "mytaxes123");
            iasCalendarSettingsClient = new IasCalendarSettingsResource(client, "mytaxes", "mytaxes123");
            iasCalendarImpersonateClient = new IasCalendarResource(client, "impersonateMytaxes",
                "impersonateMytaxes123");
            CleanDatabase();
        }

        protected IasCalendarResource GetClient(string area)
        {
            return iasCalendarClient;
        }

        protected IasCalendarResource GetImpersonateClient(string area)
        {
            return iasCalendarImpersonateClient;
        }

        protected IasCalendarSettingsResource GetSettingsClient()
        {
            return iasCalendarSettingsClient;
        }

        private static void CleanDatabase()
        {
            using (var connection = new SqlConnection(AppSettings.Config.DBConnectionString))
            {
                connection.Open();
                var transaction = connection.BeginTransaction();
                var command = connection.CreateCommand();
                command.CommandText =
                    @"use [calendar]
                exec sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';
                exec sp_MSforeachtable 'ALTER TABLE ? DISABLE TRIGGER ALL';
                exec sp_MSforeachtable 'SET QUOTED_IDENTIFIER OFF; DELETE FROM ?; SET QUOTED_IDENTIFIER ON;'
                exec sp_MSforeachtable 'ALTER TABLE ? CHECK CONSTRAINT ALL';
                exec sp_MSforeachtable 'ALTER TABLE ? ENABLE TRIGGER ALL';";
                command.Transaction = transaction;
                command.ExecuteNonQuery();
                transaction.Commit();
            }
        }
    }
}