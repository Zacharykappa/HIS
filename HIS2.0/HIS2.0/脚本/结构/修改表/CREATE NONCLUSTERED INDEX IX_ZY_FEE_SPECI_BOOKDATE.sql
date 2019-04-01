CREATE NONCLUSTERED INDEX IX_ZY_FEE_SPECI_BOOKDATE ON dbo.ZY_FEE_SPECI
	(
	BOOK_DATE
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO