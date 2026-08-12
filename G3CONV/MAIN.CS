using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace G3CONV{
    class MAIN{
        public static void P_EXIT(int STATE){
            Console.WriteLine("");
            if (STATE == 0) Console.WriteLine("PROGRAM PROCESSING HAS FINISHED SUCCESSFULLY!");
            else Console.WriteLine("PROGRAM PROCESSING HAS FINISHED UNSUCCESSFULLY!");
            Console.WriteLine("");
            Console.WriteLine("PRESS ANY KEY TO CONTINUE ...");
            Console.ReadKey();
            Environment.Exit(0);
        }
        static void Main(string[] args){

            int P = 0;

            string FPATH = Assembly.GetExecutingAssembly().Location.ToString().DFULLPTH();

            Console.WriteLine("MULTIMEDIA CONVERTOR FOR 3G PHONES V1.00");
            Console.WriteLine("");
            Console.WriteLine("> DO YOU WANT TO RESTORE PROPER FILE NAMES FROM A PATH FILE? <Y/N>");
            string REPLY = Console.ReadLine();
            if (REPLY.ToUpper() == "Y"){
                while (1 ==1){
                    Console.WriteLine("> INPUT A LOCATION OF THE PATH FILE:");
                    REPLY = Console.ReadLine();
                    if (REPLY != ""){
                        Console.WriteLine("");
                        Console.WriteLine("> DO YOU WANT TO CONTINUE WITH PATH {0}? <Y/N>", REPLY);
                        string REPLY2 = Console.ReadLine();
                        if (REPLY2.ToUpper() != "Y"){
                            Console.WriteLine("> DO YOU WANT TO EXIT THE PROCESSING? <Y/N>");
                            REPLY2 = Console.ReadLine();
                            if (REPLY2.ToUpper() == "Y"){
                                P_EXIT(8);
                                break;
                            }
                            else continue;
                        }
                        else{
                            if (!REPLY.PATHCHCK())
                                Console.WriteLine("> PATH {0} CONTAINS ILLEGAL CHARACTERS! INPUT THE PATH AGAIN.", REPLY);
                            else{
                                REPLY = REPLY.FFULLPTH();
                                if (!REPLY.ISFILE())
                                    Console.WriteLine(">> INPUTED PATH {0} IS A DIRECTORY. INPUT THE PATH AGAIN.", REPLY);
                                else if (!File.Exists(REPLY))
                                    Console.WriteLine(">> INPUTED FILE PATH {0} DOES NOT EXIST. INPUT THE PATH AGAIN.", REPLY);
                                else{
                                    var T = new Thread(() => {
                                        DIRREC(REPLY, 0);
                                    });   
                                    T.Start();
                                    T.Join();

                                    Console.WriteLine("");
                                    Console.WriteLine("-----------------------------------------");
                                    Console.WriteLine("");
                                    Console.WriteLine("PROCESSING OF FILE {0} WAS FINISHED. DO YOU WANT TO DELETE THAT FILE? <Y/N>", REPLY);
                                    REPLY2 = Console.ReadLine();
                                    if (REPLY2.ToUpper() == "Y"){
                                        File.Delete(REPLY);
                                        Console.WriteLine("");
                                        Console.WriteLine(">> FILE {0} WAS DELETED.", REPLY);
                                    }

                                    P_EXIT(0);
                                    break; 
                                }
                            }
                        }
                    }
                }
            }
            while (1 ==1){
                Console.WriteLine("");
                bool CANS = FPATH.ISCP437();
                if (CANS)
                    Console.WriteLine("> INPUT A PATH TO THE DIRECTORY WITH THE FILES OR INPUT S TO USE DEFAULT PATH {0}:", FPATH);
                else
                    Console.WriteLine("> INPUT A PATH TO THE DIRECTORY WITH THE FILES:");
                Console.WriteLine("");
                Console.WriteLine(">> NOTE: PATH CANNOT CONTAIN NON-CP437 CHARACTERS.");
                Console.WriteLine("");
                Console.WriteLine("> INPUT A PATH:");
                REPLY = Console.ReadLine();
                if (REPLY != ""){
                    if (REPLY.ToUpper() == "S" && CANS)
                        REPLY = FPATH;
                    else if (!REPLY.PATHCHCK() || !REPLY.ISCP437())
                        Console.WriteLine("> PATH {0} CONTAINS ILLEGAL CHARACTERS! INPUT THE PATH AGAIN.", REPLY);
                    else{
                        REPLY = REPLY.FFULLPTH();
                        if (REPLY.ISFILE()){
                            REPLY = REPLY.GETDIRPT();
                            Console.WriteLine(">> YOU HAVE INPUTED A FILE NAME. IT WAS CHANGED TO A DIRECTORY PATH {0} AS PER SELECTED SETTINGS.", REPLY);
                        }
                        REPLY = REPLY.DFULLPTH();
                    }
                    if (Directory.Exists(REPLY)){
                        Console.WriteLine("");
                        Console.WriteLine("> DO YOU WANT TO CONTINUE WITH PATH {0}? <Y/N>", REPLY);
                        string REPLY2 = Console.ReadLine();
                        if (REPLY2.ToUpper() != "Y"){
                            Console.WriteLine("> DO YOU WANT TO EXIT THE PROCESSING? <Y/N>");
                            REPLY2 = Console.ReadLine();
                            if (REPLY2.ToUpper() == "Y"){
                                P_EXIT(8);
                                break;
                            }
                            else continue;
                        }
                        else{
                            FPATH = REPLY;
                            var T = new Thread(() => {
                                PREPROCESS(FPATH, P);
                            });   
                            T.Start();
                            T.Join();
                            P_EXIT(0);
                            break;
                        }
                    }
                    else{
                        Console.WriteLine("> DIRECTORY {0} DOES NOT EXIST! DO YOU WANT TO EXIT THE PROCESSING? <Y/N>", REPLY);
                        REPLY = Console.ReadLine();
                        if (REPLY.ToUpper() == "Y"){
                            P_EXIT(8);
                            break;                        
                        }
                    }
                }
            }
        }

        public static void DIRREC(string PTH, int TYPE){
            string REPLY = "";
            if (File.Exists(PTH)){
                try{
                    int P = 0;

                    var R = File.ReadAllLines(PTH, Encoding.Unicode).Reverse(); 
                    
                    foreach (var LINE in R){

                        if (LINE != null){
                            string[] PLINE = LINE.Split(":#:@:#:");
                            if (PLINE.Length < 3) continue;
                            PLINE[0] = PLINE[0].Trim().ToUpper();
                            if (PLINE[0] == "D" || PLINE[0] == "F"){
                                P++;
                                PLINE[1] = PLINE[1].Trim();
                                PLINE[2] = PLINE[2].Trim();
                            }  
                            if (PLINE[0] == "D"){
                                if (Directory.Exists(PLINE[2])){
                                    if (TYPE == 0){
                                        Console.WriteLine("> DIRECTORY {0} ALREADY EXISTS! DO YOU WANT TO DELETE IT OR SKIP IT? <Y/N>", PLINE[2]);
                                        REPLY = Console.ReadLine();
                                    }
                                    else REPLY = "N";
                                    if (REPLY.ToUpper() == "Y"){
                                        Directory.Delete(PLINE[2]);
                                        break;                        
                                    }
                                    else if (TYPE == 0)
                                        Console.WriteLine(">> DIRECTORY {0} WAS SKIPPED.", PLINE[2]);
                                }
                                if (!Directory.Exists(PLINE[2])){
                                    int EXECMOVE = 1;
                                    if (!Directory.Exists(PLINE[1])){ 
                                        EXECMOVE = 0;
                                        Directory.CreateDirectory(PLINE[2]);
                                    }
                                    else if (!PLINE[2].ToUpper().Contains("SYSTEM VOLUME INFORMATION") && !PLINE[2].ToUpper().Contains("SYSTEM VOLUME INFORMATION")){

                                        var T = new Thread(() => {
                                            P++;
                                            DirectoryInfo RD = new DirectoryInfo(PLINE[1]);
                                            RD.MoveTo(PLINE[2]);
                                            int EXPWAITT = 1;
                                            int MAXEWAIT = 30;
                                            while (EXECMOVE == 1){
                                                DateTime EXPBREAK = DateTime.Now.AddSeconds(MAXEWAIT);
                                                bool BRKEXPRD = false;
                                                while (Directory.Exists(PLINE[1]) && !Directory.Exists(PLINE[2]) && !BRKEXPRD ) { 
                                                    if (DateTime.Now.CompareTo(EXPBREAK) > 0) BRKEXPRD = true;
                                                    else System.Threading.Thread.Sleep(EXPWAITT);
                                                }
                                                if (BRKEXPRD) {
                                                    EXECMOVE = 0;
                                                    while(BRKEXPRD){
                                                        BRKEXPRD = false;
                                                        Console.WriteLine(">> LIMIT FOR MOVING {0} TO {1} HAS REACHED DEFINED THRESHOLD ({2}). DO YOU WANT TO SKIP THE DIRECTORY PROCESSING / WAIT ADDITIONAL {2} SECONDS OR TERMINATE THE PROGRAM PROCESSING? <S/W/X>", PLINE[1], PLINE[2], MAXEWAIT);
                                                        REPLY = Console.ReadLine();
                                                        if (REPLY.ToUpper() == "X"){
                                                            P_EXIT(8);
                                                            return;
                                                        }
                                                        else if (REPLY.ToUpper() == "W")
                                                            EXECMOVE = 1;
                                                        else if (REPLY.ToUpper() == "S")
                                                            break;
                                                        else{
                                                            Console.WriteLine("> INVALID REPLY. PLEASE, TRY IT AGAIN.");
                                                            BRKEXPRD = true;
                                                        }
                                                    }
                                                }
                                                else
                                                    EXECMOVE = 2;
                                            }
                                            if (EXECMOVE == 2)
                                                Console.WriteLine("[{0}]: DIRECTORY PROCESSED: {1} -> {2}", P, PLINE[1], PLINE[2]);
                                            else if (EXECMOVE == 1)
                                                Console.WriteLine("[{0}]: >> DIRECTORY {1} -> {2} CANNOT BE PROCESSED. SKIPPING.", P, PLINE[1], PLINE[2]);
                                            if (EXECMOVE == 8)
                                                Console.WriteLine("[{0}]: >> DIRECTORY {1} -> {2} CANNOT BE PROCESSED. SKIPPING.", P, PLINE[1], PLINE[2]);
                                        });   
                                        T.Start();
                                        T.Join();
                                        if (TYPE == 0 && EXECMOVE == 0) Console.WriteLine("[{0}]: DIRECTORY WAS PROCESSED: {1} -> {2}", P, PLINE[1], PLINE[2]); 
                                    }
                                }
                            }
                            else if (PLINE[0] == "F"){
                                if (File.Exists(PLINE[2])){
                                    if (TYPE == 0){
                                        Console.WriteLine("> FILE {0} ALREADY EXISTS! DO YOU WANT TO DELETE IT OR SKIP IT? <Y/N>", PLINE[2]);
                                        REPLY = Console.ReadLine();
                                    }
                                    else REPLY = "N";
                                    if (REPLY.ToUpper() == "Y"){
                                        File.Delete(PLINE[2]);
                                        break;                        
                                    }
                                    else if (TYPE == 0)
                                        Console.WriteLine(">> FILE {0} WAS SKIPPED.", PLINE[2]);
                                }  
                                if (!File.Exists(PLINE[2])){
                                    int EXECMOVE = 0;
                                    if (!File.Exists(PLINE[1])){
                                        if (TYPE == 0)
                                            Console.WriteLine(">> SOURCE FILE {0} DOES NOT EXISTS! SKIPPING IT.", PLINE[1]);
                                    }
                                    else if (!PLINE[2].ToUpper().Contains("SYSTEM VOLUME INFORMATION") && !PLINE[2].ToUpper().Contains("SYSTEM VOLUME INFORMATION")){
                                        EXECMOVE = 1;

                                        var T = new Thread(() => {
                                            if (EXECMOVE == 1){
                                                P++;
                                                File.Move(PLINE[1], PLINE[2]);
                                                int EXPWAITT = 1;
                                                int MAXEWAIT = 30;
                                                while (EXECMOVE == 1){
                                                    DateTime EXPBREAK = DateTime.Now.AddSeconds(MAXEWAIT);
                                                    bool BRKEXPRD = false;
                                                    while (File.Exists(PLINE[1]) && !File.Exists(PLINE[2]) && !BRKEXPRD ) { 
                                                        if (DateTime.Now.CompareTo(EXPBREAK) > 0) BRKEXPRD = true;
                                                        else System.Threading.Thread.Sleep(EXPWAITT);
                                                    }
                                                    if (BRKEXPRD) {
                                                        EXECMOVE = 0;
                                                        while(BRKEXPRD){
                                                            BRKEXPRD = false;
                                                            Console.WriteLine(">> LIMIT FOR MOVING {0} TO {1} HAS REACHED DEFINED THRESHOLD ({2}). DO YOU WANT TO SKIP THE FILE PROCESSING / WAIT ADDITIONAL {2} SECONDS OR TERMINATE THE PROGRAM PROCESSING? <S/W/X>", PLINE[1], PLINE[2], MAXEWAIT);
                                                            REPLY = Console.ReadLine();
                                                            if (REPLY.ToUpper() == "X"){
                                                                P_EXIT(8);
                                                                return;
                                                            }
                                                            else if (REPLY.ToUpper() == "W")
                                                                EXECMOVE = 1;
                                                            else if (REPLY.ToUpper() == "S")
                                                                break;
                                                            else{
                                                                Console.WriteLine(">> INVALID REPLY. PLEASE, TRY IT AGAIN.");
                                                                BRKEXPRD = true;
                                                            }
                                                        }
                                                    }
                                                    else
                                                        EXECMOVE = 2;
                                                };
                                            }
                                            if (EXECMOVE == 2)
                                                Console.WriteLine("[{0}]: FILE PROCESSED: {1} -> {2}", P, PLINE[1], PLINE[2]);
                                            else
                                                Console.WriteLine("[{0}]: >> FILE {1} -> {2} CANNOT BE PROCESSED. SKIPPING.", P, PLINE[1], PLINE[2]);

                                        });   
                                        T.Start();
                                        T.Join();
                                        if (TYPE == 0)
                                            Console.WriteLine("[{0}]: FILE WAS PROCESSED: {1} -> {2}", P, PLINE[1], PLINE[2]);
                                    }
                                }                          
                            }
                        }
                    }
                }
                catch(Exception Ex){
                    Console.WriteLine(">> ERROR WHEN READING A PATH FILE! {0}", Ex);
                }
            }
        }

        public static void DIRMAN(string PTH, string GENFP, int TYPE, List<string> DONTPROC, int P){
            string OLDNAME = new DirectoryInfo(PTH).Name;
            if (!DONTPROC.Contains(OLDNAME.ToUpper())){
                List<string> DIRS = new List<string>(Directory.GetDirectories(PTH));
                if (TYPE == 1){
                    //execute only from recursion

                    string NEWDNAME = "D"+Guid.NewGuid().ToString("N");
                    NEWDNAME = NEWDNAME.Substring(0, Math.Min(NEWDNAME.Length, 8));
                    string GENDIR = Path.Combine(Directory.GetParent(PTH).FullName, Path.GetFileNameWithoutExtension(NEWDNAME));

                    int EXECMOVE = 1;

                    var T = new Thread(() => {
                        if (PTH.ToUpper().Contains("SYSTEM VOLUME INFORMATION") || PTH.ToUpper().Contains("SYSTEM VOLUME INFORMATION"))
                            EXECMOVE = 0;
                            if (EXECMOVE == 1){
                                P++;
                                DirectoryInfo RD = new DirectoryInfo(PTH);
                                RD.MoveTo(GENDIR);
                                int EXPWAITT = 1;
                                int MAXEWAIT = 30;
                                while (EXECMOVE == 1){
                                    DateTime EXPBREAK = DateTime.Now.AddSeconds(MAXEWAIT);
                                    bool BRKEXPRD = false;
                                    while (Directory.Exists(PTH) && !Directory.Exists(GENDIR) && !BRKEXPRD ) { 
                                        if (DateTime.Now.CompareTo(EXPBREAK) > 0) BRKEXPRD = true;
                                        else System.Threading.Thread.Sleep(EXPWAITT);
                                    }
                                    if (BRKEXPRD) {
                                        EXECMOVE = 0;
                                        while(BRKEXPRD){
                                            BRKEXPRD = false;
                                            Console.WriteLine(">> LIMIT FOR MOVING {0} TO {1} HAS REACHED DEFINED THRESHOLD ({2}). DO YOU WANT TO SKIP THE DIRECTORY PROCESSING / WAIT ADDITIONAL {2} SECONDS OR TERMINATE THE PROGRAM PROCESSING? <S/W/X>", PTH, GENDIR, MAXEWAIT);
                                            string REPLY = Console.ReadLine();
                                            if (REPLY.ToUpper() == "X"){
                                                P_EXIT(8);
                                                return;
                                            }
                                            else if (REPLY.ToUpper() == "W")
                                                EXECMOVE = 1;
                                            else if (REPLY.ToUpper() == "S")
                                                break;
                                            else{
                                                Console.WriteLine(">> INVALID REPLY. PLEASE, TRY IT AGAIN.");
                                                BRKEXPRD = true;
                                            }
                                        }
                                    }
                                    else
                                        EXECMOVE = 2;
                                }
                            }
                            if (EXECMOVE == 2)
                                Console.WriteLine("[{0}]: DIRECTORY PROCESSED: {1} -> {2}", P, PTH, GENDIR);
                            else if (EXECMOVE == 1)
                                Console.WriteLine("[{0}]: >> DIRECTORY {1} -> {2} CANNOT BE PROCESSED. SKIPPING.", P, PTH, GENDIR);
                            if (EXECMOVE == 8)
                                Console.WriteLine("[{0}]: >> DIRECTORY {1} -> {2} CANNOT BE PROCESSED. SKIPPING.", P, PTH, GENDIR);
                    });   
                    T.Start();
                    T.Join(); 

                    if (EXECMOVE == 2){
                        StreamWriter GWP = new StreamWriter(File.Open(GENFP, FileMode.Append), Encoding.Unicode);
                        GWP.WriteLine("D:#:@:#:{0}:#:@:#:{1}", GENDIR, PTH);
                        GWP.Close();

                        DIRS = new List<string>(Directory.GetDirectories(GENDIR));
                    }
                }

                foreach(var WRKDIR in DIRS){
                    string WDPTH = WRKDIR.DFULLPTH();
                    if (Directory.Exists(WDPTH) && !WDPTH.ToUpper().Contains("SYSTEM VOLUME INFORMATION") && !WDPTH.ToUpper().Contains("SYSTEM VOLUME INFORMATION")){
                        var T = new Thread(() => {
                            DIRMAN(WDPTH, GENFP, 1, DONTPROC, P);
                        });   
                        T.Start();
                        T.Join();
                    }
                }
            }
        }

        public static void PREPROCESS(string PTH, int P){
            //get current location of the program and check, if it has ffmpeg & virtualdub in its directory. if so, then will use these over settings in %PATH%
            string CPATH = Directory.GetParent(Assembly.GetExecutingAssembly().Location).ToString().DFULLPTH();
            string FFMPEGPTH = (CPATH+"/FFMPEG.EXE").FFULLPTH();

            if (!File.Exists(FFMPEGPTH)) FFMPEGPTH = "FFMPEG.EXE";
            else FFMPEGPTH = "\""+FFMPEGPTH+"\"";

            Random R = new Random();
            string N1 = R.Next(0, 99999).ToString("D");

            string NEWFILNC = new string('0', Math.Max(8-N1.Length, 0))+N1;
            NEWFILNC = "JOB"+NEWFILNC.Substring(3);
            string GENFC = Path.Combine(PTH, NEWFILNC+".bat");

            string NEWFILNP = new string('0', Math.Max(8-N1.Length, 0))+N1;
            NEWFILNP = "PTH"+NEWFILNP.Substring(3);
            string GENFP = Path.Combine(PTH, NEWFILNP+".pth");

            string NEWDNAME = new string('0', Math.Max(8-N1.Length, 0))+N1;
            NEWDNAME = "TMP"+NEWDNAME.Substring(3);
            string TMPDIR = Path.Combine(PTH, NEWDNAME);
            Directory.CreateDirectory(TMPDIR);

            DateTime DT = DateTime.Now;

            List<string> DONTPROC = new List<string>();
            DONTPROC.Add(Path.GetFileNameWithoutExtension(NEWFILNC).ToUpper());
            DONTPROC.Add(Path.GetFileNameWithoutExtension(NEWFILNP).ToUpper());
            DONTPROC.Add(Path.GetFileNameWithoutExtension(NEWDNAME).ToUpper());

            int ISCUDA = 0;

            Console.WriteLine("");
            Console.WriteLine("> DO YOU WANT TO USE CUDA HW ACCELERATION? <Y/N>");
            Console.WriteLine("");
            string REPLY = Console.ReadLine();
            if (REPLY.ToUpper() == "Y") ISCUDA = 1;

            int DOSHDWN = 0;

            Console.WriteLine("");
            Console.WriteLine("> DO YOU WANT TO SHUTDOWN COMPUTER WHEN FINISHED PROCESSING? <Y/N>");
            Console.WriteLine("");
            REPLY = Console.ReadLine();
            if (REPLY.ToUpper() == "Y") DOSHDWN = 1;


            StreamWriter GWP = new StreamWriter(File.Open(GENFP, FileMode.OpenOrCreate), Encoding.Unicode);
            GWP.WriteLine("MULTIMEDIA CONVERTOR FOR 3G PHONES V1.00 PATH FILE");
            GWP.WriteLine("");
            GWP.WriteLine("CREATED IN {0}", string.Format("{0:YYYY/MM/DD gg:mm:ss tt}", DT.ToString()));
            GWP.WriteLine("WORKING DIRECTORY: {0}", PTH.ToUpper());
            GWP.WriteLine("");
            GWP.WriteLine("CUDA HW ACCELERATION: {0}", ISCUDA == 1 ? "YES" : "NO");
            GWP.WriteLine("");
            GWP.WriteLine("----------------------------------------------");
            GWP.WriteLine("");
            GWP.Close();

            StreamWriter GWC = new StreamWriter(File.Open(GENFC, FileMode.OpenOrCreate), Encoding.ASCII);
            GWC.WriteLine("@ECHO OFF");
            GWC.WriteLine("SETLOCAL");
            GWC.WriteLine("");
            GWC.WriteLine("REM *****************************************************************************");
            GWC.WriteLine("REM");
            GWC.WriteLine("REM  MULTIMEDIA CONVERTOR FOR 3G PHONES V1.00 EXECUTION JOB");
            GWC.WriteLine("REM");
            GWC.WriteLine("REM  CREATED IN {0}", string.Format("{0:YYYY/MM/DD gg:mm:ss tt}", DT.ToString()));
            GWC.WriteLine("REM  WORKING DIRECTORY: {0}", PTH.ToUpper());
            GWC.WriteLine("REM");
            GWC.WriteLine("REM  CUDA HW ACCELERATION: {0}", ISCUDA == 1 ? "YES" : "NO");
            GWC.WriteLine("REM  SHUTDOWN AFTER COMPLETION: {0}", DOSHDWN == 1 ? "YES" : "NO");
            GWC.WriteLine("REM");
            GWC.WriteLine("REM *****************************************************************************");
            GWC.WriteLine("");
            GWC.WriteLine("ECHO.");
            GWC.WriteLine("ECHO MULTIMEDIA CONVERTOR FOR 3G PHONES V1.00 EXECUTION JOB");
            GWC.WriteLine("ECHO.");
            GWC.WriteLine("ECHO SELECTED DIRECTORY FOR PROCESSING: {0}", PTH.ToUpper());
            GWC.WriteLine("");
            GWC.WriteLine("SET SEL=%1");
            GWC.WriteLine("SET ISCUDA={0}", ISCUDA);
            GWC.WriteLine("SET DOSHDWN={0}", DOSHDWN);
            GWC.WriteLine("SET WASAUTO=1");
            GWC.WriteLine("");
            GWC.WriteLine("ECHO.");
            GWC.WriteLine("IF NOT \"%SEL%\" == \"1\" (");
            GWC.WriteLine("   SET /P SEL=INPUT 1 TO CONTINUE:");
            GWC.WriteLine("   ECHO.");
            GWC.WriteLine("   IF \"%SEL%\" == \"1\" (");
            GWC.WriteLine("      SET /P ISCUDA=INPUT 1 IF YOU WANT TO USE CUDA HW ACCELERATION:");
            GWC.WriteLine("      ECHO.");
            GWC.WriteLine("      SET /P DOSHDWN=INPUT 1 IF YOU WANT TO SHUTDOWN COMPUTER WHEN FINISHED PROCESSING:");
            GWC.WriteLine("   )");
            GWC.WriteLine("   SET WASAUTO=0");
            GWC.WriteLine(")");
            GWC.WriteLine("IF NOT \"%ISCUDAL%\" == \"1\" (");
            GWC.WriteLine("   SET ISCUDA=y");
            GWC.WriteLine(") ELSE (");
            GWC.WriteLine("   SET ISCUDA=\"hwaccel cuda -y\"");
            GWC.WriteLine(")");
            GWC.WriteLine("IF \"%SEL%\" == \"1\" (");
            GWC.WriteLine("   GOTO PROCESS");
            GWC.WriteLine(")");
            GWC.WriteLine("");
            GWC.WriteLine("GOTO PROGEXIT");
            GWC.WriteLine("");
            GWC.WriteLine(":PROCESS");
            GWC.Close();

            var T = new Thread(() => {
                DIRMAN(PTH, GENFP, 0, DONTPROC, P);
            });   
            T.Start();
            T.Join();

            T = new Thread(() => {
                PROCESS(PTH, GENFC, GENFP, TMPDIR, DONTPROC, P, FFMPEGPTH);
            });   
            T.Start();
            T.Join();

            GWC = new StreamWriter(File.Open(GENFC, FileMode.Append), Encoding.ASCII);
            GWC.WriteLine("   DEL /F /Q \"{0}\"", TMPDIR);
            GWC.WriteLine("   RMDIR /S /Q \"{0}\"", TMPDIR);
            GWC.WriteLine("   GOTO PROGEXIT");
            GWC.WriteLine("");
            GWC.WriteLine(":PROGEXIT");
            GWC.WriteLine("   IF %WASAUTO% == 0 (");
            GWC.WriteLine("      ECHO -----------------------------------------");
            GWC.WriteLine("      ECHO.");
            GWC.WriteLine("      ECHO JOB PROCESSING HAS FINISHED!");
            GWC.WriteLine("      ECHO.");
            GWC.WriteLine("      IF \"%SEL%\" == \"1\" (");
            GWC.WriteLine("         ECHO THIS JOB FILE WILL BE DELETED IN THE NEXT STEP.");
            GWC.WriteLine("      )");
            GWC.WriteLine("      ECHO.");
            GWC.WriteLine("      IF \"%DOSHDWN%\" == \"0\" (");
            GWC.WriteLine("         PAUSE");
            GWC.WriteLine("      )");
            GWC.WriteLine("   )");
            GWC.WriteLine("   IF \"%SEL%\" == \"1\" (");
            GWC.WriteLine("      IF \"%DOSHDWN%\" == \"1\" (");
            GWC.WriteLine("         START \"\" SHUTDOWN.EXE -S -T 5");
            GWC.WriteLine("      )");
            GWC.WriteLine("      DEL \"%~f0\" & EXIT /B");
            GWC.WriteLine("   ) ELSE (");
            GWC.WriteLine("      IF \"%DOSHDWN%\" == \"1\" (");
            GWC.WriteLine("         START \"\" SHUTDOWN.EXE -S -T 5");
            GWC.WriteLine("      )");
            GWC.WriteLine("      EXIT /B");
            GWC.WriteLine("   )");
            GWC.WriteLine("");
            GWC.WriteLine("ENDLOCAL");
            GWC.Close();

            Console.WriteLine("");
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine("");
            Console.WriteLine(">> EXECUTION JOB IS PREPARED IN {0}", GENFC);
            Console.WriteLine(">> PATH FILE IS PREPARED IN {0}", GENFP);
            Console.WriteLine("");
            Console.WriteLine("> DO YOU WANT TO EXECUTE IT NOW? <Y/N>");
            REPLY = Console.ReadLine();
            if (REPLY.ToUpper() == "Y"){
                Console.WriteLine("");
                Console.WriteLine(">> EXECUTING {0} ...", GENFC);
                Console.WriteLine("");

                Process C = new Process();
                C.StartInfo.FileName = GENFC;
                C.StartInfo.Arguments = "1";
                C.Start();
                C.WaitForExit();  
                
                T = new Thread(() => {
                    DIRREC(GENFP, 1);
                });   
                T.Start();
                T.Join();

                File.Delete(GENFP);

                Console.WriteLine("");
                Console.WriteLine("-----------------------------------------");
                Console.WriteLine("");
                Console.WriteLine(">> EXECUTION JOB HAS FINISHED");
            }
            else{
                Console.WriteLine("");
                Console.WriteLine(">> ONCE EXECUTION JOB WILL BE MANUALLY EXECUTED, DO LOAD THIS PROGRAM AGAIN AND POINT IT TO THIS PATH FILE TO RESTORE PROPER FILE NAMES.");
                Console.WriteLine("");
            }
            P_EXIT(0); 
        }

        public static void PROCESS(string PTH, string GENFC, string GENFP, string MPTH, List<string> DONTPROC, int P, string FFMPEGPTH){
            string OLDNAME = new DirectoryInfo(PTH).Name;
            if (!DONTPROC.Contains(OLDNAME.ToUpper())){
                List<string> FILES = new List<string>(Directory.GetFiles(PTH));
                List<string> DIRS = new List<string>(Directory.GetDirectories(PTH));

                string NEWDNAME = "W"+Guid.NewGuid().ToString("N");
                NEWDNAME = NEWDNAME.Substring(0, Math.Min(NEWDNAME.Length, 8));
                string TMPDIR = Path.Combine(PTH, Path.GetFileNameWithoutExtension(NEWDNAME));

                string NEWDNAME2 = "W"+Guid.NewGuid().ToString("N");
                NEWDNAME2 = NEWDNAME2.Substring(0, Math.Min(NEWDNAME2.Length, 8));
                string TMPDIR2 = Path.Combine(TMPDIR, Path.GetFileNameWithoutExtension(NEWDNAME2));

                DONTPROC.Add(NEWDNAME.ToUpper());
                DONTPROC.Add(NEWDNAME2.ToUpper());

                StreamWriter GWC = new StreamWriter(File.Open(GENFC, FileMode.Append), Encoding.ASCII);
                StreamWriter GWP = new StreamWriter(File.Open(GENFP, FileMode.Append), Encoding.Unicode);

                string FPTH = "";
                string FNAMEOLD = "";
                string FEXT = "";
                string FNAME = "";
                string FNEW = "";
                string FPTHNEW = "";

                int MOVS = 0;
                int MOSS = 0;
                int MOPS = 0;

                int MGEN = 0;

                //prepare a temporary directory for step 1
                foreach(var WRKFILE in FILES){
                    MGEN++;
                    FPTH = WRKFILE.FFULLPTH();
                    FNAMEOLD = Path.GetFileName(FPTH);
                    FEXT = FNAMEOLD.GETFEXT();
                    FNAME = "P"+NEWDNAME+MGEN;
                    FNAMEOLD = Path.GetFileNameWithoutExtension(FNAMEOLD);
                    FPTHNEW = Path.Combine(PTH, FNAME+"."+FEXT);

                    if (!DONTPROC.Contains(FNAMEOLD.ToUpper())){

                        bool WASMOVED = false;

                        if (FEXT.ToUpper() == "MP4" || FEXT.ToUpper() == "MKV" || FEXT.ToUpper() == "FLV" || FEXT.ToUpper() == "3GP" || FEXT.ToUpper() == "AVI" || FEXT.ToUpper() == "TS" || FEXT.ToUpper() == "WMV" || FEXT.ToUpper() == "MPG" || FEXT.ToUpper() == "MPEG" || FEXT.ToUpper() == "MOV" || FEXT.ToUpper() == "M4V"){
                            if (MOVS == 0 && MOSS == 0 && MOPS == 0){
                                GWC.WriteLine("   MKDIR \"{0}\"", TMPDIR);
                                GWC.WriteLine("   MKDIR \"{0}\"", TMPDIR2);
                            }
                            WASMOVED = true;
                            MOVS++;
                            FNEW = Path.Combine(TMPDIR, FNAME+".3gp");
                            string FTAR = Path.Combine(PTH, FNAME+".3gp");
                            string FREC = Path.Combine(PTH, FNAMEOLD+".3gp");
                            GWP.WriteLine("F:#:@:#:{0}:#:@:#:{1}", FTAR, FREC);

                            GWC.WriteLine("    {0} -%ISCUDA% -i \"{1}\" -c:v libx264 -profile:v baseline -filter_complex \"scale='iw*min(1,min(320/iw,240/ih))':-1:flags=lanczos,fps=24,pad=ceil(iw/4)*4:ceil(ih/4)*4\" -b:v 125k -minrate 100k -maxrate 150k -bufsize 1835k -map_metadata -1 -preset slow -pass 1 -an -f 3g2 NUL", FFMPEGPTH, FPTHNEW);
                            GWC.WriteLine("    {0} -%ISCUDA% -i \"{1}\" -c:v libx264 -profile:v baseline -filter_complex \"scale='iw*min(1,min(320/iw,240/ih))':-1:flags=lanczos,fps=24,pad=ceil(iw/4)*4:ceil(ih/4)*4\" -b:v 125k -minrate 100k -maxrate 150k -bufsize 1835k -map_metadata -1 -preset fast -pass 2 -c:a aac -strict experimental -b:a 32k -ar 22050 -filter:a \"volume=0.50\" -ac 1 \"{2}\"", FFMPEGPTH, FPTHNEW, FNEW);
                            
                            GWC.WriteLine("   DEL /F /Q \"{0}\"", FPTHNEW);
                            GWC.WriteLine("   MOVE /Y \"{0}\" \"{1}\"", FNEW, FTAR);
                        }
                        else if (FEXT.ToUpper() == "MP3" || FEXT.ToUpper() == "WMA" || FEXT.ToUpper() == "FLAC" || FEXT.ToUpper() == "OGG" || FEXT.ToUpper() == "M4A" || FEXT.ToUpper() == "WAV"){
                            if (MOSS == 0 && MOVS == 0 && MOPS == 0){
                                GWC.WriteLine("   MKDIR \"{0}\"", TMPDIR);
                                GWC.WriteLine("   MKDIR \"{0}\"", TMPDIR2);
                            }
                            WASMOVED = true;
                            MOSS++;
                            FNEW = Path.Combine(TMPDIR, FNAME+".mp3");
                            string FTAR = Path.Combine(PTH, FNAME+".mp3");
                            string FREC = Path.Combine(PTH, FNAMEOLD+".mp3");
                            GWP.WriteLine("F:#:@:#:{0}:#:@:#:{1}", FTAR, FREC);

                            GWC.WriteLine("   {0} -%ISCUDA% -i \"{1}\" -acodec libmp3lame -strict experimental -b:a 32k -ar 22050 -filter:a \"volume=0.50\" -map_metadata -1 -ac 1 \"{2}\"", FFMPEGPTH, FPTHNEW, FNEW);
                            
                            GWC.WriteLine("   DEL /F /Q \"{0}\"", FPTHNEW);
                            GWC.WriteLine("   MOVE /Y \"{0}\" \"{1}\"", FNEW, FTAR);
                        }
                        else if (FEXT.ToUpper() == "JPG" || FEXT.ToUpper() == "JPEG" || FEXT.ToUpper() == "JPE" || FEXT.ToUpper() == "PNG" || FEXT.ToUpper() == "BMP" || FEXT.ToUpper() == "GIF" || FEXT.ToUpper() == "TIF" || FEXT.ToUpper() == "TIFF" || FEXT.ToUpper() == "TAR" || FEXT.ToUpper() == "TARGA"){
                            if (MOSS == 0 && MOVS == 0 && MOPS == 0){
                                GWC.WriteLine("   MKDIR \"{0}\"", TMPDIR);
                                GWC.WriteLine("   MKDIR \"{0}\"", TMPDIR2);
                            }
                            WASMOVED = true;
                            MOPS++;
                            FNEW = Path.Combine(TMPDIR, FNAME+".jpg");
                            string FTAR = Path.Combine(PTH, FNAME+".jpg");
                            string FREC = Path.Combine(PTH, FNAMEOLD+".jpg");
                            GWP.WriteLine("F:#:@:#:{0}:#:@:#:{1}", FTAR, FREC);

                            GWC.WriteLine("   {0} -%ISCUDA% -i \"{1}\" -filter_complex \"scale='iw*min(1,min(640/iw,480/ih))':-1:flags=lanczos,pad=ceil(iw/4)*4:ceil(ih/4)*4\" -pix_fmt argb -map_metadata -1 \"{2}\"", FFMPEGPTH, FPTHNEW, FNEW);
                            
                            GWC.WriteLine("   DEL /F /Q \"{0}\"", FPTHNEW);
                            GWC.WriteLine("   MOVE /Y \"{0}\" \"{1}\"", FNEW, FTAR);
                        }
                        if (WASMOVED){
                            int EXECMOVE = 1;

                            var T = new Thread(() => {
                                if (FPTH.ToUpper().Contains("SYSTEM VOLUME INFORMATION") || FPTH.ToUpper().Contains("SYSTEM VOLUME INFORMATION"))
                                EXECMOVE = 0;
                                if (EXECMOVE == 1){
                                    P++;
                                    File.Move(FPTH, FPTHNEW);
                                    int EXPWAITT = 1;
                                    int MAXEWAIT = 30;
                                    while (EXECMOVE == 1){
                                        DateTime EXPBREAK = DateTime.Now.AddSeconds(MAXEWAIT);
                                        bool BRKEXPRD = false;
                                        while (File.Exists(FPTH) && !File.Exists(FPTHNEW) && !BRKEXPRD ) { 
                                            if (DateTime.Now.CompareTo(EXPBREAK) > 0) BRKEXPRD = true;
                                            else System.Threading.Thread.Sleep(EXPWAITT);
                                        }
                                        if (BRKEXPRD) {
                                            EXECMOVE = 0;
                                            while(BRKEXPRD){
                                                BRKEXPRD = false;
                                                Console.WriteLine(">> LIMIT FOR MOVING {0} TO {1} HAS REACHED DEFINED THRESHOLD ({2}). DO YOU WANT TO SKIP THE FILE PROCESSING / WAIT ADDITIONAL {2} SECONDS OR TERMINATE THE PROGRAM PROCESSING? <S/W/X>", FPTH, FPTHNEW, MAXEWAIT);
                                                string REPLY = Console.ReadLine();
                                                if (REPLY.ToUpper() == "X"){
                                                    P_EXIT(8);
                                                    return;
                                                }
                                                else if (REPLY.ToUpper() == "W")
                                                    EXECMOVE = 1;
                                                else if (REPLY.ToUpper() == "S")
                                                    break;
                                                else{
                                                    Console.WriteLine("> INVALID REPLY. PLEASE, TRY IT AGAIN.");
                                                    BRKEXPRD = true;
                                                }
                                            };
                                        }
                                        else
                                            EXECMOVE = 2;
                                    }
                                }
                                if (EXECMOVE == 2)
                                    Console.WriteLine("[{0}]: FILE PROCESSED: {1} -> {2}", P, FPTH, FPTHNEW);
                                else
                                    Console.WriteLine("[{0}]: >> FILE {1} -> {2} CANNOT BE PROCESSED. SKIPPING.", P, FPTH, FPTHNEW);
                            });   
                            T.Start();
                            T.Join();
                        }
                    }
                }

                GWC.WriteLine("   RMDIR /S /Q \"{0}\"", TMPDIR);       
                GWC.Close(); 
                GWP.Close();

                foreach(var WRKDIR in DIRS){
                    string WDPTH = WRKDIR.DFULLPTH();
                    if (Directory.Exists(WDPTH)){
                        var T = new Thread(() => {
                            PROCESS(WDPTH, GENFC, GENFP, MPTH, DONTPROC, P, FFMPEGPTH);
                        });   
                        T.Start();
                        T.Join();
                    }
                }
            }
        }
    }
}
