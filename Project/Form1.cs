using System;
using System.Windows.Forms;

namespace TestMap
{
    public partial class Form1 : Form
    {
        Map map;

        public Form1()
        {
            InitializeComponent();
            map = new Map(gMapControl1);
            map.OnSelectedMarkerChange += OnSelectedMarkerChange;
            groupBox2.Visible = false;
            var dbMarkers = DBClient.GetAllMarkers();
            for (int i = 0; i < dbMarkers.Length; i++)
                map.MarkerCreate(dbMarkers[i]);
        }

        private void MarkerCreate(object sender, EventArgs e)
        {
            var newMarker = new Marker()
            {
                Position = map.Position,
                Name = "NewMarker"
            };
            map.MarkerCreate(newMarker);
        }

        private void MarkerName_Changed(object sender, EventArgs e)
        {
            if (map.SelectedGMarker is null) return;
            map.MarkerTextChange(textBox1.Text);
        }


        private void OnSelectedMarkerChange()
        {
            if (map.SelectedGMarker is null)
            {
                groupBox2.Visible = false;
                button2.Visible = false;
            }
            else
            {
                groupBox2.Visible = true;
                button2.Visible = true;
                textBox1.Text = map.SelectedGMarker.ToolTipText;
                textBox2.Text = map.SelectedGMarker.Position.Lat.ToString();
                textBox3.Text = map.SelectedGMarker.Position.Lng.ToString();
            }
        }

        private void MarkerDelete(object sender, EventArgs e)
        {
            if (map.SelectedGMarker is null) return;
            map.MarkerDelete(map.SelectedMarker);
        }
    }
}
