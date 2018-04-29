using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace IP2C.WebAPI.Controllers
{
    public class DiagController : ApiController
    {
        [HttpGet]
        public string Echo(string text)
        {
            return text;
        }

        [HttpGet]
        public string SelfCheck(string action)
        {
            switch (action)
            {
                case "datafile":
                    return "OK";
            }

            return "NO-ACTION";
        }
    }
}