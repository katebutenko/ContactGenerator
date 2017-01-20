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

      return contactGenerator;

    }
  }
}