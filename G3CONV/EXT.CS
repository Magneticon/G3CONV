using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace G3CONV{
    public static class EXT{
        public static bool ISDIGIT(this string STR){
            if (STR == null || STR == "")
                return false;
            foreach (char C in STR){
                if (!char.IsDigit(C))
                    return false;
            }
            return true;
        }

        public static string DIG2(this string D){
           return D.Length == 1 ? "0"+D : D;
        }
        
        public static string DFORMAT(this string SD){
            SD = SD.FORMPATH();
            string[] PTHCHCK = SD.Split('\\');
            string FSD = "";
        //    if (PTHCHCK[PTHCHCK.Length-1].Split('.').Length == 1) //--it cannot detect direcotries which has dot in their name....-.- ...causing stack overflow, if any directory with a dot is envountered.
            try{
                FileAttributes A = File.GetAttributes(SD);
                if ((A & FileAttributes.Directory) == FileAttributes.Directory)
                    SD = SD[SD.Length-1] == '/' ? SD = SD.Substring(0, SD.Length-1)+"\\" : SD[SD.Length-1] != '\\' ? SD = SD+"\\" : SD;
                FSD = Path.GetDirectoryName(SD);  
            }catch(Exception Ex){
                //if the durectory won't exist, we need to throw null, otherwise it will crash
                FSD = null;
            }
            return FSD == null ? SD : FSD;
        }

        public static bool PATHCHCK(this string SD){
            SD = SD.FORMPATH();
            Regex containsABadCharacter = new Regex("["+Regex.Escape(new string(Path.GetInvalidPathChars()))+"]");
            if (SD == null || SD == "" || containsABadCharacter.IsMatch(SD)) return false;
            return true;
        }

        public static string DFULLPTH(this string SD){
            SD = SD.DFORMAT();
            return SD.Length > 1 ? SD[1] == ':' && SD.Length < 4 ? SD : Path.GetFullPath(SD) : "";
        }

        public static string FFULLPTH(this string SD){
            SD = SD.FORMPATH();
            return SD.Length > 1 ? SD[1] == ':' && SD.Length < 4 ? SD : Path.GetFullPath(SD) : "";
        }

        public static string DSTR(this string SD){
           SD = SD.FORMPATH();
           return SD.Length > 0 ? SD[SD.Length-1] == '/' ? SD = SD.Substring(0, SD.Length-1)+"\\" : SD[SD.Length-1] != '\\' ? SD = SD+"\\" : SD : "";
        }

        public static bool ISFILE(this string SD){
            SD = SD.FORMPATH();
            string[] ParseName = SD.Split('\\');
      //      if (SD == null || SD == "" || ParseName[ParseName.Length-1].IndexOf('.') == -1) return false;
            try{
                FileAttributes A = File.GetAttributes(SD);
                if ((A & FileAttributes.Directory) == FileAttributes.Directory) return false;
            } catch(Exception Ex){
                return true;
            }
            return true;
        }

        public static string GETDIRPT(this string SD){
            SD = SD.FORMPATH();
            string[] ParseName = SD.Split('\\');
            string Res = "";
            int M = ParseName.Length;
            if (ParseName[ParseName.Length-1].IndexOf('.') != -1) M--;
            for (int i = 0; i < M; i++){
                if (ParseName[i].Trim() != "")
                    Res += ParseName[i]+"\\";
            }
            return Res;
        }

        public static string FORMPATH(this string SD){
            return SD.Replace('/', '\\');
        }

        public static string FORMPATHREVERSE(this string SD){
            return SD.Replace('\\', '/');
        }

        public static string[] Split(this string S, string D){
            return S.Split(new [] {D}, StringSplitOptions.None);
        }

        public static List<string> SplitA(this string S, string D){
            List<string> R = new List<string>();
            string[] A = S.Split(D);
            foreach(string T in A){
                R.Add(T);
            }
            return R;
        }

        public static string GETFEXT(this string S){
            S = Path.GetExtension(S);
            if (S[0] == '.')
                S = S.Substring(1);
            return S;
        }

        public static bool ISCP437(this string S){
            S = S.ToUpper().Trim();
            char[] A = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', ' ', '_', '-', '+', '.', '\\', '/', ':'};
            bool R = true;
            foreach (char C in S){
                if (Array.IndexOf(A, C) == -1){
                    R = false;
                    break;
                }
            }
            return R;
        }
    }
}
