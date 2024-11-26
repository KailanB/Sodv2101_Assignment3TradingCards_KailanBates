using System;
using System.ComponentModel;
using System.Data;
using System.Numerics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Sodv2101_Assignment3TradingCards_KailanBates
{
	public partial class Form1 : Form
	{

		Rectangle PlayerCardArea = new Rectangle(350, 15, 400, 475);
		Rectangle PlayerBorderRectangle = new Rectangle(375, 35, 350, 450);
		private int red = 255;
		private int green;
		private int blue;


		private BindingList<Player> players;

		public Form1()
		{
			InitializeComponent();
			LoadData();

			// update table when changing list
			players.ListChanged += ReLoadPlayerTable;
		}


		// used to populate player list and initially display the table
		private void LoadData()
		{
			players = new BindingList<Player>(PlayerData.GetPlayers());

			DataTable table = new DataTable();
			table.Columns.Add("Name", typeof(string));
			table.Columns.Add("Team", typeof(string));
			table.Columns.Add("ID", typeof(int));
			foreach (var player in players)
			{
				table.Rows.Add(player.Name, player.Team, player.Id);
			}
			dataGridPlayers.DataSource = table;
			dataGridPlayers.SelectionChanged += DataGrid_PlayerSelected;
			//dataGridPlayerStats.DataSource = dataGridPlayers.SelectedRows[0];

		}

		// used when player list changes to update table
		private void ReLoadPlayerTable(object sender, EventArgs e)
		{
			DataTable table = new DataTable();
			table.Columns.Add("Name", typeof(string));
			table.Columns.Add("Team", typeof(string));
			table.Columns.Add("ID", typeof(int));
			foreach (var player in players)
			{
				table.Rows.Add(player.Name, player.Team, player.Id);
			}
			dataGridPlayers.DataSource = table;
		}

		// resets data binding when the user selects a new player
		private void DataGrid_PlayerSelected(object sender, EventArgs e)
		{
			if (dataGridPlayers.SelectedRows.Count > 0 && dataGridPlayers.SelectedRows.Count < 2)
			{
				lblName.DataBindings.Clear();
				lblTeam.DataBindings.Clear();
				lblInterceptions.DataBindings.Clear();
				lblPassesDefended.DataBindings.Clear();
				lblTotalTackles.DataBindings.Clear();
				lblSoloTackles.DataBindings.Clear();

				int playerId = Convert.ToInt32(dataGridPlayers.SelectedRows[0].Cells["ID"].Value);
				var player = players.Where(p => p.Id == playerId).SingleOrDefault();


				lblName.DataBindings.Add("Text", player, "Name", true, DataSourceUpdateMode.OnPropertyChanged);
				lblTeam.DataBindings.Add("Text", player, "Team", true, DataSourceUpdateMode.OnPropertyChanged);

				lblInterceptions.DataBindings.Add("Text", player, "Interceptions", true, DataSourceUpdateMode.OnPropertyChanged);
				lblPassesDefended.DataBindings.Add("Text", player, "PassesDefended", true, DataSourceUpdateMode.OnPropertyChanged);
				lblTotalTackles.DataBindings.Add("Text", player, "TotalTackles", true, DataSourceUpdateMode.OnPropertyChanged);
				lblSoloTackles.DataBindings.Add("Text", player, "SoloTackles", true, DataSourceUpdateMode.OnPropertyChanged);

				picBoxPlayer.ImageLocation = player.Image;

				red = player.TeamColorR;
				green = player.TeamColorG;
				blue = player.TeamColorB;

				this.Invalidate(PlayerCardArea);
			}

		}


		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			using (Pen pen = new Pen(Color.FromArgb(red, green, blue), 5))
			{
				g.DrawRectangle(pen, PlayerBorderRectangle);
			}
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			CreatePlayerForm createPlayerForm = new CreatePlayerForm();
			DialogResult result = createPlayerForm.ShowDialog();
			if (result == DialogResult.OK)
			{
				players.Add(createPlayerForm.GetPlayer());

			}
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			if (dataGridPlayers.SelectedRows.Count > 0 && dataGridPlayers.SelectedRows.Count < 2)
			{

				int playerId = Convert.ToInt32(dataGridPlayers.SelectedRows[0].Cells["ID"].Value);

				// https://stackoverflow.com/questions/9195727/removing-elements-from-binding-list
				players.Remove(players.First(player => player.Id == playerId));
			}
		}

		private void btnEdit_Click(object sender, EventArgs e)
		{
			if (dataGridPlayers.SelectedRows.Count > 0 && dataGridPlayers.SelectedRows.Count < 2)
			{

				int playerId = Convert.ToInt32(dataGridPlayers.SelectedRows[0].Cells["ID"].Value);
				var player = players.Where(p => p.Id == playerId).SingleOrDefault();

				CreatePlayerForm createPlayerForm = new CreatePlayerForm();


				createPlayerForm.ChangeBtnName("Update Player");
				DialogResult result = createPlayerForm.ShowDialog();
				if (result == DialogResult.OK)
				{

					Player updatedPlayer = createPlayerForm.GetPlayer();

					foreach (Player p in players)
					{
						if (p.Id == playerId)
						{
							p.Name = updatedPlayer.Name;
							p.Team = updatedPlayer.Team;
							p.TeamColorR = updatedPlayer.TeamColorR;
							p.TeamColorG = updatedPlayer.TeamColorG;
							p.TeamColorB = updatedPlayer.TeamColorB;
							p.Interceptions = updatedPlayer.Interceptions;
							p.PassesDefended = updatedPlayer.PassesDefended;
							p.TotalTackles = updatedPlayer.TotalTackles;
							p.SoloTackles = updatedPlayer.SoloTackles;
							p.Image = updatedPlayer.Image;
						}
					}
				}
			}
		}
	}
}