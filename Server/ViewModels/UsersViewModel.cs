using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DALTest.Entities;
using NetworkDataDll;
using Repository;
using Server.Helpers;
using Server.Views;
using Server.Views.Users;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Security.Cryptography;


namespace Server.ViewModels
{
    public class UsersViewModel : ObservableObject
    {
        RepositoryFilter _helper;
        private readonly IGenericRepository<User> _userRepository;

        private ObservableCollection<User> _users = new ObservableCollection<User>();
        private User _selectedUser;

        public UsersViewModel(IGenericRepository<User> userRepository, RepositoryFilter helper)
        {
            _userRepository = userRepository;
            _helper = helper;
            LoadActualList();

            AddUserCommand = new RelayCommand<object>(OnAddUserClick);
            EditUserCommand = new RelayCommand<object>(OnEditUserClick);
            DeleteUserCommand = new RelayCommand(OnDeleteUserClick);
            RestoreUserCommand = new RelayCommand(OnRestoreUserClick);
            UpdateCommand = new RelayCommand(OnUpdateUserClick);
            SeeResultsCommand = new RelayCommand(OnResultsClick);
        }

        private void OnResultsClick()
        {
            if(SelectedUser != null)
            {
                List<TestResults> testResults = _helper.GetTestResults(SelectedUser.Id, true);
                UsersResultsWindow window = new(testResults);
                window.Show();
            }
           
        }

        private void OnUpdateUserClick()
        {
            LoadActualList();
        }

        private void OnRestoreUserClick()
        {

            RestoreUserWindow window = new(_userRepository.FindAll(x => x.IsArchived).ToList());
            window.ShowDialog();

            if (window.DialogResult == true)
            {
                foreach (var selectedItem in window.SelectedUsers)
                {
                    var userToUpdate = _userRepository.FindById(selectedItem.Id);
                    if (userToUpdate != null && userToUpdate.IsArchived)
                    {
                        userToUpdate.IsArchived = false;
                        _userRepository.Update(userToUpdate);
                    }
                }
                LoadActualList();
            }
        }

        private void LoadActualList()
        {
            Users = new ObservableCollection<User>(_userRepository.FindAll(x => !x.IsArchived));
        }
        private void OnDeleteUserClick()
        {
            if(SelectedUser != null)
            {
                if (MessageBox.Show("Are you sure you want to delete this user?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    SelectedUser.IsArchived = true;
                    _userRepository.Update(SelectedUser);
                    LoadActualList();
                }
            }
        }

        private void OnEditUserClick(object? obj)
        {
            if(SelectedUser!= null)
            {
                CreateEditUser window = new(SelectedUser);
                window.ShowDialog();
                if (window.DialogResult ?? false)
                {
                    window.User.Password = HashPassword(window.User.Password);
                    _userRepository.Update(window.User);
                    LoadActualList();
                }
            }
        }

        private void OnAddUserClick(object? obj)
        {
            CreateEditUser window = new();
            window.ShowDialog();
            if (window.DialogResult ?? false)
            {
                window.User.Password = HashPassword(window.User.Password);
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
        public IRelayCommand RestoreUserCommand { get; }
        public IRelayCommand UpdateCommand { get; }
        public IRelayCommand SeeResultsCommand { get; }

        private string HashPassword(string password)
        {
            using (var sha256 = new SHA256Managed())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashedPasswordBytes = sha256.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hashedPasswordBytes);
            }
        }
    }
}
