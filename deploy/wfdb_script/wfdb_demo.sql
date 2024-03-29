USE [WfDB]
GO
INSERT [dbo].[WfProcess] ([ProcessGUID], [ProcessName], [PageUrl], [XmlFileName], [XmlFilePath], [Description], [CreatedDateTime], [LastUpdatedDateTime]) VALUES (N'8593aa9c-1c0c-45d0-8ad2-e025a1c74fa6', N'员工请假流程', N'', N'1_0.1.xml', N'1', N'员工请假流程', CAST(0x0000A260010EF540 AS DateTime), CAST(0x0000A260010EF540 AS DateTime))
/****** Object:  Table [dbo].[Wf_Demo_User]    Script Date: 11/01/2013 08:22:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Wf_Demo_User](
	[uID] [int] IDENTITY(1,1) NOT NULL,
	[uName] [varchar](32) NULL,
	[uRoleId] [int] NULL,
 CONSTRAINT [PK_Wf_Demo_Member] PRIMARY KEY CLUSTERED 
(
	[uID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[Wf_Demo_User] ON
INSERT [dbo].[Wf_Demo_User] ([uID], [uName], [uRoleId]) VALUES (1, N'员工张三', 1)
INSERT [dbo].[Wf_Demo_User] ([uID], [uName], [uRoleId]) VALUES (2, N'项目经理PM1', 2)
INSERT [dbo].[Wf_Demo_User] ([uID], [uName], [uRoleId]) VALUES (3, N'人事部经理HR1', 3)
SET IDENTITY_INSERT [dbo].[Wf_Demo_User] OFF
/****** Object:  Table [dbo].[Wf_Demo_Role]    Script Date: 11/01/2013 08:22:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Wf_Demo_Role](
	[rId] [int] IDENTITY(1,1) NOT NULL,
	[rName] [varchar](50) NULL,
 CONSTRAINT [PK_Wf_Demo_Role] PRIMARY KEY CLUSTERED 
(
	[rId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[Wf_Demo_Role] ON
INSERT [dbo].[Wf_Demo_Role] ([rId], [rName]) VALUES (1, N'员工')
INSERT [dbo].[Wf_Demo_Role] ([rId], [rName]) VALUES (2, N'项目经理')
INSERT [dbo].[Wf_Demo_Role] ([rId], [rName]) VALUES (3, N'人事部经理')
SET IDENTITY_INSERT [dbo].[Wf_Demo_Role] OFF
/****** Object:  Table [dbo].[Wf_Demo_FlowRecord]    Script Date: 11/01/2013 08:22:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Wf_Demo_FlowRecord](
	[frID] [int] IDENTITY(1,1) NOT NULL,
	[frProcessGuid] [uniqueidentifier] NULL,
	[frProcessInstanceID] [int] NULL,
	[frAppInstanceID] [int] NULL,
	[frActivityInstanceID] [int] NULL,
	[frActivityGuid] [uniqueidentifier] NULL,
	[frActivityType] [int] NULL,
	[frActivityState] [int] NULL,
	[frDealState] [int] NULL,
	[frDealAdvice] [varchar](500) NULL,
	[frFile] [varchar](150) NULL,
	[frSignInfo] [varchar](500) NULL,
	[frOperator] [int] NULL,
	[frOperatorTime] [datetime] NULL,
 CONSTRAINT [PK_Wf_Demo_FlowRecord] PRIMARY KEY CLUSTERED 
(
	[frID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowRecord', @level2type=N'COLUMN',@level2name=N'frID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'XML流程定义GUID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowRecord', @level2type=N'COLUMN',@level2name=N'frProcessGuid'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'WfProcessInstance.ProcessInstanceID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowRecord', @level2type=N'COLUMN',@level2name=N'frProcessInstanceID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'WfProcessInstance.AppInstanceID(业务表ID)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowRecord', @level2type=N'COLUMN',@level2name=N'frAppInstanceID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'WfActivityInstance.ActivityInstanceID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowRecord', @level2type=N'COLUMN',@level2name=N'frActivityInstanceID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'XML活动定义GUID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowRecord', @level2type=N'COLUMN',@level2name=N'frActivityGuid'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'活动类型' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowRecord', @level2type=N'COLUMN',@level2name=N'frActivityType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'活动状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowRecord', @level2type=N'COLUMN',@level2name=N'frActivityState'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'办理意见' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowRecord', @level2type=N'COLUMN',@level2name=N'frDealAdvice'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'附件' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowRecord', @level2type=N'COLUMN',@level2name=N'frFile'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'签字使用' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowRecord', @level2type=N'COLUMN',@level2name=N'frSignInfo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'操作人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowRecord', @level2type=N'COLUMN',@level2name=N'frOperator'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'操作时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowRecord', @level2type=N'COLUMN',@level2name=N'frOperatorTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'流程记录表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowRecord'
GO
SET IDENTITY_INSERT [dbo].[Wf_Demo_FlowRecord] ON
INSERT [dbo].[Wf_Demo_FlowRecord] ([frID], [frProcessGuid], [frProcessInstanceID], [frAppInstanceID], [frActivityInstanceID], [frActivityGuid], [frActivityType], [frActivityState], [frDealState], [frDealAdvice], [frFile], [frSignInfo], [frOperator], [frOperatorTime]) VALUES (3, N'8593aa9c-1c0c-45d0-8ad2-e025a1c74fa6', 12, 21, 25, N'4a916298-d514-41e0-8a48-a6bcc33d32b9', 4, 1, NULL, N'', N'', N'', 1, CAST(0x0000A26801721488 AS DateTime))
INSERT [dbo].[Wf_Demo_FlowRecord] ([frID], [frProcessGuid], [frProcessInstanceID], [frAppInstanceID], [frActivityInstanceID], [frActivityGuid], [frActivityType], [frActivityState], [frDealState], [frDealAdvice], [frFile], [frSignInfo], [frOperator], [frOperatorTime]) VALUES (4, N'8593aa9c-1c0c-45d0-8ad2-e025a1c74fa6', 12, 21, 26, N'48bf310f-9dab-403a-9107-2d6d7d0060c9', 4, 1, NULL, N'', N'', N'', 1, CAST(0x0000A26801721488 AS DateTime))
INSERT [dbo].[Wf_Demo_FlowRecord] ([frID], [frProcessGuid], [frProcessInstanceID], [frAppInstanceID], [frActivityInstanceID], [frActivityGuid], [frActivityType], [frActivityState], [frDealState], [frDealAdvice], [frFile], [frSignInfo], [frOperator], [frOperatorTime]) VALUES (5, N'8593aa9c-1c0c-45d0-8ad2-e025a1c74fa6', 12, 21, 27, N'f907e746-a7e0-4aef-a4ad-8ec70017fda2', 4, 1, NULL, N'', N'', N'', 2, CAST(0x0000A26801733CC1 AS DateTime))
INSERT [dbo].[Wf_Demo_FlowRecord] ([frID], [frProcessGuid], [frProcessInstanceID], [frAppInstanceID], [frActivityInstanceID], [frActivityGuid], [frActivityType], [frActivityState], [frDealState], [frDealAdvice], [frFile], [frSignInfo], [frOperator], [frOperatorTime]) VALUES (6, N'8593aa9c-1c0c-45d0-8ad2-e025a1c74fa6', 12, 21, 28, N'a5ec3397-df5e-41bd-9b21-ac476effb5da', 4, 1, NULL, N'', N'', N'', 3, CAST(0x0000A2680173565B AS DateTime))
SET IDENTITY_INSERT [dbo].[Wf_Demo_FlowRecord] OFF
/****** Object:  Table [dbo].[Wf_Demo_FlowInstance]    Script Date: 11/01/2013 08:22:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Wf_Demo_FlowInstance](
	[fiID] [int] IDENTITY(1,1) NOT NULL,
	[fiFlowDefinitionGuid] [uniqueidentifier] NULL,
	[fiType] [char](1) NULL,
	[fiObjectId] [int] NULL,
	[fiSerialNo] [varchar](100) NULL,
	[fiNo] [varchar](50) NULL,
	[fiState] [char](1) NULL,
	[fiLevel] [char](1) NULL,
	[fiTitle] [varchar](100) NULL,
	[fiApplyReason] [varchar](150) NULL,
	[fiApplyer] [varchar](20) NULL,
	[fiApplyTime] [datetime] NULL,
	[fiLastOperator] [varchar](20) NULL,
	[fiLastOptTime] [datetime] NULL,
 CONSTRAINT [PK_Wf_Demo_FlowInstance] PRIMARY KEY CLUSTERED 
(
	[fiID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'fID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowInstance', @level2type=N'COLUMN',@level2name=N'fiID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'WorkFlow_FlowDefinition表的Guid' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowInstance', @level2type=N'COLUMN',@level2name=N'fiFlowDefinitionGuid'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'0-表单流程 1-业务模块流程' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowInstance', @level2type=N'COLUMN',@level2name=N'fiType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'表单编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowInstance', @level2type=N'COLUMN',@level2name=N'fiObjectId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'业务编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowInstance', @level2type=N'COLUMN',@level2name=N'fiSerialNo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'业务流程编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowInstance', @level2type=N'COLUMN',@level2name=N'fiNo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'流程状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowInstance', @level2type=N'COLUMN',@level2name=N'fiState'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'流程重要性' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowInstance', @level2type=N'COLUMN',@level2name=N'fiLevel'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'流程标题' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowInstance', @level2type=N'COLUMN',@level2name=N'fiTitle'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'申请理由' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowInstance', @level2type=N'COLUMN',@level2name=N'fiApplyReason'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'流程发起人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowInstance', @level2type=N'COLUMN',@level2name=N'fiApplyer'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'流程发起时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowInstance', @level2type=N'COLUMN',@level2name=N'fiApplyTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最后操作人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowInstance', @level2type=N'COLUMN',@level2name=N'fiLastOperator'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最后操作时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowInstance', @level2type=N'COLUMN',@level2name=N'fiLastOptTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Wf_Demo_FlowInstance' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowInstance'
GO
SET IDENTITY_INSERT [dbo].[Wf_Demo_FlowInstance] ON
INSERT [dbo].[Wf_Demo_FlowInstance] ([fiID], [fiFlowDefinitionGuid], [fiType], [fiObjectId], [fiSerialNo], [fiNo], [fiState], [fiLevel], [fiTitle], [fiApplyReason], [fiApplyer], [fiApplyTime], [fiLastOperator], [fiLastOptTime]) VALUES (21, N'8593aa9c-1c0c-45d0-8ad2-e025a1c74fa6', N'0', 1, N'd3a78d57-435d-4e93-b033-316d68dbcff2', N'11', N'0', N'0', N'11', N'11', N'1', CAST(0x0000A26801720FDA AS DateTime), N'1', CAST(0x0000A26801720FDA AS DateTime))
SET IDENTITY_INSERT [dbo].[Wf_Demo_FlowInstance] OFF
/****** Object:  Table [dbo].[Wf_Demo_FlowDefinition]    Script Date: 11/01/2013 08:22:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Wf_Demo_FlowDefinition](
	[fdID] [int] IDENTITY(1,1) NOT NULL,
	[fdTypeID] [int] NULL,
	[fdName] [nvarchar](50) NULL,
	[fdGuid] [uniqueidentifier] NULL,
	[fdFormID] [int] NULL,
	[fdNo] [int] NULL,
	[fdClass] [char](1) NULL,
	[fdModel] [char](1) NULL,
	[fdIntro] [varchar](150) NULL,
	[fdXmlFilePath] [nvarchar](50) NULL,
	[fdXmlFileName] [nvarchar](50) NULL,
	[fdXmlFileContent] [text] NULL,
	[fdState] [char](1) NULL,
	[fdCreated] [varchar](20) NULL,
	[fdCreatedTime] [datetime] NULL,
	[fdLastOperator] [varchar](20) NULL,
	[fdLastOperationTime] [datetime] NULL,
	[fdVersion] [varchar](50) NULL,
 CONSTRAINT [PK_Wf_Demo_FlowDefinition] PRIMARY KEY CLUSTERED 
(
	[fdID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'fID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowDefinition', @level2type=N'COLUMN',@level2name=N'fdID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'WorkFlow_FlowType表ftID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowDefinition', @level2type=N'COLUMN',@level2name=N'fdTypeID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'流程名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowDefinition', @level2type=N'COLUMN',@level2name=N'fdName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'流程guid' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowDefinition', @level2type=N'COLUMN',@level2name=N'fdGuid'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'表单ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowDefinition', @level2type=N'COLUMN',@level2name=N'fdFormID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'流程编号规则' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowDefinition', @level2type=N'COLUMN',@level2name=N'fdNo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'1-表单流程 2-业务流程' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowDefinition', @level2type=N'COLUMN',@level2name=N'fdClass'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'0-固定流程、1-自由顺序流 、2-自由流程' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowDefinition', @level2type=N'COLUMN',@level2name=N'fdModel'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'流程简介' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowDefinition', @level2type=N'COLUMN',@level2name=N'fdIntro'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'xml文件路径' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowDefinition', @level2type=N'COLUMN',@level2name=N'fdXmlFilePath'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'xml文件内容' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowDefinition', @level2type=N'COLUMN',@level2name=N'fdXmlFileName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'xml文件名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowDefinition', @level2type=N'COLUMN',@level2name=N'fdXmlFileContent'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'流程状态：0-启用 、1-停用' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowDefinition', @level2type=N'COLUMN',@level2name=N'fdState'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowDefinition', @level2type=N'COLUMN',@level2name=N'fdCreated'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowDefinition', @level2type=N'COLUMN',@level2name=N'fdCreatedTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最后修改人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowDefinition', @level2type=N'COLUMN',@level2name=N'fdLastOperator'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最后修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowDefinition', @level2type=N'COLUMN',@level2name=N'fdLastOperationTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'流程版本' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowDefinition', @level2type=N'COLUMN',@level2name=N'fdVersion'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'流程定义' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Wf_Demo_FlowDefinition'
GO
SET IDENTITY_INSERT [dbo].[Wf_Demo_FlowDefinition] ON
INSERT [dbo].[Wf_Demo_FlowDefinition] ([fdID], [fdTypeID], [fdName], [fdGuid], [fdFormID], [fdNo], [fdClass], [fdModel], [fdIntro], [fdXmlFilePath], [fdXmlFileName], [fdXmlFileContent], [fdState], [fdCreated], [fdCreatedTime], [fdLastOperator], [fdLastOperationTime], [fdVersion]) VALUES (1, 1, N'员工请假流程', N'8593aa9c-1c0c-45d0-8ad2-e025a1c74fa6', NULL, NULL, N'1', N'0', N'', NULL, NULL, NULL, N'1', N'admin', CAST(0x0000A25E0103C95C AS DateTime), N'admin', CAST(0x0000A26000F21933 AS DateTime), N'0.1')
INSERT [dbo].[Wf_Demo_FlowDefinition] ([fdID], [fdTypeID], [fdName], [fdGuid], [fdFormID], [fdNo], [fdClass], [fdModel], [fdIntro], [fdXmlFilePath], [fdXmlFileName], [fdXmlFileContent], [fdState], [fdCreated], [fdCreatedTime], [fdLastOperator], [fdLastOperationTime], [fdVersion]) VALUES (2, 3, N'项目立项申请流程', N'9e7deee9-1e77-4489-83c8-32d68fce5318', NULL, NULL, N'2', N'0', N'', NULL, NULL, NULL, N'1', N'admin', CAST(0x0000A26000C16EB7 AS DateTime), N'admin', CAST(0x0000A26000F233C2 AS DateTime), N'0.1')
INSERT [dbo].[Wf_Demo_FlowDefinition] ([fdID], [fdTypeID], [fdName], [fdGuid], [fdFormID], [fdNo], [fdClass], [fdModel], [fdIntro], [fdXmlFilePath], [fdXmlFileName], [fdXmlFileContent], [fdState], [fdCreated], [fdCreatedTime], [fdLastOperator], [fdLastOperationTime], [fdVersion]) VALUES (3, 1, N'TEST', N'1ccf4856-6589-4ced-89fd-c4ff7ec5d455', NULL, NULL, N'1', N'0', N'TEST', NULL, NULL, NULL, N'0', N'shilianhua', CAST(0x0000A2640129F3ED AS DateTime), N'shilianhua', CAST(0x0000A2640129F3ED AS DateTime), N'0.1')
SET IDENTITY_INSERT [dbo].[Wf_Demo_FlowDefinition] OFF
/****** Object:  Default [DF_SSIP-WfPROCESS_CreatedDateTime]    Script Date: 11/01/2013 08:22:21 ******/
ALTER TABLE [dbo].[WfProcess] ADD  CONSTRAINT [DF_SSIP-WfPROCESS_CreatedDateTime]  DEFAULT (getdate()) FOR [CreatedDateTime]
GO
