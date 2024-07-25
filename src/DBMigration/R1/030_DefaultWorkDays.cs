using FluentMigrator;

namespace Calendar.Migration.R1
{
    [Migration(030)]
    public class DefaultWorkDays : FluentMigrator.Migration
    {
        public override void Up()
        {
            Create.Table("DefaultWorkDays")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("UserId").AsGuid().NotNullable()
                .WithColumn("Days").AsByte().NotNullable()
                .WithColumn("Timestamp").AsInt64().NotNullable();

            Create.Index("UserId").OnTable("DefaultWorkDays")
                .OnColumn("UserId").Ascending()
                .WithOptions().Unique();

            Create.Index("Timestamp").OnTable("DefaultWorkDays")
                .OnColumn("Timestamp").Descending();
        }

        public override void Down()
        {
            Delete.Table("DefaultWorkDays");
        }
    }
}