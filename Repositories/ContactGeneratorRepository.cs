using Sitecore.Mvc.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Website.Models;

namespace Website.Repositories
{
  public class ContactGeneratorRepository
  {
    public ContactGenerator GetContactGenerator()
    {
      var contactGenerator = new ContactGenerator();

      //var rendering = RenderingContext.Current.Rendering;
      //var datasource = rendering.Item;

      contactGenerator.Name = new HtmlString("A name assigned from the repo");
      contactGenerator.Text = new HtmlString("A text assigned from the repo");

      return contactGenerator;

    }
  }
}