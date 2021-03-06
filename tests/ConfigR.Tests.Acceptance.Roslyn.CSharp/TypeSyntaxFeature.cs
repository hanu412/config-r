﻿// <copyright file="TypeSyntaxFeature.cs" company="ConfigR contributors">
//  Copyright (c) ConfigR contributors. (configr.net@gmail.com)
// </copyright>

namespace ConfigR.Tests.Acceptance.Roslyn.CSharp
{
    using System;
    using System.Collections.Generic;
    using ConfigR.Tests.Acceptance.Roslyn.CSharp.Support;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

    public static class TypeSyntaxFeature
    {
        [Scenario]
        public static void UsingTypeSyntax(Foo foo)
        {
            "Given a local config file with a Bar of 'abc'"
                .f(c => ConfigFile.Create(@"Config.Bar = ""abc"";").Using(c));

            "When I load the config as Foo"
                .f(async () => foo = await new Config().UseRoslynCSharpLoader().Load<Foo>());

            "Then the Foo has a Bar of 'abc'"
                .f(() => foo.Bar.Should().Be("abc"));
        }

        [Scenario]
        public static void UsingTypeSyntaxWithAnObjectSeed(Foo foo)
        {
            "Given a local config file with a Bar of 'abc'"
                .f(c => ConfigFile.Create(@"Config.Bar = ""abc"";").Using(c));

            "When I load the config as Foo with an object seed with a Baz of 'def'"
                .f(async () => foo = await new Config().UseRoslynCSharpLoader().Load<Foo>(new { Baz = "def" }));

            "Then the Foo has a Bar of 'abc'"
                .f(() => foo.Bar.Should().Be("abc"));

            "And the Foo has a Baz of 'def'"
                .f(() => foo.Baz.Should().Be("def"));
        }

        [Scenario]
        public static void UsingTypeSyntaxWithADictionarSeed(Foo foo)
        {
            "Given a local config file with a Bar of 'abc'"
                .f(c => ConfigFile.Create(@"Config.Bar = ""abc"";").Using(c));

            "When I load the config as Foo with a dictionary seed with a Baz of 'def'"
                .f(async () =>
                {
                    var seed = new Dictionary<string, object>() { { "Baz", "def" } };
                    foo = await new Config().UseRoslynCSharpLoader().Load<Foo>(seed);
                });

            "Then the Foo has a Bar of 'abc'"
                .f(() => foo.Bar.Should().Be("abc"));

            "And the Foo has a Baz of 'def'"
                .f(() => foo.Baz.Should().Be("def"));
        }

        [Scenario]
        public static void UsingTypeSyntaxWithRedundantValues(Exception exception)
        {
            "Given a local config file with a Bazz of 'def'"
                .f(c => ConfigFile.Create(@"Config.Bazz = ""def""").Using(c));

            "When I load the config as Foo"
                .f(async () => exception = await Record.ExceptionAsync(async () => await new Config().UseRoslynCSharpLoader().Load<Foo>()));

            "Then no exception is thrown"
                .f(() => exception.Should().BeNull());
        }

        [Scenario]
        public static void UsingTypeSyntaxWithBadlyTypedValues(Exception exception)
        {
            "Given a local config file with a Bar of 42"
                .f(c => ConfigFile.Create(@"Config.Bar = 42;").Using(c));

            "When I load the config as Foo"
                .f(async () => exception = await Record.ExceptionAsync(async () => await new Config().UseRoslynCSharpLoader().Load<Foo>()));

            "Then an exception is thrown"
                .f(() => exception.Should().NotBeNull());

            "And the exception message contains 'Bar'"
                .f(() => exception.Message.Should().Contain("Bar"));
        }
    }
}
