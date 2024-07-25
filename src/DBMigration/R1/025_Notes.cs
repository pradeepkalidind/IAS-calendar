using FluentMigrator;

namespace Calendar.Migration.R1
{
    [Migration(025)]
    public class Notes : FluentMigrator.Migration
    {

        public override void Up()
        {
            Execute.Sql(@"Create View ExistedNotes As Select id, userId, date, note as Content, enterFrom, timestamp From DaysCreationView Where note is not null;");
        }

        public override void Down()
        {
            Execute.Sql(@"Drop View ExistedNotes;");
        }
    }

}
