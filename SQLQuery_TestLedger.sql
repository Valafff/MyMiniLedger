--create database MyBook

--use MyBook

exec sp_dropdevice 'NameBCDisk'
exec sp_addumpdevice 'disk', 'NameBCDisk', 'C:\Users\Vetal\source\repos\ADO.NET\MyMiniLedger\TestLedger.bak'
backup database TestLedger to NameBCDisk

restore database TestLedger
from NameBCDisk
with replace


CREATE TABLE Categories (
	Id	int not null primary key identity(1,1),
	Category	nvarchar(100) COLLATE Cyrillic_General_CI_AS unique,
);


CREATE TABLE Coins (
	Id	int not null primary key identity(1,1),
	ShortName	nvarchar(100) COLLATE Cyrillic_General_CI_AS unique not null,
	FullName	nvarchar(100) COLLATE Cyrillic_General_CI_AS,
	CoinNotes	nvarchar(100) COLLATE Cyrillic_General_CI_AS default null
);


CREATE TABLE  Statuses (
	Id	int not null primary key identity(1,1),
	StatusName	nvarchar(50) COLLATE Cyrillic_General_CI_AS unique not null,
	StatusNotes	nvarchar(100) COLLATE Cyrillic_General_CI_AS default null
);


CREATE TABLE Kinds (
	Id	int not null primary key identity(1,1),
	CategoryId	int foreign key references Categories(Id),
	Kind	nvarchar(100) COLLATE Cyrillic_General_CI_AS not null,
);



CREATE TABLE Positions (
	Id	int not null primary key identity(1,1),
	PositionKey	int not null,
	OpenDate	datetime,
	CloseDate	datetime,
	KindId	int foreign key references Kinds(Id),
	Income	Decimal(22,8),
	Expense	Decimal(22,8),
	Saldo	Decimal(22,8),
	CoinId	int foreign key references Coins(Id),
	StatusId int foreign key references Statuses(Id),
	Tag	nvarchar (1000) COLLATE Cyrillic_General_CI_AS,
	Notes nvarchar(MAX) COLLATE Cyrillic_General_CI_AS
);

insert into Statuses (StatusName, StatusNotes)
values ('Closed',N'Позиция закрыта'),
('Open', N'Позиция открыта'),
('Deleted', N'Позиция позиция удалена')

insert into Coins(ShortName, FullName, CoinNotes)
values ('RUB', N'Рубль', N'fiat'),
('USD', N'Доллар США', N'fiat'),
('BTC', N'Bitcoin', N'crypto')


select PositionKey, OpenDate, CloseDate, Kind, Income, Expense, Saldo, ShortName, StatusName Tag, Notes 
from Positions P
join Kinds K on k.Id = p.KindId
join Coins C on c.Id = p.CoinId
join Statuses S on s.Id = p.StatusId

update Categories set Category = N'новый тест' where  id = 17

select *
from Categories

select * from Coins

select * from Kinds

drop table Positions

insert into Positions (PositionKey, OpenDate, CloseDate, KindId, Income, Expense, Saldo, CoinId, StatusId, Tag, Notes)
values (100500, '04.15.2024 00:00:00', '04.15.2024 00:00:00', 7, 123, 0, 0, 1, 4, '123', '123') 

insert into Positions (PositionKey, OpenDate, CloseDate, KindId, Income, Expense, Saldo, CoinId, StatusId, Tag, Notes)
values (100500, N'04/15/2024 00:00:00',  N'04/15/2024 00:00:00', 7, 123, 0, 0, 1, 4, N'123', N'123') 


