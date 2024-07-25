Function Edit-XmlFile($filePath, $config, $block) {
	Trace-Progress "Edit-XmlFile: $filePath" {
		[xml]$xml = Get-Content $filePath
		& $block $xml $config
		$xml.Save($filePath)
	}
}

Function Config-WebConfig($siteRoot,$config) {
	Edit-XmlFile "$siteRoot\web.config" $config {
		$xml.configuration.connectionStrings.add[0].connectionString = "Data Source=$($config.DataSource);Initial Catalog=$($config.DBName);Integrated Security=True"
	}
}

Function Config-ActiveRecordConfig($siteRoot,$config) {
	Edit-XmlFile "$siteRoot\Configs\ActiveRecord.config" $config {
		$xml.activerecord.config.add[3].value = "Data Source=$($config.DataSource);Initial Catalog=$($config.DBName);Integrated Security=True"
	}
}
