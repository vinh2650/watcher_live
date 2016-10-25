using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface.SearchModel
{
    public class esResultModel<T> where T : class
    {
        public eshits<T> hits { get; set; }
    }

    public class eshits<T> where T : class
    {
        public eshits()
        {
            hits = new List<esData<T>>();
        }

        public List<esData<T>> hits { get; set; }

        public int total { get; set; }
    }

    public class esData<T> where T : class
    {
        public T _source { get; set; }
    }

    public class esUserData
    {
        public string id { get; set; }

        public string firstname { get; set; }

        public string lastname { get; set; }

        public string email { get; set; }

        public string username { get; set; }

        public string phone { get; set; }
    }
}
