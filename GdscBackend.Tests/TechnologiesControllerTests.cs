using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using Faker;
using GdscBackend.Controllers.v1;
using GdscBackend.Database;
using GdscBackend.Models;
using GdscBackend.RequestModels;
using GdscBackend.Tests.Mocks;
using GdscBackend.Utils.Mappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Xunit.Abstractions;

namespace GdscBackend.Tests;

public class TechnologiesControllerTests : TestingBase
{
    private readonly IMapper _mapper;
    private static readonly List<FileModel> _testIcons = _getTestIcons();
    private static readonly IEnumerable<TechnologyModel> _testData = _getTestData();
    
    public TechnologiesControllerTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
        
        var mapconfig = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfiles()));
        _mapper = mapconfig.CreateMapper();
    }

    [Fact]
    public async void Post_ReturnsCreatedObject()
    {
        var repository = new Repository<TechnologyModel>(new TestDbContext<TechnologyModel>().Object);
        var controller = new TechnologiesController(repository, _mapper);
        var example1 = new TechnologyRequest
        {
            Name = Lorem.Words(3).ToString(),
            Description = Lorem.Sentence(7),
            IconId = Lorem.Words(1).ToString()
        };
        var example2 = new TechnologyRequest
        {
            Name = Lorem.Words(3).ToString(),
            Description = Lorem.Sentence(7),
            IconId = Lorem.Words(1).ToString()
        };

        var added1 = await controller.Post(example1);
        var added2 = await controller.Post(example2);

        var result1 = added1.Result as CreatedResult;
        var result2 = added2.Result as CreatedResult;

        Assert.NotNull(result1);
        Assert.NotNull(result2);
        var entity1 = result1.Value as TechnologyModel;
        var entity2 = result2.Value as TechnologyModel;

        Assert.NotNull(entity1);
        Assert.NotNull(entity1.Id);
        Assert.Equal(StatusCodes.Status201Created, result1.StatusCode);
        Assert.Equal(example1.Description, entity1.Description);
        Assert.Equal(example1.IconId, entity1.Icon.Id);
        Assert.Equal(example1.Name, entity1.Name);

        Assert.NotNull(entity2);
        Assert.NotNull(entity2.Id);
        Assert.Equal(StatusCodes.Status201Created, result2.StatusCode);
        Assert.Equal(example2.Description, entity2.Description);
        Assert.Equal(example2.IconId, entity2.Icon.Id);
        Assert.Equal(example2.Name, entity2.Name);
    }

    [Fact]
    public async void Get_ReturnsAllTechnologies()
    {
        var repository = new Repository<TechnologyModel>(new TestDbContext<TechnologyModel>(_testData).Object);
        var controller = new TechnologiesController(repository, _mapper);

        var actionResult = await controller.Get();
        var result = actionResult.Result as OkObjectResult;

        Assert.NotNull(result);
        var items = Assert.IsAssignableFrom<IEnumerable<TechnologyModel>>(result.Value);
        WriteLine(items);
    }

    [Fact]
    public async void Delete_ReturnsOkResult()
    {
        var repository = new Repository<TechnologyModel>(new TestDbContext<TechnologyModel>(_testData).Object);
        var controller = new TechnologiesController(repository, _mapper);

        var actionResult = await controller.Delete(_testData.First().Id);
        var result = actionResult.Result as OkObjectResult;

        var actionGetResult = (await controller.Get()).Result as OkObjectResult;
        Assert.NotNull(actionGetResult);
        var countResult = Assert.IsAssignableFrom<IEnumerable<TechnologyModel>>(actionGetResult.Value);

        Assert.NotNull(result);
        Assert.Equal(countResult.Count(), repository.DbSet.Count());
        Assert.Equal(_testData.Count() - 1, repository.DbSet.Count());
    }

    private static List<FileModel> _getTestIcons()
    {
        var icons = new List<FileModel>();
        for (var i = 0; i < 10; i++)
        {
            icons.Add(new FileModel
            {
                Id = Guid.NewGuid().ToString(),
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow,
                Name = Lorem.Words(1).ToString(),
                Path = Lorem.Words(1).ToString(),
                Extension = Lorem.Words(1).ToString(),
                Size = RandomNumber.Next(long.MaxValue)
            });
        }

        return icons;
    }
    private static IEnumerable<TechnologyModel> _getTestData()
    {
        
        
        var models = new List<TechnologyModel>();
        for (var i = 0; i < 10; i++)
            models.Add(new TechnologyModel
            {
                Id = Guid.NewGuid().ToString(),
                Name = Lorem.Words(1).ToString(),
                Description = Lorem.Words(1).ToString(),
                Icon = _testIcons[i]
                
            });

        return models;
    }
}