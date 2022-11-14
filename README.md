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

    C:\>pl -f winword
    
        WINWORD
                  11220 process id
                     53 threads
                  1,891 handles
         19,274,203,136 virtual size
         19,521,929,216 peak virtual size
            384,090,112 working set
            384,090,112 peak working set
            158,478,336 pagefile usage
            159,408,128 peak pagefile usage
            158,478,336 private memory
              2,035,024 quota paged pool
                107,656 quota non-paged pool
                  1,562 kernel cpu time (ms)
                  3,640 user cpu time (ms)
                  5,203 total cpu time (ms)
                    214 gdi objects
                    132 user objects
      0000:00:00:09:604 runtime in dddd:hh:mm:ss:mmm
            start time: 11/14/2022 9:11:48 AM
          window title: Document1 - Word
      process, machine: 0x8664 == AMD 64, 0xaa64 == Arm 64
            executable: C:\Program Files\Microsoft Office\root\Office16\WINWORD.EXE
               command: "C:\Program Files\Microsoft Office\root\Office16\WINWORD.EXE"
