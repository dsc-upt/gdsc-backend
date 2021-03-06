using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FactoryBot;
using Faker;
using GdscBackend.Controllers.v1;
using GdscBackend.Database;
using GdscBackend.Models;
using GdscBackend.Models.Enums;
using GdscBackend.RequestModels;
using GdscBackend.Tests.Mocks;
using GdscBackend.Utils.Mappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Xunit.Abstractions;

namespace GdscBackend.Tests;

public class MenuItemsControllerTests : TestingBase
{
    private readonly IMapper _mapper;
    private readonly IEnumerable<MenuItemModel> _testData = _getTestData();

    public MenuItemsControllerTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
        var mapconfig = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfiles()));
        _mapper = mapconfig.CreateMapper();
    }

    [Fact]
    public async void Post_ReturnsCreatedObject()
    {
        var repository = new Repository<MenuItemModel>(new TestDbContext<MenuItemModel>(_testData).Object);
        var controller = new MenuItemsController(repository, _mapper);
        var model1 = new MenuItemRequest
        {
            Name = Name.First(),
            Type = MenuItemTypeEnum.ExternalLink,
            Link = Lorem.Words(3).ToString()
        };
        var model2 = new MenuItemRequest
        {
            Name = Name.First(),
            Type = MenuItemTypeEnum.InternalLink,
            Link = Lorem.Words(3).ToString()
        };
        var added1 = await controller.Post(model1);
        var added2 = await controller.Post(model2);

        var result1 = added1.Result as CreatedResult;
        var result2 = added2.Result as CreatedResult;

        Assert.NotNull(result1);
        Assert.NotNull(result2);

        var entity1 = result1.Value as MenuItemModel;
        var entity2 = result2.Value as MenuItemModel;

        Assert.NotNull(entity1);
        Assert.NotNull(entity1.Id);
        Assert.Equal(StatusCodes.Status201Created, result1.StatusCode);
        Assert.Equal(model1.Name, entity1.Name);
        Assert.Equal(model1.Link, entity1.Link);

        Assert.NotNull(entity2);
        Assert.NotNull(entity2.Id);
        Assert.Equal(StatusCodes.Status201Created, result2.StatusCode);
        Assert.Equal(model2.Name, entity2.Name);
        Assert.Equal(model2.Link, entity2.Link);
    }

    [Fact]
    public async void GetById_ReturnsOkObjectResult()
    {
        var repository = new Repository<MenuItemModel>(new TestDbContext<MenuItemModel>(_testData).Object);
        var controller = new MenuItemsController(repository, _mapper);
        var firstObj = repository.DbSet.First();
        var actionResult = await controller.Get(firstObj.Id);
        var returnedResult = actionResult.Result as OkObjectResult;

        Assert.NotNull(returnedResult);
        Assert.Equal(StatusCodes.Status200OK, returnedResult.StatusCode);
        Assert.Equal(firstObj, returnedResult.Value);
    }

    [Fact]
    public async void Get_ReturnsAllExamples()
    {
        var repository = new Repository<MenuItemModel>(new TestDbContext<MenuItemModel>(_testData).Object);
        var controller = new MenuItemsController(repository, _mapper);

        var actionResult = await controller.Get();
        var returnedResult = actionResult.Result as OkObjectResult;

        Assert.NotNull(returnedResult);
        var items = Assert.IsAssignableFrom<IEnumerable<MenuItemModel>>(returnedResult.Value);
        Assert.Equal(StatusCodes.Status200OK, returnedResult.StatusCode);
        Assert.Equal(_testData.ToList(), items.ToList());
    }

    [Fact]
    public async void Delete_Returns404NotFoundResult()
    {
        var repository = new Repository<MenuItemModel>(new TestDbContext<MenuItemModel>(_testData).Object);
        var controller = new MenuItemsController(repository, _mapper);
        var deletedobj = repository.DbSet.First();

        var actionResult = await controller.Delete(deletedobj.Id);
        var resultResult = actionResult.Result as OkObjectResult;
        var actionResult1 = await controller.Get(deletedobj.Id);
        var result1Result = actionResult1.Result as NotFoundResult;

        Assert.NotNull(result1Result);
        Assert.NotNull(resultResult);
        Assert.Equal(StatusCodes.Status404NotFound, result1Result.StatusCode);
        Assert.Equal(deletedobj, resultResult.Value);
    }

    [Fact]
    public async void Update_ReturnsOkObjectAction()
    {
        var repository = new Repository<MenuItemModel>(new TestDbContext<MenuItemModel>(_testData).Object);
        var controller = new MenuItemsController(repository, _mapper);
        var model1 = new MenuItemRequest
        {
            Name = Name.First(),
            Type = MenuItemTypeEnum.ExternalLink,
            Link = Lorem.Words(3).ToString()
        };
        var model2 = new MenuItemRequest
        {
            Name = Name.First(),
            Type = MenuItemTypeEnum.InternalLink,
            Link = Lorem.Words(3).ToString()
        };

        var actionResult1 = await controller.Update(model1);
        var actionResult2 = await controller.Update(model2);

        var resultResult1 = actionResult1.Result as OkObjectResult;
        var resultResult2 = actionResult2.Result as OkObjectResult;

        Assert.NotNull(resultResult1);
        Assert.NotNull(resultResult2);

        Assert.Equal(StatusCodes.Status200OK, resultResult1.StatusCode);
        Assert.Equal(StatusCodes.Status200OK, resultResult2.StatusCode);
    }

    private static IEnumerable<MenuItemModel> _getTestData()
    {
        Bot.Define(x => new MenuItemModel
        {
            Id = x.Strings.Guid(),
            Name = x.Names.FirstName(),
            Type = x.Enums.Any<MenuItemTypeEnum>(),
            Link = Lorem.Words(3).ToString()
        });

        return Bot.BuildSequence<MenuItemModel>().Take(10).ToList();
    }
}