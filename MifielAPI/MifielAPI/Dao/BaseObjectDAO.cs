using System.Collections.Generic;

namespace MifielAPI.Dao
{
    public abstract class BaseObjectDAO<T>
    {
        public ApiClient ApiClient { get; set; }

        public BaseObjectDAO(ApiClient apiClient)
        {
            ApiClient = apiClient;
        }

        public abstract T Find(string id);
        public abstract List<T> FindAll();
        public abstract T Save(T objectToSave);
        public abstract void Delete(string id); 
    }
}
