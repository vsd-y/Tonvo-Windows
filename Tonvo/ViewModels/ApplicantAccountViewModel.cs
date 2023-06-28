﻿using System.Collections.ObjectModel;
using System.Configuration;
using System.Windows.Controls;

namespace Tonvo.ViewModels
{
    internal class ApplicantAccountViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly ApplicantService _applicantService;
        private readonly Frame _mainFrame;
        private readonly DbTonvoContext _context;

        private string _password;
        private Applicant _initialApplicant;
        [Reactive] public Applicant CurrentApplicant { get; set; }
        [Reactive] public string SelectedProfession { get; set; }
        [Reactive] public string SelectedCity { get; set; }
        [Reactive] public string SelectedEducation { get; set; }
        [Reactive] public string SelectedStatus { get; set; }

        [Reactive] public string Name { get; set; } = "";
        [Reactive] public string Surname { get; set; } = "";
        [Reactive] public string Patronymic { get; set; } = "";
        [Reactive] public string BirthDate { get; set; } = "";
        [Reactive] public string DesiredSalary { get; set; } = "";
        [Reactive] public string Experience { get; set; } = "";
        [Reactive] public string Phone { get; set; } = "";
        [Reactive] public string Email { get; set; } = "";
        [Reactive] public string Information { get; set; } = "";
        public string Password
        {
            get => _password;
            set
            {
#if !DEBUG //TODO: Убрать "!" при демонстрации проекта в режиме DEBUG
                if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Пароль не может быть пустым");
                if (!value.Any(char.IsPunctuation)) throw new ArgumentException("Пароль должен содержать спецсимволы");
                if (!value.Any(char.IsDigit)) throw new ArgumentException("Пароль должен содержать цифры");
                if (!value.Any(char.IsLetter)) throw new ArgumentException("Пароль должен содержать буквы");
                if (!value.Any(char.IsUpper)) throw new ArgumentException("Пароль должен содержать прописные буквы");
                if (!value.Any(char.IsLower)) throw new ArgumentException("Пароль должен содержать строчные буквы");
                if (string.IsNullOrEmpty(value) || value.Length <= 7) throw new ArgumentException("Пароль должен быть больше 8 символов");
#endif
                this.RaiseAndSetIfChanged(ref _password, value);
            }
        }

        [Reactive] public ObservableCollection<string> Professions { get; set; }
        [Reactive] public ObservableCollection<string> Cities { get; set; }
        [Reactive] public ObservableCollection<string> Educations { get; set; }
        [Reactive] public ObservableCollection<string> Statuses { get; set; }

        public ReactiveCommand<Unit, Unit> ExitAccount { get; }
        public ReactiveCommand<Unit, Unit> CanselEditCommand { get; }
        public ReactiveCommand<Unit, Task> SaveEditCommand { get; }
        public ApplicantAccountViewModel(INavigationService navigationService, ApplicantService applicantService, Frame mainFrame, DbTonvoContext context)
        {
            _navigationService = navigationService;
            _applicantService = applicantService;
            _mainFrame = mainFrame;
            _context = context;

            Professions = new(Task.Run(async () => await _context.Professions.Select(p => p.Name).ToListAsync()).Result);
            Cities = new(Task.Run(async () => await _context.Cities.Select(p => p.City1).ToListAsync()).Result);
            Educations = new(Task.Run(async () => await _context.LevelEducations.Select(p => p.Education).ToListAsync()).Result);
            Statuses = new(Task.Run(async () => await _context.StatusApplicants.Select(p => p.Name).ToListAsync()).Result);

            string userID = System.Configuration.ConfigurationManager.AppSettings["UserID"];

            Task.Run(async () => { 
                CurrentApplicant = await _applicantService.GetByIdAsync(int.Parse(userID));
                _initialApplicant = new Applicant
                {
                    Name = CurrentApplicant.Name,
                    Surname = CurrentApplicant.Surname,
                    Patronymic = CurrentApplicant.Patronymic,
                    Email = CurrentApplicant.Email,
                    BirthDate = CurrentApplicant.BirthDate,
                    DesiredProfession = CurrentApplicant.DesiredProfession,
                    DesiredSalary = CurrentApplicant.DesiredSalary,
                    Experience = CurrentApplicant.Experience,
                    PhoneNumber = CurrentApplicant.PhoneNumber,
                    Information = CurrentApplicant.Information,
                    Password = CurrentApplicant.Password,
                    Status = CurrentApplicant.Status,
                    Education = CurrentApplicant.Education,
                    City = CurrentApplicant.City
                };
                Name = CurrentApplicant.Name;
                Surname = CurrentApplicant.Surname;
                Patronymic = CurrentApplicant.Patronymic;
                Email = CurrentApplicant.Email;
                BirthDate = CurrentApplicant.BirthDate.ToString();
                DesiredSalary = ((int)CurrentApplicant.DesiredSalary).ToString();
                Experience = CurrentApplicant.Experience.ToString();
                Phone = CurrentApplicant.PhoneNumber;
                Information = CurrentApplicant.Information;
                Password = CurrentApplicant.Password;
                SelectedStatus = CurrentApplicant.Status.Name;
                SelectedEducation = CurrentApplicant.Education.Education;
                SelectedCity = CurrentApplicant.City.City1;
                SelectedProfession = CurrentApplicant.DesiredProfession.Name;
            });

            ExitAccount = ReactiveCommand.Create(() =>
            {
                var applicants = Task.Run(async () => await _applicantService.GetList()).Result;
                //Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                //config.AppSettings.Settings["UserID"].Value = "";
                //config.AppSettings.Settings["UserType"].Value = "";
                //config.Save(ConfigurationSaveMode.Modified);
                //System.Configuration.ConfigurationManager.RefreshSection("appSettings");
                //_navigationService.NavigateToPage(_mainFrame, "ApplicantControlPanelView");
            });

            CanselEditCommand = ReactiveCommand.Create(() =>
            {
                Name = _initialApplicant.Name;
                Surname = _initialApplicant.Surname;
                Patronymic = _initialApplicant.Patronymic;
                Email = _initialApplicant.Email;
                BirthDate = _initialApplicant.BirthDate.ToString();
                DesiredSalary = ((int)_initialApplicant.DesiredSalary).ToString();
                Experience = _initialApplicant.Experience.ToString();
                Phone = _initialApplicant.PhoneNumber;
                Information = _initialApplicant.Information;
                Password = _initialApplicant.Password;
                SelectedStatus = _initialApplicant.Status.Name;
                SelectedEducation = _initialApplicant.Education.Education;
                SelectedCity = _initialApplicant.City.City1;
                SelectedProfession = _initialApplicant.DesiredProfession.Name;
            });

            SaveEditCommand = ReactiveCommand.Create(async () =>
            {
                CurrentApplicant.Name = Name;
                CurrentApplicant.Surname = Surname;
                CurrentApplicant.Patronymic = Patronymic;
                CurrentApplicant.Email = Email;
                CurrentApplicant.BirthDate = DateTime.Parse(BirthDate);
                CurrentApplicant.DesiredSalary = decimal.Parse(DesiredSalary);
                CurrentApplicant.Experience = int.Parse(Experience);
                CurrentApplicant.PhoneNumber = Phone;
                CurrentApplicant.Information = Information;
                CurrentApplicant.Password = Password;
                CurrentApplicant.Status = await _context.StatusApplicants.FirstOrDefaultAsync(s => s.Name == SelectedStatus);
                CurrentApplicant.Education = await _context.LevelEducations.FirstOrDefaultAsync(e => e.Education == SelectedEducation);
                CurrentApplicant.City = await _context.Cities.FirstOrDefaultAsync(c => c.City1 == SelectedCity);
                CurrentApplicant.DesiredProfession = await _context.Professions.FirstOrDefaultAsync(p => p.Name == SelectedProfession);

                var applicants = await _applicantService.GetList();
                var item = applicants.First(i => i.Id == CurrentApplicant.Id);
                var index = applicants.IndexOf(item);

                applicants.RemoveAt(index);
                applicants.Insert(index, item);

                // Сохранение изменений в базе данных
                _applicantService.SaveChanges();
            });
        }
    }
}