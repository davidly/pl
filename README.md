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
                29564 process id
                  153 threads
                2,439 handles
    2,209,569,435,648 virtual size
    2,209,839,640,576 peak virtual size
          222,019,584 working set
          227,164,160 peak working set
          135,696,384 pagefile usage
          142,004,224 peak pagefile usage
          135,696,384 private memory
            2,459,888 quota paged pool
              108,832 quota non-paged pool
                  265 kernel cpu time (ms)
                  203 user cpu time (ms)
                  468 total cpu time (ms)
               54,328 elapsed time (ms)
    0000:00:00:54:328 elapsed time in dddd:hh:mm:ss:mmm
          start time: 4/17/2023 11:22:08 AM
                  197 gdi objects
                  240 user objects
    process, machine: 0x8664 == AMD 64, 0x8664 == AMD 64
        window title: Document1 - Word
          executable: C:\Program Files\Microsoft Office\root\Office16\WINWORD.EXE
             command: "C:\Program Files\Microsoft Office\root\Office16\WINWORD.EXE"
