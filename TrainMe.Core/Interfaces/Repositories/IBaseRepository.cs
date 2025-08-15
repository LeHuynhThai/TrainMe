namespace TrainMe.Core.Interfaces.Repositories;

/// <summary>
/// Base repository interface with common CRUD operations
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public interface IBaseRepository<T> where T : class
{
    /// <summary>
    /// Gets entity by ID
    /// </summary>
    Task<T?> GetByIdAsync(int id);

    /// <summary>
    /// Gets all entities
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Creates a new entity
    /// </summary>
    Task<T> CreateAsync(T entity);

    /// <summary>
    /// Updates an existing entity
    /// </summary>
    Task<T> UpdateAsync(T entity);

    /// <summary>
    /// Deletes an entity by ID
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// Checks if entity exists by ID
    /// </summary>
    Task<bool> ExistsAsync(int id);
}
