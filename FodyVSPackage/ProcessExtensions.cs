using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

public static class ProcessExtensions
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ProcessBasicInformation
    {
        public IntPtr Reserved1;
        public IntPtr PebBaseAddress;
        public IntPtr Reserved2_0;
        public IntPtr Reserved2_1;
        public IntPtr UniqueProcessId;
        public IntPtr InheritedFromUniqueProcessId;
    }

    [DllImport("ntdll.dll")]
    static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, ref ProcessBasicInformation processInformation, int processInformationLength, out int returnLength);

    public static Process GetParentProcess(this Process process)
    {
        return GetParentProcess(process.Handle);
    }

    public static Process GetParentProcess(IntPtr handle)
    {
        var pbi = new ProcessBasicInformation();
        int returnLength;
        var status = NtQueryInformationProcess(handle, 0, ref pbi, Marshal.SizeOf(pbi), out returnLength);
        if (status != 0)
        {
            throw new Win32Exception(status);
        }

        try
        {
            return Process.GetProcessById(pbi.InheritedFromUniqueProcessId.ToInt32());
        }
        catch (ArgumentException)
        {
            // not found
            return null;
        }
    }
}