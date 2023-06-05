using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Net;
using Transportes.Controllers;
using Transportes.Models;
using Xunit;

namespace Transportes.UnitTest.ApiControllerTest
{
    public class EnviosControllerTest
    {

       
       
        [Fact]
        public async void Details_WithInvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            var dbContextMock = new Mock<TransportesContext>();

            // Configurar el comportamiento deseado del DbContext mock
            // Aquí puedes configurar métodos como DbSet<T>.Add(), DbSet<T>.Remove(), etc.
            dbContextMock.Setup(db => db.Envios.Add(It.IsAny<Envio>())).Verifiable();
            dbContextMock.Setup(db => db.SaveChangesAsync(default)).ReturnsAsync(1);

            // Pasar el DbContext mock al constructor del controlador
            var controller = new EnviosController(dbContextMock.Object);

            // Act
            var result = await controller.Details(null, 1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

       
    }
}