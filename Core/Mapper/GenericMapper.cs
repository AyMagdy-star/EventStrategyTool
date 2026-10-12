using System.Collections.Concurrent;
using System.Reflection;

namespace Core.Mapper;


public static class GenericMapper
{
    
    private static readonly ConcurrentDictionary<(Type Source, Type Destination), List<PropertyPair>> _cache =
        new ConcurrentDictionary<(Type Source, Type Destination), List<PropertyPair>>();

   
    public static TDestination Map<TSource, TDestination>(TSource source) where TDestination : new()
    {
        TDestination destination = new TDestination();
        MapInto(source, destination);
        return destination;
    }

   
    public static void MapInto<TSource, TDestination>(TSource source, TDestination destination)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (destination == null)
        {
            throw new ArgumentNullException(nameof(destination));
        }

        List<PropertyPair> pairs = GetPropertyPairs(source.GetType(), destination.GetType());

        foreach (PropertyPair pair in pairs)
        {
            object? value = pair.Source.GetValue(source);
            pair.Destination.SetValue(destination, value);
        }
    }

    public static List<TDestination> MapList<TSource, TDestination>(IEnumerable<TSource> sources) where TDestination : new()
    {
        List<TDestination> result = new List<TDestination>();

        foreach (TSource source in sources)
        {
            TDestination destination = Map<TSource, TDestination>(source);
            result.Add(destination);
        }

        return result;
    }

    private static List<PropertyPair> GetPropertyPairs(Type sourceType, Type destinationType)
    {
        (Type Source, Type Destination) key = (sourceType, destinationType);

        if (_cache.TryGetValue(key, out List<PropertyPair>? cachedPairs))
        {
            return cachedPairs;
        }

        List<PropertyPair> pairs = new List<PropertyPair>();
        PropertyInfo[] sourceProperties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (PropertyInfo sourceProperty in sourceProperties)
        {
            if (!sourceProperty.CanRead)
            {
                continue;
            }

            PropertyInfo? destinationProperty = destinationType.GetProperty(sourceProperty.Name, BindingFlags.Public | BindingFlags.Instance);
            if (destinationProperty == null || !destinationProperty.CanWrite)
            {
                continue;
            }

            
            bool typesMatch = destinationProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType);
            if (!typesMatch)
            {
                continue;
            }

            pairs.Add(new PropertyPair(sourceProperty, destinationProperty));
        }

        _cache.TryAdd(key, pairs);
        return pairs;
    }

    private sealed class PropertyPair
    {
        public PropertyPair(PropertyInfo source, PropertyInfo destination)
        {
            Source = source;
            Destination = destination;
        }

        public PropertyInfo Source { get; }
        public PropertyInfo Destination { get; }
    }
}