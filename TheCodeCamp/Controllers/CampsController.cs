using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using AutoMapper;
using TheCodeCamp.Data;
using TheCodeCamp.Models;

namespace TheCodeCamp.Controllers
{
    [RoutePrefix("api/camps")]
    public class CampsController : ApiController
    {
        private ICampRepository _repository;
        private IMapper _mapper;

        public CampsController(ICampRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        //Use the default route
        [Route()]
        public async Task<IHttpActionResult> Get(bool includeTalks = false)
        {
            try
            {
                var result = await _repository.GetAllCampsAsync(includeTalks);
                
                //Mapping
                var map = _mapper.Map<IEnumerable<CampModel>>(result);
                return Ok(map);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        //
        [Route("{moniker}")]
        public async Task<IHttpActionResult> Get(string moniker, [FromUri]bool includeTalks=true)
        {
            try
            {
                var result = await _repository.GetCampAsync(moniker,includeTalks);
                if (result == null) return NotFound();
                return Ok(_mapper.Map<CampModel>(result));
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }
    }
}