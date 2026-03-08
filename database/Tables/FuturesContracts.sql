CREATE TABLE [dbo].[FuturesContracts]
(
    [ContractId]  INT IDENTITY(1,1) NOT NULL,
    [Symbol]      NVARCHAR(20)      NOT NULL,
    [Description] NVARCHAR(200)     NOT NULL,
    [Exchange]    NVARCHAR(50)      NOT NULL,
    [AssetClass]  NVARCHAR(50)      NOT NULL,
    [ExpiryDate]  DATE              NOT NULL,
    [LotSize]     INT               NOT NULL,
    [TickSize]    DECIMAL(10,4)     NOT NULL,
    [TickValue]   DECIMAL(10,2)     NOT NULL,
    [Currency]    NCHAR(3)          NOT NULL CONSTRAINT [DF_FuturesContracts_Currency] DEFAULT 'USD',
    [Status]      NVARCHAR(20)      NOT NULL CONSTRAINT [DF_FuturesContracts_Status] DEFAULT 'Active',
    [CreatedAt]   DATETIME2         NOT NULL CONSTRAINT [DF_FuturesContracts_CreatedAt] DEFAULT GETUTCDATE(),
    [UpdatedAt]   DATETIME2         NOT NULL CONSTRAINT [DF_FuturesContracts_UpdatedAt] DEFAULT GETUTCDATE(),

    CONSTRAINT [PK_FuturesContracts] PRIMARY KEY CLUSTERED ([ContractId] ASC),
    CONSTRAINT [UQ_FuturesContracts_Symbol] UNIQUE ([Symbol]),
    CONSTRAINT [CK_FuturesContracts_Status] CHECK ([Status] IN ('Active', 'Expired', 'Suspended')),
    CONSTRAINT [CK_FuturesContracts_AssetClass] CHECK ([AssetClass] IN ('Equity', 'Commodity', 'FX', 'Rates')),
    CONSTRAINT [CK_FuturesContracts_LotSize] CHECK ([LotSize] > 0),
    CONSTRAINT [CK_FuturesContracts_TickSize] CHECK ([TickSize] > 0)
);
GO

CREATE INDEX [IX_FuturesContracts_ExpiryDate] ON [dbo].[FuturesContracts] ([ExpiryDate]);
GO
CREATE INDEX [IX_FuturesContracts_Status] ON [dbo].[FuturesContracts] ([Status]);
GO
CREATE INDEX [IX_FuturesContracts_AssetClass] ON [dbo].[FuturesContracts] ([AssetClass]);
GO

EXEC sp_addextendedproperty
    @name = N'MS_Description',
    @value = N'Futures contracts traded on the derivatives desk',
    @level0type = N'Schema', @level0name = N'dbo',
    @level1type = N'Table',  @level1name = N'FuturesContracts';
GO
