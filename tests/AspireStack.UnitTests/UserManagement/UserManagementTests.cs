using AspireStack.Application.UserManagement;
using AspireStack.Application.UserManagement.DTOs;
using AspireStack.Domain.Cache;
using AspireStack.Domain.Entities.UserManagement;
using AspireStack.Domain.Repository;
using AspireStack.Domain.Services;
using Microsoft.VisualStudio.TestPlatform.Common.Utilities;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AspireStack.UnitTests.UserManagement
{
    [TestClass]
    public sealed class UserManagementTests
    {
        [TestMethod]
        public void UserEntityTests()
        {
            var user = new Bogus.Faker<User>()
                .RuleFor(u => u.FirstName, f => f.Person.FirstName)
                .RuleFor(u => u.LastName, f => f.Person.LastName)
                .RuleFor(u => u.Email, f => f.Person.Email)
                .RuleFor(u => u.Username, f => f.Person.UserName)
                .RuleFor(u => u.PasswordHashed, f => f.Internet.Password())
                .Generate();
            Assert.IsNotNull(user);

            Assert.IsFalse(string.IsNullOrEmpty(user.FirstName));
            Assert.IsFalse(string.IsNullOrEmpty(user.LastName));
            Assert.IsFalse(string.IsNullOrEmpty(user.Email));

            user.Validate();

            var role = new Bogus.Faker<Role>()
                .RuleFor(r => r.Id, f => Guid.NewGuid())
                .RuleFor(r => r.Name, f => f.Lorem.Word())
                .RuleFor(r => r.Description, f => f.Lorem.Sentence())
                .RuleFor(r => r.Permissions, f => new string[] { f.Lorem.Word(), f.Lorem.Word() })
                .Generate();

            user.AddRole(role);

            Assert.IsNotNull(user.Roles);
            Assert.IsTrue(user.Roles.Count == 1);
            Assert.IsTrue(user.Roles.Any(r => r.RoleId == role.Id));

            user.RemoveRole(role.Id);
            Assert.IsNotNull(user.Roles);
            Assert.IsTrue(user.Roles.Count == 0);
            Assert.IsFalse(user.Roles.Any(r => r.RoleId == role.Id));
        }

        [TestMethod]
        public void UserEntityTests_2()
        {
            var user = new Bogus.Faker<User>()
                .Generate();
            Assert.IsNotNull(user);

            Assert.ThrowsException<ValidationException>(() => user.Validate());
        }

        [TestMethod]
        public async Task UserAppServiceTests_CreateUserAsync_Test()
        {
            // Arrange
            var user = new Bogus.Faker<User>()
                .RuleFor(u => u.FirstName, f => f.Person.FirstName)
                .RuleFor(u => u.LastName, f => f.Person.LastName)
                .RuleFor(u => u.Email, f => f.Person.Email)
                .RuleFor(u => u.Username, f => f.Person.UserName)
                .RuleFor(u => u.PasswordHashed, f => f.Internet.Password())
                .Generate();
            var role = new Bogus.Faker<Role>()
                .RuleFor(r => r.Id, f => Guid.NewGuid())
                .RuleFor(r => r.Name, f => f.Lorem.Word())
                .RuleFor(r => r.Description, f => f.Lorem.Sentence())
                .RuleFor(r => r.Permissions, f => new string[] { f.Lorem.Word(), f.Lorem.Word() })
                .Generate();
            var passworhasherMock = new Mock<IUserPasswordHasher<User>>();
            var randomString = new Bogus.Faker().Random.String2(10);
            passworhasherMock.Setup(p => p.HashPassword(It.IsAny<User>(), It.IsAny<string>())).Returns(randomString);
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var inMemoryUserList = new List<User>();
            var userId = Guid.NewGuid();
            unitOfWorkMock.Setup(u => u.Repository<User, Guid>().InsertAsync(It.IsAny<User>(), It.IsAny<bool>(), CancellationToken.None)).Callback<User, bool, CancellationToken>((u, b, c) =>
            {
                u.Id = userId;
                inMemoryUserList.Add(u);
            });
            unitOfWorkMock.Setup(u => u.Repository<Role, Guid>().FindAsync(It.IsAny<Expression<Func<Role, bool>>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(role);
            var cacheServiceMock = new Mock<ICacheClient>();
            var currentUserMock = new Mock<ICurrentUser>();
            var asyncExecuterMock = new Mock<IAsyncQueryableExecuter>();
            var userAppService = new UsersAppService(passworhasherMock.Object)
            {
                UnitOfWork = unitOfWorkMock.Object,
                CacheClient = cacheServiceMock.Object,
                CurrentUser = currentUserMock.Object,
                AsyncExecuter = asyncExecuterMock.Object
            };
            var input = new CreateEditUserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Username = user.Username,
                RoleIds = new List<Guid> { role.Id }
            };

            // Act
            var newId = await userAppService.CreateUserAsync(input);

            // Assert
            Assert.IsNotNull(inMemoryUserList);
            Assert.IsNotNull(user);
            Assert.IsNotNull(inMemoryUserList.FirstOrDefault());
            Assert.IsTrue(inMemoryUserList.Count == 1);
            Assert.IsTrue(inMemoryUserList.Any(u => u.Username == user.Username));
            Assert.IsTrue(inMemoryUserList.Any(u => u.Email == user.Email));
            Assert.IsTrue(inMemoryUserList.Any(u => u.PasswordHashed == randomString));
            Assert.IsTrue(inMemoryUserList.Any(u => u.Roles.Any(r => r.RoleId == role.Id)));
            Assert.AreEqual(userId, newId);
            unitOfWorkMock.Verify(u => u.Repository<User, Guid>().InsertAsync(It.IsAny<User>(), It.IsAny<bool>(), CancellationToken.None), Times.Once);
            unitOfWorkMock.Verify(u => u.Repository<Role, Guid>().FindAsync(It.IsAny<Expression<Func<Role, bool>>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task UserAppServiceTests_GetUsersAsync_Test()
        {
            // Arrange
            var user = new Bogus.Faker<User>()
                .RuleFor(u => u.FirstName, f => f.Person.FirstName)
                .RuleFor(u => u.LastName, f => f.Person.LastName)
                .RuleFor(u => u.Email, f => f.Person.Email)
                .RuleFor(u => u.Username, f => f.Person.UserName)
                .RuleFor(u => u.PasswordHashed, f => f.Internet.Password())
                .Generate();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var inMemoryUserList = new List<User> { user };
            unitOfWorkMock.Setup(u => u.Repository<User, Guid>().GetQueryable()).Returns(inMemoryUserList.AsQueryable());
            var cacheServiceMock = new Mock<ICacheClient>();
            var currentUserMock = new Mock<ICurrentUser>();
            var asyncExecuterMock = new Mock<IAsyncQueryableExecuter>();
            asyncExecuterMock.Setup(a => a.ToListAsync(It.IsAny<IEnumerable<UserDto>>(), It.IsAny<CancellationToken>())).ReturnsAsync(inMemoryUserList.Select(UserDto.FromUser).ToList());
            asyncExecuterMock.Setup(a => a.CountAsync(It.IsAny<IQueryable<User>>(), It.IsAny<CancellationToken>())).ReturnsAsync(inMemoryUserList.Count);
            var passworhasherMock = new Mock<IUserPasswordHasher<User>>();
            var userAppService = new UsersAppService(passworhasherMock.Object)
            {
                UnitOfWork = unitOfWorkMock.Object,
                CacheClient = cacheServiceMock.Object,
                CurrentUser = currentUserMock.Object,
                AsyncExecuter = asyncExecuterMock.Object
            };
            var input = new GetUsersInput
            {
                PageSize = 10,
                Page = 1
            };
            // Act
            var pagedResult = await userAppService.GetUsersAsync(input);
            var users = pagedResult.Items;
            // Assert
            Assert.IsNotNull(users);
            Assert.IsTrue(users.Count == 1);
            Assert.IsTrue(users.Any(u => u.Username == user.Username));
            Assert.IsTrue(users.Any(u => u.Email == user.Email));
            asyncExecuterMock.Verify(a => a.CountAsync(It.IsAny<IQueryable<User>>(), It.IsAny<CancellationToken>()), Times.Once);
            asyncExecuterMock.Verify(a => a.ToListAsync(It.IsAny<IEnumerable<UserDto>>(), It.IsAny<CancellationToken>()), Times.Once);
            unitOfWorkMock.Verify(u => u.Repository<User, Guid>().GetQueryable(), Times.Once);
        }
    }
}
