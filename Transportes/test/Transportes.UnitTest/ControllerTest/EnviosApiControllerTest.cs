using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Net;
using Transportes.APIs;
using Transportes.Controllers;
using Transportes.Models;
using Xunit;

namespace Transportes.UnitTest.ApiControllerTest
{
    public class EnviosApiControllerTest
    {
        private readonly Mock<TransportesContext> _dbContextMock;
        private readonly EnviosApiController _controller;

        public EnviosApiControllerTest()
        {
            _dbContextMock = new Mock<TransportesContext>();
            _controller = new EnviosApiController(_dbContextMock.Object);
        }

        [Fact]
        public async Task GetEnvios_WithEnvios_ReturnsOkResult()
        {
            // Arrange
            var envios = new List<Envio> { new Envio { NumeroGuia = "123" } };
            var enviosQueryable = envios.AsQueryable();
            var enviosDbSetMock = new Mock<DbSet<Envio>>();
            enviosDbSetMock.As<IQueryable<Envio>>().Setup(e => e.Provider).Returns(enviosQueryable.Provider);
            enviosDbSetMock.As<IQueryable<Envio>>().Setup(e => e.Expression).Returns(enviosQueryable.Expression);
            enviosDbSetMock.As<IQueryable<Envio>>().Setup(e => e.ElementType).Returns(enviosQueryable.ElementType);
            enviosDbSetMock.As<IQueryable<Envio>>().Setup(e => e.GetEnumerator()).Returns(() => enviosQueryable.GetEnumerator());
            _dbContextMock.Setup(db => db.Envios).Returns(enviosDbSetMock.Object);

            // Act
            var result = _controller.GetEnvios();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;
            var returnedEnvios = (IEnumerable<Envio>)okResult.Value;
            Assert.Single(returnedEnvios);
        }

        // Add more tests for other methods in the EnviosApiController class
        // ...

        [Fact]
        public async Task Delete_WithExistingEnvio_ReturnsOkResult()
        {
            // Arrange
            var envio = new Envio { NumeroGuia = "123" };
            var enviosDbSetMock = new Mock<DbSet<Envio>>();
            enviosDbSetMock.Setup(dbSet => dbSet.FindAsync(envio.NumeroGuia)).ReturnsAsync(envio);
            _dbContextMock.Setup(db => db.Envios).Returns(enviosDbSetMock.Object);

            // Act
            var result = await _controller.Delete(envio.NumeroGuia);

            // Assert
            Assert.IsType<OkResult>(result);
        }


    }
}