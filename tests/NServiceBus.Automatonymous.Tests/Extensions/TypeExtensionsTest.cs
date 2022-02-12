using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FluentAssertions;
using NServiceBus.Automatonymous.Extensions;
using Xunit;
using static Xunit.Assert;

namespace NServiceBus.Automatonymous.Tests.Extensions;

public class TypeExtensionsTest
{
    [Fact]
    public void ClosesType_Should_Throw_When_TypeIsNull() 
        => Throws<ArgumentNullException>(() => NServiceBus.Automatonymous.Extensions.TypeExtensions.ClosesType(null!, null!, out _));
    
    [Fact]
    public void ClosesType_Should_Throw_When_OpenTypeIsNull() 
        => Throws<ArgumentNullException>(() => typeof(object).ClosesType(null!, out _));
    
    [Fact]
    public void ClosesType_Should_Throw_When_OpenTypeIsNotGeneric() 
        => Throws<ArgumentException>(() => typeof(object).ClosesType(typeof(object), out _));
    
    [Fact]
    public void ClosesType_Should_ReturnFalse_When_IsNotCloseType() 
        => typeof(object).ClosesType(typeof(List<>), out _).Should().BeFalse();
    
    [Fact]
    public void ClosesType_Should_ReturnFalse_When_IsNotInterfaceCloseType() 
        => typeof(object).ClosesType(typeof(IList<>), out _).Should().BeFalse(); 
}