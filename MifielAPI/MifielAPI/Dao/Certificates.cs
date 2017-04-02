using MifielAPI.Objects;
using MifielAPI.Utils;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace MifielAPI.Dao
{
    public class Certificates : BaseObjectDAO<Certificate>
    {
        private string _certificatesPath = "keys";

        public Certificates(ApiClient apiClient) : base(apiClient) { }

        public override void Delete(string id)
        {
            ApiClient.Delete(_certificatesPath + "/" + id);
        }

        public override Certificate Find(string id)
        {
            string response = ApiClient.Get(_certificatesPath);
            return MifielUtils.ConvertJsonToObject<Certificate>(response);
        }

        public override List<Certificate> FindAll()
        {
            string response = ApiClient.Get(_certificatesPath);
            return MifielUtils.ConvertJsonToObject<List<Certificate>>(response);
        }

        public override Certificate Save(Certificate certificate)
        {
            HttpContent httpContent = BuildHttpBody(certificate);
            string response = ApiClient.Post(_certificatesPath, httpContent);
            return MifielUtils.ConvertJsonToObject<Certificate>(response);
        }

        private HttpContent BuildHttpBody(Certificate certificate)
        {
            throw new NotImplementedException();
        }
    }
}
