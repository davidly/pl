# pl
Process List. Windows command line app for showing process information.

    Usage: pl [PID|APPNAME] [-c] [-f] [-k] [-m] [-p] [-s:X]
    
      Process List
      arguments: -c             Used with -f, hides command line arguments (it can be slow)
                 -f             Shows full process information
                 -k             Kills the process(es) (only when not all processes)
                 -m             Used with -f, shows modules loaded in the process
                 -p             Match [APPNAME] with the path of processes in addition to name
                 -s:[C|N|P|W]   Sort by CPU Time, Name, PID, or Working Set (default)
      examples: pl              list all processes
                pl 1542         list process with this PID
                pl -s:w msedge  list processes with this name sorted by working set
                pl -f 1392      list full information about this process
                pl -f chrome    list full information about this process
                pl -f -c chrome list full information about this process, hiding command prompt
                pl -f           list full information about all processes
                pl -k notepad   kills all instances of notepad
                pl -k 1642      kills process with PID 1392
                pl -s:n         list information about all processes sorted by name
                pl *service     finds processes whose names end in service
                pl ms*          finds processes whose names start with ms
                pl ms????       finds processes whose names start with ms and are 6 chars long
                pl -p *adobe*   finds processes whose names and/or paths contain 'adobe'
      notes:    APPNAME can have wildcards '*' and/or '?'
                APPNAME is case-insensitive

Sample results:

    C:\>pl *adobe*

        Pid        Working set  Threads  Handles  Nonpaged pool     CPU (ms)  Name
      17024          3,866,624       45      483         33,696          140  AdobeNotificationClient
      25444         12,677,120        3      225         16,896           62  Adobe Installer
      22404         13,504,512        6      258         18,160           31  AdobeUpdateService
      21872         20,901,888       42      464         33,232        4,812  AdobeIPCBroker
      20692         37,285,888        8      476         35,412        2,437  Adobe CEF Helper
      21080        130,785,280       41      981         82,544       11,703  Adobe Desktop Service
      20836        330,067,968       36      740         83,368       14,187  Adobe CEF Helper
    
      7        549,089,280      181    3,627        303,308       33,372  (TOTAL)

    C:\>pl -f "adobe installer"
    
        Adobe Installer
                  25444 process id
                      3 threads
                    225 handles
            118,194,176 virtual size
            126,058,496 peak virtual size
             12,677,120 working set
             13,217,792 peak working set
              3,403,776 pagefile usage
              4,079,616 peak pagefile usage
              3,403,776 private memory
                162,728 quota paged pool
                 16,896 quota non-paged pool
                     31 kernel cpu time (ms)
                     31 user cpu time (ms)
                     62 total cpu time (ms)
            Executable: C:\Program Files (x86)\Common Files\Adobe\Adobe Desktop Common\ElevationManager\Adobe Installer.exe
            Command:    "C:\Program Files (x86)\Common Files\Adobe\Adobe Desktop Common\ElevationManager\Adobe Installer.exe" --pipename={871D3EC1-DED0-4797-9433-7CE476AE62B2}
