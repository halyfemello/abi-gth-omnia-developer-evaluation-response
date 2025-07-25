using MongoDB.Driver;
using MongoDB.Bson;
using DeveloperEvaluation.Core.Domain.Repositories;
using DeveloperEvaluation.Core.Domain.Common;
using DeveloperEvaluation.Infra.Configuration;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace DeveloperEvaluation.Infra.Repositories;

public class MongoRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly IMongoCollection<T> _collection;
    protected readonly MongoCollectionBuilder _collectionBuilder;

    public MongoRepository(IOptions<MongoDbSettings> mongoSettings, MongoCollectionBuilder collectionBuilder)
    {
        _collectionBuilder = collectionBuilder;

        var client = new MongoClient(mongoSettings.Value.ConnectionString);
        var database = client.GetDatabase(mongoSettings.Value.DatabaseName);

        var collectionName = _collectionBuilder.GetCollectionName<T>();
        _collection = database.GetCollection<T>(collectionName);
    }

    public virtual async Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
        return entity;
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<T>.Filter.Eq(e => e.Id, id);
        return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(int page = 1, int size = 10, CancellationToken cancellationToken = default)
    {
        var skip = (page - 1) * size;
        return await _collection.Find(_ => true)
            .Skip(skip)
            .Limit(size)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        var filter = Builders<T>.Filter.Eq(e => e.Id, entity.Id);
        await _collection.ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken);
        return entity;
    }

    public virtual async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<T>.Filter.Eq(e => e.Id, id);
        var result = await _collection.DeleteOneAsync(filter, cancellationToken);
        return result.DeletedCount > 0;
    }

    public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return (int)await _collection.CountDocumentsAsync(_ => true, cancellationToken: cancellationToken);
    }

    public virtual async Task<(IEnumerable<T> Items, long TotalCount)> GetPagedAsync(
        Expression<Func<T, bool>>? filter = null,
        string? orderBy = null,
        int page = 1,
        int size = 10,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        size = Math.Max(1, Math.Min(100, size));

        var mongoFilter = filter != null
            ? Builders<T>.Filter.Where(filter)
            : Builders<T>.Filter.Empty;

        var totalCount = await _collection.CountDocumentsAsync(mongoFilter, cancellationToken: cancellationToken);

        var query = _collection.Find(mongoFilter);

        if (!string.IsNullOrWhiteSpace(orderBy))
        {
            query = ApplyOrdering(query, orderBy);
        }

        var skip = (page - 1) * size;
        var items = await query
            .Skip(skip)
            .Limit(size)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public virtual async Task<long> CountAsync(Expression<Func<T, bool>>? filter, CancellationToken cancellationToken = default)
    {
        var mongoFilter = filter != null
            ? Builders<T>.Filter.Where(filter)
            : Builders<T>.Filter.Empty;

        return await _collection.CountDocumentsAsync(mongoFilter, cancellationToken: cancellationToken);
    }

    protected virtual IFindFluent<T, T> ApplyOrdering(IFindFluent<T, T> query, string orderBy)
    {
        if (string.IsNullOrWhiteSpace(orderBy))
            return query;

        var orderParts = orderBy.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(part => part.Trim())
            .ToList();

        if (!orderParts.Any())
            return query;

        var sortDefinitions = new List<SortDefinition<T>>();

        foreach (var orderPart in orderParts)
        {
            var parts = orderPart.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var fieldName = parts[0];
            var direction = parts.Length > 1 && parts[1].ToLowerInvariant() == "desc"
                ? SortDirection.Descending
                : SortDirection.Ascending;

            var fieldSort = direction == SortDirection.Ascending
                ? Builders<T>.Sort.Ascending(fieldName)
                : Builders<T>.Sort.Descending(fieldName);

            sortDefinitions.Add(fieldSort);
        }

        var combinedSort = sortDefinitions.Count == 1
            ? sortDefinitions[0]
            : Builders<T>.Sort.Combine(sortDefinitions.ToArray());

        return query.Sort(combinedSort);
    }
}
