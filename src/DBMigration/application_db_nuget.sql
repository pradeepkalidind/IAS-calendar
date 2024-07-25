USE [master]
GO
 
declare @spid varchar(15), @killstring varchar(20)

select convert(varchar(15), spid) AS spid
into #procs
from master..sysdatabases sd, master..sysprocesses sp
where sd.name in ('$(ApplicationDatabaseName)') and sd.dbid = sp.dbid and spid >= 50
set rowcount 1

while (select count(*) from #procs) > 0
begin
       select @spid = spid from #procs
       select @killstring = 'kill ' + @spid
       exec (@killstring)
       print 'Killed session ' + @spid + ' on $(ApplicationDatabaseName) database'
       delete from #procs where spid = @spid
end
set rowcount 0
go

drop table #procs
go


IF  EXISTS (SELECT name FROM sys.databases WHERE name = N'$(ApplicationDatabaseName)')
DROP DATABASE [$(ApplicationDatabaseName)]


USE [master]
GO
CREATE DATABASE [$(ApplicationDatabaseName)]
GO
ALTER DATABASE [$(ApplicationDatabaseName)] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [$(ApplicationDatabaseName)] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [$(ApplicationDatabaseName)] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [$(ApplicationDatabaseName)] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [$(ApplicationDatabaseName)] SET ARITHABORT OFF 
GO
ALTER DATABASE [$(ApplicationDatabaseName)] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [$(ApplicationDatabaseName)] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [$(ApplicationDatabaseName)] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [$(ApplicationDatabaseName)] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [$(ApplicationDatabaseName)] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [$(ApplicationDatabaseName)] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [$(ApplicationDatabaseName)] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [$(ApplicationDatabaseName)] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [$(ApplicationDatabaseName)] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [$(ApplicationDatabaseName)] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [$(ApplicationDatabaseName)] SET  ENABLE_BROKER 
GO
ALTER DATABASE [$(ApplicationDatabaseName)] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [$(ApplicationDatabaseName)] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [$(ApplicationDatabaseName)] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [$(ApplicationDatabaseName)] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [$(ApplicationDatabaseName)] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [$(ApplicationDatabaseName)] SET  READ_WRITE 
GO
ALTER DATABASE [$(ApplicationDatabaseName)] SET RECOVERY FULL 
GO
ALTER DATABASE [$(ApplicationDatabaseName)] SET  MULTI_USER 
GO
ALTER DATABASE [$(ApplicationDatabaseName)] SET PAGE_VERIFY TORN_PAGE_DETECTION  
GO
ALTER DATABASE [$(ApplicationDatabaseName)] SET DB_CHAINING OFF
GO
ALTER DATABASE [$(ApplicationDatabaseName)] SET READ_COMMITTED_SNAPSHOT ON 
GO
