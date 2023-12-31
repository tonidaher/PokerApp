USE [Poker]
GO
/****** Object:  Table [dbo].[Join_PlayerHand]    Script Date: 12/23/2020 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
if EXISTS (select 1 from sys.tables where name = 'join_playerHand') BEGIN DROP TABLE dbo.Join_PlayerHand END
if EXISTS (select 1 from sys.tables where name = 'join_HandAction') BEGIN DROP TABLE dbo.Join_HandAction END
if EXISTS (select 1 from sys.tables where name = 'Hand') BEGIN DROP TABLE dbo.Hand END
if EXISTS (select 1 from sys.tables where name = 'Player') BEGIN DROP TABLE dbo.Player END
if EXISTS (select 1 from sys.tables where name = 'Tournament') BEGIN DROP TABLE dbo.Tournament END
if EXISTS (select 1 from sys.tables where name = 'MetricThreshold') BEGIN DROP TABLE dbo.MetricThreshold END
if EXISTS (select 1 from sys.tables where name = 'Dim_Action') BEGIN DROP TABLE dbo.Dim_Action END
if EXISTS (select 1 from sys.tables where name = 'Dim_Step') BEGIN DROP TABLE dbo.Dim_Step END

CREATE TABLE [dbo].[Join_PlayerHand](
	[PlayerName] [nvarchar](100) NOT NULL,
	[HandId] [nvarchar](200) NOT NULL,
	[SeatNumber] [int] NOT NULL,
	[InitialStack] [float] NOT NULL,
	[Card1] [nvarchar](6) NULL,
	[Card2] [nvarchar](6) NULL,
	[Card3] [nvarchar](6) NULL,
	[Card4] [nvarchar](6) NULL,
	[EndStack] [float] NOT NULL,
	[IsSmallBlind] [bit] NULL,
	[IsBigBlind] [bit] NULL,
	[FoldPreflop] [bit] NULL,
	[CheckPreflop] [bit] NULL,
	[CallPreflop] [bit] NULL,
	[BetPreflop] [bit] NULL,
	[RaisePreflop] [bit] NULL,
	[SeeFlop] [bit] NULL,
	[FoldFlop] [bit] NULL,
	[CheckFlop] [bit] NULL,
	[CallFlop] [bit] NULL,
	[BetFlop] [bit] NULL,
	[RaiseFlop] [bit] NULL,
	[SeeTurn] [bit] NULL,
	[FoldTurn] [bit] NULL,
	[CheckTurn] [bit] NULL,
	[CallTurn] [bit] NULL,
	[BetTurn] [bit] NULL,
	[RaiseTurn] [bit] NULL,
	[SeeRiver] [bit] NULL,
	[FoldRiver] [bit] NULL,
	[CheckRiver] [bit] NULL,
	[CallRiver] [bit] NULL,
	[BetRiver] [bit] NULL,
	[RaiseRiver] [bit] NULL,
	[SeeShowdown] [bit] NULL,
	[Collected] [bit] NULL,
 CONSTRAINT [PK_Join_PlayerHand] PRIMARY KEY CLUSTERED 
(
	[PlayerName] ASC,
	[HandId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Hand]    Script Date: 12/23/2020 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Hand](
	[HandId] [nvarchar](200) NOT NULL,
	[HandName] [nvarchar](1000) NOT NULL,
	[DealerPosition] [int] NOT NULL,
	[TimeStamp] [datetime] NOT NULL,
	[TournamentFileName] [nvarchar](200) NOT NULL,
	[MaxPlayers] [int] NULL,
	[HandLevel] [int] NULL,
	[BigBlind] [float] NULL,
	[SmallBlind] [float] NULL,
	[Ante] [float] NULL,
	[Line] [int] NULL,
 CONSTRAINT [PK_Hand] PRIMARY KEY CLUSTERED 
(
	[HandId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tournament]    Script Date: 12/23/2020 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tournament](
	[TournamentId] [int] NOT NULL,
	[TournamentName] [nvarchar](200) NOT NULL,
	[TournamentType] [nvarchar](20) NOT NULL,
	[BuyIn] [nvarchar](20) NULL,
	[Filename] [nvarchar](200) NOT NULL,
	[PokerSite] [nvarchar](50) NULL,
	[Date] [datetime] NOT NULL,
	[IsRealMoney] [bit] NOT NULL,
	[TournamentIndicator] [nvarchar](20) NULL,
	[Lines] [int] NULL,
	[Bytes] [int] NULL,
 CONSTRAINT [PK_Tournament] PRIMARY KEY CLUSTERED 
(
	[Filename] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[CurrentPlayers]    Script Date: 12/23/2020 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
  
CREATE OR ALTER  VIEW [dbo].[CurrentPlayers]  
AS  
WITH CurrentTournaments as(  
select distinct TournamentFileName from hand (nolock) where datediff(MINUTE,dateadd(Hour,-5,TimeStamp),getdate())<5  
union   
select top 1 tournamentFileName from hand (nolock) order by TimeStamp desc  
),  
HandsOfTournaments as (  
SELECT    t.TournamentId, t.TournamentName, h.HandId, HandName, DealerPosition, ct.TournamentFileName, HandLevel, BigBlind, SmallBlind, Ante, Line,h.timestamp, RANK() OVER (  
 PARTITION BY ct.tournamentFilename  
 ORDER BY timestamp desc) as [ranking]  
 FROM            dbo.Hand (nolock) AS h  
 INNER JOIN CurrentTournaments ct on ct.TournamentFileName = h.TournamentFileName   
 INNER JOIN Tournament t on ct.TournamentFileName = t.Filename)    
,lastHand AS (SELECT * FROM HandsOfTournaments  
     WHERE ranking = 1)  
, currentPlayers AS  
    (SELECT DISTINCT h.TournamentId, h.TournamentName, h.TournamentFileName, ph.PlayerName,ph.SeatNumber  
      FROM dbo.Join_PlayerHand (nolock) AS ph   
   INNER JOIN lastHand  AS h ON h.HandId = ph.HandId)    
,PlayerHandsCount AS  
    (SELECT cp.TournamentId, cp.TournamentName, cp.TournamentFileName, cp.PlayerName,cp.SeatNumber, COUNT(1) AS NbHands  
      FROM currentPlayers AS cp   
   INNER JOIN dbo.Join_PlayerHand (nolock) AS ph ON ph.PlayerName = cp.PlayerName  
   INNER JOIN lastHand h on h.HandId = ph.HandId  
      GROUP BY cp.TournamentId, cp.TournamentName,cp.TournamentFileName, cp.PlayerName, cp.SeatNumber)  
    SELECT TournamentId, TournamentName, TournamentFileName, PlayerName, NbHands,SeatNumber  
     FROM            PlayerHandsCount AS PlayerHandsCount_1  
GO
/****** Object:  Table [dbo].[Dim_Action]    Script Date: 12/23/2020 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Dim_Action](
	[ActionTypeId] [smallint] NOT NULL,
	[ActionName] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_Dim_Action] PRIMARY KEY CLUSTERED 
(
	[ActionTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Dim_Step]    Script Date: 12/23/2020 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Dim_Step](
	[StepId] [smallint] NOT NULL,
	[StepName] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_Dim_Step] PRIMARY KEY CLUSTERED 
(
	[StepId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Join_HandAction]    Script Date: 12/23/2020 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Join_HandAction](
	[HandId] [nvarchar](200) NOT NULL,
	[PlayerName] [nvarchar](100) NOT NULL,
	[ActionTypeId] [smallint] NOT NULL,
	[Amount] [float] NOT NULL,
	[PotAmount] [float] NOT NULL,
	[ActionOrder] [int] NOT NULL,
	[StepId] [smallint] NOT NULL,
	[IsAllIn] [bit] NOT NULL,
 CONSTRAINT [PK_Join_HandAction] PRIMARY KEY CLUSTERED 
(
	[HandId] ASC,
	[PlayerName] ASC,
	[ActionOrder] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MetricThreshold]    Script Date: 12/23/2020 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MetricThreshold](
	[MetricName] [nvarchar](50) NULL,
	[MetricValue] [int] NULL,
	[Range] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Player]    Script Date: 12/23/2020 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Player](
	[PlayerName] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Player] PRIMARY KEY CLUSTERED 
(
	[PlayerName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Tournament] ADD  CONSTRAINT [DF_Tournament_Bytes]  DEFAULT ((0)) FOR [Bytes]
GO
ALTER TABLE [dbo].[Hand]  WITH CHECK ADD  CONSTRAINT [FK_Hand_Tournament] FOREIGN KEY([TournamentFileName])
REFERENCES [dbo].[Tournament] ([Filename])
GO
ALTER TABLE [dbo].[Hand] CHECK CONSTRAINT [FK_Hand_Tournament]
GO
ALTER TABLE [dbo].[Join_HandAction]  WITH CHECK ADD  CONSTRAINT [FK_Join_HandAction_Dim_Action] FOREIGN KEY([ActionTypeId])
REFERENCES [dbo].[Dim_Action] ([ActionTypeId])
GO
ALTER TABLE [dbo].[Join_HandAction] CHECK CONSTRAINT [FK_Join_HandAction_Dim_Action]
GO
ALTER TABLE [dbo].[Join_HandAction]  WITH CHECK ADD  CONSTRAINT [FK_Join_HandAction_Dim_Step] FOREIGN KEY([StepId])
REFERENCES [dbo].[Dim_Step] ([StepId])
GO
ALTER TABLE [dbo].[Join_HandAction] CHECK CONSTRAINT [FK_Join_HandAction_Dim_Step]
GO
ALTER TABLE [dbo].[Join_HandAction]  WITH CHECK ADD  CONSTRAINT [FK_Join_HandAction_Hand] FOREIGN KEY([HandId])
REFERENCES [dbo].[Hand] ([HandId])
GO
ALTER TABLE [dbo].[Join_HandAction] CHECK CONSTRAINT [FK_Join_HandAction_Hand]
GO
ALTER TABLE [dbo].[Join_HandAction]  WITH CHECK ADD  CONSTRAINT [FK_Join_HandAction_Player] FOREIGN KEY([PlayerName])
REFERENCES [dbo].[Player] ([PlayerName])
GO
ALTER TABLE [dbo].[Join_HandAction] CHECK CONSTRAINT [FK_Join_HandAction_Player]
GO
ALTER TABLE [dbo].[Join_PlayerHand]  WITH CHECK ADD  CONSTRAINT [FK_Join_PlayerHand_Hand] FOREIGN KEY([HandId])
REFERENCES [dbo].[Hand] ([HandId])
GO
ALTER TABLE [dbo].[Join_PlayerHand] CHECK CONSTRAINT [FK_Join_PlayerHand_Hand]
GO
ALTER TABLE [dbo].[Join_PlayerHand]  WITH CHECK ADD  CONSTRAINT [FK_Join_PlayerHand_Player] FOREIGN KEY([PlayerName])
REFERENCES [dbo].[Player] ([PlayerName])
GO
ALTER TABLE [dbo].[Join_PlayerHand] CHECK CONSTRAINT [FK_Join_PlayerHand_Player]
GO



TRUNCATE TABLE dbo.Dim_Action
INSERT INTO [dbo].[Dim_Action]
           ([ActionTypeId]
           ,[ActionName])
SELECT 1,'Ante' union
SELECT 2,'Small Blind' union
SELECT 3,'Big Blind' union
SELECT 4,'Raise' union
SELECT 5,'Fold' union
SELECT 6,'Call' union
SELECT 7,'Check' union
SELECT 8,'Bet' union
SELECT 9,'Collected' union
SELECT 10, 'Shows Hand'
GO


GO
TRUNCATE TABLE dbo.Dim_Step
INSERT INTO [dbo].[Dim_Step]
           ([StepId]
           ,[StepName])
SELECT 1,'AnteBlind' union
SELECT 2,'PreFlop' union
SELECT 3,'Flop' union
SELECT 4,'Turn' union
SELECT 5,'River' union
SELECT 6,'Showdown'
GO

GO
TRUNCATE TABLE [MetricThreshold]
INSERT INTO [dbo].[MetricThreshold]
           ([MetricName]
           ,[MetricValue]
           ,[Range])
SELECT 'Vpip',20,3 union
SELECT 'PreFlopRaise',17,3 union
SELECT 'Af',2,1 union
SELECT 'CBet',70,5 union 
SELECT 'Roi',0,null
GO

