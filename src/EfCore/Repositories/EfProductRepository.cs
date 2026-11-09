using System.Data.Common;
using EfCore.Data;
using EfCore.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EfCore.Repositories;

public sealed class EfProductRepository : IProductRepository
{
  private readonly AppDbContext _db;
  public EfProductRepository(AppDbContext appDbContext)
  {
    _db = appDbContext;
  }

  public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken)
  {
    return await _db.Products.AsNoTracking().OrderBy(p => p.Code).ToListAsync(cancellationToken);  }

  public async Task<Product> GetByCodeAsync(string code, CancellationToken cancellationToken)
  {
    return await _db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Code == code, cancellationToken);
  }

  public async Task<bool> TryActivateAsync(string code, CancellationToken cancellationToken)
  {
    var exists = await _db.Products.FirstOrDefaultAsync(p => p.Code == code.Trim().ToUpperInvariant(), cancellationToken);
    if (exists is null)
    {
      return false;
    }
    exists.Activate(true);

    await _db.SaveChangesAsync(cancellationToken);
    return true;
  }

  public async Task<bool> TryAddAsync(Product product, CancellationToken cancellationToken)
  {
    _db.Products.Add(product);
    try
    {
      await _db.SaveChangesAsync(cancellationToken);
      return true;
    }
    catch (DbException e) when (e.InnerException is SqlException {Number: 2601 or 2627})
    {
      return false;
    }
  }

  public async Task<bool> TryDeactivateAsync(string code, CancellationToken cancellationToken)
  {
    var exists = await _db.Products.FirstOrDefaultAsync(p => p.Code == code.Trim().ToUpperInvariant());

    if(exists is null){
      return false;
    }

    exists.Deactivate(true);
    await _db.SaveChangesAsync(cancellationToken);

    return true;
  }

  public async Task<bool> TryDeleteAsync(string code, CancellationToken cancellationToken)
  {
    var existing = await _db.Products.FirstOrDefaultAsync(p => p.Code == code.Trim().ToUpperInvariant(), cancellationToken);
    if(existing is null)
    {
      return false;
    }

    _db.Products.Remove(existing);
    await _db.SaveChangesAsync(cancellationToken);

    return true;
  }

  public async Task<bool> TryUpdateAsync(Product product, CancellationToken cancellationToken)
  {
    var existing = await _db.Products.FirstOrDefaultAsync(p => p.Code == product.Code, cancellationToken);
    if(existing is null)
    {
      return false;
    }

    existing.UpdateDetail(
      product.Name,
      product.Price,
      product.Stock,
      product.Category,
      product.IsActive,
      product.DeactivatedAtUtc
    );

    await _db.SaveChangesAsync(cancellationToken);
    return true;
  }
}

public interface IProductRepository
{
  Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken);
  Task<Product> GetByCodeAsync(string code, CancellationToken cancellationToken);
  Task<bool> TryAddAsync(Product product, CancellationToken cancellationToken);
  Task<bool> TryUpdateAsync(Product product, CancellationToken cancellationToken);
  Task<bool> TryDeleteAsync(string code, CancellationToken cancellationToken);
  Task<bool> TryActivateAsync(string code, CancellationToken cancellationToken);
  Task<bool> TryDeactivateAsync(string code, CancellationToken cancellationToken);
}