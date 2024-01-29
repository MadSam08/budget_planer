﻿namespace BudgetPlaner.Models.Domain;

public record SpendingEntity : BaseEntity
{
    public int CurrencyId { get; set; }
    
    public int CategoryId { get; set; }

    public string? Description { get; set; }
    
    public decimal Value { get; set; }
    
    /// <summary>
    /// Gets or sets the actual date of spending when it was received, not the date on it was inserted in the system.
    /// </summary>
    public DateTime ActualDateOfSpending { get; set; }
    
    public virtual CurrencyEntity? Currency { get; set; }
    
    public virtual CategoryEntity? Category { get; set; }
}
