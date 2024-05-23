using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Recipe__MVCProject.Models;

public partial class Category
{
    public decimal Categoryid { get; set; }

    public string? Categoryname { get; set; }

    public string? Categoryimage { get; set; }
    [NotMapped]
    public IFormFile? ImageFile { get; set; }

    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
}
