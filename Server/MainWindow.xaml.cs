using Azure;
using DALTest;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Repository;
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
        private readonly GenericUnitOfWork _unitOfWork;
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
        }
    }
}
