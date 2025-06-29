using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagementApi.Core.Wrapper
{
    public class PaganationResponse<T>
    {
        public List<T> Data { get; set; } = [];
        public int TotalRecords { get; set; }
        public int TotalPages  => (int)Math.Ceiling((double) TotalRecords / PageSize);
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
    }
}
