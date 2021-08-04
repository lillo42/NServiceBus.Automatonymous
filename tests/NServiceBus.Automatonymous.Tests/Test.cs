using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace NServiceBus.Automatonymous.Tests
{
    public class Test : BaseTest
    {
        [Fact]
        public async Task A()
        {
            await GenerateMapperAsync(await File.ReadAllTextAsync("../../../OrderStateMachine.cs"));
        }
    }
}