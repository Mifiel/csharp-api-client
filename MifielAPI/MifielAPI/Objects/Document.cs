using System;
using System.Collections.Generic;

namespace MifielAPI.Objects
{
    public class Document
    {
        public string id;
        public string originalHash;
        public string fileName;
        public Boolean signedByAll;
        public Boolean signed;
        public string signedAt;
        public List<object> status;
        //public Owner owner;
        public string callbackUrl;
        public string file;
        public string fileDownload;
        public string fileSigned;
        public string fileSignedDownload;
        public string fileZipped;
        //public List<Signature> signatures;
    }
}
