using MongoDB.Bson.Serialization;

namespace DeveloperEvaluation.Infra.Configuration;

public class MongoCollectionBuilder
{
    private readonly Dictionary<Type, string> _collections = new();
    private static readonly HashSet<Type> _registeredTypes = new();
    private static readonly object _lock = new();

    public MongoCollectionBuilder RegisterCollection<T>(string collectionName, Action<BsonClassMap<T>>? classMapConfig = null)
    {
        _collections[typeof(T)] = collectionName;

        if (classMapConfig != null)
        {
            lock (_lock)
            {
                if (!_registeredTypes.Contains(typeof(T)) && !BsonClassMap.IsClassMapRegistered(typeof(T)))
                {
                    var classMap = new BsonClassMap<T>();
                    classMap.AutoMap();
                    classMap.SetIgnoreExtraElements(true);

                    classMapConfig(classMap);

                    BsonClassMap.RegisterClassMap(classMap);
                    _registeredTypes.Add(typeof(T));
                }
            }
        }

        return this;
    }

    public string GetCollectionName<T>() => GetCollectionName(typeof(T));

    public string GetCollectionName(Type type)
    {
        if (_collections.TryGetValue(type, out var collectionName))
        {
            return collectionName;
        }

        return $"{type.Name}s";
    }

    public void ApplyClassMaps()
    {
        // Os class maps já foram aplicados no método RegisterCollection
        // Este método fica para compatibilidade, mas não faz nada
    }

    public IReadOnlyDictionary<Type, string> GetCollections() => _collections;
}
