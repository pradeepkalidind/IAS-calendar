param($config, $packageInfo)

# Function Edit-XmlFile($filePath, $config, $block) {
#     Trace-Progress "Edit-XmlFile: $filePath" {
#         [xml]$xml = Get-Content $filePath
#         & $block $xml $config
#         $xml.Save($filePath)
#     }
# }

Function Config-WebConfig($webRoot, $version, $config) {

    Update-XmlFileContent "$webRoot\web.config" {
        $_.SelectSingleNode("//appSettings/add[@key='ApplicationVersion']").value = $version
        $_.configuration.connectionStrings.add.connectionString = "Data Source=$($config.DataSource);Initial Catalog=$($config.DBName);Integrated Security=True"
    }
}

Function Config-ActiveRecordConfig($webRoot,$config) {

    Update-XmlFileContent "$webRoot\Configs\ActiveRecord.config" {        
        $_.activerecord.config.add[3].value = "Data Source=$($config.DataSource);Initial Catalog=$($config.DBName);Integrated Security=True"
    }
}

$webRoot = $packageInfo.sourcePath
$version = $packageInfo.version

Config-WebConfig $webRoot $version $config
Config-ActiveRecordConfig $webRoot $config