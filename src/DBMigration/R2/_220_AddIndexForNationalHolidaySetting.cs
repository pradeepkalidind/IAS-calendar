using FluentMigrator;

namespace Calendar.Migration.R2
{
    [Migration(220)]
    public class _220_AddIndexForNationalHolidaySetting:FluentMigrator.Migration
    {
        public override void Up()
        {
            Execute.Sql(@"
if exists (select name from sys.indexes
			where name = N'IX_UserId_Country')
begin
	DROP INDEX [IX_UserId_Country] on [NationalHolidaySetting]
end
CREATE NONCLUSTERED INDEX [IX_UserId_Country] ON [dbo].[NationalHolidaySetting]
(
	[UserId] ASC
)
INCLUDE ([Country])
GO");
        }

        public override void Down()
        {
            Execute.Sql(@"DROP INDEX [IX_UserId_Country] on [NationalHolidaySetting]");
        }
    }
}
