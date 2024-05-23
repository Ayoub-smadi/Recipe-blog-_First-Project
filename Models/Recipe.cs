using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Recipe__MVCProject.Models;

public partial class Recipe
{
    public decimal Recipeid { get; set; }

    public decimal? Userid { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Ingredients { get; set; }

    public string? Instructions { get; set; }

    public decimal? Categoryid { get; set; }

    public DateTime? Addedtime { get; set; }

    public string? Mainimage { get; set; }
    [NotMapped]
    public IFormFile? ImageFile { get; set; }

    public decimal? Price { get; set; }

    public RecipeStatus Status { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual User? User { get; set; }

    public enum RecipeStatus
    {
        Pending, // default status when a recipe is added
        Approved,
        Rejected
    }
}
