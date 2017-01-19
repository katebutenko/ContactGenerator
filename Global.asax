<%@Application Language='C#' Inherits="Sitecore.Web.Application" %>

<script runat="server">
 protected void Application_Start(object sender, EventArgs e)
        {
           Website.App_Start.RouteConfig.RegisterRoutes(RouteTable.Routes);        
        }
 </script>