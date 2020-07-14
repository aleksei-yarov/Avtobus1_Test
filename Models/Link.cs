using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Test_Bus.Models
{
    public class Link
    {
        public int Id { get; set; }
        [Required]
        [Url]
        public string LongUrl { get; set; }
        public string ShortUrl { get; set; }
        public DateTime DateCreate { get; set; } = DateTime.Now;
        public int Count { get; set; } = 0;        
    }
}
