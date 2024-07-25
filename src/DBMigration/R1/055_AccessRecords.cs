using FluentMigrator;

namespace Calendar.Migration.R1
{
    [Migration(055)]
    public class AccessRecords : FluentMigrator.Migration
    {
        private const string TABLE_NAME = "AccessRecords";

        public override void Up()
        {
            Create.Table(TABLE_NAME)
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("UserId").AsGuid().NotNullable()
                .WithColumn("AccessTime").AsDateTime().NotNullable();
        }

        public override void Down()
        {
            Delete.Table(TABLE_NAME);
        }
    }
}