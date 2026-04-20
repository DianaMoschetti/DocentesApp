using DocentesApp.Tests.Helpers;

namespace DocentesApp.Tests.Integration
{
    public abstract class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory>
    {
        protected readonly CustomWebApplicationFactory Factory;
        protected readonly HttpClient Client;
        protected readonly AuthHelper Auth;

        protected IntegrationTestBase(CustomWebApplicationFactory factory)
        {
            Factory = factory;
            Client = factory.CreateClient();
            Auth = new AuthHelper(factory);
        }
    }
}
