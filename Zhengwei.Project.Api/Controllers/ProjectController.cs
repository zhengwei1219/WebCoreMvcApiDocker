using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Zhengwei.Project.Api.Applications.Commands;
using Zhengwei.Project.Domain.AggregatesModel;
using Zhengwei.Project.Api.Applications.Service;
using Zhengwei.Project.Api.Applications.Queries;

namespace Zhengwei.Project.Api.Controllers
{
    [Route("api/[controller]")]
    public class ProjectController : BaseController
    {
        public IProjectQueries _projectQueries;
        public IMediator _mediator;
        public IRecommendService _recommendService;
        public ProjectController(IMediator mediator, IRecommendService recommendService, IProjectQueries projectQueries)
        {
            _mediator = mediator;
            _recommendService = recommendService;
            _projectQueries = projectQueries;
        }
        [HttpPost]
        [Route("")]
        public async Task<IActionResult> GetProjects()
        {
            var projects = await _projectQueries.GetProjectByUserId(UserIdentity.UserId);
            return Ok(projects);
        }
        [HttpPost]
        [Route("my/{projectId}")]
        public async Task<IActionResult> GetMyProjectDetail(int projectId)
        {
            var project = await _projectQueries.GetProjectDetail(projectId);
            if(project.UserId == UserIdentity.UserId)
            {
                return Ok(project);
            }else
            {
                return BadRequest("无权查看该项目。");
            }
            
        }
        [HttpGet]
        [Route("recommends/{projectId}")]
        public async Task<IActionResult> GetRecommendProjectDetail(int projectId)
        {
            if( await _recommendService.IsProjectInRecommend(projectId,UserIdentity.UserId))
            {
                var project = await _projectQueries.GetProjectDetail(projectId);
                return Ok(project);
            }else
            {
                return BadRequest("无权查看该项目");
            }
        }
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreatProject([FromBody] Zhengwei.Project.Domain.AggregatesModel.Project project)
        {
            var command = new CreateProjectCommand() { Project = project };
            var pro = await _mediator.Send(command);
            return Ok(pro);
        }
        [HttpPut]
        [Route("view/{projectId}")]
        public async Task<IActionResult> ViewProject(int projectId)
        {
            if(await _recommendService.IsProjectInRecommend(projectId,UserIdentity.UserId))
            {
                return BadRequest();
            }
            var command = new ViewProjectCommand() {
                ProjectId = projectId,
                UserId = UserIdentity.UserId,
                UserName = UserIdentity.Name,
                Avatar = UserIdentity.Avatar
            };
            await _mediator.Send(command);
            return Ok();
        }
        [HttpPut]
        [Route("join/{projectId}")]
        public async Task<IActionResult> JoinProject(ProjectContributor contributor)
        {
            var command = new JoinProjectCommand()
            {
                Contributor = contributor
            };
           await _mediator.Send(command);
            return Ok();
        }
    }
}
