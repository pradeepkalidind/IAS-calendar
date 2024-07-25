using System;
using FluentMigrator;

namespace Calendar.Migration.R2
{
    [Migration(190)]
    public class _190_Add_Column_Application_Id_To_UserActivity_And_UserActivityArchived : FluentMigrator.Migration
    {
        public override void Up()
        {
            Create.Column("ApplicationId")
                  .OnTable("UserActivity")
                  .AsString()
                  .Nullable()
                  .WithDefaultValue(false);

            Create.Column("ApplicationId")
                  .OnTable("UserActivityArchived")
                  .AsString()
                  .Nullable()
                  .WithDefaultValue(false);
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}