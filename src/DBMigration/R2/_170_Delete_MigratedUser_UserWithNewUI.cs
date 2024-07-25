using System;
using FluentMigrator;

namespace Calendar.Migration.R2
{
    [Migration(170)]
    public class _170_Delete_MigratedUser_UserWithNewUI : FluentMigrator.Migration
    {
        private const string MIGRATED_USER = "MigratedUser";
        private const string USER_WITH_NEW_UI = "UserWithNewUI";

        public override void Up()
        {
            Delete.Table(MIGRATED_USER);
            Delete.Table(USER_WITH_NEW_UI);
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}