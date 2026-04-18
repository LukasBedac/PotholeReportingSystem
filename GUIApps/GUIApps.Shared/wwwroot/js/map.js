window.mainMapInstance = null;

window.initMap = function (elementId, markers, centerLat, centerLng, zoom) {
     // Basic check to see if map is already initialized on this element
    var container = L.DomUtil.get(elementId);
    if(container != null){
        if (window.mainMapInstance) {
            window.mainMapInstance.remove();
            window.mainMapInstance = null;
        }
        container._leaflet_id = null;
    }
    
    // Default to Slovakia center if no coords provided
    var lat = centerLat || 48.669;
    var lng = centerLng || 19.699;
    var z = zoom || 8;

    var map = L.map(elementId).setView([lat, lng], z);
    window.mainMapInstance = map;

    L.tileLayer('https://ofmozaika.tiles.freemap.sk/{z}/{x}/{y}.jpg', {
        maxZoom: 19,
        attribution: '&copy; <a href="https://www.freemap.sk">FreeMap</a>, &copy; <a href="https://www.freemap.sk">GKÚ, NLC</a>'
    }).addTo(map);

    if (markers) {
        markers.forEach(function (m) {
            if (m.lat && m.lng) {
                var popupContent = `<div style="min-width: 150px; text-align: center;">
                                        <strong>${m.category}</strong>
                                        <div style="margin: 4px 0;">
                                            <span style="background-color: #eee; padding: 2px 6px; border-radius: 4px; font-size: 0.8em;">${m.status}</span>
                                        </div>
                                        <img src="${m.imageUrl}" style="width: 100%; max-height: 120px; object-fit: cover; border-radius: 4px; margin: 4px 0;" onerror="this.style.display='none'" />
                                        <div style="font-size: 0.9em; color: #555;">${m.description || ''}</div>
                                    </div>`;              
                L.marker([m.lat, m.lng]).addTo(map)
                    .bindPopup(popupContent);
            }
        });
    }
};

window.focusMap = function(lat, lng) {
    if(window.mainMapInstance) {
        window.mainMapInstance.setView([lat, lng], 15);
    }
};

window.getUserLocation = function () {
    return new Promise(function (resolve, reject) {
        if (!navigator.geolocation) {
            reject("Geolocation is not supported by your browser");
        } else {
            navigator.geolocation.getCurrentPosition(function (position) {
                resolve([position.coords.latitude, position.coords.longitude]);
            }, function (error) {
                reject("Unable to retrieve your location: " + error.message);
            });
        }
    });
};

let pickerMapInstance = null;
let pickerMarker = null;
let pickerDotNetHelper = null;

window.initPickerMap = function (elementId, dotNetHelper, initialLat, initialLng) {
    pickerDotNetHelper = dotNetHelper;
    var container = L.DomUtil.get(elementId);
    if(container != null){
         if(pickerMapInstance) { 
             pickerMapInstance.remove(); 
             pickerMapInstance = null;
         }
         container._leaflet_id = null;
    }
    
    // Default to Slovakia center if no coords provided
    var lat = initialLat || 48.669;
    var lng = initialLng || 19.699;
    var z = (initialLat && initialLng) ? 14 : 8;

    pickerMapInstance = L.map(elementId).setView([lat, lng], z);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution: '&copy; OpenStreetMap contributors'
    }).addTo(pickerMapInstance);
    
    if(initialLat && initialLng) {
        pickerMarker = L.marker([lat, lng], { draggable: true }).addTo(pickerMapInstance);
        pickerMarker.on('dragend', function(e) {
            var pos = pickerMarker.getLatLng();
            dotNetHelper.invokeMethodAsync('UpdateLocation', pos.lat, pos.lng);
        });
    } else {
        pickerMarker = null;
    }

    // click on map
    pickerMapInstance.on('click', function(e) {
        var coord = e.latlng;
        if(pickerMarker) {
            pickerMarker.setLatLng(coord);
        } else {
            pickerMarker = L.marker(coord, {draggable: true}).addTo(pickerMapInstance);
            pickerMarker.on('dragend', function(e) {
                var pos = pickerMarker.getLatLng();
                dotNetHelper.invokeMethodAsync('UpdateLocation', pos.lat, pos.lng);
            });
        }
        dotNetHelper.invokeMethodAsync('UpdateLocation', coord.lat, coord.lng);
    });

    // Try geolocation if no initial coords
    if (navigator.geolocation && (!initialLat || !initialLng)) {
        navigator.geolocation.getCurrentPosition(function(position) {
            var pos = {
                lat: position.coords.latitude,
                lng: position.coords.longitude
            };
            pickerMapInstance.setView([pos.lat, pos.lng], 14);
            if(pickerMarker) {
                pickerMarker.setLatLng(pos);
            } else {
                pickerMarker = L.marker(pos, {draggable: true}).addTo(pickerMapInstance);
                pickerMarker.on('dragend', function(e) {
                    var mpos = pickerMarker.getLatLng();
                    dotNetHelper.invokeMethodAsync('UpdateLocation', mpos.lat, mpos.lng);
                });
            }
            dotNetHelper.invokeMethodAsync('UpdateLocation', pos.lat, pos.lng);
        }, function(error) {
            console.log("Geolocation error or denied:", error);
        });
    }
};

window.updatePickerMapPosition = function (lat, lng) {
    if (pickerMapInstance) {
        pickerMapInstance.setView([lat, lng], 14);
        if (pickerMarker) {
            pickerMarker.setLatLng([lat, lng]);
        } else {
            pickerMarker = L.marker([lat, lng], {draggable: true}).addTo(pickerMapInstance);
            if (pickerDotNetHelper) {
                pickerMarker.on('dragend', function(e) {
                    var pos = pickerMarker.getLatLng();
                    pickerDotNetHelper.invokeMethodAsync('UpdateLocation', pos.lat, pos.lng);
                });
            }
        }
    }
};