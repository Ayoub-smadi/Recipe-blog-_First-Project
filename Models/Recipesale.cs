using System;
using System.Collections.Generic;

namespace Recipe__MVCProject.Models
{
    public partial class Recipesale
    {
        public decimal Saleid { get; set; }
        public decimal? Userid { get; set; }
        public decimal? Recipeid { get; set; }
        public DateTime? Saledate { get; set; }
        public decimal? Amount { get; set; }

        public virtual Recipe? Recipe { get; set; }
        public virtual User? User { get; set; }
    }
}
