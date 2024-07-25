param($packageRoot)
@{
    'type' = 'website'
    'defaultFeatures' = @('renew', 'restart-iis')
    'applyConfig' = {
        param($config, $packageInfo)

         & "$packageRoot\tools\apply-config.ps1" $config $packageInfo
    }
}