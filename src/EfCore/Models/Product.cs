namespace EfCore.Models;

public class Product(string code, string name, decimal price, int stock, string category, bool isActive = true, DateTime? deactivatedAtUtc = null)
{
  public string Code {get; private set;} = code.Trim().ToUpperInvariant();
  public string Name {get; private set;} = name;
  public decimal Price {get; private set;} = price;
  public int Stock {get; private set;} = stock;
  public string Category{get; private set;} = category;
  public bool IsActive {get; private set;} = isActive;
  public DateTime? DeactivatedAtUtc {get; private set;} = deactivatedAtUtc;

  public void UpdateDetail(string name, decimal price, int stock, string category, bool isActive, DateTime? deactivatedTime)
  {
    var validated = new Product(Code, name, price, stock, category, isActive, deactivatedTime);
    
    Name = validated.Name;
    Price = validated.Price;
    Stock = validated.Stock;
    Category = validated.Category;
    IsActive = validated.IsActive;
    DeactivatedAtUtc = validated.DeactivatedAtUtc;
  }

  public void Activate(bool isActive)
  {
    IsActive = isActive;
    if (isActive)
    {
      DeactivatedAtUtc = null;
    }
  }

  public void Deactivate(bool isActive)
  {
    IsActive = isActive;
    if (isActive)
    {
      DeactivatedAtUtc = DateTime.UtcNow;
      IsActive = false;
    }
    else
    {
      DeactivatedAtUtc = null;
    }
  }
}