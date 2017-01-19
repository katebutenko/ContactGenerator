using Sitecore.Analytics.Model.Entities;
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

namespace Website.Controllers
{
  public class ContactGeneratorController : Controller
  {

    public ActionResult RandomGenerate()
    {
      var repo = new ContactGeneratorRepository();
      var generator = repo.GetContactGenerator();
      return View("ContactGeneratorView", generator);
    }

    [HttpPost]
    public ActionResult RandomGenerate(ContactGenerator viewModel)
    {
      //do the generation. Validation of input should also go here 


      var contactTemplateFactory = Sitecore.Configuration.Factory.CreateObject("/sitecore/model/entities/contact/template", true) as IContactTemplateFactory;

      //IContactTemplate destination = contactTemplateFactory.Create();
      //foreach (string str in mapping.Keys)
      //{
      //  if ((mapping[str] >= 0) && (mapping[str] < fields.Length))
      //  {
      //    string propertyValue = fields[mapping[str]];
      //    MappingContext context = new MappingContext(this.ContactMapper);
      //    this.ContactMapper.Map(new MappingInfo(str, propertyValue), destination, context);
      //  }
      //}
      //return destination;

      //List<ImportContactsResult> results = new List<ImportContactsResult>();

      //foreach (IEnumerable<IContactTemplate> enumerable in args.ContactTemplates.Batch<IContactTemplate>(bulkSize))
      //{
      //  ImportContactsArgs args2 = new ImportContactsArgs(args.ImportBatchId, enumerable, args.FieldMappings);
      //  results.Add(args2.ImportContactsResult);
      //  this.CorePipeline.Run("listManagement.importContacts", args2);
      //}
      //this.MapResults(args, results);

      for (int i = 1; i <= 5; i++)
      {
        Guid contactId = CreateContact().ContactId;

        //create interaction with event

        TouchPointRecord touchPointRecord = new TouchPointRecord(Guid.Parse("{ACD0EFA8-2023-4436-910D-0B51E721A3FA}"), DateTime.UtcNow, TimeSpan.FromSeconds(1.0));

        EventRecord item = new EventRecord("Flight Purchase", DateTime.UtcNow)
        {
          Data = "some data"
        };        touchPointRecord.Events.Add(item);



        InteractionRecord interaction = new InteractionRecord("offline", Guid.Parse("{2CF195C6-C36C-4512-BAB3-E2C7CF26882E}"), null)
        {
          //Ip = IPAddress.Parse(args.EmailOpen.IPAddress),
          //UserAgent = args.EmailOpen.UserAgent,
          //Language = args.TouchPointRecord.Language
        };
        interaction.TouchPoints.Add(touchPointRecord);
        var interactionRegistry = Sitecore.Configuration.Factory.CreateObject("/sitecore/interactionRegistry", true) as InteractionRegistry;

        interactionRegistry.Register("master", contactId, interaction);

      }
      return View("GenerationProcessView", model: "The contacts are being generated");
    }

    private Contact CreateContact()
    {
      var contactRepository = Factory.CreateObject("tracking/contactRepository", true) as ContactRepository;      
      var contactIdentifier = Guid.NewGuid();
      var contact = contactRepository.CreateContact(contactIdentifier);

      SetPersonalInfo(contact);
      SetAddress(contact);
      SetEmail(contact);

      var leaseOwner = new LeaseOwner("ContactGenerator", LeaseOwnerType.OutOfRequestWorker);
      contactRepository.SaveContact(contact, new ContactSaveOptions(true, leaseOwner));
      return contact;
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

    private void SetEmail(Contact contact)
    {
      var emails = contact.GetFacet<IContactEmailAddresses>("Emails");
      var primaryEmail = emails.Entries.Create("Primary");
      primaryEmail.SmtpAddress = Faker.Internet.Email();
      emails.Preferred = "Primary";
    }

    private void SetPersonalInfo(Contact contact)
    {
      var personalInfo = contact.GetFacet<IContactPersonalInfo>("Personal");
      personalInfo.FirstName = Faker.Name.First();
      personalInfo.Surname = Faker.Name.Last();            
    }
    public void ProcessContacts()
    {
      int bulkSize = 100;
      //List<ImportContactsResult> results = new List<ImportContactsResult>();
      List<IContactTemplate> contactTemplates = new List<IContactTemplate>();
      foreach (IEnumerable<IContactTemplate> enumerable in contactTemplates.Batch<IContactTemplate>(bulkSize))
      {
        //ImportContactsArgs args2 = new ImportContactsArgs(args.ImportBatchId, enumerable, args.FieldMappings);
        //results.Add(args2.ImportContactsResult);
        //this.CorePipeline.Run("listManagement.importContacts", args2);
        ImportContacts(enumerable as List<IContactTemplate>);
      }
    }
    private readonly IWorkItemSetManager<KnownContactSet, IContactTemplate> WorkItemSetManager;
    private void ImportContacts(List<IContactTemplate> incomingContacts)
    {
      //Guid guid;

      //ProcessContactsResult<ContactData> result = new ProcessContactsResult<ContactData>();
      
      //int count = incomingContacts.Count;
      //List<IContactTemplate> list = new List<IContactTemplate>(count);
      //string str = "CustomContactGenerator_" + Guid.NewGuid().ToString("N");
      //KnownContactSet set = this.WorkItemSetManager.Open(str, count, true);
      
      //foreach (IContactTemplate local in list)
      //{
      //  IContactTemplate template;
      //  if (string.IsNullOrWhiteSpace(local.Identifiers.Identifier))
      //  {
      //    result.UnidentifiedContactsCount++;
      //    continue;
      //  }
      //  Sitecore.Analytics.Tracking.Contact existingContact = null;
      //  ContactStatus contactStatus = this.GetContactStatus(local);
      //  if (contactStatus.Exists)
      //  {
      //    if (contactStatus.IsLeased)
      //    {
      //      ID id;
      //      if (ID.TryParse(tagValue, out id))
      //      {
      //        if ((contextData == "ListManagementImportContacts_") || (contextData == "ListManagementUpdateContactTags_"))
      //        {
      //          if (ContactExtensions.AddContactToList(this.ContactRepository, new ID(contactStatus.Contact.ContactId), id.Guid) == 0)
      //          {
      //            result.ProcessedContactsCount++;
      //            result.ExistingContactsCount++;
      //          }
      //          else
      //          {
      //            result.LockedOrUnavailableContactsCount++;
      //          }
      //        }
      //        if (contextData == "ListManagementRemoveContactTags_")
      //        {
      //          if (ContactExtensions.RemoveContactFromList(this.ContactRepository, new ID(contactStatus.Contact.ContactId), id.Guid) == 0)
      //          {
      //            result.ProcessedContactsCount++;
      //            result.ExistingContactsCount++;
      //          }
      //          else
      //          {
      //            result.LockedOrUnavailableContactsCount++;
      //          }
      //        }
      //      }
      //      else
      //      {
      //        result.LockedOrUnavailableContactsCount++;
      //      }
      //    }
      //    else
      //    {
      //      result.ExistingContactsCount++;
      //      existingContact = contactStatus.Contact;
      //      ITag tag = existingContact.Tags.Find(tagName);
      //      if ((tag != null) && tag.Values.Any<ITagValue>((<> 9__0 ?? (<> 9__0 = v => v.Value == tagValue))))
      //                  {
      //        result.PreviouslyAssociatedContactsCount++;
      //        goto Label_02D0;
      //      }
      //      if (contextData != "ListManagementRemoveContactTags_")
      //      {
      //        goto Label_02D0;
      //      }
      //    }
      //    continue;
      //  }
      //  result.NewContactsCount++;
      //  if (contextData == "ListManagementRemoveContactTags_")
      //  {
      //    continue;
      //  }
      //Label_02D0:
      //  template = this.MapContact(local, existingContact);
      //  template.Identifiers.IdentificationLevel = ContactIdentificationLevel.Known;
      //  list.Add(local);
      //  result.ProcessedContactsCount++;
      //  if (string.IsNullOrEmpty(tagName) || string.IsNullOrEmpty(tagValue))
      //  {
      //    set.Add(template);
      //  }
      //  else
      //  {
      //    DateTime utcNow = DateTime.UtcNow;
      //    ITagValue local1 = (template.Tags.Entries.Contains(tagName) ? template.Tags.Entries[tagName] : template.Tags.Entries.Create(tagName)).Values.Create();
      //    local1.Value = tagValue;
      //    local1.DateTime = utcNow;
      //    set.Add(template);
      //  }
      //}
      //IBulkOperation<IContactUpdateResult> operation = this.operationManager.SubmitWorkItemSet(set, str);
      //Assert.IsNotNull(operation, "Bulk operation cannot be null.");
      //Assert.IsNotNull(operation.Id, "Bulk operation id cannot be null.");
      //result.OperationId = operation.Id.ToString();
      //result.Contacts = list;
      //return result;
    }

    protected class ContactStatus
    {
      // Properties
      public Contact Contact { get; set; }

      public bool Exists { get; set; }

      public bool IsActive { get; set; }

      public bool IsLeased { get; set; }
    }

    ContactRepository ContactRepository = Factory.CreateObject("tracking/contactRepository", true) as ContactRepository;

   

    private ContactStatus GetContactStatus(IContactTemplate incomingContact)
    {      
      ContactStatus status = new ContactStatus();
      Sitecore.Analytics.Tracking.Contact contact = this.ContactRepository.LoadContactReadOnly(incomingContact.Identifiers.Identifier);
      if (contact != null)
      {
        status.Contact = contact;
        status.Exists = true;
        ContactContext context = contact as ContactContext;
        if (context == null)
        {
          return status;
        }
        LeaseData lease = context.Lease;
        if (((lease != null) && (lease.Owner != null)) && ((lease.Owner.Identifier != null) && (lease.ExpirationTime > DateTime.UtcNow)))
        {
          status.IsLeased = true;
        }
      }
      return status;
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

    public class SimplifiedContact
    {
      public string Name;
      public float Latitude;
      public float Longitude;

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

    [HttpPost]
    public JsonResult GetNContacts(int n)
    {
      List<SimplifiedContact> list = new List<SimplifiedContact>();
      for (int i = 0; i <= n; i++)
      {
        Contact contact = CreateContact();

        list.Add(new SimplifiedContact(contact));
      }

      return Json(list);
    }
  }

}
