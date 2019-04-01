IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ZY_CZFEE]') AND type in (N'U'))
CREATE TABLE [dbo].[ZY_CZFEE](
	[FEEID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_ZY_CZFEE] PRIMARY KEY CLUSTERED 
(
	[FEEID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


