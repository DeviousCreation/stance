﻿using Stance.Domain.Events;
using Xunit;

namespace Stance.Tests.Domain.Events
{
    public class PasswordChangedEventTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var @event =
                new PasswordChangedEvent("email-address", "first-name", "last-name");

            Assert.Equal("email-address", @event.EmailAddress);
            Assert.Equal("first-name", @event.FirstName);
            Assert.Equal("last-name", @event.LastName);
        }
    }
}