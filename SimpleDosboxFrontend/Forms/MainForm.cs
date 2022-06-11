using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SimpleDosboxFrontend.Data;
using SimpleDosboxFrontend.Run;

namespace SimpleDosboxFrontend.Forms
{
    class MainForm : Form
    {
        private readonly ImageList _iconImageList;

        private IProfileService ProfileService
        {
            get { return Ioc.Get<IProfileService>(); }
        }

        public MainForm()
            : base()
        {
            _iconImageList = new ImageList() { ImageSize = new Size(32, 32) };

            Build();
            RegisterEvents();

            FillProfileList();
        }

        #region Build

        private void Build()
        {
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Width = 500;
            Height = 400;
            Text = "Simple DOSBox Frontend";
            StartPosition = FormStartPosition.CenterScreen;
            Icon = Properties.Resources.Icon;

            var root = new TableLayoutPanel();
            root.Dock = DockStyle.Fill;
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            // Add content
            var content = BuildContent();
            root.Controls.Add(content);
            root.SetRow(content, 0);

            // Finalize building
            Controls.Add(root);
        }

        private Control BuildContent()
        {
            var content = new TableLayoutPanel();
            content.Dock = DockStyle.Fill;
            content.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            content.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // List on the top
            var list = new ListView();
            list.Name = "list";
            list.Dock = DockStyle.Fill;
            content.Controls.Add(list);
            content.SetRow(list, 0);

            list.View = View.LargeIcon;
            list.LargeImageList = _iconImageList;
            list.FullRowSelect = true;
            list.MultiSelect = false;
            list.Columns.Add(new ColumnHeader() { Text = "Name", Width = 150 });
            list.Columns.Add(new ColumnHeader() { Text = "Developer", Width = 100 });
            list.Columns.Add(new ColumnHeader() { Text = "Play count", Width = 80 });
            list.Columns.Add(new ColumnHeader() { Text = "Play duration", Width = 100 });
            list.SelectedIndexChanged += list_SelectedIndexChanged;
            list.MouseDoubleClick += list_MouseDoubleClick;
            list.KeyUp += list_KeyUp;

            // Details group on the bottom
            var details = new TableLayoutPanel();
            details.Dock = DockStyle.Fill;
            details.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            details.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60f));
            details.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40f));
            content.Controls.Add(details);
            content.SetRow(details, 1);

            // Detail text box
            var txtDetails = new TextBox();
            txtDetails.Name = "txtDetails";
            txtDetails.Dock = DockStyle.Fill;
            txtDetails.BorderStyle = BorderStyle.None;
            txtDetails.ScrollBars = ScrollBars.Both;
            txtDetails.ReadOnly = true;
            txtDetails.Multiline = true;
            details.Controls.Add(txtDetails);
            details.SetColumn(txtDetails, 0);
            details.SetRow(txtDetails, 0);


            // Preview image
            var imgCapture = new PictureBox();
            imgCapture.Name = "imgCapture";
            imgCapture.Dock = DockStyle.Fill;
            imgCapture.SizeMode = PictureBoxSizeMode.Zoom;
            details.Controls.Add(imgCapture);
            details.SetColumn(imgCapture, 1);
            details.SetRow(imgCapture, 0);
            return content;
        }

        #endregion

        private void list_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                RunSelectedProfile();
            }
        }

        private void list_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDetailsForCurrentSelection();
        }

        private void list_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            RunSelectedProfile();
        }

        private void RegisterEvents()
        {
            ProfileService.ProfileUpdated += OnProfileUpdated;
        }

        private void OnProfileUpdated(object sender, ProfileUpdatedEventArgs e)
        {
            Invoke((Action)(() =>
            {
                UpdateProfileInList(e.Profile);
                UpdateDetailsForCurrentSelection();
            }));
        }

        private void UpdateDetailsForCurrentSelection()
        {
            var txtDetails = this.FindControl<TextBox>("txtDetails");
            txtDetails.Clear();

            var imgCapture = this.FindControl<PictureBox>("imgCapture");
            imgCapture.Image = null;

            var list = this.FindControl<ListView>("list");

            if (list.SelectedItems.Count == 0)
            {
                return;
            }

            var profile = (Profile)list.SelectedItems[0].Tag;

            var text = new StringBuilder();

            text.AppendLine(profile.Name);

            if (!string.IsNullOrWhiteSpace(profile.Developer))
            {
                text.AppendLine();
                text.AppendFormat("By {0}", profile.Developer);
            }

            text.AppendLine();
            text.AppendFormat("Played {0} time(s), {1:hh\\:mm\\:ss} in total", profile.PlayCount, profile.PlayDuration);

            if (profile.LastPlayedAt != default)
            {
                text.AppendLine();
                text.AppendFormat("Last played at {0:F}", profile.LastPlayedAt);
            }

            if (!string.IsNullOrWhiteSpace(profile.Description))
            {
                text.AppendLine();
                text.AppendLine();
                text.Append(profile.Description);
            }

            txtDetails.Text = text.ToString();

            imgCapture.Image = ProfileService.GetPreviewImage(profile);
        }

        private void RunSelectedProfile()
        {
            var list = this.FindControl<ListView>("list");

            if (list.SelectedItems.Count != 1)
            {
                return;
            }

            var profile = (Profile)list.SelectedItems[0].Tag;

            var runService = Ioc.Get<IRunService>();
            runService.Run(profile);
        }

        private void FillProfileList()
        {
            var target = this.FindControl<ListView>("list");

            target.Items.Clear();
            _iconImageList.Images.Clear();

            var itemsToAdd = new List<ListViewItem>();

            foreach (var profile in ProfileService.GetProfiles())
            {
                var item = new ListViewItem(profile.Name);
                item.Name = profile.Name;
                item.SubItems.Add(profile.Developer);
                item.SubItems.Add(profile.PlayCount.ToString());
                item.SubItems.Add(string.Format("{0:hh\\:mm\\:ss}", profile.PlayDuration));
                item.Tag = profile;

                var icon = ProfileService.GetProfileImage(profile);
                var iconIndex = _iconImageList.Images.Add(icon, Color.Transparent);
                item.ImageIndex = iconIndex;

                itemsToAdd.Add(item);
            }

            target.Items.AddRange(itemsToAdd.ToArray());

            var txtDetails = this.FindControl<TextBox>("txtDetails");
            txtDetails.Clear();

            var imgCapture = this.FindControl<PictureBox>("imgCapture");
            imgCapture.Image = null;
        }

        private void UpdateProfileInList(Profile profile)
        {
            var target = this.FindControl<ListView>("list");

            var item = target.Items.Find(profile.Name, false).FirstOrDefault();

            if (item != null)
            {
                item.SubItems[2].Text = profile.PlayCount.ToString();
                item.SubItems[3].Text = string.Format("{0:hh\\:mm\\:ss}", profile.PlayDuration);
            }
        }
    }
}