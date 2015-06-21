using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using NLog;

namespace NimbleBluetoothImpedanceManager
{
    public static class ChunkParser
    {


        private static Logger logger = LogManager.GetCurrentClassLogger();
        static Regex regex_ok_start = new Regex(@"\AOK\+[A-Za-z:0-9]+(?=OK|\s|$)");
        static Regex regex_randomAtStart = new Regex(@"[A-Za-z:0-9\s.|(){}]+(?=OK|\s|$)");
        public static List<string> ParseChunk(string chunk)
        {
            logger.Debug("Parsing chunk: {0}", chunk);
            List<string> tokens = new List<string>();

            
            chunk = chunk.Trim(new char[]{'\r','\n','\0'});
            string originalChunck = chunk;

            while (chunk.Length > 0)
            {
                if (chunk.StartsWith("OK+"))
                {
                    var match = regex_ok_start.Match(chunk);
                    if (match.Success)
                    {
                        string m = match.Groups[0].Value;
                        tokens.Add(m);
                        chunk = chunk.Remove(0, m.Length);
                    }
                    else
                    {
                        throw new ArgumentException("regex problem");
                    }
                }
                else if (chunk.StartsWith("OK"))
                {
                    //is just an ok
                    //logger.Debug("Parsed: 'OK'");
                    chunk=chunk.Remove(0, 2);
                    tokens.Add("OK");
                }
                else if (chunk.StartsWith("AT+"))
                {
                    string temptoken = "AT+";
                    var match = regex_randomAtStart.Match(chunk);
                    if (match.Success)
                    {
                        string m = match.Groups[0].Value;
                        temptoken = temptoken + m;
                        tokens.Add(temptoken);
                        chunk = chunk.Remove(0, temptoken.Length);
                    }
                    else
                    {
                        throw new ArgumentException("regex problem");
                    }
                }
                else
                {
                    var match = regex_randomAtStart.Match(chunk);
                    if (match.Success)
                    {
                        string m = match.Groups[0].Value;
                        tokens.Add(m);
                        chunk = chunk.Remove(0, m.Length);
                    }
                    else
                    {
                        throw new ArgumentException("regex problem");
                    }
                }
            }

            StringBuilder reassembledChunk = new StringBuilder();
            foreach (string token in tokens)
            {
                logger.Debug("Got token: {0}", token);
                reassembledChunk.Append(token);
            }
            if (reassembledChunk.ToString().Trim() != originalChunck.Trim())
            {
                string msg = string.Format(
                    "Tokens extracted from chunk did not reassemble into original chunk: {0}", originalChunck);
                logger.Error(msg);
                throw new ApplicationException(msg);
            }


            logger.Debug("finished parseing chunk");
            return tokens;
        }
    }
}