USE [master]
GO
/****** Object:  Database [DotNetLab1]    Script Date: 11/05/2013 21:52:01 ******/
CREATE DATABASE [DotNetLab1] ON  PRIMARY 
( NAME = N'DotNetLab1', FILENAME = N'C:\jablonskim\Dropbox\S5\NET\Lab1\DB\DotNetLab1.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'DotNetLab1_log', FILENAME = N'C:\jablonskim\Dropbox\S5\NET\Lab1\DB\DotNetLab1_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [DotNetLab1] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [DotNetLab1].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [DotNetLab1] SET ANSI_NULL_DEFAULT OFF
GO
ALTER DATABASE [DotNetLab1] SET ANSI_NULLS OFF
GO
ALTER DATABASE [DotNetLab1] SET ANSI_PADDING OFF
GO
ALTER DATABASE [DotNetLab1] SET ANSI_WARNINGS OFF
GO
ALTER DATABASE [DotNetLab1] SET ARITHABORT OFF
GO
ALTER DATABASE [DotNetLab1] SET AUTO_CLOSE OFF
GO
ALTER DATABASE [DotNetLab1] SET AUTO_CREATE_STATISTICS ON
GO
ALTER DATABASE [DotNetLab1] SET AUTO_SHRINK OFF
GO
ALTER DATABASE [DotNetLab1] SET AUTO_UPDATE_STATISTICS ON
GO
ALTER DATABASE [DotNetLab1] SET CURSOR_CLOSE_ON_COMMIT OFF
GO
ALTER DATABASE [DotNetLab1] SET CURSOR_DEFAULT  GLOBAL
GO
ALTER DATABASE [DotNetLab1] SET CONCAT_NULL_YIELDS_NULL OFF
GO
ALTER DATABASE [DotNetLab1] SET NUMERIC_ROUNDABORT OFF
GO
ALTER DATABASE [DotNetLab1] SET QUOTED_IDENTIFIER OFF
GO
ALTER DATABASE [DotNetLab1] SET RECURSIVE_TRIGGERS OFF
GO
ALTER DATABASE [DotNetLab1] SET  DISABLE_BROKER
GO
ALTER DATABASE [DotNetLab1] SET AUTO_UPDATE_STATISTICS_ASYNC OFF
GO
ALTER DATABASE [DotNetLab1] SET DATE_CORRELATION_OPTIMIZATION OFF
GO
ALTER DATABASE [DotNetLab1] SET TRUSTWORTHY OFF
GO
ALTER DATABASE [DotNetLab1] SET ALLOW_SNAPSHOT_ISOLATION OFF
GO
ALTER DATABASE [DotNetLab1] SET PARAMETERIZATION SIMPLE
GO
ALTER DATABASE [DotNetLab1] SET READ_COMMITTED_SNAPSHOT OFF
GO
ALTER DATABASE [DotNetLab1] SET HONOR_BROKER_PRIORITY OFF
GO
ALTER DATABASE [DotNetLab1] SET  READ_WRITE
GO
ALTER DATABASE [DotNetLab1] SET RECOVERY SIMPLE
GO
ALTER DATABASE [DotNetLab1] SET  MULTI_USER
GO
ALTER DATABASE [DotNetLab1] SET PAGE_VERIFY CHECKSUM
GO
ALTER DATABASE [DotNetLab1] SET DB_CHAINING OFF
GO
USE [DotNetLab1]
GO
/****** Object:  User [LabUser]    Script Date: 11/05/2013 21:52:01 ******/
CREATE USER [LabUser] FOR LOGIN [LabUser] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  Table [dbo].[Warehouses]    Script Date: 11/05/2013 21:52:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Warehouses](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[internal] [bit] NOT NULL,
	[tel] [nvarchar](20) NULL,
	[mail] [nvarchar](50) NULL,
	[name] [nvarchar](30) NOT NULL,
	[street] [nvarchar](50) NOT NULL,
	[num] [nvarchar](8) NOT NULL,
	[city] [nvarchar](30) NOT NULL,
	[code] [nvarchar](7) NOT NULL,
	[deleted] [bit] NOT NULL,
	[version] [timestamp] NOT NULL,
 CONSTRAINT [PK_Warehouses] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Warehouses] ON
INSERT [dbo].[Warehouses] ([id], [internal], [tel], [mail], [name], [street], [num], [city], [code], [deleted]) VALUES (773, 1, N'+48500100100', N'mail@wp.pl', N'Magazyn 1', N'Ulica 1', N'12A', N'Miasto 1', N'10-222', 0)
INSERT [dbo].[Warehouses] ([id], [internal], [tel], [mail], [name], [street], [num], [city], [code], [deleted]) VALUES (774, 1, N'a', N'asd@wp.pl', N'a', N'a', N'a', N'a', N'a', 1)
INSERT [dbo].[Warehouses] ([id], [internal], [tel], [mail], [name], [street], [num], [city], [code], [deleted]) VALUES (775, 1, N'ads', N'ads@asd.asd', N'asd', N'ads', N'ads', N'ads', N'ads', 1)
INSERT [dbo].[Warehouses] ([id], [internal], [tel], [mail], [name], [street], [num], [city], [code], [deleted]) VALUES (776, 1, N'123456789', N'mail@gmail.com', N'Magazyn 2', N'Ulica 2', N'10B', N'Miasto 2', N'11-222', 0)
INSERT [dbo].[Warehouses] ([id], [internal], [tel], [mail], [name], [street], [num], [city], [code], [deleted]) VALUES (777, 1, N'000000', N'mmm@aaa.pl', N'Magazyn 3', N'Ulica 3', N'30A', N'Miasto 3', N'33-333', 0)
INSERT [dbo].[Warehouses] ([id], [internal], [tel], [mail], [name], [street], [num], [city], [code], [deleted]) VALUES (778, 0, N'444444444', N'email@mail.com', N'Partner 1', N'Ulica 1', N'12', N'Miasto 1', N'12-133', 0)
INSERT [dbo].[Warehouses] ([id], [internal], [tel], [mail], [name], [street], [num], [city], [code], [deleted]) VALUES (779, 0, N'123321123', N'asdfg@live.com', N'Partner 2', N'Ulica 2', N'12', N'Miasto 2', N'11-111', 0)
INSERT [dbo].[Warehouses] ([id], [internal], [tel], [mail], [name], [street], [num], [city], [code], [deleted]) VALUES (780, 1, N'123123123', N'mail@mail.pl', N'Pełny', N'Ulica', N'123', N'M1', N'12-123', 0)
INSERT [dbo].[Warehouses] ([id], [internal], [tel], [mail], [name], [street], [num], [city], [code], [deleted]) VALUES (781, 1, N'09090909090', N'mail.mail@mail.pl', N'Pusty', N'Ulica', N'33', N'Miasto', N'11-111', 0)
SET IDENTITY_INSERT [dbo].[Warehouses] OFF
/****** Object:  Table [dbo].[Products]    Script Date: 11/05/2013 21:52:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](30) NOT NULL,
	[date] [date] NULL,
	[price] [money] NOT NULL,
	[version] [timestamp] NOT NULL,
 CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Products] ON
INSERT [dbo].[Products] ([id], [name], [date], [price]) VALUES (151, N'Produkt 1', CAST(0xC0370B00 AS Date), 123.1200)
INSERT [dbo].[Products] ([id], [name], [date], [price]) VALUES (152, N'Produkt 2', CAST(0xC2370B00 AS Date), 0.9900)
SET IDENTITY_INSERT [dbo].[Products] OFF
/****** Object:  Table [dbo].[Sectors]    Script Date: 11/05/2013 21:52:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sectors](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[warehouse_id] [int] NOT NULL,
	[number] [int] NOT NULL,
	[limit] [int] NOT NULL,
	[deleted] [bit] NOT NULL,
	[version] [timestamp] NOT NULL,
 CONSTRAINT [PK_Sectors] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Sectors] ON
INSERT [dbo].[Sectors] ([id], [warehouse_id], [number], [limit], [deleted]) VALUES (519, 773, 1, 3, 0)
INSERT [dbo].[Sectors] ([id], [warehouse_id], [number], [limit], [deleted]) VALUES (520, 773, 2, 5, 0)
INSERT [dbo].[Sectors] ([id], [warehouse_id], [number], [limit], [deleted]) VALUES (521, 773, 3, 5, 0)
INSERT [dbo].[Sectors] ([id], [warehouse_id], [number], [limit], [deleted]) VALUES (522, 776, 1, 5, 0)
INSERT [dbo].[Sectors] ([id], [warehouse_id], [number], [limit], [deleted]) VALUES (523, 776, 2, 5, 0)
INSERT [dbo].[Sectors] ([id], [warehouse_id], [number], [limit], [deleted]) VALUES (524, 777, 1, 10, 0)
INSERT [dbo].[Sectors] ([id], [warehouse_id], [number], [limit], [deleted]) VALUES (525, 777, 2, 3, 1)
INSERT [dbo].[Sectors] ([id], [warehouse_id], [number], [limit], [deleted]) VALUES (526, 777, 3, 1, 1)
INSERT [dbo].[Sectors] ([id], [warehouse_id], [number], [limit], [deleted]) VALUES (527, 778, 1, 0, 0)
INSERT [dbo].[Sectors] ([id], [warehouse_id], [number], [limit], [deleted]) VALUES (528, 779, 1, 0, 0)
INSERT [dbo].[Sectors] ([id], [warehouse_id], [number], [limit], [deleted]) VALUES (529, 780, 1, 1, 0)
SET IDENTITY_INSERT [dbo].[Sectors] OFF
/****** Object:  Table [dbo].[Partners]    Script Date: 11/05/2013 21:52:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Partners](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[warehouse_id] [int] NOT NULL,
	[street] [nvarchar](30) NOT NULL,
	[num] [nvarchar](8) NOT NULL,
	[city] [nvarchar](30) NOT NULL,
	[code] [nvarchar](7) NOT NULL,
	[tel] [nvarchar](20) NOT NULL,
	[mail] [nvarchar](50) NULL,
	[version] [timestamp] NOT NULL,
 CONSTRAINT [PK_Partners] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Partners] ON
INSERT [dbo].[Partners] ([id], [warehouse_id], [street], [num], [city], [code], [tel], [mail]) VALUES (79, 778, N'Ulica 1', N'12', N'Miasto 1', N'12-133', N'444444444', N'email@mail.com')
INSERT [dbo].[Partners] ([id], [warehouse_id], [street], [num], [city], [code], [tel], [mail]) VALUES (80, 779, N'Ulica 2', N'12', N'Miasto 2', N'11-111', N'123321123', N'asdfg@live.com')
SET IDENTITY_INSERT [dbo].[Partners] OFF
/****** Object:  Table [dbo].[Group]    Script Date: 11/05/2013 21:52:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Group](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[sector_id] [int] NOT NULL,
	[version] [timestamp] NOT NULL,
 CONSTRAINT [PK_Group] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Group] ON
INSERT [dbo].[Group] ([id], [sector_id]) VALUES (288, 522)
INSERT [dbo].[Group] ([id], [sector_id]) VALUES (289, 527)
INSERT [dbo].[Group] ([id], [sector_id]) VALUES (290, 520)
INSERT [dbo].[Group] ([id], [sector_id]) VALUES (291, 521)
INSERT [dbo].[Group] ([id], [sector_id]) VALUES (292, 529)
SET IDENTITY_INSERT [dbo].[Group] OFF
/****** Object:  Table [dbo].[Shift]    Script Date: 11/05/2013 21:52:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Shift](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[sender_id] [int] NOT NULL,
	[recipient_id] [int] NOT NULL,
	[date] [datetime] NOT NULL,
	[group_id] [int] NOT NULL,
	[latest] [bit] NOT NULL,
	[version] [timestamp] NOT NULL,
 CONSTRAINT [PK_Shift] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Shift] ON
INSERT [dbo].[Shift] ([id], [sender_id], [recipient_id], [date], [group_id], [latest]) VALUES (141, 778, 773, CAST(0x0000A26D015856C0 AS DateTime), 288, 0)
INSERT [dbo].[Shift] ([id], [sender_id], [recipient_id], [date], [group_id], [latest]) VALUES (142, 779, 773, CAST(0x0000A26D01593296 AS DateTime), 289, 0)
INSERT [dbo].[Shift] ([id], [sender_id], [recipient_id], [date], [group_id], [latest]) VALUES (143, 773, 776, CAST(0x0000A26D015962FA AS DateTime), 288, 1)
INSERT [dbo].[Shift] ([id], [sender_id], [recipient_id], [date], [group_id], [latest]) VALUES (144, 773, 777, CAST(0x0000A26D01598058 AS DateTime), 289, 0)
INSERT [dbo].[Shift] ([id], [sender_id], [recipient_id], [date], [group_id], [latest]) VALUES (145, 777, 777, CAST(0x0000A26D01598724 AS DateTime), 289, 0)
INSERT [dbo].[Shift] ([id], [sender_id], [recipient_id], [date], [group_id], [latest]) VALUES (146, 777, 778, CAST(0x0000A26D01599264 AS DateTime), 289, 1)
INSERT [dbo].[Shift] ([id], [sender_id], [recipient_id], [date], [group_id], [latest]) VALUES (147, 778, 773, CAST(0x0000A26D0159C693 AS DateTime), 290, 1)
INSERT [dbo].[Shift] ([id], [sender_id], [recipient_id], [date], [group_id], [latest]) VALUES (148, 778, 773, CAST(0x0000A26D0159D583 AS DateTime), 291, 1)
INSERT [dbo].[Shift] ([id], [sender_id], [recipient_id], [date], [group_id], [latest]) VALUES (149, 779, 780, CAST(0x0000A26D015A6623 AS DateTime), 292, 1)
SET IDENTITY_INSERT [dbo].[Shift] OFF
/****** Object:  Table [dbo].[Product_Group]    Script Date: 11/05/2013 21:52:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Product_Group](
	[product_id] [int] NOT NULL,
	[group_id] [int] NOT NULL,
	[count] [int] NOT NULL,
	[version] [timestamp] NOT NULL,
 CONSTRAINT [PK_Product_Group] PRIMARY KEY CLUSTERED 
(
	[product_id] ASC,
	[group_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[Product_Group] ([product_id], [group_id], [count]) VALUES (151, 288, 3)
INSERT [dbo].[Product_Group] ([product_id], [group_id], [count]) VALUES (151, 289, 1)
INSERT [dbo].[Product_Group] ([product_id], [group_id], [count]) VALUES (151, 290, 20)
INSERT [dbo].[Product_Group] ([product_id], [group_id], [count]) VALUES (151, 292, 1)
INSERT [dbo].[Product_Group] ([product_id], [group_id], [count]) VALUES (152, 288, 5)
INSERT [dbo].[Product_Group] ([product_id], [group_id], [count]) VALUES (152, 289, 1)
INSERT [dbo].[Product_Group] ([product_id], [group_id], [count]) VALUES (152, 291, 2)
/****** Object:  ForeignKey [FK_Sectors_Warehouses]    Script Date: 11/05/2013 21:52:01 ******/
ALTER TABLE [dbo].[Sectors]  WITH CHECK ADD  CONSTRAINT [FK_Sectors_Warehouses] FOREIGN KEY([warehouse_id])
REFERENCES [dbo].[Warehouses] ([id])
GO
ALTER TABLE [dbo].[Sectors] CHECK CONSTRAINT [FK_Sectors_Warehouses]
GO
/****** Object:  ForeignKey [FK_Partners_Warehouses]    Script Date: 11/05/2013 21:52:01 ******/
ALTER TABLE [dbo].[Partners]  WITH CHECK ADD  CONSTRAINT [FK_Partners_Warehouses] FOREIGN KEY([warehouse_id])
REFERENCES [dbo].[Warehouses] ([id])
GO
ALTER TABLE [dbo].[Partners] CHECK CONSTRAINT [FK_Partners_Warehouses]
GO
/****** Object:  ForeignKey [FK_Group_Sectors]    Script Date: 11/05/2013 21:52:01 ******/
ALTER TABLE [dbo].[Group]  WITH CHECK ADD  CONSTRAINT [FK_Group_Sectors] FOREIGN KEY([sector_id])
REFERENCES [dbo].[Sectors] ([id])
GO
ALTER TABLE [dbo].[Group] CHECK CONSTRAINT [FK_Group_Sectors]
GO
/****** Object:  ForeignKey [FK_Shift_Group]    Script Date: 11/05/2013 21:52:01 ******/
ALTER TABLE [dbo].[Shift]  WITH CHECK ADD  CONSTRAINT [FK_Shift_Group] FOREIGN KEY([group_id])
REFERENCES [dbo].[Group] ([id])
GO
ALTER TABLE [dbo].[Shift] CHECK CONSTRAINT [FK_Shift_Group]
GO
/****** Object:  ForeignKey [FK_Shift_Warehouses_recipient]    Script Date: 11/05/2013 21:52:01 ******/
ALTER TABLE [dbo].[Shift]  WITH CHECK ADD  CONSTRAINT [FK_Shift_Warehouses_recipient] FOREIGN KEY([recipient_id])
REFERENCES [dbo].[Warehouses] ([id])
GO
ALTER TABLE [dbo].[Shift] CHECK CONSTRAINT [FK_Shift_Warehouses_recipient]
GO
/****** Object:  ForeignKey [FK_Shift_Warehouses_sender]    Script Date: 11/05/2013 21:52:01 ******/
ALTER TABLE [dbo].[Shift]  WITH CHECK ADD  CONSTRAINT [FK_Shift_Warehouses_sender] FOREIGN KEY([sender_id])
REFERENCES [dbo].[Warehouses] ([id])
GO
ALTER TABLE [dbo].[Shift] CHECK CONSTRAINT [FK_Shift_Warehouses_sender]
GO
/****** Object:  ForeignKey [FK_Product_Group_Group]    Script Date: 11/05/2013 21:52:01 ******/
ALTER TABLE [dbo].[Product_Group]  WITH CHECK ADD  CONSTRAINT [FK_Product_Group_Group] FOREIGN KEY([group_id])
REFERENCES [dbo].[Group] ([id])
GO
ALTER TABLE [dbo].[Product_Group] CHECK CONSTRAINT [FK_Product_Group_Group]
GO
/****** Object:  ForeignKey [FK_Product_Group_Products]    Script Date: 11/05/2013 21:52:01 ******/
ALTER TABLE [dbo].[Product_Group]  WITH CHECK ADD  CONSTRAINT [FK_Product_Group_Products] FOREIGN KEY([product_id])
REFERENCES [dbo].[Products] ([id])
GO
ALTER TABLE [dbo].[Product_Group] CHECK CONSTRAINT [FK_Product_Group_Products]
GO
