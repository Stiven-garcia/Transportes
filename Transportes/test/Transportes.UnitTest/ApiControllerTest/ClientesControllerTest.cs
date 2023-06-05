using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Net;
using Transportes.Controllers;
using Transportes.Models;
using Xunit;

namespace Transportes.UnitTest.ApiControllerTest
{
    public class ClientesControllerTest
    {
       

        [Fact]
        public void Search_WithEmptyFilter_RedirectsToIndex()
        {
            // Arrange
            var contextMock = new Mock<TransportesContext>();
            var controller = new ClientesController(contextMock.Object);

            // Act
            var result = controller.Search(string.Empty);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

       
        [Fact]
        public void CreateWithValidModelReturnsRedirectToIndex()
        {
            // Arrange
            var dbContextMock = new Mock<TransportesContext>();

            // Configurar el DbSet simulado
            var clientes = new List<Cliente>
            {
                new Cliente { Cedula = 1, Nombres = "Cliente 1" },
                new Cliente { Cedula = 2, Nombres = "Cliente 2" }
            }.AsQueryable();

            var dbSetMock = new Mock<DbSet<Cliente>>();
            dbSetMock.As<IQueryable<Cliente>>().Setup(m => m.Provider).Returns(clientes.Provider);
            dbSetMock.As<IQueryable<Cliente>>().Setup(m => m.Expression).Returns(clientes.Expression);
            dbSetMock.As<IQueryable<Cliente>>().Setup(m => m.ElementType).Returns(clientes.ElementType);
            dbSetMock.As<IQueryable<Cliente>>().Setup(m => m.GetEnumerator()).Returns(() => clientes.GetEnumerator());

            // Configurar el contexto simulado para devolver el DbSet simulado
            dbContextMock.Setup(m => m.Clientes).Returns(dbSetMock.Object);

            var controller = new ClientesController(dbContextMock.Object);
            var cliente = new Cliente { Cedula = 4, Nombres = "John", Apellidos = "Doe", Telefono = 123456789 };

            // Act
            var result = controller.Create(cliente).Result;

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public void Create_WithInvalidModel_ReturnsUnprocessableEntity()
        {
            // Arrange
            var contextMock = new Mock<TransportesContext>();
            var controller = new ClientesController(contextMock.Object);
            controller.ModelState.AddModelError("Nombres", "The Nombres field is required.");
            var cliente = new Cliente { Cedula = 1, Nombres = null, Apellidos = "Doe", Telefono = 123456789 };

            // Act
            var result = controller.Create(cliente).Result;

            // Assert
            var unprocessableEntityResult = Assert.IsType<UnprocessableEntityResult>(result);
            Assert.Equal(422, unprocessableEntityResult.StatusCode);
        }

       
        [Fact]
        public async Task Edit_WithInvalidCedula_ReturnsNotFound()
        {
            // Arrange
            var dbContextMock = new Mock<TransportesContext>();

            // Configurar el DbSet simulado
            var clientes = new List<Cliente>
            {
                new Cliente { Cedula = 1, Nombres = "Cliente 1" },
                new Cliente { Cedula = 2, Nombres = "Cliente 2" }
            }.AsQueryable();

            var dbSetMock = new Mock<DbSet<Cliente>>();
            dbSetMock.As<IQueryable<Cliente>>().Setup(m => m.Provider).Returns(clientes.Provider);
            dbSetMock.As<IQueryable<Cliente>>().Setup(m => m.Expression).Returns(clientes.Expression);
            dbSetMock.As<IQueryable<Cliente>>().Setup(m => m.ElementType).Returns(clientes.ElementType);
            dbSetMock.As<IQueryable<Cliente>>().Setup(m => m.GetEnumerator()).Returns(() => clientes.GetEnumerator());

            // Configurar el contexto simulado para devolver el DbSet simulado
            dbContextMock.Setup(m => m.Clientes).Returns(dbSetMock.Object);


            var controller = new ClientesController(dbContextMock.Object);

            // Act
            var result = await controller.Edit(10);

                // Assert
                var notFoundResult = Assert.IsType<NotFoundResult>(result);
                Assert.Equal(404, notFoundResult.StatusCode);
        }

      
       
        private List<Cliente> GetTestClientes()
        {
            return new List<Cliente>
        {
            new Cliente { Cedula = 1, Nombres = "John", Apellidos = "Doe", Telefono = 123456789 },
            new Cliente { Cedula = 2, Nombres = "Jane", Apellidos = "Doe", Telefono = 987654321 }
        };
        }

    }
}