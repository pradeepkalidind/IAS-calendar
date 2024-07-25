/*
This is the script to create scheduled job for archiving historical feeds from FeedEntries table to FeedArchivedEntries table.

This script needs to be executed with sysadmin permission.

The current frequency is set to be daily at 12:00am. The whole move has been broken down into 1000 batches to bound transaction space usage.
*/
USE [msdb]
GO

if EXISTS (SELECT job_id FROM msdb.dbo.sysjobs where name='PwC IAS Calendar Feed Archiving')
begin
	print 'job already existed'
	GOTO Quit
end

print 'begin creating job ...'

DECLARE @jobId BINARY(16)
EXEC  msdb.dbo.sp_add_job @job_name=N'PwC IAS Calendar Feed Archiving', 
		@enabled=1, 
		@notify_level_eventlog=0, 
		@notify_level_email=2, 
		@notify_level_netsend=2, 
		@notify_level_page=2, 
		@delete_level=0, 
		@category_name=N'Database Maintenance', 
		@owner_login_name=N'calendar', @job_id = @jobId OUTPUT
select @jobId

EXEC msdb.dbo.sp_add_jobserver @job_name=N'PwC IAS Calendar Feed Archiving'/*,@server_name = N'(local)'*/


EXEC msdb.dbo.sp_add_jobstep @job_name=N'PwC IAS Calendar Feed Archiving', @step_name=N'Move Historical Feeds', 
		@step_id=1, 
		@cmdexec_success_code=0, 
		@on_success_action=1, 
		@on_fail_action=2, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'declare @days bigint
set @days =convert(bigint,datediff(d,Cast(''0001-01-01'' as date),getdate())) - 60;
declare @dateTime2MonthAgoInTicks bigint
set @dateTime2MonthAgoInTicks = @days * 24 * 60 * 60 * power(10, 7);

while (select count(id) from feedentries where timestamp < @dateTime2MonthAgoInTicks) > 0
begin
begin tran
	insert into FeedArchivedEntries select top 1000 * from feedentries where timestamp < @dateTime2MonthAgoInTicks order by timestamp
	delete from feedentries where  id in (select top 1000 id from feedentries where timestamp < @dateTime2MonthAgoInTicks order by timestamp)
	waitfor DELAY ''00:00:01''
commit
end

while (select count(id) from UserActivity where timestamp < @dateTime2MonthAgoInTicks) > 0
begin
begin tran
	insert into UserActivityArchived select top 1000 * from UserActivity where timestamp < @dateTime2MonthAgoInTicks order by timestamp
	delete from UserActivity where  id in (select top 1000 id from UserActivity where timestamp < @dateTime2MonthAgoInTicks order by timestamp)
	waitfor DELAY ''00:00:01''
commit
end 
', 
		@database_name=N'calendar', 
		@database_user_name=N'calendar', 
		@flags=0

EXEC msdb.dbo.sp_update_job @job_name=N'PwC IAS Calendar Feed Archiving', 
		@enabled=1, 
		@start_step_id=1, 
		@notify_level_eventlog=0, 
		@notify_level_email=2, 
		@notify_level_netsend=2, 
		@notify_level_page=2, 
		@delete_level=0, 
		@description=N'', 
		@category_name=N'Database Maintenance', 
		@owner_login_name=N'calendar', 
		@notify_email_operator_name=N'', 
		@notify_netsend_operator_name=N'', 
		@notify_page_operator_name=N''

DECLARE @schedule_id int
EXEC msdb.dbo.sp_add_jobschedule @job_name=N'PwC IAS Calendar Feed Archiving', @name=N'Daily Historical Feed Archiving', 
		@enabled=1, 
		@freq_type=4, 
		@freq_interval=1, 
		@freq_subday_type=1, 
		@freq_subday_interval=0, 
		@freq_relative_interval=0, 
		@freq_recurrence_factor=1, 
		@active_start_date=20120905, 
		@active_end_date=99991231, 
		@active_start_time=0,
		@active_end_time=235959, @schedule_id = @schedule_id OUTPUT
select @schedule_id

Quit:
	print 'goodbye!'
