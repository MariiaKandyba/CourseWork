using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DALTest.Entities;
using Repository;
using Server.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Server.ViewModels
{
    public class GroupViewModel : ObservableObject
    {
        private readonly IGenericRepository<Group> _groupRepository;

        private ObservableCollection<Group> _groups = new();
        private Group _selectedGroup;

        public GroupViewModel(IGenericRepository<Group> groupRepository)
        {
            _groupRepository = groupRepository;
            Groups = new ObservableCollection<Group>(_groupRepository.GetAll());

            AddGroupCommand = new RelayCommand(OnAddGroupClick);
            EditGroupCommand = new RelayCommand(OnEditGroupClick);
            DeleteGroupCommand = new RelayCommand(OnDeleteGroupClick);
        }

        public Group SelectedGroup
        {
            get { return _selectedGroup; }
            set { SetProperty(ref _selectedGroup, value); }
        }

        public ObservableCollection<Group> Groups
        {
            get { return _groups; }
            set { SetProperty(ref _groups, value); }
        }

        private void OnDeleteGroupClick()
        {
            if (MessageBox.Show("Are you sure you want to delete this product?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _groupRepository.Remove(SelectedGroup);
                Groups = new ObservableCollection<Group>(_groupRepository.GetAll());
            }

        }

        private void OnEditGroupClick()
        {
            CreateEditGroup window = new(SelectedGroup);
            window.ShowDialog();
            if (window.DialogResult ?? false)
            {
                _groupRepository.Update(window.Group);
                Groups = new ObservableCollection<Group>(_groupRepository.GetAll());
            }
        }

        private void OnAddGroupClick()
        {
            CreateEditGroup window = new();
            window.ShowDialog();
            if (window.DialogResult ?? false)
            {
                _groupRepository.Add(window.Group);
                Groups = new ObservableCollection<Group>(_groupRepository.GetAll());
            }

        }

        public IRelayCommand AddGroupCommand { get; }
        public IRelayCommand EditGroupCommand { get; }
        public IRelayCommand DeleteGroupCommand { get; }
    }
}
