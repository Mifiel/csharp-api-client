using MifielAPI;
using MifielAPI.Dao;
using MifielAPI.Objects;
using MifielAPI.Rest;
using MifielAPI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string appId = "1de27e16406cf9b485a1cd9b19a51015049fc245";
            string appSecret = "ul2NEPJyNKD/3MILk5K2IKFb45I7dsZvC21DkvOkNDyhVM0SzBUeeXsDloGoWoIoVBLpayIzTEvRg50YDvI5/A==";

            ApiClient api = new ApiClient(appId, appSecret);
            api.Url = "https://sandbox.mifiel.com";

            try
            {
                Documents docs = new Documents(api);
                Document doc = docs.Find("1");

                Console.WriteLine("Document = " + doc);
            }
            catch (MifielAPI.Exceptions.MifielException e)
            {
                Console.WriteLine("Exception: " + e);
            }
            
            Console.ReadLine();
        }
    }
}
