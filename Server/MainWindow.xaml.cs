using Azure;
using DALTest;
using DALTest.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Repository;
using Server.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<Group> _groupRepository;
        private readonly IGenericRepository<Test> _testRepository;
        private readonly IGenericRepository<Question> _questionRepository;
        private readonly IGenericRepository<Answer> _answerRepository;
        private readonly IGenericRepository<UserTest> _userTestRepository;
        private readonly GenericUnitOfWork _unitOfWork;

        private readonly UsersViewModel _usersViewModel;
        private readonly GroupViewModel _groupViewModel;
        private readonly AssigmentViewModel _assigmentViewModel;
        private readonly ServerViewModel _serverViewModel;
        public MainWindow()
        {
            InitializeComponent();
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            var config = builder.Build();
            string conStr = config.GetConnectionString("DefaultConnection")!;

            var optionsBuilder = new DbContextOptionsBuilder<Context>();
            var options = optionsBuilder.UseLazyLoadingProxies().UseSqlServer(conStr).Options;
            _unitOfWork = new GenericUnitOfWork(new Context(options));
            _userRepository = _unitOfWork.Repository<User>();
            _groupRepository = _unitOfWork.Repository<Group>();
            _testRepository = _unitOfWork.Repository<Test>();
            _questionRepository = _unitOfWork.Repository<Question>();
            _answerRepository = _unitOfWork.Repository<Answer>();
            _userTestRepository = _unitOfWork.Repository<UserTest>();

            _usersViewModel = new UsersViewModel(_userRepository);
            _groupViewModel = new GroupViewModel(_groupRepository, _userRepository);
            _assigmentViewModel = new AssigmentViewModel(_userTestRepository, _testRepository, _groupRepository, _userRepository, _questionRepository, _answerRepository);
            _serverViewModel = new ServerViewModel();

            usersTab.DataContext = _usersViewModel;
            groupsTab.DataContext = _groupViewModel;
            assignedTestTab.DataContext = _assigmentViewModel;
            serversTab.DataContext = _serverViewModel;
        }
    }
}
