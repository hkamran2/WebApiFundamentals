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

        //Specify the name of the route as well so that it can be used by the Post req to get the location of the object created
        [Route("{moniker}" , Name = "GetCamp")]
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

        //{baseURI}/camps/searchByDate/2018-03-01
        [Route("searchByDate/{date:datetime}")]//the datetime specifies the type contstraint for the route
        [HttpGet]
        public async Task<IHttpActionResult> SearchByEventDate(DateTime date, bool includeTalks = false)
        {
            try
            {
                var res = await _repository.GetAllCampsByEventDate(date, includeTalks);
                return Ok(_mapper.Map<CampModel[]>(res));
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [Route()]
        public async Task<IHttpActionResult> Post(CampModel model)
        {
            try
            {
                //Check if the moniker is already defined
                if (await _repository.GetCampAsync(model.Moniker) != null)
                {
                    ModelState.AddModelError("Moniker", "Moniker in use");
                }

                if (ModelState.IsValid)
                {
                    var camp = _mapper.Map<Camp>(model);

                    _repository.AddCamp(camp);

                    if (await _repository.SaveChangesAsync())
                    {
                        var newModel = _mapper.Map<CampModel>(camp);

                        return CreatedAtRoute("GetCamp", new {moniker = newModel.Moniker}, newModel);
                    }
                }

            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }

            return BadRequest(ModelState);
        }

        [Route("{moniker}")]
        public async Task<IHttpActionResult> Put(string moniker, CampModel model)
        {
            try
            {
                var camp = await _repository.GetCampAsync(moniker);
                if (camp == null) return NotFound();
                //We have to store the Camp obj(not CampModel)
                _mapper.Map(model, camp);

                if (await _repository.SaveChangesAsync())
                {
                    return Ok(_mapper.Map(camp, model));
                }
                else
                {
                    //Use internal server error instead of the not found because if there is an error during saving the results it is not really the user's fault
                    return InternalServerError();
                }
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        public async Task<IHttpActionResult> Delete(string moniker)
        {
            try
            {
                var camp = await _repository.GetCampAsync(moniker);
                if (camp == null) return NotFound();

                _repository.DeleteCamp(camp);

                if (await _repository.SaveChangesAsync())
                {
                    return Ok();
                }
                else
                {
                    return InternalServerError();
                }
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }
    }
}