using System.Net;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
namespace RoadStatus.Tests;

public class GetUKRoadStatusTests
{
    private readonly Mock<IConfiguration> _configuration = new();
    private readonly Mock<HttpMessageHandler> _handlerMock = new();
    private readonly HttpClient _httpClient;

    public GetUKRoadStatusTests(){
        _httpClient = new HttpClient(_handlerMock.Object);
    }

    [SetUp]
    public void Setup()
    {
        _configuration.SetupGet(x => x["RoadApiUrl"]).Returns("https://road-api.uk");
        _configuration.SetupGet(x => x["AppId"]).Returns("app-id");
        _configuration.SetupGet(x => x["AppKey"]).Returns("app-key");
        _handlerMock.Reset();
    }

    [OneTimeTearDown]
    public void TearDown(){
        _httpClient.Dispose();
    }
    
    #region Validations

    [Test]
    public async Task GivenNullRoadName_WhenGetLiveRoadStatusIsCalled_ThenReturnErrorMessage()
    {
        //Arrange
        var sut = new GetUKRoadStatus(_httpClient, _configuration.Object);

        //Act
        var result = await sut.GetLiveRoadStatus(null);

        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo("Please provide a non-empty road name"));
            Assert.That(sut.IsValid, Is.False);
        });
    }

    [Test]
    public async Task GivenWhiteSpaceRoadName_WhenGetLiveRoadStatusIsCalled_ThenReturnErrorMessage()
    {
        //Arrange
        var sut = new GetUKRoadStatus(_httpClient, _configuration.Object);

        //Act
        var result = await sut.GetLiveRoadStatus(" ");

        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo("Please provide a non-empty road name"));
            Assert.That(sut.IsValid, Is.False);
        });
    }

    [Test]
    public async Task GivenMissingRoadApiUrl_WhenGetLiveRoadStatusIsCalled_ThenReturnErrorMessage()
    {
        //Arrange
        _configuration.SetupGet(x => x["RoadApiUrl"]).Returns("");
        var sut = new GetUKRoadStatus(_httpClient, _configuration.Object);

        //Act
        var result = await sut.GetLiveRoadStatus("A2");

        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo("There's some problem getting the status of A2. Please try again later."));
            Assert.That(sut.IsValid, Is.False);
        });
    }

    [Test]
    public async Task GivenMissingAppId_WhenGetLiveRoadStatusIsCalled_ThenReturnErrorMessage()
    {
        //Arrange
        _configuration.SetupGet(x => x["AppId"]).Returns(" ");
        var sut = new GetUKRoadStatus(_httpClient, _configuration.Object);

        //Act
        var result = await sut.GetLiveRoadStatus("A2");

        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo("There's some problem getting the status of A2. Please try again later."));
            Assert.That(sut.IsValid, Is.False);
        });
    }

    [Test]
    public async Task GivenMissingAppKey_WhenGetLiveRoadStatusIsCalled_ThenReturnErrorMessage()
    {
        //Arrange
        _configuration.SetupGet(x => x["AppKey"]).Returns("");
        var sut = new GetUKRoadStatus(_httpClient, _configuration.Object);

        //Act
        var result = await sut.GetLiveRoadStatus("A2");

        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo("There's some problem getting the status of A2. Please try again later."));
            Assert.That(sut.IsValid, Is.False);
        });
    }
    
     [Test]
    public async Task GivenRoadApiUrlDoesNotExisInConfiguration_WhenGetLiveRoadStatusIsCalled_ThenReturnErrorMessage()
    {
        //Arrange
        _configuration.Reset();
        _configuration.SetupGet(x => x["AppId"]).Returns("app-id");
        _configuration.SetupGet(x => x["AppKey"]).Returns("app-key");
        var sut = new GetUKRoadStatus(_httpClient, _configuration.Object);

        //Act
        var result = await sut.GetLiveRoadStatus("A23");

        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo("There's some problem getting the status of A23. Please try again later."));
            Assert.That(sut.IsValid, Is.False);
        });
    }

    [Test]
    public async Task GivenAppIdDoesNotExisInConfiguration_WhenGetLiveRoadStatusIsCalled_ThenReturnErrorMessage()
    {
        //Arrange
        _configuration.Reset();
        _configuration.SetupGet(x => x["RoadApiUrl"]).Returns("https://road-api.uk");
        _configuration.SetupGet(x => x["AppKey"]).Returns("app-key");
        var sut = new GetUKRoadStatus(_httpClient, _configuration.Object);

        //Act
        var result = await sut.GetLiveRoadStatus("A23");

         //Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo("There's some problem getting the status of A23. Please try again later."));
            Assert.That(sut.IsValid, Is.False);
        });
    }

    [Test]
    public async Task GivenAppKeyDoesNotExisInConfiguration_WhenGetLiveRoadStatusIsCalled_ThenReturnErrorMessage()
    {
        //Arrange
        _configuration.Reset();
        _configuration.SetupGet(x => x["AppId"]).Returns("app-id");
        _configuration.SetupGet(x => x["RoadApiUrl"]).Returns("https://road-api.uk");
        var sut = new GetUKRoadStatus(_httpClient, _configuration.Object);

        //Act
        var result = await sut.GetLiveRoadStatus("A23");

         //Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo("There's some problem getting the status of A23. Please try again later."));
            Assert.That(sut.IsValid, Is.False);
        });
    }
    #endregion

    #region RequestResponse
    
    [Test]
    public async Task GivenValidRoad_WhenGetLiveRoadStatusIsCalled_ThenReturnAppropriateStatusMessage()
    {
        //Arrange
        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
        )
        .ReturnsAsync(new HttpResponseMessage{
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("[{\"$type\":\"Tfl.Api.Presentation.Entities.RoadCorridor, Tfl.Api.Presentation.Entities\",\"id\":\"a2\",\"displayName\":\"A2\",\"statusSeverity\":\"Good\",\"statusSeverityDescription\":\"No Exceptional Delays\",\"bounds\":\"[[-0.0857,51.44091],[0.17118,51.49438]]\",\"envelope\":\"[[-0.0857,51.44091],[-0.0857,51.49438],[0.17118,51.49438],[0.17118,51.44091],[-0.0857,51.44091]]\",\"url\":\"/Road/a2\"}]")
        })
        .Verifiable();

        var sut = new GetUKRoadStatus(_httpClient, _configuration.Object);

        //Act
        var result = await sut.GetLiveRoadStatus("A2");

        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo("The status of the A2 is as follows\n        Road Status is Good\n        Road Status Description is No Exceptional Delays"));
            Assert.That(sut.IsValid, Is.True);
        });
        _handlerMock.VerifyAll();
    }

    [Test]
    public async Task GivenValidRoad_WhenGetLiveRoadStatusIsCalledAndContentisUnexpected_ThenReturnDefaultStatusMessage()
    {
        //Arrange
        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
        )
        .ReturnsAsync(new HttpResponseMessage{
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("random")
        })
        .Verifiable();

        var sut = new GetUKRoadStatus(_httpClient, _configuration.Object);

        //Act
        var result = await sut.GetLiveRoadStatus("A2");

        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo("There's some problem getting the status of A2. Please try again later."));
            Assert.That(sut.IsValid, Is.False);
        });
        _handlerMock.VerifyAll();
    }

    [Test]
    public async Task GivenInValidRoad_WhenGetLiveRoadStatusIsCalledAndResponseIsNotFound_ThenReturnCustomErrorStatusMessage()
    {
        //Arrange
        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
        )
        .ReturnsAsync(new HttpResponseMessage{
            StatusCode = HttpStatusCode.NotFound,
            Content = new StringContent("{\"$type\":\"Tfl.Api.Presentation.Entities.ApiError, Tfl.Api.Presentation.Entities\",\"timestampUtc\":\"2024-05-04T19:13:35.4090142Z\",\"exceptionType\":\"EntityNotFoundException\",\"httpStatusCode\":404,\"httpStatus\":\"NotFound\",\"relativeUri\":\"/Road/A233?app_id=AppId&app_key=AppKey\",\"message\":\"The following road id is not recognised: A233\"}")
        })
        .Verifiable();

        var sut = new GetUKRoadStatus(_httpClient, _configuration.Object);

        //Act
        var result = await sut.GetLiveRoadStatus("A233");

        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo("A233 is not a valid road"));
            Assert.That(sut.IsValid, Is.False);
        });
        _handlerMock.VerifyAll();
    }

    [Test]
    public async Task GivenInValidRoad_WhenGetLiveRoadStatusIsCalledAndResponseIsOtherThanNotFound_ThenReturnMessageFromApiResponse()
    {
        //Arrange
        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
        )
        .ReturnsAsync(new HttpResponseMessage{
            StatusCode = HttpStatusCode.NotAcceptable,
            Content = new StringContent("{\"$type\":\"Tfl.Api.Presentation.Entities.ApiError, Tfl.Api.Presentation.Entities\",\"timestampUtc\":\"2024-05-04T19:13:35.4090142Z\",\"exceptionType\":\"EntityNotFoundException\",\"httpStatusCode\":404,\"httpStatus\":\"NotFound\",\"relativeUri\":\"/Road/A233?app_id=AppId&app_key=AppKey\",\"message\":\"The following road id is not recognised: A233\"}")
        })
        .Verifiable();

        var sut = new GetUKRoadStatus(_httpClient, _configuration.Object);

        //Act
        var result = await sut.GetLiveRoadStatus("A233");

        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo("The following road id is not recognised: A233"));
            Assert.That(sut.IsValid, Is.False);
        });
        _handlerMock.VerifyAll();
    }

    [Test]
    public async Task GivenInValidRoad_WhenGetLiveRoadStatusIsCalledAndResponseIsOtherThanNotFoundAndApiMessageIsEmpty_ThenReturnDefaultMessage()
    {
        //Arrange
        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
        )
        .ReturnsAsync(new HttpResponseMessage{
            StatusCode = HttpStatusCode.NotAcceptable,
            Content = new StringContent("{\"$type\":\"Tfl.Api.Presentation.Entities.ApiError, Tfl.Api.Presentation.Entities\",\"timestampUtc\":\"2024-05-04T19:13:35.4090142Z\",\"exceptionType\":\"EntityNotFoundException\",\"httpStatusCode\":404,\"httpStatus\":\"NotFound\",\"relativeUri\":\"/Road/A233?app_id=AppId&app_key=AppKey\",\"message\":\"\"}")
        })
        .Verifiable();

        var sut = new GetUKRoadStatus(_httpClient, _configuration.Object);

        //Act
        var result = await sut.GetLiveRoadStatus("A234");

        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo("There's some problem getting the status of A234. Please try again later."));
            Assert.That(sut.IsValid, Is.False);
        });
        _handlerMock.VerifyAll();
    }

     [Test]
    public async Task GivenInValidRoad_WhenGetLiveRoadStatusIsCalledAndResponseContentIsUnexpected_ThenReturnDefaultMessage()
    {
        //Arrange
        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
        )
        .ReturnsAsync(new HttpResponseMessage{
            StatusCode = HttpStatusCode.NotAcceptable,
            Content = new StringContent("{}")
        })
        .Verifiable();

        var sut = new GetUKRoadStatus(_httpClient, _configuration.Object);

        //Act
        var result = await sut.GetLiveRoadStatus("A234");

        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo("There's some problem getting the status of A234. Please try again later."));
            Assert.That(sut.IsValid, Is.False);
        });
        _handlerMock.VerifyAll();
    }
    #endregion
}