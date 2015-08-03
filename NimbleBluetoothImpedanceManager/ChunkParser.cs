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
        static Regex regex_ErronousATPlus = new Regex(@"AT\+[A-Z]+(?=OK|\s|$)");
        static Regex regex_CommandResponse = new Regex(@"{([A-Za-z0-9]+):([ A-Z0-9a-z-:_|()\[\]]+)}");
        public static List<string> ParseChunk(string chunk)
        {
            logger.Debug("Parsing chunk of length {1}: {0}", chunk.EscapeWhiteSpace(), chunk.Length);
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
                    var match = regex_ErronousATPlus.Match(chunk);
                    if (match.Success)
                    {
                        string m = match.Groups[0].Value;
                        temptoken =  m;
                        tokens.Add(temptoken);
                        chunk = chunk.Remove(0, temptoken.Length);
                    }
                    else
                    {
                        throw new ArgumentException("regex problem");
                    }
                }
                else if (chunk.StartsWith("{"))
                {
                  
                    var match = regex_CommandResponse.Match(chunk);
                    if (match.Success)
                    {
                        string m = match.Groups[0].Value;
                        tokens.Add(m);
                        chunk = chunk.Remove(0, m.Length);
                    }
                    else
                    {
                        logger.Debug("Bad chunk: {0}", chunk.EscapeWhiteSpace());
                        chunk = "";
                        //throw new ArgumentException("regex problem");
                    }
                }
                else
                {
                    chunk = chunk.Remove(0, 1);
                    //var match = regex_ErronousATPlus.Match(chunk);
                    //if (match.Success)
                    //{
                    //    string m = match.Groups[0].Value;
                    //    //tokens.Add(m);
                    //    chunk = chunk.Remove(0, m.Length);
                    //}
                    //else
                    //{
                    //    throw new ArgumentException("regex problem");
                    //}
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
                logger.Debug(msg);
                //throw new ApplicationException(msg);
            }


            logger.Debug("finished parsing chunk");
            return tokens;
        }
    }
}