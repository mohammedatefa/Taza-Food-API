using TazaFood_API.DTO;
using TazaFood_Core.Models;

namespace TazaFood_API.Helpers
{
    public class ProductPagination<T> 
    {
        public int pagesize { get; set; }
        public int pageindex { get; set; }
        public int count { get; set; }

        public IReadOnlyList<T>? data { get; set;}
        public ProductPagination(int pagesize, int pageindex, int count, IReadOnlyList<T>? data)
        {
            this.pagesize = pagesize;
            this.pageindex = pageindex;
            this.count = count;
            this.data = data;
        }

       


    }
}
