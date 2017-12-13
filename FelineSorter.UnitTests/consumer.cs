using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using AutoFixture;
using AutoFixture.AutoMoq;
using FelineSorter.Components;
using FelineSorter.UnitTests.FixtureHelpers;
using FelineSorter.WebserviceContract;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace FelineSorter.UnitTests
{
    public class consumer
    {
        private readonly Fixture _fixture;

        private readonly MockRepository _mockRepository;

        private readonly Mock<ILogger<Consumer>> _loggerMock;
        private readonly Mock<IHttpClient> _httpClientMock;
        private readonly Mock<IOwnerSorter> _ownerSorterMock;
        private readonly Mock<IConsoleWriter> _consoleWriteMock;

        public consumer()
        {
            _fixture = new Fixture();
            _fixture.Customize(new AutoConfiguredMoqCustomization());

            _mockRepository = new MockRepository(MockBehavior.Default);
            _loggerMock = _mockRepository.Create<ILogger<Consumer>>();
            _httpClientMock = _mockRepository.Create<IHttpClient>();
            _ownerSorterMock = _mockRepository.Create<IOwnerSorter>();
            _consoleWriteMock = _mockRepository.Create<IConsoleWriter>();
        }

        private void InjectParameters(ILogger<Consumer> logger, WebserviceOptions webserviceOptions, Func<Uri, IHttpClient> httpClientFactory, IOwnerSorter felineOwnerSorter, IConsoleWriter consoleWriter)
        {
            _fixture.Inject(logger);
            _fixture.Inject(webserviceOptions);
            _fixture.Inject(httpClientFactory);
            _fixture.Inject(felineOwnerSorter);
            _fixture.Inject(consoleWriter);
        }

        [Fact]
        public void ctor_should_throw_if_no_logger_provided()
        {
            InjectParameters(null, new WebserviceOptions(), uri => _httpClientMock.Object, _ownerSorterMock.Object, _consoleWriteMock.Object);
            _fixture.Should().ThrowExceptionWhileCreating<ArgumentNullException, Consumer>($"Value cannot be null.{Environment.NewLine}Parameter name: logger");
        }

        [Fact]
        public void ctor_should_throw_if_no_webserviceoptions_provided()
        {
            InjectParameters(_loggerMock.Object, null, uri => _httpClientMock.Object, _ownerSorterMock.Object, _consoleWriteMock.Object);
            _fixture.Should().ThrowExceptionWhileCreating<ArgumentNullException, Consumer>($"Value cannot be null.{Environment.NewLine}Parameter name: webserviceOptions");
        }

        [Fact]
        public void ctor_should_throw_if_no_http_client_factory_provided()
        {
            InjectParameters(_loggerMock.Object, new WebserviceOptions(), null, _ownerSorterMock.Object, _consoleWriteMock.Object);
            _fixture.Should().ThrowExceptionWhileCreating<ArgumentNullException, Consumer>($"Value cannot be null.{Environment.NewLine}Parameter name: httpClientFactory");
        }

        [Fact]
        public void ctor_should_throw_if_no_feline_owner_sorter_provided()
        {
            InjectParameters(_loggerMock.Object, new WebserviceOptions(), uri => _httpClientMock.Object, null, _consoleWriteMock.Object);
            _fixture.Should().ThrowExceptionWhileCreating<ArgumentNullException, Consumer>($"Value cannot be null.{Environment.NewLine}Parameter name: felineOwnerSorter");
        }

        [Fact]
        public void ctor_should_throw_if_no_console_writer_provided()
        {
            InjectParameters(_loggerMock.Object, new WebserviceOptions(), uri => _httpClientMock.Object, _ownerSorterMock.Object, null);
            _fixture.Should().ThrowExceptionWhileCreating<ArgumentNullException, Consumer>($"Value cannot be null.{Environment.NewLine}Parameter name: consoleWriter");
        }

        [Fact]
        public async void consume_should_return_false_if_http_client_cannot_be_created()
        {
            InjectParameters(_loggerMock.Object, new WebserviceOptions(), uri => throw new Exception(), _ownerSorterMock.Object, _consoleWriteMock.Object);

            var consumer = _fixture.Create<Consumer>();
            bool result = await consumer.Consume();
            result.Should().BeFalse("because http client cannot be created");
            _httpClientMock.Verify(h => h.GetAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async void consume_should_return_false_if_http_client_get_throws_exception()
        {
            _httpClientMock.Setup(h => h.GetAsync(It.IsAny<string>())).Throws<Exception>();

            InjectParameters(_loggerMock.Object, new WebserviceOptions{EndpointBaseUrl = "https://www.google.com/", PeopleEndpoint = "people.json"}, uri => _httpClientMock.Object, _ownerSorterMock.Object, _consoleWriteMock.Object);

            var consumer = _fixture.Create<Consumer>();
            bool result = await consumer.Consume();
            result.Should().BeFalse("because http client get threw an exception");
            _httpClientMock.Verify(h => h.GetAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void consume_should_return_false_if_http_response_message_is_null()
        {
            _httpClientMock.Setup(h => h.GetAsync(It.IsAny<string>())).ReturnsAsync((HttpResponseMessage) null);

            InjectParameters(_loggerMock.Object, new WebserviceOptions { EndpointBaseUrl = "https://www.google.com/", PeopleEndpoint = "people.json" }, uri => _httpClientMock.Object, _ownerSorterMock.Object, _consoleWriteMock.Object);

            var consumer = _fixture.Create<Consumer>();
            bool result = await consumer.Consume();
            result.Should().BeFalse();
            _httpClientMock.Verify(h => h.GetAsync(It.IsAny<string>()), Times.Once);
            _loggerMock.Verify(x => x.Log(LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<FormattedLogValues>(y => y.ToString() == "Response message from endpoint was invalid"),
                It.IsAny<Exception>(),
                It.IsAny<Func<object, Exception, string>>()
            ), Times.Once);
        }

        [Theory]
        [InlineData(HttpStatusCode.BadGateway)]
        [InlineData(HttpStatusCode.InternalServerError)]
        public async void consume_should_return_false_if_http_response_message_has_non_success_status_code(HttpStatusCode httpStatusCode)
        {
            _httpClientMock.Setup(h => h.GetAsync(It.IsAny<string>())).ReturnsAsync(new HttpResponseMessage(httpStatusCode));

            InjectParameters(_loggerMock.Object, new WebserviceOptions { EndpointBaseUrl = "https://www.google.com/", PeopleEndpoint = "people.json" }, uri => _httpClientMock.Object, _ownerSorterMock.Object, _consoleWriteMock.Object);

            var consumer = _fixture.Create<Consumer>();
            bool result = await consumer.Consume();
            result.Should().BeFalse();
            _httpClientMock.Verify(h => h.GetAsync(It.IsAny<string>()), Times.Once);
            _loggerMock.Verify(x => x.Log(LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<FormattedLogValues>(y => y.ToString() == $"Response message from endpoint had an invalid Http status code of <{httpStatusCode}>"),
                It.IsAny<Exception>(),
                It.IsAny<Func<object, Exception, string>>()
            ), Times.Once);
        }

        [Fact]
        public async void consume_should_return_false_if_http_response_message_has_no_content()
        {
            _httpClientMock.Setup(h => h.GetAsync(It.IsAny<string>())).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = null
            });

            InjectParameters(_loggerMock.Object, new WebserviceOptions { EndpointBaseUrl = "https://www.google.com/", PeopleEndpoint = "people.json" }, uri => _httpClientMock.Object, _ownerSorterMock.Object, _consoleWriteMock.Object);

            var consumer = _fixture.Create<Consumer>();
            bool result = await consumer.Consume();
            result.Should().BeFalse();
            _httpClientMock.Verify(h => h.GetAsync(It.IsAny<string>()), Times.Once);
            _loggerMock.Verify(x => x.Log(LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<FormattedLogValues>(y => y.ToString() == "Response message from endpoint has null content"),
                It.IsAny<Exception>(),
                It.IsAny<Func<object, Exception, string>>()
            ), Times.Once);
        }

        [Fact]
        public async void consume_should_pass_owners_to_sorter_if_content_is_valid()
        {
            var owners = new []
            {
                new Owner(),
                new Owner()
            };
            _httpClientMock.Setup(h => h.GetAsync(It.IsAny<string>())).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(owners))
            });

            var ownerAndCats = new[]
            {
                new OwnerAndCats("male", new List<Pet>().OrderBy(p => p.Name)), 
                new OwnerAndCats("female", new List<Pet>().OrderBy(p => p.Name)),
            };
            _ownerSorterMock.Setup(o => o.Sort(It.IsAny<Owner[]>())).Returns(ownerAndCats);

            InjectParameters(_loggerMock.Object, new WebserviceOptions { EndpointBaseUrl = "https://www.google.com/", PeopleEndpoint = "people.json" }, uri => _httpClientMock.Object, _ownerSorterMock.Object, _consoleWriteMock.Object);

            var consumer = _fixture.Create<Consumer>();
            bool result = await consumer.Consume();
            result.Should().BeTrue();
            _httpClientMock.Verify(h => h.GetAsync(It.IsAny<string>()), Times.Once);
            _ownerSorterMock.Verify(o => o.Sort(It.Is<Owner[]>(os => os.Length == owners.Length)), Times.Once);

            // one for each group
            _consoleWriteMock.Verify(c => c.Write(It.IsAny<object>()), Times.Exactly(ownerAndCats.Length));
        }
    }
}
