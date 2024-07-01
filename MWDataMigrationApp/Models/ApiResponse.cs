using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWDataMigrationApp.Models
{
    public class ApiResponse<T>
    {
        public string Endpoint { get; set; }
        public int Status { get; set; }
        public int Code { get; set; }
        public string Locale { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }
        public List<string> Warnings { get; set; }
        public List<T> Data { get; set; }
        public string CacheSession { get; set; }
    }
}
