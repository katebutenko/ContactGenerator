/// <reference path="test.js" />
/**
 * SVG path for target icon
 */
var targetSVG = "M9,0C4.029,0,0,4.029,0,9s4.029,9,9,9s9-4.029,9-9S13.971,0,9,0z M9,15.93 c-3.83,0-6.93-3.1-6.93-6.93S5.17,2.07,9,2.07s6.93,3.1,6.93,6.93S12.83,15.93,9,15.93 M12.5,9c0,1.933-1.567,3.5-3.5,3.5S5.5,10.933,5.5,9S7.067,5.5,9,5.5 S12.5,7.067,12.5,9z";

/**
 * SVG path for plane icon
 */
var planeSVG = "m2,106h28l24,30h72l-44,-133h35l80,132h98c21,0 21,34 0,34l-98,0 -80,134h-35l43,-133h-71l-24,30h-28l15,-47";

/**
 * Create the map
 */
var map = AmCharts.makeChart("chartdiv", {
  "type": "map",
  "theme": "chalk",


  "dataProvider": {
    "map": "worldLow",
    "zoomLevel": 2,
    "zoomLongitude": -71,
    "zoomLatitude": 35,
   
    "lines": [], 

    "images": [{    
      "svgPath": targetSVG,
      "title": "Hawaii",
      "latitude": 21.30694,
      "longitude": -157.85833,
      "color": "#C27690"
    }]
  },

  "areasSettings": {
    "unlistedAreasColor": "#8dd9ef"
  },

  "imagesSettings": {
    "color": "#FFFFFF",
    "rollOverColor": "#585869",
    "selectedColor": "#585869",
    "pauseDuration": 0.2,
    "animationDuration": 2.5,
    "adjustAnimationSpeed": true
  },

  //"linesSettings": {
  //  "color": "#585869",
  //  "alpha": 0.4
  //},

});

//map.addListener("click", function (event) {
//  stopRealTimeMapUpdate();
//});

//map.addListener("dragCompleted", function (event) {
//  stopRealTimeMapUpdate();
//});

function addContact(point) {
    
  point.svgPath = targetSVG;
  point.zoomLevel = 5;
  point.scale = 0.7;

  map.dataProvider.images.push(point);
  //map.validateData();
}

var timer = null;

function tick() {
  draw5Images();

  restartRealTimeMapUpdate(); 
};

function restartRealTimeMapUpdate() {
  timer = setTimeout(tick, 100);
};

function stopRealTimeMapUpdate() {
  clearTimeout(timer);
};

var imagesToDraw = [];

function draw5Images() {

  if (imagesToDraw.length > 0) {
    var take = imagesToDraw.splice(0, 5);


    $.each(take, function (index, point) {

      point.svgPath = targetSVG;
      point.zoomLevel = 5;
      point.scale = 0.7;

      if (point.destination == "Hawaii") {
        point.svgPath = targetSVG;
        point.zoomLevel = 5;
        point.scale = 1;
        point.color = "#C27690";
        
        var line = {
          "arc": -0.85,
          "alpha": 0.3,
          "latitudes": [point.latitude, 21.30694],
          "longitudes": [point.longitude, -157.85833]
        };
        line.id = littleId();
        map.dataProvider.lines.push(line);

        var plane = {
            "svgPath": planeSVG,
            "positionOnLine": 0,
            "color": "#FFFFFF",
            "alpha": 0.5,
            "animateAlongLine": true,
            "lineId": line.id,
            "flipDirection": false,
            "loop": false,
            "scale": 0.01,
            "positionScale": 4
        }
        map.dataProvider.images.push(plane);
      }
      map.dataProvider.images.push(point);
    })
    
    redrawWithPreservedPosition();
  }

}

function redrawWithPreservedPosition(){
  //preserve map position
  map.dataProvider.zoomLevel = map.zoomLevel();

  if (map.dataProvider.zoomLatitudeC == undefined) {
    map.dataProvider.zoomLatitudeC = map.dataProvider.zoomLatitude = map.zoomLatitude();// - weird bug with map moving
  }
  else {
    map.dataProvider.zoomLatitudeC = map.dataProvider.zoomLatitude;
  }
  map.dataProvider.zoomLongitude = map.dataProvider.zoomLongitudeC = map.zoomLongitude();
  map.validateData();
}

function littleId() {
  return ("0000" + (Math.random() * Math.pow(36, 4) << 0).toString(36)).slice(-4)
}

function cleanMap() {
  var hawaiiPoint = {
    "svgPath": targetSVG,
    "title": "Hawaii",
    "latitude": 21.30694,
    "longitude": -157.85833,
    "color": "#C27690"
  }
  map.dataProvider.images = [];
  map.dataProvider.lines = [];
  map.dataProvider.images.push(hawaiiPoint);
  redrawWithPreservedPosition();
}