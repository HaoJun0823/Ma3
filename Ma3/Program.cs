using System;
using System.Collections;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Ma3
{
    static class Program
    {

        private static readonly string APPLICATION_VERSION = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        static void Main(string[] args)
        {
            Log.Info("Ma3 application starting! version:"+APPLICATION_VERSION);

            if (args.Length <= 0 || args[0].Equals("-help") || args[0].Equals("/?")) { PrintHelp(); Stop(-99); }
            GetCmdArgs(args);

            if (String.IsNullOrEmpty(Config.InputData)&&!Config.IsForceMode)
            {
                Log.Error("-input data is null or empty.");
                Stop(-98);
            }
            if (String.IsNullOrEmpty(Config.KeyData) && !Config.IsForceMode)
            {
                Log.Error("-key data is null or empty.");
                Stop(-98);
            }
            if (String.IsNullOrEmpty(Config.DictionaryData) && !Config.IsForceMode)
            {
                Log.Error("-dictionary data is null or empty.");
                Stop(-98);
            }

            if (!Config.HaveEncryptArgs)
            {
                Log.Warning("No get -encrypt or -decrypt,set default mode:[encrypt].");
            }
            if (Config.DictionaryData.Equals(Config.DEFAULT_DICTIONARY))
            {
                Log.Warning("No get -dictionary,set default dictionary:["+Config.DEFAULT_DICTIONARY+"].");
            }
            else
            {
                double mathLog = Math.Log(Config.DictionaryData.Length,2);

                if (Regex.IsMatch(Convert.ToString(mathLog),@"^[1-9][0-9]*\.0$"))
                {
                    Log.Warning("Dictionary ["+Config.DictionaryData+"] is a valid, "+ "(Math.log("+Config.DictionaryData.Length+",2) = "+mathLog+" is an integer.)");
                }
                else
                {
                    Log.Error("Dictionary [" + Config.DictionaryData + "] isn't a valid, " + "(Math.log(" + Config.DictionaryData.Length + ",2) = " + mathLog + "is not an integer.)"); ;
                    Stop(-98);
                }


            }

            if (Config.IsEncrypt)
            {
                Log.Info("Do encrypt!");
            }
            else
            {
                Log.Info("Do decrypt!");
            }
            string result = Coder.translate(Config.InputData, Config.KeyData, Config.DictionaryData, Config.IsEncrypt, Config.IsReverse);
            Console.WriteLine("FINAL:["+ result + "].");
            Log.Info("FINAL:[" + result + "].");
            if (Config.IsRecovery)
            {
                Log.Info("Try Recovery!");
                Config.IsEncrypt = !Config.IsEncrypt;
                if (Config.IsEncrypt)
                {
                    Log.Info("Do encrypt!");
                }
                else
                {
                    Log.Info("Do decrypt!");
                }
                string resultRecovery = Coder.translate(result, Config.KeyData, Config.DictionaryData, Config.IsEncrypt, Config.IsReverse);
                Console.WriteLine("FINAL:[" + resultRecovery + "].");
                Log.Info("FINAL:[" + resultRecovery + "].");
            }
            Stop(1);

        }

        static void PrintHelp()
        {
            Console.WriteLine("-input <string>:(required)Enter the text to be encoded.");
            Console.WriteLine("-key <string>:(required)Enter some keywords to generate the key.");

            Console.WriteLine("-encrypt:Perform encryption operations.(Encrypted by default)");
            Console.WriteLine("-decrypt:Perform a decryption operation.");

            Console.WriteLine("-fill <char>:Enter a char to aglin input text.(space is default fill char)");
            Console.WriteLine("-dictionary <string>,Enter a custom dictionary or use the default dictionary without typing. Log(2,Dictionary.Length) must be an integer.(default:" + Config.DEFAULT_DICTIONARY + ")");

            Console.WriteLine("-help or /?:Print help information.");
            
            Console.WriteLine("-recovery:Encrypt then decrypt or decrypt then encrypt.");
            Console.WriteLine("-reverse:Reverse key.");
            Console.WriteLine("-nolog:Only print result.");
            Console.WriteLine("-debug:Print debug text.");
            Console.WriteLine("-force:Ignore the error message.");





        }

        static void CheckNextIndexOutOfArray(int length,int next)
        {
            if (next > length)
            {
                Log.Error("Args number is wrong!");
                PrintHelp();
                Stop(-97);
            }

        }

        static void GetCmdArgs(string[] args)
        {
            
            for(int i = 0; i < args.Length; i++)
            {
                if (args[i].Equals("-input"))
                {
                    CheckNextIndexOutOfArray(args.Length,i+1);
                    if (!String.IsNullOrEmpty(args[i + 1]))
                    {
                        Config.InputData = args[++i];
                        continue;
                    }
                    else
                    {
                        Log.Error("Unknown -input args:[" + args[i+1]+"].");
                        Stop(-100);
                    }
                }else
                if (args[i].Equals("-key"))
                {
                    CheckNextIndexOutOfArray(args.Length, i + 1);
                    if (!String.IsNullOrEmpty(args[i + 1]))
                    {
                        Config.KeyData = args[++i];
                        continue;
                    }
                    else
                    {
                        Log.Error("Unknown -key args:[" + args[i + 1] + "].");
                        Stop(-100);
                    }
                }else
                if (args[i].Equals("-dictionary"))
                {
                    CheckNextIndexOutOfArray(args.Length, i + 1);
                    if (!String.IsNullOrEmpty(args[i + 1]))
                    {
                        Config.DictionaryData = args[++i];
                        continue;
                    }
                    else
                    {
                        Log.Error("Unknown -dictionary args:[" + args[i + 1] + "].");
                        Stop(-100);
                    }
                }else
                if (args[i].Equals("-debug"))
                {
                    Config.LogDebug = true;
                    Log.Debug("Debug log mode enabled!");
                    Log.Debug("Get "+args.Length+" cmd args:" + string.Join(",", args));
                    continue;
                }else
                if (args[i].Equals("-fill"))
                {
                    CheckNextIndexOutOfArray(args.Length, i + 1);
                    if (!String.IsNullOrEmpty(args[i + 1]))
                    {
                        Config.FillChar = args[++i][0];
                        continue;
                    }
                    else
                    {
                        Log.Error("Unknown -fill args:[" + args[i + 1] + "].");
                        Stop(-100);
                    }
                }
                else
                if (args[i].Equals("-encrypt"))
                {
                    Config.HaveEncryptArgs = true;
                    Config.IsEncrypt = true;
                }else
                if (args[i].Equals("-decrypt"))
                {
                    Config.HaveEncryptArgs = true;
                    Config.IsEncrypt = false;
                }else
                if (args[i].Equals("-nolog"))
                {
                    //Config.LogError = false;
                    Config.LogWarning = false;
                    Config.LogInfo = false; 
                }else
                if (args[i].Equals("-force"))
                {
                    Config.LogError = false;
                    Config.IsForceMode = true;
                }
                if (args[i].Equals("-reverse"))
                {
                    Config.IsReverse = true;
                }
                if (args[i].Equals("-recovery"))
                {
                    Config.IsRecovery = true;
                }
                //if (args[i].equals("-task"))
                //{
                //    if (!string.isnullorempty(args[i + 1]) && regex.ismatch(args[i + 1], "^[1-9][0-9]*$"))
                //    {
                //        config.tasknumber = int.parse(args[i + 1]);
                //        continue;
                //    }
                //    else
                //    {
                //        log.error("unknown -task args:[" + args[i + 1] + "].");
                //        stop(-100);
                //    }
                //}




            }


        }

        static void Stop(int code = 0)
        {

            if (code == -100)
            {
                PrintHelp();
            }


            Log.Info("Application Will Stop(ExitCode=" + code + ")");
            System.Environment.Exit(code);
        }


    }

    static class Config
    {

        public const string DEFAULT_DICTIONARY = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz+-";

        //public static bool IsMultipleThread = true;
        public static bool LogInfo = true;
        public static char FillChar = ' ';
        public static bool LogDebug = false;
        public static bool LogWarning = true;
        public static bool LogError = true;
        public static bool IsForceMode = false;
        public static bool IsReverse = false;
        public static string InputData = "";
        public static string KeyData = "";
        public static string DictionaryData = DEFAULT_DICTIONARY;
        public static bool IsEncrypt = true;
        public static bool HaveEncryptArgs = false;
        public static bool IsRecovery = false;
        //public static int TaskNumber = Environment.ProcessorCount;

    }


    static class Log
    {


        public static void Info(string message)
        {
            System.Diagnostics.Trace.WriteLineIf(Config.LogInfo,message,"Info");
        }

        public static void Warning(string message)
        {
            System.Diagnostics.Trace.WriteLineIf(Config.LogWarning,message, "Warning");
        }

        public static void Debug(string message)
        {
            System.Diagnostics.Trace.WriteLineIf(Config.LogDebug,message,"Debug");
        }

        public static void Error(string message)
        {
            System.Diagnostics.Trace.WriteLineIf(Config.LogError,message,"Error");
        }


    }

    static class Coder
    {

        public static int GetLargestCommonDivisor(int n1, int n2)
        {
            int max = n1 > n2 ? n1 : n2;
            int min = n1 < n2 ? n1 : n2;
            int remainder;
            while (min != 0)
            {
                remainder = max % min;
                max = min;
                min = remainder;
            }
            return max;
        }

        public static int GetLeastCommonMutiple(int n1, int n2)
        {
            return n1 * n2 / GetLargestCommonDivisor(n1, n2);
        }
        public static string translate(string data,string key,string dictionary,bool encrypt,bool reverse = false,char fill = ' ')
        {
            Log.Debug("Execute translate method:[data:"+data+",key:"+key+",dictionary:"+dictionary+",encrypt:"+encrypt+",reverse:"+reverse+"].");

            int mathLog = (int)Math.Log(dictionary.Length,2);

            Log.Debug("Math.Log("+dictionary.Length+",2) = "+mathLog+".");




            if (encrypt)
            {

                int dataBinaryLength  = data.Length * 8;
                Log.Debug("Data binary length:"+dataBinaryLength);
                int leastCommonMutiple = GetLeastCommonMutiple(dataBinaryLength,mathLog);
                Log.Debug("Get least common mutiple(data length & dictionary lentgh):" + leastCommonMutiple);
                int lastNumber = leastCommonMutiple / 8;
                Log.Debug("Get fill number:" + (lastNumber - data.Length));

                if (lastNumber > 0) { 

                StringBuilder strBuild = new StringBuilder();
                strBuild.Append(data);
                for(int i = 0; i < lastNumber - data.Length; i++)
                {
                    strBuild.Append(fill);
                }
                data = strBuild.ToString();
                    Log.Debug("Finish fill data:["+data+"],length:"+data.Length);
                }
                byte[] dataByteArray = StringToByteArray(data);
                BitArray dataBitArray = ByteArrayToBitArray(dataByteArray);
                Log.Debug("Original bit array:" +BitArrayToString(dataBitArray));
                int[] realKey = GenerateKey(dataBitArray.Count, key);
                StringBuilder result = new StringBuilder();

                if (reverse)
                {
                    Array.Reverse(realKey);
                }

                BitArray resultBitArray = new BitArray(dataBitArray.Length + dataBitArray.Length % mathLog);

                for (int i = 0; i < realKey.Length; i++)
                {

                    resultBitArray[i] = dataBitArray[realKey[i]];





                }

                Log.Debug("Encrypted bit array:" + BitArrayToString(resultBitArray));


                int offset = 0;
                char[] binaryCharArray = new char[mathLog];
                for (int i = 0; i < resultBitArray.Length; i++)
                {
                    
                    binaryCharArray[offset] = resultBitArray[i]?'1':'0';

                    if (offset+1 == binaryCharArray.Length)
                    {

                        int number = Convert.ToInt32(new string(binaryCharArray), 2);
                        result.Append(dictionary[number]);
                        offset = 0;
                    }
                    else
                    {
                        offset++;
                    }
                    

                }

                return result.ToString();

            }
            else
            {
                Log.Debug("Get original binary.");
                StringBuilder originalBinaryStringBuild = new StringBuilder();
                for(int x = 0; x < data.Length; x++)
                {

                   
                            string temp = Convert.ToString(dictionary.IndexOf(data[x]),2);

                            int offset = mathLog - temp.Length;

                            while (offset > 0)
                            {
                                originalBinaryStringBuild.Append('0');
                        offset--;
                            }
                            originalBinaryStringBuild.Append(temp);
                        


                    
                }
                //Log.Debug("Original binary string:"+originalBinaryStringBuild);

                BitArray originalBitArray = new BitArray(originalBinaryStringBuild.Length+  originalBinaryStringBuild.Length%mathLog);

                for (int i = 0; i < originalBitArray.Length; i++)
                {
                    if (originalBinaryStringBuild[i] == '1')
                    {
                        originalBitArray[i] = true;
                    }
                }
                Log.Debug("Original bit array:" + BitArrayToString(originalBitArray));

                int[] realKey = GenerateKey(originalBitArray.Length, key);

                if (reverse)
                {
                    Array.Reverse(realKey);
                }

                BitArray newBitArray = new BitArray(originalBitArray.Length);

                


                for (int i = 0; i < realKey.Length; i++)
                {

                    newBitArray[realKey[i]] = originalBitArray[i];
                    //Log.Debug("Index:" + realKey[i] + "=" + originalBitArray[i] + ".");


                }
                Log.Debug("Decrypt bit array:" + BitArrayToString(newBitArray));
                byte[] byteArray = BitArrayToByteArray(newBitArray);
                Log.Debug("Bit array convert to byte array:"+ArrayToString<byte>(byteArray));
                string result = ByteArrayToString(byteArray);


                return result;

            }

            

           

        }



        public static byte[] StringToByteArray(string text)
        {

            //byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(text);
            //sbyte[] result = new sbyte[byteArray.Length];
            //Buffer.BlockCopy(byteArray, 0, result, 0, result.Length);

            //return result;
            return System.Text.Encoding.UTF8.GetBytes(text);
        }

        public static string ByteArrayToString(byte[] array)
        {
            //byte[] byteArray = new byte[array.Length];
            //Buffer.BlockCopy(array, 0, byteArray, 0, byteArray.Length);
            //return System.Text.Encoding.UTF8.GetString(byteArray);
            return System.Text.Encoding.UTF8.GetString(array);
        }

        public static BitArray ByteArrayToBitArray(byte[] array)
        {
            return new BitArray(array);
        }

        public static byte[] BitArrayToByteArray(BitArray array)
        {
            byte[] result = new byte[(array.Length - 1) / 8 + 1];
            array.CopyTo(result,0);
            return result;
        }

        private static string ArrayToString<T>(T[] array)
        {

            StringBuilder strBuild = new StringBuilder();

            strBuild.Append("[");


            for(int i = 0; i < array.Length; i++)
            {
                if (i + 1 != array.Length)
                {
                    strBuild.Append(i);
                    strBuild.Append('=');
                    strBuild.Append(array[i].ToString());
                    strBuild.Append(',');
                }
                else
                {
                    strBuild.Append(i);
                    strBuild.Append('=');
                    strBuild.Append(array[i].ToString());
                    strBuild.Append(']');
                }
                
            }


            return strBuild.ToString();
        }

        private static string BitArrayToString(BitArray array)
        {

            StringBuilder strBuild = new StringBuilder();

            strBuild.Append("[");


            for (int i = 0; i < array.Length; i++)
            {
                if (i + 1 != array.Length)
                {
                    strBuild.Append(array[i]?1:0);
                    //strBuild.Append(i);
                    //strBuild.Append('=');
                    //strBuild.Append(array[i].ToString());
                    //strBuild.Append(',');
                }
                else
                {
                    strBuild.Append(array[i] ? 1 : 0);
                    //strBuild.Append(i);
                    //strBuild.Append('=');
                    //strBuild.Append(array[i].ToString());
                    strBuild.Append(']');
                }

            }


            return strBuild.ToString();
        }

        public static int[] GenerateKey(int length,string key)
        {

            Log.Debug("Generate Key:[Original:"+key+",length:"+length+"]");

            int[] list = new int[length];
            
            for(int i = 0; i < list.Length; i++)
            {
                //Log.Debug("Generate Index:"+i);
                list[i] = Math.Abs(i % 2 == 0 ? key[i%key.Length] + key[Math.Abs(key.Length + i)%key.Length] + i: key[i%key.Length] - key[Math.Abs(key.Length - i)%key.Length] - i) * i % length;
            }
            Log.Debug("Generate list:"+ArrayToString<int>(list));

            int[] sortList = (int[])list.Clone();

            Array.Sort(sortList);
            Log.Debug("Sorted list:" + ArrayToString<int>(sortList));
            int[] keyList = new int[length];
            bool[] flagList = new bool[length];

            for(int x = 0; x < list.Length; x++)
            {
                for(int y = 0; y < sortList.Length; y++)
                {
                    if (list[x] == sortList[y]&&!flagList[y])
                    {
                        flagList[y] = true;
                        keyList[x] = y;
                        break;
                    }
                }
            }






            Log.Debug("Result list:" + ArrayToString<int>(keyList));



            return keyList;
        }

    }


    



}
