using FluentMigrator;

namespace Calendar.Migration.R1
{
    [Migration(060)]
    public class AccessInfos : FluentMigrator.Migration
    {
        private const string TABLE_NAME = "AccessInfos";

        public override void Up()
        {
            Create.Table(TABLE_NAME)
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("AccessRecordId").AsGuid().NotNullable()
                .WithColumn("InfoKey").AsString().NotNullable()
                .WithColumn("InfoValue").AsString().Nullable();

            Create.ForeignKey("FK_AccessInfos_AccessRecords").FromTable(TABLE_NAME).ForeignColumn("AccessRecordId")
                .ToTable("AccessRecords").PrimaryColumn("Id");
        }

        public override void Down()
        {
            Delete.Table(TABLE_NAME);
        }
    }
}