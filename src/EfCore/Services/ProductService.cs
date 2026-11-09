using EfCore.Dtos;
using EfCore.Models;
using EfCore.Repositories;

namespace EfCore.Services;

public sealed class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
      _repository = repository;
    }

    // 1. READ ALL
    public async Task<IReadOnlyList<ProductResponse>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var products = await _repository.GetAllAsync(
            cancellationToken);

        return products
            .Select(ProductResponse.FromModel)
            .ToArray();
    }

    // 2. READ DETAIL
    public async Task<ProductResponse?> GetByCodeAsync(
        string productCode,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(productCode)
            || productCode.Trim().Length > 30)
        {
            return null;
        }

        var product = await _repository.GetByCodeAsync(
            productCode,
            cancellationToken);

        return product is null
            ? null
            : ProductResponse.FromModel(product);
    }

    // 3. CREATE
    public async Task<ProductResponse?> CreateAsync(
        ProductRequest request,
        CancellationToken cancellationToken)
    {
        var product = new Product(
            request.Code,
            request.Name,
            request.Price,
            request.Stock,
            request.Category,
						request.IsActive,
						request.DeactivatedAtUtc
        );

        var created = await _repository.TryAddAsync(
            product,
            cancellationToken);

        return created
            ? ProductResponse.FromModel(product)
            : null;
    }

    // 4. UPDATE
    public async Task<ProductResponse?> UpdateAsync(
        string productCode,
        UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var product = new Product(
            productCode,
            request.Name,
            request.Price,
            request.Stock,
            request.Category,
						request.IsActive,
						request.DeactivatedAtUtc
					);

        var updated = await _repository.TryUpdateAsync(
            product,
            cancellationToken);

        return updated
            ? ProductResponse.FromModel(product)
            : null;
    }

    // 5. DELETE
    public Task<bool> DeleteAsync(
        string productCode,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(productCode)
            || productCode.Trim().Length > 30)
        {
            return Task.FromResult(false);
        }

        return _repository.TryDeleteAsync(
            productCode,
            cancellationToken);
    }

  public Task<bool> ActivateAsync(string code, CancellationToken cancellationToken)
  {
		if (string.IsNullOrWhiteSpace(code) || code.Trim().Length > 30)
		{
			return Task.FromResult(false);
		}

		return _repository.TryActivateAsync(code, cancellationToken);
  }

  public Task<bool> DeactivateAsync(string code, CancellationToken cancellationToken)
  {
    if (string.IsNullOrWhiteSpace(code) || code.Trim().Length > 30)
		{
			return Task.FromResult(false);
		}

		return _repository.TryDeactivateAsync(code, cancellationToken);
  }
}
public interface IProductService
{
    Task<IReadOnlyList<ProductResponse>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<ProductResponse?> GetByCodeAsync(
        string productCode,
        CancellationToken cancellationToken);

    Task<ProductResponse?> CreateAsync(
        ProductRequest request,
        CancellationToken cancellationToken);

    Task<ProductResponse?> UpdateAsync(
        string productCode,
        UpdateProductRequest request,
        CancellationToken cancellationToken);

    Task<bool> DeleteAsync(
        string productCode,
        CancellationToken cancellationToken);

    Task<bool> ActivateAsync(string code, CancellationToken cancellationToken);
    Task<bool> DeactivateAsync(string code, CancellationToken cancellationToken);
}