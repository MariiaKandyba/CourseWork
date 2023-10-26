using DALTest.Entities;
using System;
using System.Windows;

namespace Server.Views
{
    public partial class CreateEditGroup : Window
    {
        public Group Group { get; set; }

        public CreateEditGroup(Group group = null)
        {
            InitializeComponent();
            Group = group ?? new Group();
            InitializeGroupFields();
        }

        private void InitializeGroupFields()
        {
            NameTextBox.Text = Group.Name;
            DescriptionTextBox.Text = Group.Description;
            IsAdminGroupCheckBox.IsChecked = Group.IsAdminGroup;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            UpdateGroupFromFields();
            DialogResult = true;
            Close();
        }

        private void UpdateGroupFromFields()
        {
            Group.Name = NameTextBox.Text;
            Group.Description = DescriptionTextBox.Text;
            Group.IsAdminGroup = IsAdminGroupCheckBox.IsChecked ?? false;
        }
    }
}
