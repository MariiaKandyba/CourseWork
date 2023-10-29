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
    public class UsersViewModel : ObservableObject
    {
        private readonly IGenericRepository<User> _userRepository;

        private ObservableCollection<User> _users = new ObservableCollection<User>();
        private User _selectedUser;

        public UsersViewModel(IGenericRepository<User> userRepository)
        {
            _userRepository = userRepository;
            LoadActualList();

            AddUserCommand = new RelayCommand<object>(OnAddUserClick);
            EditUserCommand = new RelayCommand<object>(OnEditUserClick);
            DeleteUserCommand = new RelayCommand(OnDeleteUserClick);
        }
        private void LoadActualList()
        {
            Users = new ObservableCollection<User>(_userRepository.GetAll().Where(x => !x.IsArchived));

        }
        private void OnDeleteUserClick()
        {
            if (MessageBox.Show("Are you sure you want to delete this product?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                SelectedUser.IsArchived = true;
                _userRepository.Update(SelectedUser);
                LoadActualList();

            }

        }

        private void OnEditUserClick(object? obj)
        {
            CreateEditUser window = new(SelectedUser);
            
            window.ShowDialog();
            if (window.DialogResult ?? false)
            {
                _userRepository.Update(window.User);
                LoadActualList();
            }
        }

        private void OnAddUserClick(object? obj)
        {
            CreateEditUser window = new();
            window.ShowDialog();
            if (window.DialogResult ?? false)
            {
                _userRepository.Add(window.User);
                LoadActualList();
            }

        }

        public User SelectedUser
        {
            get { return _selectedUser; }
            set { SetProperty(ref _selectedUser, value); }
        }

        public ObservableCollection<User> Users
        {
            get { return _users; }
            set { SetProperty(ref _users, value); }
        }
        public IRelayCommand<object> AddUserCommand { get; }
        public IRelayCommand<object> EditUserCommand { get; }
        public IRelayCommand DeleteUserCommand { get; }



    }
}
