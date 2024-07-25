USE [$(LoggingDatabaseName)]
GO

exec sp_MSforeachtable 'DROP TABLE ?'

CREATE TABLE LogEntries
(
   Id uniqueidentifier primary key not null,
   LogOwner nvarchar(512) not null,
   LoggedAt datetime not null,
   LogLevel nvarchar(64) not null,
   Message nvarchar(max) not null
)
create nonclustered index idx_LogEntries_LoggedAt on LogEntries(LoggedAt);
create nonclustered index idx_LogEntries_LogOwner on LogEntries(LogOwner);

exec sp_MSforeachtable 'GRANT SELECT, INSERT, UPDATE, DELETE ON ? TO PUBLIC'