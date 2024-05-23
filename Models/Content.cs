using System;
using System.Collections.Generic;

namespace Recipe__MVCProject.Models;

public partial class Content
{
    public decimal Contentid { get; set; }

    public string? Name { get; set; }

    public string? Key { get; set; }

    public string? Value { get; set; }
}
