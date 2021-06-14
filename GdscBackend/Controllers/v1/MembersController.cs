using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using AutoMapper;
using GdscBackend.Database;
using GdscBackend.Models;
using GdscBackend.RequestModels;
using GdscBackend.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GdscBackend.Controllers.v1
{
    [ApiController]
    [ApiVersion("1")]
    [Authorize(Roles = "admin")]
    [Route("v1/members")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class MembersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IRepository<MemberModel> _repository;

        public MembersController(IRepository<MemberModel> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<MemberRequest>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<MemberRequest>>> Get()
        {
            return Ok(Map((await _repository.GetAsync()).ToList()));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MemberRequest>> Get([FromRoute] string id)
        {
            var entity = Map(await _repository.GetAsync(id));

            return entity is null ? NotFound() : Ok(entity);
        }


        [HttpPost]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MemberRequest), StatusCodes.Status201Created)]
        public async Task<ActionResult<MemberRequest>> Post(MemberRequest entity)
        {
            entity = Map(await _repository.AddAsync(Map(entity)));

            return CreatedAtAction(nameof(Post),Map(entity).Id, entity);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(MemberRequest), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MemberRequest>> Delete([FromBody] string id)
        {
            var entity = Map(await _repository.DeleteAsync(id));

            return entity is null ? NotFound() : Ok(entity);
        }

        private MemberModel Map(MemberRequest entity)
        {
            return _mapper.Map<MemberModel>(entity);
        }

        private MemberRequest Map(MemberModel entity)
        {
            return _mapper.Map<MemberRequest>(entity);
        }

        private IEnumerable<MemberRequest> Map(IEnumerable<MemberModel> entity)
        {
            return _mapper.Map<IEnumerable<MemberRequest>>(entity);
        }

        private IEnumerable<MemberModel> Map(IEnumerable<MemberRequest> entity)
        {
            return _mapper.Map<IEnumerable<MemberModel>>(entity);
        }
    }
}