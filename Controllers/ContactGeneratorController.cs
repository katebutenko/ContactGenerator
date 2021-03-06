﻿using Sitecore.Analytics.Model.Entities;
using Sitecore.ListManagement.ContentSearch.Pipelines.ImportContacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website.Models;
using Website.Repositories;
using Sitecore.ListManagement.ContentSearch.Extensions;
using Sitecore.ListManagement.Data;
using Sitecore.ListManagement;
using Sitecore.ListManagement.ContentSearch.Model;
using Sitecore.Analytics.Data.Bulk.Contact;
using Sitecore.Analytics.Data.Bulk;
using Sitecore.Analytics.Tracking;
using Sitecore.Analytics.Data;
using Sitecore.Configuration;
using Sitecore.Analytics.Model;
using Sitecore.Analytics.DataAccess;
using Sitecore.Diagnostics;
using Sitecore.Analytics.Tracking.External;
using RealAddressing;
using Sitecore.Analytics.Model.Generated;
using Website.Common;
using Sitecore.Data;
using Sitecore.Data.Items;
using System.Globalization;
using Sitecore.SecurityModel;
using RandomNameGenerator;
using Sitecore.Shell.Applications.Analytics.SegmentBuilder;

namespace Website.Controllers
{
  public class ContactGeneratorController : Controller
  {    

    public class SimplifiedContact
    {
      public string Name;
      public float Latitude;
      public float Longitude;
      public string DestinationCountry;


      public SimplifiedContact()
      {

      }
      public SimplifiedContact(Contact contact)
      {
        var personalInfo = contact.GetFacet<IContactPersonalInfo>("Personal");
        if (personalInfo != null)
        {
          Name = personalInfo.FirstName + " " + personalInfo.Surname;
        }

        var addresses = contact.GetFacet<IContactAddresses>("Addresses");
        if (addresses != null)
        {
          var primaryAddress = addresses.Entries[addresses.Preferred];

          Latitude = primaryAddress.Location.Latitude;
          Longitude = primaryAddress.Location.Longitude;
        }
      }
    }

    ContactRepository contactRepository = Factory.CreateObject("tracking/contactRepository", true) as ContactRepository;

    public ActionResult RandomGenerate()
    {
      var repo = new ContactGeneratorRepository();
      var generator = repo.GetContactGenerator();
      return View("ContactGeneratorView", generator);
    }


    private Contact CreateContact()
    {      
      var contactIdentifier = Guid.NewGuid();
      var contact = contactRepository.CreateContact(contactIdentifier);

      SetPersonalInfo(contact);
      SetAddress(contact);
      SetEmail(contact);

      var leaseOwner = new LeaseOwner("ContactGenerator", LeaseOwnerType.OutOfRequestWorker);
      contactRepository.SaveContact(contact, new ContactSaveOptions(true, leaseOwner));
      return contact;
    }

    private Contact CreateContactAndSetTag(Guid tagValue)
    {
      var contactIdentifier = Guid.NewGuid();
      var contact = contactRepository.CreateContact(contactIdentifier);

      SetPersonalInfo(contact);
      SetAddress(contact);
      SetEmail(contact);
      SetTag(contact, tagValue);

      var leaseOwner = new LeaseOwner("ContactGenerator", LeaseOwnerType.OutOfRequestWorker);
      contactRepository.SaveContact(contact, new ContactSaveOptions(true, leaseOwner));
      return contact;
    }

    private void AddInteraction(Contact contact, Airport airport)
    {
      TouchPointRecord touchPointRecord = new TouchPointRecord(Guid.Parse(Common.Constants.TouchPointItemId), DateTime.UtcNow, TimeSpan.FromSeconds(1.0));

      EventRecord item = new EventRecord(Common.Constants.EventName, DateTime.UtcNow)
      {
        Data = "{" + airport.Id.ToString() + "}"
      };
      touchPointRecord.Events.Add(item);

      InteractionRecord interaction = new InteractionRecord(Common.Constants.EventSiteName, Guid.Parse(Common.Constants.ChannelItemId), null);
      interaction.TouchPoints.Add(touchPointRecord);
      var interactionRegistry = Sitecore.Configuration.Factory.CreateObject("/sitecore/interactionRegistry", true) as InteractionRegistry;

      interactionRegistry.Register(Common.Constants.DefinitionDatabase, contact.ContactId, interaction);

    }

    private void SetAddress(Contact contact)
    {
      var addresses = contact.GetFacet<IContactAddresses>("Addresses");
      var primaryAddress = addresses.Entries.Create("Primary");
      City city = CityGenerator.GetRandomCity();
            
      primaryAddress.City = city.Name;
      primaryAddress.Country = city.Country;      
      primaryAddress.Location.Latitude = city.Latitude;
      primaryAddress.Location.Longitude = city.Longitude;

      addresses.Preferred = "Primary";      
    }

    private void SetTag(Contact contact,Guid tagValue)
    {
      contact.Tags.Add("Airport", Sitecore.Data.ID.Parse(tagValue).ToString());
    }

    private void SetEmail(Contact contact)
    {
      var emails = contact.GetFacet<IContactEmailAddresses>("Emails");
      var primaryEmail = emails.Entries.Create("Primary");
      primaryEmail.SmtpAddress = Faker.Internet.Email();
      emails.Preferred = "Primary";
    }

    private void SetPersonalInfo(Contact contact)
    {
      Random rand = new Random();
      
      var personalInfo = contact.GetFacet<IContactPersonalInfo>("Personal");

      double genderType = rand.NextDouble();

      if (genderType < 0.475)
      {
        personalInfo.Gender = "Female";
        personalInfo.FirstName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(NameGenerator.GenerateFirstName(Gender.Female).ToLower());
        personalInfo.Surname = Faker.Name.Last();
      }

      if (genderType >= 0.475 && genderType < 0.95)
      {
        personalInfo.Gender = "Male";
        personalInfo.FirstName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(NameGenerator.GenerateFirstName(Gender.Male).ToLower());
        personalInfo.Surname = Faker.Name.Last();
      }

      if (genderType >= 0.95) //approximately :)
      {
        personalInfo.Gender = "Other";
        personalInfo.FirstName = Faker.Name.First();
        personalInfo.Surname = Faker.Name.Last();
      }
    
    }
        
    [HttpPost]
    public JsonResult GetContactDetails()
    {
      Contact contact = CreateContact();
      return Json(contact);
    }

    [HttpPost]
    public JsonResult GetSimplifiedContact()
    {
      Contact contact = CreateContact();

      return Json(new SimplifiedContact(contact));
    }   

    [HttpPost]
    public JsonResult GetNContacts(int n)
    {
      List<SimplifiedContact> list = new List<SimplifiedContact>();

      List<Airport> airportCollection = GetPopularAirports();
      Random randomizer = new Random();      

      for (int i = 1; i <= n; i++)
      {
        int randomAirportIndex = randomizer.Next(airportCollection.Count());
        Airport randomAirport = airportCollection[randomAirportIndex];
        Contact contact = CreateContactAndSetTag(randomAirport.Id);
        
        AddInteraction(contact, randomAirport);

        list.Add(new SimplifiedContact(contact) { DestinationCountry = randomAirport.Country});
      }

      return Json(list);
    }

    private List<Airport> GetPopularAirports()
    {
      List<Airport> resultList = new List<Airport>();

      using (new SecurityDisabler())
      {
        Database db = Factory.GetDatabase(Common.Constants.DefinitionDatabase);

        foreach (Item item in db.GetItem(Common.Constants.AirportsItemId).Children)
        {
          resultList.Add(new Airport()
          {
            Id = item.ID.ToGuid(),
            City = item["City"],
            Country = item["Country"],
            Latitude = float.Parse(item["Latitude"], CultureInfo.InvariantCulture.NumberFormat),
            Longitude = float.Parse(item["Longitude"], CultureInfo.InvariantCulture.NumberFormat)
          });
        }
      }
      return resultList;
    }

    [HttpPost]
    public JsonResult RunSegmentRuleOnAllContacts()
    {
      //country should be passed as parameter
      string country = "Hawaii";

      Database db = Factory.GetDatabase(Common.Constants.DefinitionDatabase);
      var rule = db.GetItem(Common.Constants.DestinationAirportRuleItemId)["Query"];
      SegmentBuilder builder = new SegmentBuilder();
      List<Guid> listOfContacts = builder.RunContactIDsQueryAction<List<Guid>>(rule, db, q => q.ToList<Guid>());
      
      List<SimplifiedContact> resultList = new List<SimplifiedContact>();
      foreach (var contactId in listOfContacts)
      {
        Contact contact = contactRepository.LoadContactReadOnly(contactId);
        SimplifiedContact simplifiedContact = new SimplifiedContact(contact);
        simplifiedContact.DestinationCountry = country;
        resultList.Add(simplifiedContact);
      }
      return Json(resultList);
    }
}

}
