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
                    //
                    // TODO: 查詢 8.8.8.8 的地區是否是美國 (Google DNS) ?
                    // TODO: 查詢 168.95.1.1 的地區是否是台灣 (Hinet DNS) ?
                    //
                    return "OK";

                    
            }

            return "NO-ACTION";
        }
    }
}