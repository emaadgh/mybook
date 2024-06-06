using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Moq;
using MyBook.API.Controllers;
using MyBook.API.Models;

namespace MyBook.Test.Controllers;

public class RootControllerTests
{
    [Fact]
    public void GetRoot_GetLinks_MustReturnsOkResult()
    {
        // Arrange
        RootController controller = new RootController();

        // Mocking IUrlHelper
        var expectedProtocol = "testprotocol://";
        var expectedHost = "www.example.com";

        var httpContext = new DefaultHttpContext
        {
            Request =
            {
                Scheme = expectedProtocol,
                Host = new HostString(expectedHost),
            }
        };

        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        var mockUrlHelper = CreateMockUrlHelper(actionContext);
        mockUrlHelper.Setup(h => h.RouteUrl(It.IsAny<UrlRouteContext>())).Returns("callbackUrl");
        mockUrlHelper.Setup(h => h.Action(It.IsAny<UrlActionContext>())).Returns("callbackUrl");

        controller.Url = mockUrlHelper.Object;

        // Act
        var result = controller.GetRoot();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var links = okResult.Value.Should().BeAssignableTo<List<LinkDto>>().Subject;
        links.Count.Should().Be(4);
    }

    private static Mock<IUrlHelper> CreateMockUrlHelper(ActionContext context)
    {
        if (context == null)
        {
            context = GetActionContextForPage("/Page");
        }

        var urlHelper = new Mock<IUrlHelper>();
        urlHelper.SetupGet(h => h.ActionContext)
            .Returns(context);
        return urlHelper;
    }

    private static ActionContext GetActionContextForPage(string page)
    {
        return new ActionContext
        {
            ActionDescriptor = new ActionDescriptor
            {
                RouteValues = new Dictionary<string, string?>
                {
                    { "page", page },
                },
            },
            RouteData = new RouteData
            {
                Values =
                {
                    [ "page" ] = page
                },
            },
        };
    }
}
