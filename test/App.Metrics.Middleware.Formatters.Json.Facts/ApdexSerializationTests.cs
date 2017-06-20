// <copyright file="ApdexSerializationTests.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using System.Linq;
using App.Metrics.Apdex;
using App.Metrics.Formatters.Json.Serialization;
using App.Metrics.Middleware.Formatters.Json.Facts.Helpers;
using App.Metrics.Middleware.Formatters.Json.Facts.TestFixtures;
using FluentAssertions;
using FluentAssertions.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace App.Metrics.Middleware.Formatters.Json.Facts
{
    public class ApdexSerializationTests : IClassFixture<MetricProviderTestFixture>
    {
        private readonly ApdexValueSource _apdex;
        private readonly ITestOutputHelper _output;
        private readonly MetricDataSerializer _serializer;

        public ApdexSerializationTests(ITestOutputHelper output, MetricProviderTestFixture fixture)
        {
            _output = output;
            _serializer = new MetricDataSerializer();
            _apdex = fixture.ApdexScores.First();

            _apdex = fixture.ApdexScores.First(x => x.Name == fixture.ApdexNameDefault);
        }

        [Fact]
        public void Can_deserialize()
        {
            var jsonApdex = MetricType.Apdex.SampleJson();

            var result = _serializer.Deserialize<ApdexValueSource>(jsonApdex.ToString());

            result.Name.Should().BeEquivalentTo(_apdex.Name);
            result.Value.Score.Should().Be(_apdex.Value.Score);
            result.Value.SampleSize.Should().Be(_apdex.Value.SampleSize);
            result.Value.Satisfied.Should().Be(_apdex.Value.Satisfied);
            result.Value.Tolerating.Should().Be(_apdex.Value.Tolerating);
            result.Value.Frustrating.Should().Be(_apdex.Value.Frustrating);
            result.Tags.Keys.Should().Contain(_apdex.Tags.Keys.ToArray());
            result.Tags.Values.Should().Contain(_apdex.Tags.Values.ToArray());
        }

        [Fact]
        public void Produces_expected_json()
        {
            var expected = MetricType.Apdex.SampleJson();

            var result = _serializer.Serialize(_apdex).ParseAsJson();

            result.Should().Be(expected);
        }

        [Fact]
        public void Produces_valid_Json()
        {
            var json = _serializer.Serialize(_apdex);
            _output.WriteLine("Json Apdex Score: {0}", json);

            Action action = () => JToken.Parse(json);
            action.ShouldNotThrow<Exception>();
        }
    }
}