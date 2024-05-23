using System;
using System.Collections.Generic;

namespace Recipe__MVCProject.Models;

public partial class Payment
{
    public decimal Paymentid { get; set; }

    public decimal? Userid { get; set; }

    public decimal? Amount { get; set; }

    public string? Paymentstatus { get; set; }

    public DateTime? Transactiondate { get; set; }

    public decimal? Recipeid { get; set; }

    public string? Cardid { get; set; }

    public string? Cardholdername { get; set; }

    public string? Cardsecuritynumber { get; set; }

    public DateTime? Cardexpirydate { get; set; }

    public virtual Recipe? Recipe { get; set; }

    public virtual User? User { get; set; }
}
