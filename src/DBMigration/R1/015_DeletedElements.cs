using FluentMigrator;

namespace Calendar.Migration.R1
{
    [Migration(015)]
    public class DeletedElements : FluentMigrator.Migration
    {
        private string tableName;

        public override void Up()
        {
            tableName = "DeletedElements";

            Create.Table(tableName)
                .WithColumn("UserId").AsGuid().NotNullable()
                .WithColumn("Date").AsDate().NotNullable()
                .WithColumn("EnterFrom").AsString().Nullable()
                .WithColumn("Timestamp").AsInt64().NotNullable();

            TableHelper.CreatePrimaryKey(Execute, tableName);

            TableHelper.Create(Create, tableName);         
        }

        public override void Down()
        {
            Delete.Table(tableName);
        }
    }
}