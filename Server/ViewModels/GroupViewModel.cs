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
        private readonly IGenericRepository<User> _userRepository;
        private ObservableCollection<Group> _groups = new();
        private Group _selectedGroup;
        private ObservableCollection<User> _users = new();

        public GroupViewModel(IGenericRepository<Group> groupRepository, IGenericRepository<User> userRepository)
        {
            _groupRepository = groupRepository;
            _userRepository = userRepository;
            Groups = new ObservableCollection<Group>(_groupRepository.GetAll());
            Users = new ObservableCollection<User>(_userRepository.GetAll());

            AddGroupCommand = new RelayCommand(OnAddGroupClick);
            EditGroupCommand = new RelayCommand(OnEditGroupClick);
            DeleteGroupCommand = new RelayCommand(OnDeleteGroupClick);
            AddUserToGroupCommand = new RelayCommand(OnAddUserToGroupClick);
        }

        private void OnAddUserToGroupClick()
        {
            if(SelectedGroup != null)
            {
                List<User> usersInGroup = SelectedGroup.Users.ToList();
                List<User> otherUsers = (Users.ToList().Except(usersInGroup).ToList());
                ManageGroupsWindow window = new(usersInGroup, otherUsers);
                if (window.ShowDialog() == true)
                {
                    SelectedGroup.Users = new ObservableCollection<User>( window.UsersInGroup);
                    _groupRepository.Update(SelectedGroup);
                }

            }

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

        public ObservableCollection<User> Users // Властивість для користувачів
        {
            get { return _users; }
            set { SetProperty(ref _users, value); }
        }

        private void OnDeleteGroupClick()
        {
            if (MessageBox.Show("Are you sure you want to delete this group?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
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
        public IRelayCommand AddUserToGroupCommand { get; }
    }
}
