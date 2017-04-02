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
            //"canonical-string"
            string appId = "1de27e16406cf9b485a1cd9b19a51015049fc245";
            string appSecret = "ul2NEPJyNKD/3MILk5K2IKFb45I7dsZvC21DkvOkNDyhVM0SzBUeeXsDloGoWoIoVBLpayIzTEvRg50YDvI5/A==";

            ApiClient api = new ApiClient(appId, appSecret);
            api.Url = "https://sandbox.mifiel.com";

            try
            {
                Documents docs = new Documents(api);

                Console.WriteLine("Get all documents...");
                var allDocs = docs.FindAll();
                foreach (var item in allDocs)
                {
                    Console.WriteLine("file name: " + item.FileName);
                }
                Console.WriteLine("Documents = " + allDocs.Count);

                Console.WriteLine("Saving document...");
                Document doc1 = new Document()
                {
                    //File = @"C:\Users\Jose Luis\Desktop\REBA sheet.pdf"
                    FileName = "test_file_name&",
                    OriginalHash = MifielUtils.GetDocunentHash(@"C:\Users\Jose Luis\Desktop\REBA sheet.pdf"),
                    Signatures = new List<Signature>()
                    {
                        new Signature()
                        {
                            Email = "enrique_test@test.com",
                            SignatureStr = "EnriqueAlonso",
                            TaxId = "RINE9110301Q3"
                        }
                    }
                };

                doc1 = docs.Save(doc1);
                doc1.CallbackUrl = "djfskjdsfs";

                doc1 = docs.Save(doc1);
                

                Console.WriteLine("Get all documents...");
                allDocs = docs.FindAll();
                Console.WriteLine("Documents = " + allDocs.Count);
            }
            catch (MifielAPI.Exceptions.MifielException e)
            {
                Console.WriteLine(e.MifielError.ToString());
            }
            
            Console.ReadLine();
        }
    }
}
