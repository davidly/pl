using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.ComponentModel;
using System.Management;

class ProcessList
{
    static StringBuilder sbOut = new StringBuilder();

    static void Usage()
    {
        sbOut.AppendLine( "Usage: pl [PID|APPNAME] [-c] [-f] [-k] [-m] [-p] [-s:X]" );
        sbOut.AppendLine( "  Process List" );
        sbOut.AppendLine( "  arguments: -c             Used with -f, hides command line arguments (it can be slow)" );
        sbOut.AppendLine( "             -f             Shows full process information" );
        sbOut.AppendLine( "             -k             Kills the process(es) (only when not all processes)" );
        sbOut.AppendLine( "             -m             Used with -f, shows modules loaded in the process" );
        sbOut.AppendLine( "             -p             Match [APPNAME] with the path of processes in addition to name" );
        sbOut.AppendLine( "             -s:[C|N|P|W]   Sort by CPU Time, Name, PID, or Working Set (default)" );
        sbOut.AppendLine( "  examples: pl              list all processes" );
        sbOut.AppendLine( "            pl 1542         list process with this PID" );
        sbOut.AppendLine( "            pl -s:w msedge  list processes with this name sorted by working set" );
        sbOut.AppendLine( "            pl -f 1392      list full information about this process" );
        sbOut.AppendLine( "            pl -f chrome    list full information about this process" );
        sbOut.AppendLine( "            pl -f -c chrome list full information about this process, hiding command prompt" );
        sbOut.AppendLine( "            pl -f           list full information about all processes" );
        sbOut.AppendLine( "            pl -k notepad   kills all instances of notepad" );
        sbOut.AppendLine( "            pl -k 1642      kills process with PID 1392" );
        sbOut.AppendLine( "            pl -s:n         list information about all processes sorted by name" );
        sbOut.AppendLine( "            pl *service     finds processes whose names end in service" );
        sbOut.AppendLine( "            pl ms*          finds processes whose names start with ms" );
        sbOut.AppendLine( "            pl ms????       finds processes whose names start with ms and are 6 chars long" );
        sbOut.AppendLine( "            pl -p *adobe*   finds processes whose names and/or paths contain 'adobe'" );
        sbOut.AppendLine( "  notes:    APPNAME can have wildcards '*' and/or '?'" );
        sbOut.AppendLine( "            APPNAME is case-insensitive" );

        Console.Write( sbOut.ToString() );

        Environment.Exit(1);
    }

    static bool g_FullInfo = false;
    static bool g_ModuleInfo = false;
    static bool g_HeaderShown = false;
    static bool g_CommandLineInfo = true;

    static long g_WorkingSetTotal = 0;
    static long g_ThreadsTotal = 0;
    static long g_HandlesTotal = 0;
    static long g_NonPagedPoolTotal = 0;
    static long g_CPUTimeTotal = 0;

    static string GetCommandLine( Process process )
    {
        try
        {
            string wmiQuery = string.Format( "select CommandLine from Win32_Process where ProcessId='{0}'", process.Id );
            ManagementObjectSearcher searcher = new ManagementObjectSearcher( wmiQuery );
            ManagementObjectCollection retObjectCollection = searcher.Get();

            if ( 1 == retObjectCollection.Count )
            {
                foreach ( ManagementObject retObject in retObjectCollection )
                    return retObject["CommandLine"].ToString();
            }
        }
        catch ( Exception e )
        {
            // Intentionally empty for when security exceptions occur
        }

        return "";   
    } //GetCommandLine

    static long GetTotalMilliseconds( Process proc )
    {
        try
        {
            return proc.TotalProcessorTime.Ticks / 10000;
        }
        catch ( Exception e )
        {
            // probably access-denied
        }

        return 0;
    } //GetTotalMilliseconds

    static long GetKernelMilliseconds( Process proc )
    {
        try
        {
            return proc.PrivilegedProcessorTime.Ticks / 10000;
        }
        catch ( Exception e )
        {
            // probably access-denied
        }

        return 0;
    } //GetKernelMilliseconds

    static long GetUserMilliseconds( Process proc )
    {
        try
        {
            return proc.UserProcessorTime.Ticks / 10000;
        }
        catch ( Exception e )
        {
            // probably access-denied
        }

        return 0;
    } //GetUserMilliseconds

    static void PrintProcessInfo( Process proc )
    {
        if ( g_FullInfo )
        {
            sbOut.AppendLine( proc.ProcessName );

            sbOut.AppendFormat( "{0,19:D} process id\n", proc.Id );
            sbOut.AppendFormat( "{0,19:N0} threads\n", proc.Threads.Count );
            sbOut.AppendFormat( "{0,19:N0} handles\n", proc.HandleCount );
            sbOut.AppendFormat( "{0,19:N0} virtual size\n", proc.VirtualMemorySize64 );
            sbOut.AppendFormat( "{0,19:N0} peak virtual size\n", proc.PeakVirtualMemorySize64 );
            sbOut.AppendFormat( "{0,19:N0} working set\n", proc.WorkingSet64 );
            sbOut.AppendFormat( "{0,19:N0} peak working set\n", proc.PeakWorkingSet64 );
            sbOut.AppendFormat( "{0,19:N0} pagefile usage\n", proc.PagedMemorySize64 );
            sbOut.AppendFormat( "{0,19:N0} peak pagefile usage\n", proc.PeakPagedMemorySize64 );
            sbOut.AppendFormat( "{0,19:N0} private memory\n", proc.PrivateMemorySize64 );
            sbOut.AppendFormat( "{0,19:N0} quota paged pool\n", proc.PagedSystemMemorySize64 );
            sbOut.AppendFormat( "{0,19:N0} quota non-paged pool\n", proc.NonpagedSystemMemorySize64 );
            sbOut.AppendFormat( "{0,19:N0} kernel cpu time (ms)\n", GetKernelMilliseconds( proc ) );
            sbOut.AppendFormat( "{0,19:N0} user cpu time (ms)\n", GetUserMilliseconds( proc ) );
            sbOut.AppendFormat( "{0,19:N0} total cpu time (ms)\n", GetTotalMilliseconds( proc ) );

            TimeSpan runtime = DateTime.Now - proc.StartTime;
            sbOut.AppendFormat( " {0,4:D4}:{1,2:D2}:{2,2:D2}:{3,2:D2}:{4,4:D4} runtime in dddd:hh:mm:ss:mmmm\n",
                                runtime.Days, runtime.Hours, runtime.Minutes, runtime.Seconds, runtime.Milliseconds );

            try
            {
                ProcessModule pm = proc.MainModule;
                sbOut.AppendLine( "        Executable: " + pm.FileName );

                if ( g_CommandLineInfo )
                    sbOut.AppendLine( "           Command: " + GetCommandLine( proc ) );

                if ( g_ModuleInfo )
                {
                    ProcessModuleCollection modules = proc.Modules;

                    foreach ( ProcessModule m in modules )
                    {
                        if ( m.FileName != pm.FileName )
                            sbOut.AppendFormat( "          {0,13:N0} bytes -- {1}\n", m.ModuleMemorySize, m.FileName );
                    }
                }
            }
            catch ( Exception e )
            {
                // probably access-denied
            }
        }
        else
        {
            if ( ! g_HeaderShown )
            {
                sbOut.AppendLine( "    Pid        Working set  Threads  Handles  Nonpaged pool     CPU (ms)  Name" );
                g_HeaderShown = true;
            }

            sbOut.AppendFormat( "{0,7:D} ", proc.Id );
            sbOut.AppendFormat( "{0,18:N0} ", proc.WorkingSet64 );
            sbOut.AppendFormat( "{0,8:N0} ", proc.Threads.Count );
            sbOut.AppendFormat( "{0,8:N0} ", proc.HandleCount );
            sbOut.AppendFormat( "{0,14:N0} ", proc.NonpagedSystemMemorySize64 );
            sbOut.AppendFormat( "{0,12:N0} ", GetTotalMilliseconds( proc ) );
            sbOut.AppendFormat( " " + proc.ProcessName );

            g_WorkingSetTotal += proc.WorkingSet64;
            g_ThreadsTotal += proc.Threads.Count;
            g_HandlesTotal += proc.HandleCount;
            g_NonPagedPoolTotal += proc.NonpagedSystemMemorySize64;
            g_CPUTimeTotal += GetTotalMilliseconds( proc );
        }

        sbOut.AppendLine();
    } //PrintProcessInfo

    public class ProcComparerWorkingSet : IComparer
    {
        public int Compare( Object x, Object y )
        {
            long wsX = ( (Process) x ).WorkingSet64;
            long wsY = ( (Process) y ).WorkingSet64;

            return ( wsX > wsY ) ? 1 : ( wsX < wsY ) ? -1 : 0;
        }
    } //ProcComparerWorkingSet

    public class ProcComparerCPUTime : IComparer
    {
        public int Compare( Object x, Object y )
        {
            long wsX = GetTotalMilliseconds( (Process) x );
            long wsY = GetTotalMilliseconds( (Process) y );

            return ( wsX > wsY ) ? 1 : ( wsX < wsY ) ? -1 : 0;
        }
    } //ProcComparerCPUTime

    public class ProcComparerPID : IComparer
    {
        public int Compare( Object x, Object y )
        {
            return ( (Process) x ).Id - ( (Process) y ).Id;
        }
    } //ProcComparerPID

    public class ProcComparerName : IComparer
    {
        public int Compare( Object x, Object y )
        {
            return String.Compare( ( (Process) x ).ProcessName, ( (Process) y ).ProcessName );
        }
    } //ProcComparerName

    static public Regex WildcardsToRegex( string pattern )
    {
        string regexPattern = "^";
    
        foreach ( char c in pattern )
        {
            if ( '*' == c )
                regexPattern += ".*";
            else if ( '?' == c )
                regexPattern += ".";
            else
                regexPattern += "[" + c + "]";
        }

        return new Regex( regexPattern + "$", RegexOptions.IgnoreCase );
    } //WildcardstoRegex

    static public bool MatchesRegex( Regex regex, string input )
    {
        return regex.IsMatch( input );
    } //MatchesRegex

    static public bool PathMatchesRegex( Regex regex, Process proc )
    {
        try
        {
            ProcessModule pm = proc.MainModule;
            return MatchesRegex( regex, pm.FileName );
        }
        catch ( Exception e )
        {
            // probably access-denied
        }

        return false;
    } //PathMatchesRegex

    static public bool HasWildcards( string s )
    {
        if ( null == s )
            return false;

        foreach ( char c in s )
            if ( '?' == c || '*' == c )
                return true;

        return false;
    } //HasWildcards

    static void Main( string[] args )
    {
        int PID = 0;
        string ProcessName = null;
        IComparer comp = null;
        bool KillProcess = false;
        bool matchPath = false;

        for ( int i = 0; i < args.Length; i++ )
        {
            if ( '-' == args[i][0] || '/' == args[i][0] )
            {
                string argUpper = args[i].ToUpper();
                string arg = args[i];
                char c = argUpper[1];

                if ( 'C' == c )
                {
                    // getting command line info slows the app 10x, so allow it to be turned off

                    g_CommandLineInfo = false;
                }
                else if ( 'F' == c )
                {
                    if ( arg.Length > 2 )
                        Usage();

                    g_FullInfo = true;
                }
                else if ( 'K' == c )
                {
                    KillProcess = true;
                }
                else if ( 'M' == c )
                {
                    g_ModuleInfo = true;
                }
                else if ( 'P' == c )
                {
                    matchPath = true;
                }
                else if ( 'S' == c )
                {
                    if ( arg.Length != 4 || arg[2] != ':' )
                        Usage();

                    char s = argUpper[ 3 ];

                    if ( 'W' == s )
                        comp = new ProcComparerWorkingSet();
                    else if ( 'P' == s )
                        comp = new ProcComparerPID();
                    else if ( 'N' == s )
                        comp = new ProcComparerName();
                    else if ( 'C' == s )
                        comp = new ProcComparerCPUTime();
                    else
                        Usage();
                }
                else
                {
                    Usage();
                }
            }
            else
            {
                if ( PID != 0 || ProcessName != null )
                    Usage();

                char c = args[i][0];

                if ( c >= '0' && c <= '9' )
                    PID = Convert.ToInt32( args[i] );
                else
                {
                    ProcessName = args[i];

                    // The .net api for processes doesn't want ".exe" on the end of the process name

                    int iEXE = ProcessName.LastIndexOf( ".exe", StringComparison.OrdinalIgnoreCase );

                    if ( -1 != iEXE )
                        ProcessName = ProcessName.Remove( iEXE, 4 );
                }
            }
        }

        // don't kill all processes; only kill well-identified processes

        if ( ( 0 == PID ) && ( null == ProcessName ) && ( KillProcess ) )
            Usage();

        try
        {
            if ( PID != 0 )
            {
                Process proc = Process.GetProcessById( PID );

                PrintProcessInfo( proc );

                if ( KillProcess )
                    proc.Kill();
            }
            else
            {
                Process[] procs = null;
                bool wildCards = HasWildcards( ProcessName );

                if ( null != ProcessName && !wildCards  )
                    procs = Process.GetProcessesByName( ProcessName );
                else
                    procs = Process.GetProcesses();

                if ( wildCards )
                {
                    Regex regex = WildcardsToRegex( ProcessName );
                    Process [] fullList = procs;
                    procs = new Process[ 0 ];
                    int matchCount = 0;

                    foreach ( Process proc in fullList )
                    {
                        if ( ( MatchesRegex( regex, proc.ProcessName ) ) ||
                             ( matchPath && PathMatchesRegex( regex, proc ) ) )
                        {
                            Array.Resize( ref procs, matchCount + 1 );
                            procs[ matchCount++ ] = proc;
                        }
                    }
                }

                if ( null == comp )
                    comp = new ProcComparerWorkingSet();

                Array.Sort( procs, comp );

                foreach ( Process proc in procs )
                {
                    PrintProcessInfo( proc );

                    if ( KillProcess )
                        proc.Kill();
                }

                if ( ! g_FullInfo && g_HeaderShown )
                {
                    sbOut.AppendLine();
                    sbOut.AppendFormat( "{0,7:D} ", procs.Length );
                    sbOut.AppendFormat( "{0,18:N0} ", g_WorkingSetTotal );
                    sbOut.AppendFormat( "{0,8:N0} ", g_ThreadsTotal );
                    sbOut.AppendFormat( "{0,8:N0} ", g_HandlesTotal );
                    sbOut.AppendFormat( "{0,14:N0} ", g_NonPagedPoolTotal );
                    sbOut.AppendFormat( "{0,12:N0} ", g_CPUTimeTotal );
                    sbOut.AppendFormat( " (TOTAL)" );
                    sbOut.AppendLine();
                }
            }

            Console.Write( sbOut.ToString() );
        }
        catch (Exception e)
        {
            Console.WriteLine( "pl caught an exception {0}", e.ToString() );
            Usage();
        }
    } //Main
} //ProcessList

