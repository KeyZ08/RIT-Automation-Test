using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

namespace TestMap
{
    public class Map
    {
        private GMapControl _gMap;
        private GMapOverlay _gMapMarkers;
        private PointLatLng _defaultPos = new PointLatLng(66.4169575018027, 94.25025752215694);
        private MouseButtons _dragAndDropMouseButton = MouseButtons.Left;

        private List<(Marker, GMapMarker)> _markers;

        public PointLatLng Position { get { return _gMap.Position; } }

        private GMapMarker _selectedGMarker;
        public GMapMarker SelectedGMarker
        {
            get { return _selectedGMarker; }
            private set
            { 
                _selectedGMarker = value;
                _selectedMarker = GetMarker(_selectedGMarker);
                OnSelectedMarkerChange.Invoke(); 
            }
        }

        private Marker _selectedMarker;
        public Marker SelectedMarker
        {
            get { return _selectedMarker; }
        }


        public Map(GMapControl map) 
        { 
            _gMap = map;
            Initialize();
            _markers = new List<(Marker, GMapMarker)>();
        }

        private void Initialize()
        {
            _gMap.Position = _defaultPos;
            _gMap.MapProvider = GoogleMapProvider.Instance;
            _gMap.MouseDown += MouseDown;
            _gMap.MouseMove += MouseMove;
            _gMap.MouseUp += MouseUp;

            _gMapMarkers = new GMapOverlay("Markers");
            _gMap.Overlays.Add(_gMapMarkers);
        }

        private void MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != _dragAndDropMouseButton) return;
            SelectedGMarker = _gMap.Overlays
                .SelectMany(o => o.Markers)
                .LastOrDefault(m => m.IsMouseOver == true);
        }

        private void MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != _dragAndDropMouseButton) return;
            if (SelectedGMarker is null) return;

            var latlng = _gMap.FromLocalToLatLng(e.X, e.Y);
            SelectedGMarker.Position = latlng;
            SelectedMarker.Position = latlng;

            OnSelectedMarkerChange.Invoke();
        }

        private void MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != _dragAndDropMouseButton) return;
            if (SelectedGMarker == null) return;
            DBClient.MarkerUpdate(SelectedMarker);
        }

        public void MarkerCreate(Marker marker)
        {
            var newMarker = GetMarker(marker);
            _gMapMarkers.Markers.Add(newMarker);
            _markers.Add((marker, newMarker));
            SelectedGMarker = newMarker;
            if (marker.Id == 0)
            {
                var id = DBClient.MarkerAdd(marker);
                marker.Id = id;
            }
        }
        
        public void MarkerDelete(Marker marker)
        {
            var markers = _markers.Find(x => x.Item1 == marker);
            _gMapMarkers.Markers.Remove(markers.Item2);
            _markers.Remove(markers);
            SelectedGMarker = null;
            DBClient.MarkerDelete(markers.Item1);
        }

        public void MarkerTextChange(string text)
        {
            SelectedGMarker.ToolTipText = text;
            SelectedMarker.Name = text;
            DBClient.MarkerUpdate(SelectedMarker);
        }

        private GMarkerGoogle GetMarker(Marker marker, GMarkerGoogleType gMarkerGoogleType = GMarkerGoogleType.red)
        {
            GMarkerGoogle mapMarker = new GMarkerGoogle(marker.Position, gMarkerGoogleType);
            mapMarker.ToolTip = new GMap.NET.WindowsForms.ToolTips.GMapRoundedToolTip(mapMarker);
            mapMarker.ToolTipText = marker.Name;
            mapMarker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
            return mapMarker;
        }

        private Marker GetMarker(GMapMarker gMarker)
        {
            return _markers.Find(x => x.Item2 == gMarker).Item1;
        }

        public event Action OnSelectedMarkerChange;
    }
}
