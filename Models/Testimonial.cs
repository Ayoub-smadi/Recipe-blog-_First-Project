using System;
using System.Collections.Generic;

namespace Recipe__MVCProject.Models;

public partial class Testimonial
{
    public decimal Testimonialid { get; set; }


    public decimal? Userid { get; set; }

    public decimal? Recipeid { get; set; }

    public string? Testimonialcontent { get; set; }

    public DateTime? Testimonialdate { get; set; }

    public virtual Recipe? Recipe { get; set; }

    public virtual User? User { get; set; }
}
