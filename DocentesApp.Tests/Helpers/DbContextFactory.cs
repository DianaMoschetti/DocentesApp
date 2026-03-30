using DocentesApp.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DocentesApp.Tests.Helpers
{
    public static class DbContextFactory
    {
        public static DocentesDbContext Create()
        {
            var options = new DbContextOptionsBuilder<DocentesDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new DocentesDbContext(options);
        }
    }
}
