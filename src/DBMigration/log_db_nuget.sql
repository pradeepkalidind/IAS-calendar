USE [master]
GO
 
declare @spid varchar(15), @killstring varchar(20)

select convert(varchar(15), spid) AS spid
into #procs
from master..sysdatabases sd, master..sysprocesses sp
where sd.name in ('$(LoggingDatabaseName)') and sd.dbid = sp.dbid and spid >= 50
set rowcount 1

while (select count(*) from #procs) > 0
begin
       select @spid = spid from #procs
       select @killstring = 'kill ' + @spid
       exec (@killstring)
       print 'Killed session ' + @spid + ' on $(LoggingDatabaseName) database'
       delete from #procs where spid = @spid
end
set rowcount 0
go

drop table #procs
go


IF  EXISTS (SELECT name FROM sys.databases WHERE name = N'$(LoggingDatabaseName)')
DROP DATABASE [$(LoggingDatabaseName)]


USE [master]
GO
CREATE DATABASE [$(LoggingDatabaseName)]
GO

ALTER DATABASE [$(LoggingDatabaseName)] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [$(LoggingDatabaseName)] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [$(LoggingDatabaseName)] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [$(LoggingDatabaseName)] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [$(LoggingDatabaseName)] SET ARITHABORT OFF 
GO

ALTER DATABASE [$(LoggingDatabaseName)] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [$(LoggingDatabaseName)] SET AUTO_CREATE_STATISTICS ON 
GO

ALTER DATABASE [$(LoggingDatabaseName)] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [$(LoggingDatabaseName)] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [$(LoggingDatabaseName)] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [$(LoggingDatabaseName)] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [$(LoggingDatabaseName)] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [$(LoggingDatabaseName)] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [$(LoggingDatabaseName)] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [$(LoggingDatabaseName)] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [$(LoggingDatabaseName)] SET  DISABLE_BROKER 
GO

ALTER DATABASE [$(LoggingDatabaseName)] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [$(LoggingDatabaseName)] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [$(LoggingDatabaseName)] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [$(LoggingDatabaseName)] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [$(LoggingDatabaseName)] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [$(LoggingDatabaseName)] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [$(LoggingDatabaseName)] SET  READ_WRITE 
GO

ALTER DATABASE [$(LoggingDatabaseName)] SET RECOVERY FULL 
GO

ALTER DATABASE [$(LoggingDatabaseName)] SET  MULTI_USER 
GO

ALTER DATABASE [$(LoggingDatabaseName)] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [$(LoggingDatabaseName)] SET DB_CHAINING OFF 
GO