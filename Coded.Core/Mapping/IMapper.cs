namespace Coded.Core.Mapping
{
    /// <summary>
    ///     Maps an <typeparamref name="TSource" /> to an <typeparamref name="TDestination" />
    /// </summary>
    /// <typeparam name="TSource">The source type</typeparam>
    /// <typeparam name="TDestination">The destination type</typeparam>
    public interface IMapper<in TSource, out TDestination>
    {
        /// <summary>
        ///     Map the source object to te destination object
        /// </summary>
        /// <param name="sourceObject">The source object</param>
        /// <returns>A mapped <typeparamref name="TDestination" /> object</returns>
        TDestination Map(TSource sourceObject);
    }
}