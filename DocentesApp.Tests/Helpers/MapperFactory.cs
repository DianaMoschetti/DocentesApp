

using DocentesApp.Application.Mappings;
using Mapster;
using MapsterMapper;

namespace DocentesApp.Tests.Helpers
{
    public static class MapperFactory
    {
        public static IMapper Create()
        {
            var config = new TypeAdapterConfig();
            MapsterConfig.Register(config);

            return new Mapper(config);
        }

    }
}
