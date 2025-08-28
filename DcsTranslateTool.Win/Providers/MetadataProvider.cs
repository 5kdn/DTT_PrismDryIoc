using System.Reflection;

using DcsTranslateTool.Win.Contracts.Providers;

namespace DcsTranslateTool.Win.Providers;
internal class MetadataProvider : IMetadataProvider {
    public string GetMetadata( string key ) {
        var assembly = Assembly.GetExecutingAssembly();
        IEnumerable<AssemblyMetadataAttribute> attributes = assembly.GetCustomAttributes<AssemblyMetadataAttribute>();
        var value = attributes.FirstOrDefault( attr => attr.Key == key )?.Value ?? throw new KeyNotFoundException();
        return value;
    }
}