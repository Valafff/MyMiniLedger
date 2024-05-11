SET IDENTITY_INSERT [dbo].[Coins] ON
INSERT INTO [dbo].[Coins] ([Id], [ShortName], [FullName], [CoinNotes]) VALUES (1, N'RUB', N'Рубль', N'fiat')
INSERT INTO [dbo].[Coins] ([Id], [ShortName], [FullName], [CoinNotes]) VALUES (2, N'USD', N'Доллар США', N'fiat')
INSERT INTO [dbo].[Coins] ([Id], [ShortName], [FullName], [CoinNotes]) VALUES (3, N'BTC', N'Bitcoin', N'crypto')
INSERT INTO [dbo].[Coins] ([Id], [ShortName], [FullName], [CoinNotes]) VALUES (21002, N'KAS', N'KASPA', N'crypto')
SET IDENTITY_INSERT [dbo].[Coins] OFF
