﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace TheCodeCamp.Controllers
{
    public class OperationsController : ApiController
    {
        [HttpOptions]
        [Route("api/refreshConfig")]
        public IHttpActionResult RefreshAppSettings()
        {
            try
            {
                ConfigurationManager.RefreshSection("AppSettings");
                return Ok();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }
    }
}