USE [master]
GO
/****** Object:  Database [DotNetLab1]    Script Date: 11/05/2013 21:51:04 ******/
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
/****** Object:  User [LabUser]    Script Date: 11/05/2013 21:51:04 ******/
CREATE USER [LabUser] FOR LOGIN [LabUser] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  Table [dbo].[Warehouses]    Script Date: 11/05/2013 21:51:06 ******/
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
/****** Object:  Table [dbo].[Products]    Script Date: 11/05/2013 21:51:06 ******/
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
/****** Object:  Table [dbo].[Sectors]    Script Date: 11/05/2013 21:51:06 ******/
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
/****** Object:  Table [dbo].[Partners]    Script Date: 11/05/2013 21:51:06 ******/
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
/****** Object:  Table [dbo].[Group]    Script Date: 11/05/2013 21:51:06 ******/
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
/****** Object:  Table [dbo].[Shift]    Script Date: 11/05/2013 21:51:06 ******/
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
/****** Object:  Table [dbo].[Product_Group]    Script Date: 11/05/2013 21:51:06 ******/
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
/****** Object:  ForeignKey [FK_Sectors_Warehouses]    Script Date: 11/05/2013 21:51:06 ******/
ALTER TABLE [dbo].[Sectors]  WITH CHECK ADD  CONSTRAINT [FK_Sectors_Warehouses] FOREIGN KEY([warehouse_id])
REFERENCES [dbo].[Warehouses] ([id])
GO
ALTER TABLE [dbo].[Sectors] CHECK CONSTRAINT [FK_Sectors_Warehouses]
GO
/****** Object:  ForeignKey [FK_Partners_Warehouses]    Script Date: 11/05/2013 21:51:06 ******/
ALTER TABLE [dbo].[Partners]  WITH CHECK ADD  CONSTRAINT [FK_Partners_Warehouses] FOREIGN KEY([warehouse_id])
REFERENCES [dbo].[Warehouses] ([id])
GO
ALTER TABLE [dbo].[Partners] CHECK CONSTRAINT [FK_Partners_Warehouses]
GO
/****** Object:  ForeignKey [FK_Group_Sectors]    Script Date: 11/05/2013 21:51:06 ******/
ALTER TABLE [dbo].[Group]  WITH CHECK ADD  CONSTRAINT [FK_Group_Sectors] FOREIGN KEY([sector_id])
REFERENCES [dbo].[Sectors] ([id])
GO
ALTER TABLE [dbo].[Group] CHECK CONSTRAINT [FK_Group_Sectors]
GO
/****** Object:  ForeignKey [FK_Shift_Group]    Script Date: 11/05/2013 21:51:06 ******/
ALTER TABLE [dbo].[Shift]  WITH CHECK ADD  CONSTRAINT [FK_Shift_Group] FOREIGN KEY([group_id])
REFERENCES [dbo].[Group] ([id])
GO
ALTER TABLE [dbo].[Shift] CHECK CONSTRAINT [FK_Shift_Group]
GO
/****** Object:  ForeignKey [FK_Shift_Warehouses_recipient]    Script Date: 11/05/2013 21:51:06 ******/
ALTER TABLE [dbo].[Shift]  WITH CHECK ADD  CONSTRAINT [FK_Shift_Warehouses_recipient] FOREIGN KEY([recipient_id])
REFERENCES [dbo].[Warehouses] ([id])
GO
ALTER TABLE [dbo].[Shift] CHECK CONSTRAINT [FK_Shift_Warehouses_recipient]
GO
/****** Object:  ForeignKey [FK_Shift_Warehouses_sender]    Script Date: 11/05/2013 21:51:06 ******/
ALTER TABLE [dbo].[Shift]  WITH CHECK ADD  CONSTRAINT [FK_Shift_Warehouses_sender] FOREIGN KEY([sender_id])
REFERENCES [dbo].[Warehouses] ([id])
GO
ALTER TABLE [dbo].[Shift] CHECK CONSTRAINT [FK_Shift_Warehouses_sender]
GO
/****** Object:  ForeignKey [FK_Product_Group_Group]    Script Date: 11/05/2013 21:51:06 ******/
ALTER TABLE [dbo].[Product_Group]  WITH CHECK ADD  CONSTRAINT [FK_Product_Group_Group] FOREIGN KEY([group_id])
REFERENCES [dbo].[Group] ([id])
GO
ALTER TABLE [dbo].[Product_Group] CHECK CONSTRAINT [FK_Product_Group_Group]
GO
/****** Object:  ForeignKey [FK_Product_Group_Products]    Script Date: 11/05/2013 21:51:06 ******/
ALTER TABLE [dbo].[Product_Group]  WITH CHECK ADD  CONSTRAINT [FK_Product_Group_Products] FOREIGN KEY([product_id])
REFERENCES [dbo].[Products] ([id])
GO
ALTER TABLE [dbo].[Product_Group] CHECK CONSTRAINT [FK_Product_Group_Products]
GO
