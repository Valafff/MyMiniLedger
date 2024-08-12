using Dapper;
using MyMiniLedger.BLL.Context;
using MyMiniLedger.WPF.Models;
using MyMiniLedger.WPF.ViewTools;
using MyMiniLedger.WPF.Windows.EditContinuePosition;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyMiniLedger.WPF.WindowsModels
{
    //Блокировка события комбобоксов
    public delegate void UpdateDelegate();
    public delegate void LockDelegate();

    public class EditContinuePositionWindowsModel : BaseNotify
    {
        public event UpdateDelegate UpdateEvent;
        public event LockDelegate LockEvent;

        private ComplexPositionHendler complexPositionsHendler = new ComplexPositionHendler();

        bool block = false;
        bool dateTimeWasChanged = false;
        bool dontEditthis = false;

        private readonly Context _context;

        //Название окна
        private string _titleEditContinuePos = "Редактирование/Продолжение позиции";
        public string TitleEditContinuePosition
        {
            get => _titleEditContinuePos;
            set => SetField(ref _titleEditContinuePos, value);
        }

        //Модель выбранной позиции
        private PositionUIModel? _selectedPosition;
        public PositionUIModel? SelectedPosition
        {
            get => _selectedPosition;
            set => SetField(ref _selectedPosition, value);
        }

        //Оригинальная модель выбранной позиции - для отката изменений
        private PositionUIModel? _originalSelectedPosition;
        public PositionUIModel? DublecateSelectedPosition
        {
            get => _originalSelectedPosition;
            set => SetField(ref _originalSelectedPosition, value);
        }

        private PositionUIModel? _dataGridtSelectedItem;
        public PositionUIModel? DataGridSelectedItem
        {
            get => _dataGridtSelectedItem;
            set => SetField(ref _dataGridtSelectedItem, value);
        }

        //Определение выбранной позиции в cb_Category
        private string? _selectedCategory;
        public string? SelectedCategory
        {
            get => _selectedCategory;
            set => SetField(ref _selectedCategory, value);
        }

        private string? _selectedKind;
        public string? SelectedKind
        {
            get => _selectedKind;
            set => SetField(ref _selectedKind, value);
        }

        //Для корректного переводы в нужный формат
        private DateTime? _tempselectedOpenDate;
        public DateTime? TempSelectedOpenDate
        {
            get => _tempselectedOpenDate;
            set => SetField(ref _tempselectedOpenDate, value);
        }

        private string? _selectedOpenDate;
        public string? SelectedOpenDate
        {
            get => _selectedOpenDate;
            set => SetField(ref _selectedOpenDate, value);
        }

        private string? _selectedCloseDate;
        public string? SelectedCloseDate
        {
            get => _selectedCloseDate;
            set => SetField(ref _selectedCloseDate, value);
        }

        private string? _textKind;
        public string? TextKind
        {
            get => _textKind;
            set => SetField(ref _textKind, value);
        }


        //Тупо записывается название цвета через биндинг

        //один на опен и клоус дэйт
        private string? _tb_date_color;
        public string? TB_DateColor
        {
            get => _tb_date_color;
            set => SetField(ref _tb_date_color, value);
        }

        private string? _tb_category_color;
        public string? TB_CategoryColor
        {
            get => _tb_category_color;
            set => SetField(ref _tb_category_color, value);
        }

        private string? _tb_kind_color;
        public string? TB_KindColor
        {
            get => _tb_kind_color;
            set => SetField(ref _tb_kind_color, value);
        }

        private string? _tb_coincolor;
        public string? TB_CoinColor
        {
            get => _tb_coincolor;
            set => SetField(ref _tb_coincolor, value);
        }

        private string? _tb_statuscolor;
        public string? TB_StatusColor
        {
            get => _tb_statuscolor;
            set => SetField(ref _tb_statuscolor, value);
        }

        //Названия полей позиции для комбобокса
        private string? _selectedCoin;
        public string? SelectedCoin
        {
            get => _selectedCoin;
            set => SetField(ref _selectedCoin, value);
        }

        private string? _selectedStatus;
        public string? SelectedStatus
        {
            get => _selectedStatus;
            set => SetField(ref _selectedStatus, value);
        }

        private List<TotalBalance> _totalBalances;
        public List<TotalBalance> TotalBalances
        {
            get => _totalBalances;
            set => SetField(ref _totalBalances, value);
        }

        //Позиции которые должны подтянуться, если позиция является комплексной, всегда подтягивается первая позиция
        //https://stackoverflow.com/questions/45292905/wpf-collectionview-error-collection-was-modified-enumeration-operation-may-no
        //https://stackoverflow.com/questions/23108045/how-to-make-observablecollection-thread-safe/29288294#29288294
        public ObservableCollection<PositionUIModel> SelectedPositions { get; set; }

        //Ссылка на список позиций из главного окна
        public ObservableCollection<PositionUIModel> MAINPOSITIONSCOLLECTION { get; set; }
        public ObservableCollection<CategoryUIModel> Categories { get; set; }
        //Для ограниченного выбора при фильтрации в комбобоксе отдельного окна
        public ObservableCollection<string> StringCategories { get; set; }
        public ObservableCollection<CoinUIModel> Coins { get; set; }
        public ObservableCollection<string> StringCoins { get; set; }
        public ObservableCollection<KindUIModel> Kinds { get; set; }
        public ObservableCollection<string> StringKinds { get; set; }

        ////Для ограниченного выбора при фильтрации в комбобоксе отдельного окна
        //public ObservableCollection<KindUIModel> TempKinds { get; set; }

        public ObservableCollection<StatusUIModel> Statuses { get; set; }
        public ObservableCollection<string> StringStatuses { get; set; }


        public LambdaCommand UndoSelectedPosition { get; set; }
        public LambdaCommand UpdatePosition { get; set; }
        public LambdaCommand AddComplexPosition { get; set; }
        public LambdaCommand SelectNewPosition { get; set; }
        public LambdaCommand Dp_OpenDateChange { get; set; }
        public LambdaCommand Cb_KindTextChange { get; set; }
        public LambdaCommand cb_CoinSelectionChanged { get; set; }
        public LambdaCommand cb_StatusSelectionChanged { get; set; }
        public LambdaCommand Cb_CategorySelectionChanged { get; set; }
        public LambdaCommand Cb_KindSelected { get; set; }
        public LambdaCommand DeleteComplexPosition { get; set; }
        public LambdaCommand DeleteAllComplexPositionsAtRootKey { get; set; }


        public EditContinuePositionWindowsModel()
        {
            _context = new Context();

            LockEvent += SetBlock;

            //Иницмализируется в классе *xaml.cs
            //Создание выбранной позиции
            SelectedPosition = new PositionUIModel();
            DublecateSelectedPosition = new PositionUIModel();

            SelectedPositions = new ObservableCollection<PositionUIModel>();

            MAINPOSITIONSCOLLECTION = new ObservableCollection<PositionUIModel>(); //Ссылки на все позиции из главного окна

            //Инициализация цвета
            TB_KindColor = "White";

            Categories = new ObservableCollection<CategoryUIModel>(); //Изначально подгруженные категории

            StringCategories = new ObservableCollection<string>();

            Kinds = new ObservableCollection<KindUIModel>();  //Изначально подгруженные виды

            StringKinds = new ObservableCollection<string>();

            //TempKinds = new ObservableCollection<KindUIModel>(); // Не подгружаются

            Coins = new ObservableCollection<CoinUIModel>(); //Изначально подгруженные монеты

            StringCoins = new ObservableCollection<string>();

            Statuses = new ObservableCollection<StatusUIModel>(); //Изначально подгруженные статусы

            StringStatuses = new ObservableCollection<string>(); //Подгружаются из Statuses

            TotalBalances = new List<TotalBalance>();

            //Команды

            Dp_OpenDateChange = new LambdaCommand(execute =>
            {
                dateTimeWasChanged = true;
                DateTime temp = TempSelectedOpenDate.Value + DateTime.Now.TimeOfDay;
                SelectedOpenDate = temp.ToString();
                if (SelectedStatus == "Закрыта" || SelectedStatus == "Closed")
                {
                    SelectedCloseDate = SelectedOpenDate;
                    //Console.WriteLine($"Мы в закрытой позиции дата: {SelectedCloseDate}");
                }
                else if (SelectedStatus == "Открыта" || SelectedStatus == "Open")
                {
                    SelectedCloseDate = "";
                    //Console.WriteLine($"Мы в открытой позиции дата: {SelectedCloseDate}") ;
                }
                SelectedPosition.OpenDate = SelectedOpenDate;
                SelectedPosition.CloseDate = SelectedCloseDate;
                TB_DateColor = "Yellow";
            },
            canExecute => TempSelectedOpenDate != null);


            Cb_CategorySelectionChanged = new LambdaCommand(execute =>
            {
                TB_CategoryColor = "Yellow";
                StringKinds.Clear();
                SetStringKinds(StringKinds, Kinds, SelectedCategory);
                SelectedKind = StringKinds.FirstOrDefault();
            },
            canExecute => !block);

            Cb_KindTextChange = new LambdaCommand(execute =>
            {
                LockEvent();
                if (TextKind != null && TextKind != "")
                {
                    if (!StringKinds.Contains(SelectedKind))
                    {
                        SetStringKinds(StringKinds, Kinds);
                    }
                }
                else
                {
                    SetStringKinds(StringKinds, Kinds);
                }

                if (Kinds.Any(k => k.Kind == TextKind))
                {
                    SelectedCategory = (Kinds.First(k => k.Kind == TextKind)).Category.Category;
                    ValueKindCopy(SelectedPosition.Kind, Kinds, TextKind);
                    TB_CategoryColor = "Yellow";
                }
                block = false;
                TB_KindColor = "Yellow";
            },
            canExecute => true);

            Cb_KindSelected = new LambdaCommand(execute =>
            {
                ValueKindCopy(SelectedPosition.Kind, Kinds, SelectedKind);
                TB_KindColor = "Yellow";
            },
            canExecute => true);

            cb_CoinSelectionChanged = new LambdaCommand(execute =>
            {
                ValueCoinCopy(SelectedPosition.Coin, Coins, SelectedCoin);
                TB_CoinColor = "Yellow";

            },
            canExecute => true);

            //сброс отображения времени если статус изменился
            cb_StatusSelectionChanged = new LambdaCommand(execute =>
            {
                ValueStatusCopy(SelectedPosition.Status, Statuses, SelectedStatus);
                TB_StatusColor = "Yellow";

                if (SelectedStatus == "Закрыта" || SelectedStatus == "Closed")
                {
                    SelectedCloseDate = SelectedOpenDate;
                }
                else if (SelectedStatus == "Открыта" || SelectedStatus == "Open")
                {
                    SelectedCloseDate = "";
                }
                TB_DateColor = "Yellow";
            },
            canExecute => true);

            UndoSelectedPosition = new LambdaCommand(execute =>
            {
                //Console.WriteLine($"Оригинальная позиция {DublecateSelectedPosition.Kind.Kind}");
                ValuePositionCopy(SelectedPosition, DublecateSelectedPosition);
                //Безопасный откат выбранных позиций в комбобокс
                UpdateSelectedStringFields();
                SelectedPositionsInitialization(SelectedPositions);
                UpdateEvent();
            },
            canExecute => SelectedPosition != null && DublecateSelectedPosition != null);

            SelectNewPosition = new LambdaCommand(

                async execute =>
                {
                    LockEvent();
                    ValuePositionCopy(SelectedPosition, DataGridSelectedItem);
                    //Безопасный откат выбранных позиций в комбобокс
                    UpdateSelectedStringFields();
                    UpdateEvent();
                    block = false;
                },
                canExecute => SelectedPosition.Kind != null);


            UpdatePosition = new LambdaCommand(
            async execute =>
            {
                //Перезапись позиции
                SelectedPosition.CloseDate = FormatterPositions.SetCloseDate(SelectedPosition.Status.StatusName);
                _context.PositionsTableBL.Update(Mappers.UIMapper.MapPositionUIToPositionBLL(SelectedPosition));

                Thread.CurrentThread.CurrentCulture = CultureInfo.CurrentUICulture; //fix 12.08.2024 Важен порядок установки до работы с БД работает некорректно. Важно вставлять после записи чтения из БД
                UpdateEvent();
                TempSelectedOpenDate = null;
                double.TryParse(SelectedPosition.Income.ToString(), out double r1);
                double.TryParse(SelectedPosition.Expense.ToString(), out double r2);
                SelectedPosition.Saldo = (r1 - r2).ToString();
                SelectedPositionsInitialization(SelectedPositions);
            },
                canExecute => SelectedPosition.Kind is not null && SelectedPosition.Income != null && SelectedPosition.Expense != null);

            //Продолжение комплексной позиции
            AddComplexPosition = new LambdaCommand(

                async execute =>
                {
                    var editPosition = (PositionUIModel)SelectedPosition.Clone();
                    ValuePositionCopy(SelectedPosition, DublecateSelectedPosition);
                    SetStringKinds(StringKinds, Kinds, SelectedCategory);
                    UpdateSelectedStringFields();
                    if (!dateTimeWasChanged) editPosition.OpenDate = DateTime.Now.ToString();

                    complexPositionsHendler.AddComplexPosition(_context, SelectedPositions, editPosition);

                    UpdateEvent();
                    SelectedPositionsInitialization(SelectedPositions);
                    UpdateEvent();
                    dateTimeWasChanged = false;
                },
                //Условие: все позиции из SelectedPositions должны иметь статус Открыта или Open (!Закрыта !Closed)
                canExecute => SelectedPosition.Kind is not null && SelectedPosition.Income != null && SelectedPosition.Expense != null
                && !SelectedPositions.Any(s => s.Status.StatusName == "Закрыта" || s.Status.StatusName == "Closed") && !dontEditthis);

            DeleteComplexPosition = new LambdaCommand(async execute =>
            {
                int delPosKey = DataGridSelectedItem.PositionKey;
                var t = await complexPositionsHendler.DeleteComplexPosition(_context, SelectedPositions, DataGridSelectedItem);
                //участвует в обновлении главного окна и сбросе цветов
                UpdateEvent();

                //Если удалена корневая позиция
                if (t.PositionKey != 0 && t.Kind != null && t.ZeroParrentKey == null)
                {
                    if (delPosKey == SelectedPosition.PositionKey)
                    {
                        ValuePositionCopy(SelectedPosition, t);
                        ValuePositionCopy(DublecateSelectedPosition, t);
                    }
                    SelectedPosition.ZeroParrentKey = t.PositionKey;
                    DublecateSelectedPosition.ZeroParrentKey = t.PositionKey;
                }
                else
                {
                    //Возвращается предыдущая позиция
                    if (delPosKey == SelectedPosition.PositionKey && t.PositionKey != 0 && t.Kind != null)
                    {
                        ValuePositionCopy(SelectedPosition, t);
                        ValuePositionCopy(DublecateSelectedPosition, t);
                    }
                }
            },
            canExecute => DataGridSelectedItem != null && !dontEditthis);

            DeleteAllComplexPositionsAtRootKey = new LambdaCommand(async execute =>
            {
                complexPositionsHendler.DeleteAllComplexPositionsAtRootKey(_context, SelectedPositions);
                UpdateEvent();

            },
            canExecute => SelectedPositions.Count > 0 && !dontEditthis);

        }


        //Методы класса
        public void SetStringStatusesAndTranslation(ObservableCollection<StatusUIModel> _input, ObservableCollection<string> _output)
        {
            _output.Clear();
            foreach (var stat in _input)
            {
                if (stat.StatusName == "Open")
                {
                    stat.StatusName = "Открыта";
                    _output.Add(stat.StatusName);
                }
                else if (stat.StatusName == "Closed")
                {
                    stat.StatusName = "Закрыта";
                    _output.Add(stat.StatusName);
                }
                else if (stat.StatusName == "Deleted")
                {
                    continue;
                }
                else
                {
                    _output.Add(stat.StatusName);
                }
            }
        }


        //Инициализация временных категорий для добавления новой позиции
        public void SetStringCaregories(ObservableCollection<string> _output, ObservableCollection<CategoryUIModel> _input)
        {
            _output.Clear();
            foreach (var stat in _input)
            {
                _output.Add(stat.Category);
            }
        }

        //Добавление всех категорий из входящей коллекции
        public void SetStringKinds(ObservableCollection<string> _output, ObservableCollection<KindUIModel> _input)
        {
            _output.Clear();
            foreach (var stat in _input)
            {
                _output.Add(stat.Kind);
            }
        }
        //Добавление видов по категории
        public void SetStringKinds(ObservableCollection<string> _output, ObservableCollection<KindUIModel> _input, CategoryUIModel _category)
        {
            _output.Clear();
            foreach (var stat in _input)
            {
                if (stat.Category.Category == _category.Category)
                {
                    _output.Add(stat.Kind);
                }
            }
        }
        //Добавление видов по string категории 
        public void SetStringKinds(ObservableCollection<string> _output, ObservableCollection<KindUIModel> _input, string _category)
        {
            _output.Clear();
            foreach (var stat in _input)
            {
                if (stat.Category.Category == _category)
                {
                    _output.Add(stat.Kind);
                }
            }
        }

        public void SetStringCoins(ObservableCollection<string> _output, ObservableCollection<CoinUIModel> _input)
        {
            _output.Clear();
            foreach (var stat in _input)
            {
                _output.Add(stat.ShortName);
            }
        }

        //Инициализация выбранных позиций
        public void SelectedPositionsInitialization(ObservableCollection<PositionUIModel> _selectedPositions, int _flagSource = 0)
        {
            try
            {
                PositionUIModel sp = new PositionUIModel();
                if (_flagSource == 0)
                {
                    sp = (PositionUIModel)SelectedPosition.Clone();
                }
                else
                {
                    sp = (PositionUIModel)DublecateSelectedPosition.Clone();
                }

                ComplexPositionBalanceCalculator calculator = new ComplexPositionBalanceCalculator();

                _selectedPositions.Clear();
                //Если позиция простая или выбран родитель
                if (sp.ZeroParrentKey == null)
                {
                    List<PositionUIModel> filtredList = MAINPOSITIONSCOLLECTION.AsList().FindAll(f => f.ZeroParrentKey == sp.PositionKey);
                    BLL.Context.ListOfPositions tempPos = new BLL.Context.ListOfPositions();
                    //List<PositionUIModel> controlPosList = (tempPos.GetAll().Result.Select(p => Mappers.UIMapper.MapPositionBLLToPositionUI(p)).ToList());
                    List<PositionUIModel> controlPosList = (tempPos.GetAll().Select(p => Mappers.UIMapper.MapPositionBLLToPositionUI(p)).ToList());
                    List<PositionUIModel> filtredControlList = controlPosList.AsList().FindAll(f => f.ZeroParrentKey == sp.PositionKey);
                    if (filtredList.Count != filtredControlList.Count)
                    {
                        dontEditthis = true;                       
                        MessageBox.Show("В текущей выборке отсутствует родитель комплексной позиции. Возможна потеря данных",
                            "Отсутствует родитель позиции", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    _selectedPositions.Add(sp);
                    filtredList = (filtredList.OrderBy(f => f.ParrentKey)).ToList();
                    foreach (var item in filtredList)
                    {
                        if (item.Status.StatusName != "Deleted")
                        {
                            _selectedPositions.Add(item);
                        }
                    }
                    TotalBalances = calculator.GetTotalBalances(_selectedPositions);
                }
                //Если выбрана дочерняя позиция
                else
                {
                    var childs = MAINPOSITIONSCOLLECTION.AsList().FindAll(f => f.ZeroParrentKey == sp.ZeroParrentKey);
                    BLL.Context.ListOfPositions tempPos = new BLL.Context.ListOfPositions();
                    //List<PositionUIModel> controlPosList = (tempPos.GetAll().Result.Select(p => Mappers.UIMapper.MapPositionBLLToPositionUI(p)).ToList());
                    List<PositionUIModel> controlPosList = (tempPos.GetAll().Select(p => Mappers.UIMapper.MapPositionBLLToPositionUI(p)).ToList());
                    List<PositionUIModel> filtredControlList = controlPosList.AsList().FindAll(f => f.ZeroParrentKey == sp.ZeroParrentKey);
                    if (childs.Count != filtredControlList.Count)
                    {
                        dontEditthis = true;
                        MessageBox.Show("В текущей выборке отсутствует родитель комплексной позиции. Возможна потеря данных",
                            "Отсутствует родитель позиции", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    var zeroParent = MAINPOSITIONSCOLLECTION.AsList().Find(f => f.PositionKey == sp.ZeroParrentKey);
                    if (zeroParent != null) childs.Add(zeroParent);
                    childs = (childs.OrderBy(f => f.ParrentKey)).ToList();
                    foreach (var item in childs)
                    {
                        if (item.Status.StatusName != "Deleted")
                        {
                            _selectedPositions.Add(item);
                        }
                    }
                    TotalBalances = calculator.GetTotalBalances(_selectedPositions);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message} ZeroParrentKey: {_selectedPosition.ZeroParrentKey} ParrentKey: {_selectedPosition.ParrentKey}");
            }
        }

        void ValuePositionCopy(PositionUIModel _tocopy, PositionUIModel _fromCopy)
        {
            try
            {
                if (_tocopy != null && _fromCopy != null && _tocopy.Kind != null && _tocopy.Kind.Category != null)
                {
                    _tocopy.Id = _fromCopy.Id;
                    _tocopy.PositionKey = _fromCopy.PositionKey;
                    _tocopy.OpenDate = _fromCopy.OpenDate;
                    _tocopy.CloseDate = _fromCopy.CloseDate;

                    //_tocopy.Kind = _fromCopy.Kind;
                    _tocopy.Kind.Id = _fromCopy.Kind.Id;
                    _tocopy.Kind.Kind = _fromCopy.Kind.Kind;
                    _tocopy.Kind.Category.Id = _fromCopy.Kind.Category.Id;
                    _tocopy.Kind.Category.Category = _fromCopy.Kind.Category.Category;
                    _tocopy.Kind.Category.RefNumber = _fromCopy.Kind.Category.RefNumber;

                    _tocopy.Income = _fromCopy.Income;
                    _tocopy.Expense = _fromCopy.Expense;
                    _tocopy.Saldo = _fromCopy.Saldo;

                    //_tocopy.Coin = _fromCopy.Coin;
                    _tocopy.Coin.Id = _fromCopy.Coin.Id;
                    _tocopy.Coin.FullName = _fromCopy.Coin.FullName;
                    _tocopy.Coin.ShortName = _fromCopy.Coin.ShortName;
                    _tocopy.Coin.RefNumber = _fromCopy.Coin.RefNumber;
                    _tocopy.Coin.CoinNotes = _fromCopy.Coin.CoinNotes;

                    //_tocopy.Status = _fromCopy.Status;
                    _tocopy.Status.Id = _fromCopy.Status.Id;
                    _tocopy.Status.StatusName = _fromCopy.Status.StatusName;
                    _tocopy.Status.StatusNotes = _fromCopy.Status.StatusNotes;

                    _tocopy.Tag = _fromCopy.Tag;
                    _tocopy.Notes = _fromCopy.Notes;
                    _tocopy.ZeroParrentKey = _fromCopy.ZeroParrentKey;
                    _tocopy.ParrentKey = _fromCopy.ParrentKey;
                }
                else
                {
                    //Console.WriteLine($"Некорректные аргументы метода ValuePositionCopy Сработала передача вида по новой ссылке (не исключение)");
                    //SelectedPosition.Kind = (KindUIModel)DublecateSelectedPosition.Kind.Clone();
                    _tocopy.Id = DublecateSelectedPosition.Id;
                    _tocopy.PositionKey = DublecateSelectedPosition.PositionKey;
                    _tocopy.OpenDate = DublecateSelectedPosition.OpenDate;
                    _tocopy.CloseDate = DublecateSelectedPosition.CloseDate;

                    //_tocopy.Kind = _fromCopy.Kind;
                    _tocopy.Kind.Id = DublecateSelectedPosition.Kind.Id;
                    _tocopy.Kind.Kind = DublecateSelectedPosition.Kind.Kind;
                    _tocopy.Kind.Category.Id = DublecateSelectedPosition.Kind.Category.Id;
                    _tocopy.Kind.Category.Category = DublecateSelectedPosition.Kind.Category.Category;
                    _tocopy.Kind.Category.RefNumber = DublecateSelectedPosition.Kind.Category.RefNumber;

                    _tocopy.Income = DublecateSelectedPosition.Income;
                    _tocopy.Expense = DublecateSelectedPosition.Expense;
                    _tocopy.Saldo = DublecateSelectedPosition.Saldo;

                    //_tocopy.Coin = _fromCopy.Coin;
                    _tocopy.Coin.Id = DublecateSelectedPosition.Coin.Id;
                    _tocopy.Coin.FullName = DublecateSelectedPosition.Coin.FullName;
                    _tocopy.Coin.ShortName = DublecateSelectedPosition.Coin.ShortName;
                    _tocopy.Coin.RefNumber = DublecateSelectedPosition.Coin.RefNumber;
                    _tocopy.Coin.CoinNotes = DublecateSelectedPosition.Coin.CoinNotes;

                    //_tocopy.Status = _fromCopy.Status;
                    _tocopy.Status.Id = DublecateSelectedPosition.Status.Id;
                    _tocopy.Status.StatusName = DublecateSelectedPosition.Status.StatusName;
                    _tocopy.Status.StatusNotes = DublecateSelectedPosition.Status.StatusNotes;

                    _tocopy.Tag = DublecateSelectedPosition.Tag;
                    _tocopy.Notes = DublecateSelectedPosition.Notes;
                    _tocopy.ZeroParrentKey = DublecateSelectedPosition.ZeroParrentKey;
                    _tocopy.ParrentKey = DublecateSelectedPosition.ParrentKey;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message} _tocopy {_tocopy.Kind.Kind} _fromcopy {_fromCopy.Kind.Kind}");
            }
        }


        void ValueCategoryCopy(CategoryUIModel _tocopy, ObservableCollection<CategoryUIModel> _fromCollection, string _categoryName)
        {
            if (_tocopy != null && _fromCollection != null)
            {
                var _fromCopy = _fromCollection.First(c => c.Category == _categoryName);
                _tocopy.Id = _fromCopy.Id;
                _tocopy.Category = _fromCopy.Category;
                _tocopy.RefNumber = _fromCopy.RefNumber;
            }
            else
            {
                Console.WriteLine("Некорректные аргументы метода ValueCategoryCopy");
            }
        }

        void ValueKindCopy(KindUIModel _tocopy, ObservableCollection<KindUIModel> _fromCollection, string _kindName)
        {
            if (_tocopy != null && _fromCollection != null && _fromCollection.Any(k => k.Kind == _kindName))
            {
                var _fromCopy = _fromCollection.First(k => k.Kind == _kindName);
                _tocopy.Id = _fromCopy.Id;
                _tocopy.Kind = _fromCopy.Kind;
                _tocopy.Category.Id = _fromCopy.Category.Id;
                _tocopy.Category.Category = _fromCopy.Category.Category;
                _tocopy.Category.RefNumber = _fromCopy.Category.RefNumber;
            }
        }

        void ValueKindCopy(KindUIModel _tocopy, KindUIModel _fromCopy)
        {
            _tocopy.Id = _fromCopy.Id;
            _tocopy.Kind = _fromCopy.Kind;
            _tocopy.Category.Id = _fromCopy.Category.Id;
            _tocopy.Category.Category = _fromCopy.Category.Category;
            _tocopy.Category.RefNumber = _fromCopy.Category.RefNumber;
        }

        void ValueCoinCopy(CoinUIModel _tocopy, CoinUIModel _fromCopy)
        {
            _tocopy.Id = _fromCopy.Id;
            _tocopy.FullName = _fromCopy.FullName;
            _tocopy.ShortName = _fromCopy.ShortName;
            _tocopy.RefNumber = _fromCopy.RefNumber;
            _tocopy.CoinNotes = _fromCopy.CoinNotes;
        }

        void ValueCoinCopy(CoinUIModel _tocopy, ObservableCollection<CoinUIModel> _fromCollection, string _coinShortName)
        {
            if (_tocopy != null && _fromCollection != null)
            {
                var _fromCopy = _fromCollection.First(c => c.ShortName == _coinShortName);
                _tocopy.Id = _fromCopy.Id;
                _tocopy.FullName = _fromCopy.FullName;
                _tocopy.ShortName = _fromCopy.ShortName;
                _tocopy.RefNumber = _fromCopy.RefNumber;
                _tocopy.CoinNotes = _fromCopy.CoinNotes;
            }
        }

        void ValueStatusCopy(StatusUIModel _tocopy, ObservableCollection<StatusUIModel> _fromCollection, string _statusName)
        {
            if (_tocopy != null && _fromCollection != null)
            {
                var _fromCopy = _fromCollection.First(s => s.StatusName == _statusName);
                _tocopy.Id = _fromCopy.Id;
                _tocopy.StatusName = _fromCopy.StatusName;
                _tocopy.StatusNotes = _fromCopy.StatusNotes;
            }
        }

        void SetBlock()
        {
            block = true;
        }

        void UpdateSelectedStringFields()
        {
            SelectedOpenDate = SelectedPosition.OpenDate;
            SelectedCloseDate = SelectedPosition.CloseDate;
            SelectedCategory = SelectedPosition.Kind.Category.Category;
            SetStringKinds(StringKinds, Kinds, SelectedPosition.Kind.Category);
            SelectedKind = SelectedPosition.Kind.Kind;
            SelectedCoin = SelectedPosition.Coin.ShortName;
            SelectedStatus = SelectedPosition.Status.StatusName;
            TempSelectedOpenDate = null;
        }
    }

}
