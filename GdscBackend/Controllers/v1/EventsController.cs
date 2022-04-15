using System.Net.Mime;
using AutoMapper;
using GdscBackend.Database;
using GdscBackend.Models;
using GdscBackend.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GdscBackend.Controllers.v1;

[ApiController]
[ApiVersion("1")]
[Authorize(Roles = "admin")]
[Route("v1/events")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class EventsController : ControllerBase
{
    private readonly IRepository<FileModel> _filesRepository;
    private readonly IMapper _mapper;
    private readonly IRepository<EventModel> _repository;

    public EventsController(IRepository<EventModel> repository, IMapper mapper,
        IRepository<FileModel> filesRepository)
    {
        _repository = repository;
        _mapper = mapper;
        _filesRepository = filesRepository;
    }
/*<<<<<<< HEAD

=======
    
>>>>>>> dev*/
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EventModel>> Post(EventRequest entity)
    {
        var mappedEntity = Map(entity);
        mappedEntity.Image = await _filesRepository.GetAsync(entity.ImageId);
        var newEntity = await _repository.AddAsync(mappedEntity);
        return Created("v1/event", newEntity);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventModel>> Delete([FromRoute] string id)
    {
        var entity = await _repository.DeleteAsync(id);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventModel>> Update(EventRequest entity)
    {
        var newEntity = await _repository.UpdateAsync(Map(entity));
        return newEntity is null ? BadRequest() : Ok(newEntity);
    }

    private EventModel Map(EventRequest entity)
    {
        return _mapper.Map<EventModel>(entity);
    }

    private EventRequest Map(EventModel entity)
    {
        return _mapper.Map<EventRequest>(entity);
    }

    private IEnumerable<EventRequest> Map(IEnumerable<EventModel> entity)
    {
        return _mapper.Map<IEnumerable<EventRequest>>(entity);
    }

    private IEnumerable<EventModel> Map(IEnumerable<EventRequest> entity)
    {
        return _mapper.Map<IEnumerable<EventModel>>(entity);
    }
}