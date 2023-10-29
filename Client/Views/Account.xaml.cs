using Client.ViewModels;
using DALTest.Entities;
using Microsoft.Extensions.Options;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client.Views
{
    /// <summary>
    /// Interaction logic for Account.xaml
    /// </summary>
    public partial class Account : Window
    {
        IGenericRepository<User> _userRepository;
        IGenericRepository<Group> _groupRepository;
        IGenericRepository<Test> _testRepository;
        IGenericRepository<Question> _questionRepository;
        IGenericRepository<Answer> _answerRepository;
        IGenericRepository<UserTest> _userTestRepository;
        private readonly AssigmentViewModel _assigmentViewModel;

        public Account(GenericUnitOfWork _unitOfWork, User user)
        {
            InitializeComponent();
            _assigmentViewModel = new AssigmentViewModel(user,_unitOfWork.Repository<UserTest>(), _unitOfWork.Repository<Test>(), _unitOfWork.Repository<Group>(), _unitOfWork.Repository<User>());
            assignedTestTab.DataContext = _assigmentViewModel;
        }
    }
}
