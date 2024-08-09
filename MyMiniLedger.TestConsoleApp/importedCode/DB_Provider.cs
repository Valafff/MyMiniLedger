using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlTypes;
using System.Data.OleDb;
using System.ComponentModel;
using System.Security.Cryptography;

namespace ImportedClasses
{
    internal class DB_Provider
    {

        OleDbConnection DBConnection;

        //Подключение
        public bool openConnection(string pass = "!!!ВВЕДИ ПАРОЛЬ ОТ БД!!!", string path = "oldDB.mdb")
        {
            try
            {
                if (pass == "")
                {
                    string connectString = $"Microsoft.ACE.OLEDB.12.0;Data Source = {path};";
                    DBConnection = new OleDbConnection(connectString);
                    DBConnection.Open();
                    return true;
                }
                else
                {
                    string connectString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source = {path}; Jet OLEDB:Database Password = {pass};";
                    DBConnection = new OleDbConnection(connectString);
                    DBConnection.Open();
                    return true;
                }

            }
            catch (Exception)
            {
                return false;
            }
        }

        //Отключение
        public void closeConnection() { DBConnection.Close(); }


        //Получение листа данных приходов расходов
        public List<IncomeExpenseData> IEDataListRead()
        {
            List<IncomeExpenseData> tempIEData = new List<IncomeExpenseData>();
            //Формирование строки запроса к БД
            string query = "SELECT key, type, category, kind, defaultStatus, note FROM IncomeExpenceData";
            // создаем объект OleDbCommand для выполнения запроса к БД MS Access
            OleDbCommand command = new OleDbCommand(query, DBConnection);
            // получаем объект OleDbDataReader для чтения табличного результата запроса SELECT

            try
            {
                OleDbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    IncomeExpenseData temp = new IncomeExpenseData(Convert.ToUInt32(reader[0]), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString());
                    tempIEData.Add(temp);
                }
                return tempIEData;
            }
            catch (Exception)
            {
                Console.WriteLine("Ошибка чтения таблицы IncomeExpenceData");
                return null;
            }
        }

        //Получение одной позиции
        public impoetedPosition GetPosition(uint _id)
        {
            impoetedPosition temp = new impoetedPosition();
            string query = $"SELECT key, myKey, openDate, closeDate, kind, category, income, expence, lotCount, currCoin, saldo, status, tag, notes FROM Positions WHERE myKey = {_id}";
            OleDbCommand command = new OleDbCommand(query, DBConnection);

            try
            {
                OleDbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    temp = new impoetedPosition(Convert.ToUInt32(reader[0]), Convert.ToUInt32(reader[1]),
                        Convert.ToDateTime(reader[2]), Convert.ToDateTime(reader[3]),
                        reader[4].ToString(), reader[5].ToString(), Convert.ToDecimal(reader[6]),
                        Convert.ToDecimal(reader[7]), Convert.ToUInt32(reader[8]), reader[9].ToString(),
                        Convert.ToDecimal(reader[10]), reader[11].ToString(), reader[12].ToString(), reader[13].ToString());
                }
                //Console.WriteLine("*************");
                return temp;
            }
            catch (Exception)
            {
                Console.WriteLine("Позиция не прочитана");
                return null;
            }
        }

        //Получение всех позиций
        public List<impoetedPosition> GetAllPositions()
        {
            List<impoetedPosition> list = new List<impoetedPosition>();
            impoetedPosition temp = new impoetedPosition();
            string query = $"SELECT * FROM Positions";
            OleDbCommand command = new OleDbCommand(query, DBConnection);
            OleDbDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                temp = new impoetedPosition(Convert.ToUInt32(reader[0]), Convert.ToUInt32(reader[1]),
                    Convert.ToDateTime(reader[2]), Convert.ToDateTime(reader[3]),
                    reader[4].ToString(), reader[5].ToString(), Convert.ToDecimal(reader[6]),
                    Convert.ToDecimal(reader[7]), Convert.ToUInt32(reader[8]), reader[9].ToString(),
                    Convert.ToDecimal(reader[10]), reader[11].ToString(), reader[12].ToString(), reader[13].ToString());
                list.Add(temp);
            }
            //Console.WriteLine("*************");
            return list;
        }

        //Получение листа данных валют
        public List<Currency> CurrencyNameReader()
        {
            //ASC - сортировка от меньшего к большему
            List<Currency> tempCurrPair = new List<Currency>();
            string query = "SELECT key, addDate, currencyPairName, shortPairName FROM coins ORDER BY key ASC";
            OleDbCommand command = new OleDbCommand(query, DBConnection);

            try
            {
                OleDbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Currency temp = new Currency(Convert.ToUInt32(reader[0]), Convert.ToDateTime(reader[1]), reader[2].ToString(), reader[3].ToString());
                    tempCurrPair.Add(temp);
                }
                return tempCurrPair;
            }
            catch (Exception)
            {
                Console.WriteLine("Ошибка чтения таблицы currencyPair");
                return null;
            }
        }

        //Запись валюты в БД
        public uint addCurrencyNameToDB(Currency currency)
        {
            string query = $"INSERT INTO coins (addDate, currencyPairName, shortPairName) VALUES ('{currency.date}', '{currency.currencyName}', '{currency.shortCurrencyName}')";
            OleDbCommand command = new OleDbCommand(query, DBConnection);
            try
            {
                command.ExecuteNonQuery();

                ////string query2 = "SELECT MAX(key) FROM coins";
                string query2 = $"SELECT key FROM coins WHERE currencyPairName ='{currency.currencyName}'";
                OleDbCommand command2 = new OleDbCommand(query2, DBConnection);
                return Convert.ToUInt32(command2.ExecuteScalar().ToString());
            }
            catch (Exception)
            {
                return 0;
            }
        }

        //Обновление записи о валюте
        public uint updateCurrency(Currency curr)
        {
            string query = $"UPDATE coins SET addDate = '{curr.date}', currencyPairName ='{curr.currencyName}', shortPairName = '{curr.shortCurrencyName}' WHERE key = {curr.key}";
            OleDbCommand command = new OleDbCommand(query, DBConnection);
            try
            {
                command.ExecuteNonQuery();

                //string query2 = "SELECT MAX(key) FROM coins";
                //string query2 = $"SELECT key FROM coins WHERE currencyPairName ='{curr.currencyName}'";
                //OleDbCommand command2 = new OleDbCommand(query2, DBConnection);
                //return Convert.ToUInt32(command2.ExecuteScalar().ToString());
                return curr.key;

            }
            catch (Exception)
            {
                return 0;
            }
        }

        //Удаление записи о валюте
        public bool deleteCurrency(Currency curr)
        {
            string query = $"DELETE FROM coins WHERE key = {curr.key}";
            OleDbCommand command = new OleDbCommand(query, DBConnection);
            try
            {
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }



        //Запись позиции в БД.
        public bool addPositionToDB(impoetedPosition inputPos)
        {
            //Неактуально, лечится в dotTocommaRepair()
            //string income = inputPos.income.ToString(), expence = inputPos.expence.ToString(), saldo = inputPos.saldo.ToString();
            //income =income.Replace(',', '.');
            //expence = expence.Replace(',', '.');
            //saldo = saldo.Replace(',', '.');

            // текст запроса
            string query = $"INSERT INTO Positions (myKey, openDate, closeDate, kind, category, income, expence, lotCount, currCoin, saldo, status, tag, notes)" +
            $" VALUES ({inputPos.myKey}, '{inputPos.openDate}', '{inputPos.closeDate}', '{inputPos.kind}', '{inputPos.category}', '{inputPos.income}', '{inputPos.expense}', " +
            $"{inputPos.lotCount}, '{inputPos.currCoin}', '{inputPos.saldo}', '{inputPos.status}', '{inputPos.tag}','{inputPos.notes}')";
            // создаем объект OleDbCommand для выполнения запроса к БД MS Access
            OleDbCommand command = new OleDbCommand(query, DBConnection);
            try
            {
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //Получение максимального значения myKey
        public uint getMyKeyMax()
        {
            try
            {
                string query = "SELECT MAX(myKey) FROM Positions";
                OleDbCommand command = new OleDbCommand(query, DBConnection);
                return Convert.ToUInt32(command.ExecuteScalar().ToString());
            }
            catch (Exception)
            {
                //MessageBox.Show("Ошибка получения максимума или позиции отсутствуют");
                return 0;
            }
        }

        //Чтение позиций из БД
        public List<impoetedPosition> readPositionsFromBD(DateTime _startDate = new DateTime(), DateTime _endDate = new DateTime())
        {
            List<impoetedPosition> tempList = new List<impoetedPosition>();
            string query = "SELECT key, myKey, openDate, closeDate, kind, category, income, expence, lotCount, currCoin, saldo, status, tag, notes FROM Positions ORDER BY openDate DESC";
            OleDbCommand command = new OleDbCommand(query, DBConnection);

            try
            {
                OleDbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var temp = new impoetedPosition(Convert.ToUInt32(reader[0]), Convert.ToUInt32(reader[1]),
                        Convert.ToDateTime(reader[2]), Convert.ToDateTime(reader[3]),
                        reader[4].ToString(), reader[5].ToString(), Convert.ToDecimal(reader[6]),
                        Convert.ToDecimal(reader[7]), Convert.ToUInt32(reader[8]), reader[9].ToString(),
                        Convert.ToDecimal(reader[10]), reader[11].ToString(), reader[12].ToString(), reader[13].ToString());
                    tempList.Add(temp);
                    //Console.WriteLine(temp.openDate);
                }
                //Console.WriteLine("*************");
                return tempList;
            }
            catch (Exception)
            {
                Console.WriteLine("Данные не прочитаны");
                return null;
            }
        }

        //Обновление позиции в БД
        public bool updatePosition(impoetedPosition pos)
        {


            string income = pos.income.ToString(), expence = pos.expense.ToString(), saldo = pos.saldo.ToString();
            income = income.Replace(',', '.');
            expence = expence.Replace(',', '.');
            saldo = saldo.Replace(',', '.');

            string query = $"UPDATE Positions SET myKey = {pos.myKey}, openDate = '{pos.openDate}', closeDate ='{pos.closeDate}', kind = '{pos.kind}'," +
                $" category = '{pos.category}', income = {income}, expence = {expence}, lotCount = {pos.lotCount}, currCoin = '{pos.currCoin}', saldo = {saldo}," +
                $" status = '{pos.status}', tag = '{pos.tag}', notes = '{pos.notes}' WHERE key = {pos.key}";
            OleDbCommand command = new OleDbCommand(query, DBConnection);
            try
            {
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //Удаление позиции в БД
        public bool deletePosition(impoetedPosition pos)
        {

            //string query = $"DELETE FROM Positions key = {pos.key}, myKey = {pos.myKey}, openDate = '{pos.openDate}', closeDate ='{pos.closeDate}', kind = '{pos.kind}'," +
            //	$" category = '{pos.category}', income = {pos.income}, expence = {pos.expence}, lotCount = {pos.lotCount}, currCoin = '{pos.currCoin}', saldo = {pos.saldo}," +
            //	$" status = '{pos.status}', tag = '{pos.tag}', notes = '{pos.notes}' WHERE key = {pos.key}";
            string query = $"DELETE FROM Positions WHERE key = {pos.key}";
            OleDbCommand command = new OleDbCommand(query, DBConnection);
            try
            {
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        ////Чтение позиций из старой БД 1_07
        //public List<OldDBData_1_07> readPositionsFromOldBD_1_07(DateTime _startDate = new DateTime(), DateTime _endDate = new DateTime())
        //{
        //    List<OldDBData_1_07> tempList = new List<OldDBData_1_07>();

        //    string queryIncome = $"SELECT Код, Дата, Тип_дохода, Доход, Примечание FROM Tb_доходы";
        //    string queryExpence = $"SELECT Код, Дата, Тип_расходов, Расходы, Примечание FROM Tb_расходы";

        //    try
        //    {
        //        OleDbCommand commandIncome = new OleDbCommand(queryIncome, DBConnection);
        //        OleDbDataReader reader = commandIncome.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            var temp = new OldDBData_1_07(Convert.ToUInt32(reader[0]), Convert.ToDateTime(reader[1]), reader[2].ToString(), "null", Convert.ToDecimal(reader[3]), reader[4].ToString());
        //            tempList.Add(temp);
        //            //Console.WriteLine(temp.openDate);
        //        }
        //        OleDbCommand commandExpence = new OleDbCommand(queryExpence, DBConnection);
        //        OleDbDataReader readerExpence = commandExpence.ExecuteReader();
        //        while (readerExpence.Read())
        //        {
        //            var temp = new OldDBData_1_07(Convert.ToUInt32(readerExpence[0]), Convert.ToDateTime(readerExpence[1]), "null", readerExpence[2].ToString(), Convert.ToDecimal(readerExpence[3]), readerExpence[4].ToString());
        //            tempList.Add(temp);
        //        }
        //        //Console.WriteLine("*************");
        //        return tempList;
        //    }
        //    catch (Exception)
        //    {
        //        MessageBox.Show("Данные не прочитаны");
        //        return null;
        //    }
        //}

    }

    //Хранение данных из БД расходы/приходы
    public class IncomeExpenseData : ICloneable, INotifyPropertyChanged
    {
        uint KEY;
        public uint key
        {
            get => KEY;
            set
            {
                KEY = value;
                ChangeProperty("KEY");
            }
        }
        string TYPE;
        public string type
        {
            get => TYPE;
            set
            {
                TYPE = value;
                ChangeProperty("TYPE");
            }
        }
        string CATEGORY;
        public string category
        {
            get => CATEGORY;
            set
            {
                CATEGORY = value;
                ChangeProperty("CATEGORY");
            }
        }
        string KIND;
        public string kind
        {
            get => KIND;
            set
            {
                KIND = value;
                ChangeProperty("KIND");
            }
        }
        string DEFSTATUS;
        public string defaultStatus
        {
            get => DEFSTATUS;
            set
            {
                DEFSTATUS = value;
                ChangeProperty("DEFSTATUS");
            }
        }
        string NOTE;
        public string note
        {
            get => NOTE;
            set
            {
                NOTE = value;
                ChangeProperty("NOTE");
            }
        }

        public IncomeExpenseData() { }

        public IncomeExpenseData(uint _key, string _type, string _category, string _kind, string _defaultStatus, string _note)
        {
            key = _key;
            type = _type;
            category = _category;
            kind = _kind;
            defaultStatus = _defaultStatus;
            note = _note;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void ChangeProperty(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        public object Clone() => MemberwiseClone();
    }
    //Хранение пар валют из БД расходы/приходы
    public class Currency : INotifyPropertyChanged
    {
        uint KEY;
        public uint key
        {
            get => KEY;
            set
            {
                KEY = value;
                ChangeProperty("KEY");
            }
        }
        DateTime DATE;
        public DateTime date
        {
            get => DATE;
            set
            {
                DATE = value;
                ChangeProperty("DATE");
            }
        }

        string NAME;
        public string currencyName
        {
            get => NAME;
            set
            {
                NAME = value;
                ChangeProperty("NAME");
            }
        }
        string SHNAME;
        public string shortCurrencyName
        {
            get => SHNAME;
            set
            {
                SHNAME = value;
                ChangeProperty("SHNAME");
            }
        }





        public Currency() { }

        public Currency(uint _key, DateTime _addDate, string _currencyName, string _shortCurrencyName)
        {
            key = _key;
            date = _addDate;
            currencyName = _currencyName;
            shortCurrencyName = _shortCurrencyName;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void ChangeProperty(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }


        public override string ToString()
        {
            return $"Дата доб/обнов: {date.ToShortDateString()}\t  |  Название валюты: {currencyName}  |  Короткое название: {shortCurrencyName}";
        }
    }

    public class impoetedPosition : ICloneable, IComparable /*: INotifyPropertyChanged*/
    {
        //0 Ключ БД
        public uint key { get; set; }

        //1 Ключ приложения - идентифицирует позицию для программы
        public uint myKey { get; set; }

        //2 Дата открытия
        public DateTime openDate { get; set; }

        //лишнее
        //////3 Дата изменения позиции (редактирует всле записи с полем myKey)
        ////public DateTime editDate { get; set; }

        //2 Дата закрытия позиции (редактирует всле записи с полем myKey)
        public DateTime closeDate { get; set; }

        //3 Название позиции (вводит пользователь)
        public string kind { get; set; }

        //4 Категория  - должна устанавливаться автоматически, либо вручную
        public string category { get; set; }

        //5 Приход (вводит пользователь если поле активно, по умолчанию 0)
        public decimal income { get; set; }

        public decimal getIncome()
        {
            return income;
        }

        //6 Расход (вводит пользователь если поле активно, по умолчанию 0)
        public decimal expense { get; set; }
        public decimal getExpense()
        {
            return income;
        }

        //7 Количество лотов (по умолчанию при открытой позиции 1, при закрытой 0
        public uint lotCount { get; set; }

        //По задумке должна автоматически пересчитываться на курс текущих валют
        //8 Валюта или валютная пара позиции
        public string currCoin { get; set; }

        //9 Разность прихода и расхода
        public decimal saldo { get; set; }

        //10 Статус позиции(открыта true/закрыта false)
        public string status { get; set; }

        //11 Вид(Тэг) позиции - используется для индивидуальной подписи позиции для группировки
        public string tag { get; set; }

        //лишнее
        //////8 Тип позиции (разовый - по умолчанию 'single'/периодический 'period'/ непредвиденный 'unexcepted')
        ////public string positionType { get; set; }

        //12 Заметки/примечания (вводит пользователь при необходимости)
        public string notes { get; set; }

        //// ОТКАЗАЛСЯ ОТ ДАННОЙ КОНЦЕПЦИИ 25.01.24
        ////По задумке при запуске приложения курсы должны парситься в excel таблицу дата/торговая пара/ курс
        ////13 Курсы (набор курсов - хранятся отдельно представляют структуру значение 1, значение 2, отношение )
        //public List<ratesData> Rates { get; set; }
        //public struct ratesData
        //{
        //	public string coinPair { get; set; }
        //	public double Ratio { get; set; }
        //          public ratesData(string _coinPair, double _ratio)
        //          {
        //		coinPair = _coinPair;
        //		Ratio = _ratio;
        //          }
        //      }



        public impoetedPosition()
        {
            lotCount = 1;
        }

        public impoetedPosition(uint _key, uint _myKey, DateTime _openDate, DateTime _closeDate, string _posKind, string _category, decimal _income, decimal _expence, uint _lotCount, string _currCoin, decimal _saldo, string _status, string _tag, string _note)
        {
            key = _key;
            myKey = _myKey;
            openDate = _openDate;
            closeDate = _closeDate;
            kind = _posKind;
            category = _category;
            income = _income;
            expense = _expence;
            lotCount = _lotCount;
            currCoin = _currCoin;
            saldo = _saldo;
            status = _status;
            tag = _tag;
            notes = _note;
        }


        public override string ToString()
        {
            if (closeDate.Year == 9999 || (status == "открыто" || status == "open"))
            {
                return $"ID: {key}\t Дата открытия: {openDate.ToShortDateString()}\t Дата закрытия: ---\t Вид: {kind}\t Тег: {tag}\t Категория: {category}\t" +
                    $" Приход: {income}\t Расход: {expense}\t Сальдо: {saldo} {currCoin} \t" +
                    $" Количество лотов: {lotCount}\t Состояние: {status}";
            }
            else
                return $"ID: {key}\t Дата открытия: {openDate.ToShortDateString()}\t Дата закрытия: {closeDate.ToShortDateString()}\t Вид: {kind}\t Тег: {tag}\t Категория: {category}\t" +
        $" Приход: {income}\t Расход: {expense}\t Сальдо: {saldo} {currCoin} \t" +
        $" Количество лотов: {lotCount}\t Состояние: {status} примечания {notes}";
        }

        //Копирование для безссылочных типов
        public object Clone() => MemberwiseClone();

        ////Глубокое копирование
        //public object Clone()
        //{
        //	Position clone = (Position)this.MemberwiseClone();
        //	clone.Rates = new List<ratesData>();
        //	foreach (var item in Rates)
        //	{
        //		clone.Rates.Add(item);
        //	}
        //	return clone;
        //}

        public int CompareTo(object obj)
        {
            impoetedPosition x = obj as impoetedPosition;
            return string.Compare(this.kind, x.kind);

            throw new NotImplementedException();
        }
    }
}
