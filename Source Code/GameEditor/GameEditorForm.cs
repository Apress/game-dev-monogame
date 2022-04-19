using PipelineExtensions;
using System.Windows.Forms;

namespace GameEditor
{
    public partial class GameEditorForm : Form
    {
        public GameEditorForm()
        {
            InitializeComponent();

            gameControl.ClientSize = new System.Drawing.Size(1280, 720);
            gameControl.OnInitialized += GameControl_OnInitialized;
            gameControl.OnEventSelected += GameControl_OnEventSelected;
            gameControl.OnEventDeselected += GameControl_OnEventDeselected;

            listViewScreenEvents.Items.Add(typeof(GameEditorGenerate2Choppers).Name);
            listViewScreenEvents.Items.Add(typeof(GameEditorGenerate4Choppers).Name);
            listViewScreenEvents.Items.Add(typeof(GameEditorGenerate6Choppers).Name);
            listViewScreenEvents.Items.Add(typeof(GameEditorStartLevel).Name);
            listViewScreenEvents.Items.Add(typeof(GameEditorEndLevel).Name);

            comboLevelNb.SelectedIndex = 0;
        }

        private void GameControl_OnEventSelected(object sender, EventSelectedArgs e)
        {
            groupBoxEventDetails.Visible = true;
            labelEventDetails.Text = e.GameEditorEvent.GetType().Name;
        }

        private void GameControl_OnEventDeselected(object sender, System.EventArgs e)
        {
            groupBoxEventDetails.Visible = false;
        }

        private void GameControl_OnInitialized(object sender, System.EventArgs e)
        {
            InitializeListsOfTiles();

            groundListView.ItemSelectionChanged += GroundListView_ItemSelectionChanged;
            buildingsListView.ItemSelectionChanged += BuildingsListView_ItemSelectionChanged;
            objectsListView.ItemSelectionChanged += ObjectsListView_ItemSelectionChanged;
            listViewScreenEvents.ItemSelectionChanged += ListViewScreenEvents_ItemSelectionChanged;

            objectTabControl.SelectedIndexChanged += ObjectTabControl_SelectedIndexChanged;
        }

        private void ObjectTabControl_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            gameControl.CurrentLayer = objectTabControl.SelectedTab.Text;
            gameControl.CurrentElementName = null;
        }

        private void GroundListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            gameControl.CurrentElementName = e.Item.Text;
        }

        private void BuildingsListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            gameControl.CurrentElementName = e.Item.Text;
        }

        private void ObjectsListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            gameControl.CurrentElementName = e.Item.Text;
        }
        private void ListViewScreenEvents_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            gameControl.CurrentElementName = e.Item.Text;
        }

        private void InitializeListsOfTiles()
        {
            foreach (var tile in gameControl.Atlas[GameControl.GROUND])
            {
                groundListView.Items.Add(tile.Name);
            }

            foreach (var tile in gameControl.Atlas[GameControl.BUILDINGS])
            {
                buildingsListView.Items.Add(tile.Name);
            }

            foreach (var gameObject in gameControl.GameObjects)
            {
                objectsListView.Items.Add(gameObject.Key);
            }
        }

        private void buttonLoad_Click(object sender, System.EventArgs e)
        {
            gameControl.LoadLevel();
        }

        private void buttonSave_Click_1(object sender, System.EventArgs e)
        {
            gameControl.SaveCurrentLevel();
        }

        private void comboLevelNb_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            gameControl.CurrentLevel = comboLevelNb.SelectedIndex + 1;
        }
    }
}
