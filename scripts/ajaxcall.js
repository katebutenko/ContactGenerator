$(function () {
     
      $("#generateoneid").bind("click", function () {
        getOneContact();
      });

      $("#generatetenid").bind("click", function () {
        for (i = 1; i < 10; i++) {
          getOneContact();
        }
      });

      $("#generateNcontacts").bind("click", function () {
        cleanMap();
        var value = document.getElementById('numberOfContactsSlider').value;
        getNContacts(value);
      });

      $("#showHawaiiVisitors").bind("click", function () {

        cleanMap();

        $.ajax({
          url: "/RunSegmentRuleOnAllContacts",
          type: "POST",
          //data: { n: number },
          context: this,
          success: function (data) {
            console.log("success");

            drawPointsOnMap(data);
            restartRealTimeMapUpdate();
          },
          error: function (data) {
            console.log("error", data);
          }
        });
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
            console.log("success");

            drawPointsOnMap(data);
            restartRealTimeMapUpdate();
          },
          error: function (data) {
            console.log("error", data);
          }
        });
      }

      function drawPointsOnMap(data)
      {
        
        $.each(data, function (index, contact) {

          var point = new Object();
          point.latitude = contact.Latitude;
          point.longitude = contact.Longitude;
          point.title = contact.Name;
          point.destination = contact.DestinationCountry;

          imagesToDraw.push(point);
        })
       }
                      
        
      $(function () {
        var val = $('#numberOfContactsSlider').val();
        output = $('#numberOfContactsOutput');

        output.html(val);

        $('#numberOfContactsSlider').change(function () {
          output.html(this.value);
        });

        $("#numberOfContactsSlider").mousemove(function () {
          $("#numberOfContactsOutput").text($("#numberOfContactsSlider").val())
        })

      });

    });

function readFile(index) {
  
}

