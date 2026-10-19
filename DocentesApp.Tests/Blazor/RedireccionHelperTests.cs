extern alias BlazorUI;

using AwesomeAssertions;
using BlazorUI::DocentesApp.Blazor.UI.Helpers;

namespace DocentesApp.Tests.Blazor
{
    public class RedireccionHelperTests
    {
        [Theory]
        [InlineData("/")]
        [InlineData("/docentes")]
        [InlineData("/cambiar-password?cambiada=true")]
        public void EsUrlLocal_RutaLocal_True(string url)
        {
            RedireccionHelper.EsUrlLocal(url).Should().BeTrue();
        }

        [Theory]
        [InlineData("//google.com")]
        [InlineData("/\\google.com")]
        [InlineData("https://google.com")]
        [InlineData("javascript:alert(1)")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("/\t/google.com")]
        public void EsUrlLocal_NoLocal_False(string? url)
        {
            RedireccionHelper.EsUrlLocal(url).Should().BeFalse();
        }
    }
}
