USE [ProcessDiscoveryDB]
GO
/****** Object:  Table [dbo].[ProcessCapture]    Script Date: 4/14/2020 3:43:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProcessCapture](
	[Timestamp] [datetime] NOT NULL,
	[ProcessName] [nvarchar](max) NULL,
	[Steps] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[uspProcessCapture]    Script Date: 4/14/2020 3:43:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Create PROCEDURE  [dbo].[uspProcessCapture]  (@CurrTime DateTime,@ProcessName nvarchar(max),@ProcessSteps nvarchar(max))
AS
Begin
	SET NOCOUNT ON;	  
Insert into [dbo].[ProcessCapture] values (CURRENT_TIMESTAMP,@ProcessName,@ProcessSteps);
END
GO
