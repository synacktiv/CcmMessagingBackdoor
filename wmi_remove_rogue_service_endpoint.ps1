
$namespace_persist = "root\ccm\Policy\DefaultMachine\RequestedConfig" 
$namespace_live = "root\ccm\Policy\Machine\ActualConfig"  
$className = "CCM_Service_EndpointConfiguration"

$rogue_servicename = "MP_Backdoor"

Get-WmiObject -Class $className -Namespace $namespace_persist -Filter "Name = '$rogue_servicename'" | Remove-WmiObject
Get-WmiObject -Class $className -Namespace $namespace_live -Filter "Name = '$rogue_servicename'" | Remove-WmiObject

