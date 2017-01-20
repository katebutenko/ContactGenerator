using Sitecore.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Website.App_Start
{
  public class RouteConfig
  {

    public virtual void Process(PipelineArgs args)
    {      
      RouteTable.Routes.MapRoute("ContactRoute", "GetContact", new { controller = "ContactGenerator", action = "GetContactDetails" });
      RouteTable.Routes.MapRoute("SimplifiedContactRoute", "GetSimplifiedContact", new { controller = "ContactGenerator", action = "GetSimplifiedContact" });
      RouteTable.Routes.MapRoute("GetNContactsRoute", "GetNContacts", new { controller = "ContactGenerator", action = "GetNContacts" });
      RouteTable.Routes.MapRoute("RunSegmentedRuleRoute", "RunSegmentRuleOnAllContacts", new { controller = "ContactGenerator", action = "RunSegmentRuleOnAllContacts" });
    }
    
  }
}