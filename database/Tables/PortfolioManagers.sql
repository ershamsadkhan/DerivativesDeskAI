CREATE TABLE [dbo].[PortfolioManagers]
(
    [PmId]      INT IDENTITY(1,1) NOT NULL,
    [Name]      NVARCHAR(100)     NOT NULL,
    [Type]      NVARCHAR(10)      NOT NULL,
    [Email]     NVARCHAR(200)     NOT NULL,
    [IsActive]  BIT               NOT NULL CONSTRAINT [DF_PortfolioManagers_IsActive] DEFAULT 1,
    [CreatedAt] DATETIME2         NOT NULL CONSTRAINT [DF_PortfolioManagers_CreatedAt] DEFAULT GETUTCDATE(),

    CONSTRAINT [PK_PortfolioManagers] PRIMARY KEY CLUSTERED ([PmId] ASC),
    CONSTRAINT [CK_PortfolioManagers_Type] CHECK ([Type] IN ('PM', 'IPM')),
    CONSTRAINT [UQ_PortfolioManagers_Email] UNIQUE ([Email])
);
GO

EXEC sp_addextendedproperty
    @name = N'MS_Description',
    @value = N'Portfolio Managers and Institutional Portfolio Managers who trade derivatives',
    @level0type = N'Schema', @level0name = N'dbo',
    @level1type = N'Table',  @level1name = N'PortfolioManagers';
GO
