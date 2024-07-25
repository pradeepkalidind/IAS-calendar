Function Deploy-DB {
	param ($dbRoot, $dbServer, $dbName, $dbUsers, $password, $resetDB)
	
	if($resetDB){
		Clean-DB $dbServer $dbName
	}

	$global:restoreCalendarDB = (During-RestoreDB $dbRoot $dbServer $dbName $dbUsers $password {
		Restore-CalendarDb $dbRoot $dbServer $dbName
	})
	Grant-CalendarDBAccess $dbServer $dbName $dbUsers $password

	Migrate-Db $dbRoot $dbServer $dBName
}

Function Migrate-Db{
	param ($dbRoot, $dbServer, $dbName)
	   
	iexf "$dbRoot\DBMigration\Tools\Migrate.exe -a $dbRoot\DBMigration\DBMigration.dll -db SqlServer2008 -conn ""Data Source=$dbServer;Database=$dbName;Integrated Security=True"" -profile ""Debug"""
}
   
Function During-RestoreDB ($dbRoot, $dbServer, $dbName, $dbUsers, $password, $block) {
	if(!(Test-DBExisted $dbServer $dBName)){
		& $block
		return $true
	}Else{
		Write-Debug "$dbName already exists,restore canceled."
		return $false
	}	
}

Function Restore-CalendarDb{
	param ($dbRoot, $dbServer, $dbName)

	Invoke-SqlScript -server $dbServer -file "$dbRoot\DBMigration\application_db_nuget.sql" -variables @{ ApplicationDatabaseName = $dbName	}
	iexf "$dbRoot\DBMigration\Tools\Migrate.exe -a '$dbRoot\DBMigration\DBMigration.dll' -db SqlServer2008 -conn ""Data Source=$dbServer;Database=$dbName;Integrated Security=True"" -profile ""Debug""  --task=rollback:all"
}

Function Grant-CalendarDBAccess {
	param ($dbServer, $dbName, $dbUsers, $password)

	if($dbUsers){
		$dbUsers.Split(",") | %{ Grant-DBAccess $dbServer $dbName $_ $password }
	}
}

Function Clean-DB($server,$dbNameList) {
	$dbNameList | %{ Remove-Database -server $server -database $_ }
}