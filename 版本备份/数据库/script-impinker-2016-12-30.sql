USE [ImPinker]
GO
/****** Object:  Table [dbo].[Article]    Script Date: 2016/12/30 15:15:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Article](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ArticleName] [nvarchar](100) NOT NULL,
	[Url] [varchar](200) NOT NULL,
	[CoverImage] [varchar](200) NULL,
	[UserId] [int] NOT NULL,
	[KeyWords] [nvarchar](100) NULL,
	[Description] [nvarchar](200) NOT NULL,
	[State] [tinyint] NOT NULL,
	[PublishTime] [datetime] NULL,
	[CreateTime] [datetime] NOT NULL,
	[UpdateTime] [datetime] NOT NULL,
	[ComPany] [nvarchar](100) NULL,
 CONSTRAINT [PK_Article] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_Article] UNIQUE NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ArticleComment]    Script Date: 2016/12/30 15:15:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ArticleComment](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ArticleId] [bigint] NOT NULL,
	[UserId] [int] NOT NULL,
	[Content] [nvarchar](200) NOT NULL,
	[ToCommentId] [bigint] NULL,
	[CreateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_COMMENT] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ArticleSnaps]    Script Date: 2016/12/30 15:15:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ArticleSnaps](
	[ArticleId] [bigint] NOT NULL,
	[FirstImageUrl] [varchar](200) NULL,
	[KeyWords] [nvarchar](100) NULL,
	[Description] [nvarchar](300) NULL,
	[ConTent] [text] NOT NULL,
	[CreateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_ARTICLESNAPS] PRIMARY KEY CLUSTERED 
(
	[ArticleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ArticleTag]    Script Date: 2016/12/30 15:15:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ArticleTag](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TagName] [nvarchar](50) NOT NULL,
	[UserId] [int] NOT NULL,
	[FrountShowState] [int] NOT NULL CONSTRAINT [DF_ArticleTag_FrountShowState]  DEFAULT ((0)),
	[IsDelete] [bit] NOT NULL CONSTRAINT [DF_ArticleTag_IsDelete]  DEFAULT ((0)),
	[CreateTime] [datetime] NULL,
	[UpdateTime] [datetime] NULL,
 CONSTRAINT [PK_ARTICLETAG] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ArticleVote]    Script Date: 2016/12/30 15:15:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ArticleVote](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ArticleId] [bigint] NULL,
	[UserId] [int] NULL,
	[Vote] [bit] NULL,
	[CreateTime] [datetime] NULL,
	[UpdateTime] [datetime] NULL,
 CONSTRAINT [PK_ARTICLEVOTE] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Users]    Script Date: 2016/12/30 15:15:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [varchar](100) NOT NULL,
	[ShowName] [varchar](100) NULL,
	[PassWord] [varchar](20) NULL,
	[Sex] [bit] NULL,
	[PhoneNum] [varchar](20) NULL,
	[Email] [varchar](50) NULL,
	[Age] [int] NULL,
	[ImgUrl] [varchar](max) NULL,
	[IsEnable] [bit] NOT NULL CONSTRAINT [DF_Users_IsEnable]  DEFAULT ((1)),
	[CreateTime] [datetime] NOT NULL CONSTRAINT [DF_Users_CreateTime]  DEFAULT (getdate()),
	[UpdateTime] [datetime] NOT NULL CONSTRAINT [DF_Users_UpdateTime]  DEFAULT (getdate()),
	[AspNetId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_USERS] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_Users] UNIQUE NONCLUSTERED 
(
	[AspNetId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[Article]  WITH CHECK ADD  CONSTRAINT [FK_ARTICLE_REFERENCE_USERS] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Article] CHECK CONSTRAINT [FK_ARTICLE_REFERENCE_USERS]
GO
ALTER TABLE [dbo].[ArticleComment]  WITH CHECK ADD  CONSTRAINT [FK_COMMENT_REFERENCE_TRAVELPA] FOREIGN KEY([ArticleId])
REFERENCES [dbo].[Article] ([Id])
GO
ALTER TABLE [dbo].[ArticleComment] CHECK CONSTRAINT [FK_COMMENT_REFERENCE_TRAVELPA]
GO
ALTER TABLE [dbo].[ArticleComment]  WITH CHECK ADD  CONSTRAINT [FK_COMMENT_REFERENCE_USER1] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[ArticleComment] CHECK CONSTRAINT [FK_COMMENT_REFERENCE_USER1]
GO
ALTER TABLE [dbo].[ArticleSnaps]  WITH CHECK ADD  CONSTRAINT [FK_ARTICLES_REFERENCE_ARTICLE] FOREIGN KEY([ArticleId])
REFERENCES [dbo].[Article] ([Id])
GO
ALTER TABLE [dbo].[ArticleSnaps] CHECK CONSTRAINT [FK_ARTICLES_REFERENCE_ARTICLE]
GO
ALTER TABLE [dbo].[ArticleTag]  WITH CHECK ADD  CONSTRAINT [FK_ARTICLET_REFERENCE_USERS] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[ArticleTag] CHECK CONSTRAINT [FK_ARTICLET_REFERENCE_USERS]
GO
ALTER TABLE [dbo].[ArticleVote]  WITH CHECK ADD  CONSTRAINT [FK_ARTICLEV_REFERENCE_ARTICLE] FOREIGN KEY([ArticleId])
REFERENCES [dbo].[Article] ([Id])
GO
ALTER TABLE [dbo].[ArticleVote] CHECK CONSTRAINT [FK_ARTICLEV_REFERENCE_ARTICLE]
GO
ALTER TABLE [dbo].[ArticleVote]  WITH CHECK ADD  CONSTRAINT [FK_ARTICLEV_REFERENCE_USERS] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[ArticleVote] CHECK CONSTRAINT [FK_ARTICLEV_REFERENCE_USERS]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N' 状态:    -1:删除   0:默认    1:正常可显示   2: 待审核   3:审核不通过' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Article', @level2type=N'COLUMN',@level2name=N'State'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'1:like   0: dislike' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ArticleVote', @level2type=N'COLUMN',@level2name=N'Vote'
GO
