$(function () {
      $("#labelId").bind("click", function () {
       
          $.ajax({
            url: "/GetContact",
            type: "POST",            
            context: this,
            success: function (data) {
              var BookString = "Book Title: " + data.ContactId;
              $("#BookDetail").text(BookString);
              console.log("success", data);
            },
            error: function (data) {
              console.log("error", data);
            }
          });
        
      });
      $("#generateoneid").bind("click", function () {
        getOneContact();
      });

      $("#generatetenid").bind("click", function () {
        for (i = 1; i < 10; i++) {
          getOneContact();
        }
      });

      $("#generateNcontacts").bind("click", function () {
        var value = document.getElementById('numberOfContacts').value;
        getNContacts(value);
      });

      
      function getOneContact(){
        $.ajax({
          url: "/GetSimplifiedContact",
          type: "POST",
          context: this,
          success: function (data) {
           
            var point = new Object();
            point.latitude = data.Latitude;
            point.longitude = data.Longitude;
            point.title = data.Name;
            addContact(point);
            console.log("success", data);
          },
          error: function (data) {
            console.log("error", data);
          }
        });
      }
     
      function getNContacts(number) {
        $.ajax({
          url: "/GetNContacts",
          type: "POST",
          data: { n: number },
          context: this,
          success: function (data) {
            console.log("success", data);

            $.each(data, function (index, contact) {
              var point = new Object();
              point.latitude = contact.Latitude;
              point.longitude = contact.Longitude;
              point.title = contact.Name;
              addContact(point);
            });
          },
          error: function (data) {
            console.log("error", data);
          }
        });
      }
    });

