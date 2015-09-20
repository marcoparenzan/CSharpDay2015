using System;
using System.Collections.Generic;

namespace TestBed
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var x = new Dictionary<string, string>
                {
                    ["10"] = "PRINT 'Hello'"
                    ,
                    ["20"] = "Good"
                };
            }
            catch (Exception ex) when (ex?.InnerException?.Message == "aaaaa")
            {
            }
        }
    }
}
