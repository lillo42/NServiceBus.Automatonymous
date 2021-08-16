using AutoFixture;
using FluentAssertions;
using GreenPipes.Payloads;
using NServiceBus.ObjectBuilder;
using NSubstitute;
using Xunit;

namespace NServiceBus.Automatonymous.Tests
{
    public abstract class BuilderPayloadCacheTest<T>
        where T: class
    {
        private Fixture _fixture;
        private readonly IBuilder _builder;
        private readonly IConfigureComponents _configureComponents;
        private readonly ListPayloadCache _cache;
        private readonly BuilderPayloadCache _payloadCache;

        protected BuilderPayloadCacheTest()
        {
            _fixture = new Fixture();
            _builder = Substitute.For<IBuilder>();
            _configureComponents = Substitute.For<IConfigureComponents>();
            _cache = new ListPayloadCache();
            _payloadCache = new BuilderPayloadCache(_builder, _configureComponents, _cache);
        }

        #region HasPayloadType
        
        [Fact]
        public void HasPayloadType_Should_ReturnTrue_When_CacheHasTypeAndTheDiDoesNotHave()
        {
            _cache.GetOrAddPayload(() => _fixture.Create<T>());
            _payloadCache.HasPayloadType(typeof(T)).Should().BeTrue();
        }
        
        [Fact]
        public void HasPayloadType_Should_ReturnTrue_When_CacheDoesNotHaveTypeAndTheDiHas()
        {
            _configureComponents.HasComponent(typeof(T)).Returns(true);
            _payloadCache.HasPayloadType(typeof(T)).Should().BeTrue();
        }
        
        [Fact]
        public void HasPayloadType_Should_ReturnFalse_When_CacheAndTheDiDoesNotHaveType()
        {
            _payloadCache.HasPayloadType(typeof(string)).Should().BeFalse();
        }
        #endregion
        
        #region TryGetPayload
        
        [Fact]
        public void TryGetPayload_Should_ReturnTrue_When_CacheHasTypeAndTheDiDoesNotHave()
        {
            var expected = _fixture.Create<T>(); 
            _cache.GetOrAddPayload(() => expected);
            _payloadCache.TryGetPayload(out T result).Should().BeTrue();
            result.Should().Be(expected);
        }
        
        [Fact]
        public void TryGetPayload_Should_ReturnTrue_When_CacheDoesNotHaveTypeAndTheDiHas()
        {
            var expected = _fixture.Create<T>();
            _configureComponents.HasComponent(typeof(T)).Returns(true);
            _builder.Build<T>().Returns(expected);
            _payloadCache.TryGetPayload(out T result).Should().BeTrue();
            result.Should().Be(expected);
        }
        
        [Fact]
        public void TryGetPayload_Should_ReturnFalse_When_CacheAndTheDiDoesNotHaveType()
        {
            _payloadCache.TryGetPayload(out T result).Should().BeFalse();
            result.Should().BeNull();
        }
        #endregion
        
        #region GetOrAddPayload
        
        [Fact]
        public void GetOrAddPayload_Should_AddValue_When_CacheAndTheDiDoesNotHaveType()
        {
            var expected = _fixture.Create<T>();
            _payloadCache.GetOrAddPayload(() => expected).Should().Be(expected);
        }
        
        [Fact]
        public void GetOrAddPayload_Should_TheCacheValue_When_CacheHaveType()
        {
            var expected = _fixture.Create<T>();
            _cache.GetOrAddPayload(() => expected);
            _payloadCache.GetOrAddPayload(() => _fixture.Create<T>()).Should().Be(expected);
        }
        
        [Fact]
        public void GetOrAddPayload_Should_TheDiValue_When_DiHaveType()
        {
            _configureComponents.HasComponent<T>().Returns(true);
            var expected = _fixture.Create<T>();
            _builder.Build<T>().Returns(expected);
            _payloadCache.GetOrAddPayload(() => _fixture.Create<T>()).Should().Be(expected);
        }
        #endregion
        
        
        #region AddOrUpdatePayload
        
        [Fact]
        public void AddOrUpdatePayload_Should_AddValue_When_CacheAndTheDiDoesNotHaveType()
        {
            var expected = _fixture.Create<T>();
            _payloadCache.AddOrUpdatePayload(() => expected, old =>
            {
                Assert.False(true);
                return _fixture.Create<T>();
            }).Should().Be(expected);
        }
        
        [Fact]
        public void AddOrUpdatePayload_Should_UpdateValue_When_CacheHaveType()
        {
            var actual = _fixture.Create<T>();
            var expected = _fixture.Create<T>();
            _cache.GetOrAddPayload(() => actual);
            _payloadCache.AddOrUpdatePayload(() =>
                {
                    Assert.False(true);
                    return _fixture.Create<T>();
                },
                old =>
                {
                    actual.Should().Be(old);
                    return expected;
                }).Should().Be(expected);
        }
        #endregion
        
    }
    
    public class StringBuilderPayloadCacheTest : BuilderPayloadCacheTest<string>
    {
    }
    
    public class PersonBuilderPayloadCacheTest : BuilderPayloadCacheTest<PersonCache>
    {
    }
    
    public class PersonCache
    {
        public string Name { get; set; } = string.Empty;
    }
}