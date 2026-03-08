CREATE TABLE [dbo].[Orders]
(
    [OrderId]      INT IDENTITY(1,1) NOT NULL,
    [ContractId]   INT               NOT NULL,
    [PmId]         INT               NOT NULL,
    [Side]         NVARCHAR(10)      NOT NULL,
    [Status]       NVARCHAR(20)      NOT NULL,
    [Quantity]     INT               NOT NULL,
    [Price]        DECIMAL(18,4)     NOT NULL,
    [OrderDate]    DATETIME2         NOT NULL CONSTRAINT [DF_Orders_OrderDate] DEFAULT GETUTCDATE(),
    [FillDate]     DATETIME2         NULL,
    [CancelDate]   DATETIME2         NULL,
    [CancelReason] NVARCHAR(500)     NULL,
    [Notes]        NVARCHAR(1000)    NULL,

    CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED ([OrderId] ASC),
    CONSTRAINT [FK_Orders_Contract] FOREIGN KEY ([ContractId])
        REFERENCES [dbo].[FuturesContracts] ([ContractId]),
    CONSTRAINT [FK_Orders_Pm] FOREIGN KEY ([PmId])
        REFERENCES [dbo].[PortfolioManagers] ([PmId]),
    CONSTRAINT [CK_Orders_Side] CHECK ([Side] IN ('Buy', 'Sell')),
    CONSTRAINT [CK_Orders_Status] CHECK ([Status] IN ('Placed', 'Cancelled', 'Filled', 'PartialFill')),
    CONSTRAINT [CK_Orders_Quantity] CHECK ([Quantity] > 0),
    CONSTRAINT [CK_Orders_Price] CHECK ([Price] > 0)
);
GO

CREATE INDEX [IX_Orders_ContractId] ON [dbo].[Orders] ([ContractId]);
GO
CREATE INDEX [IX_Orders_PmId] ON [dbo].[Orders] ([PmId]);
GO
CREATE INDEX [IX_Orders_Status] ON [dbo].[Orders] ([Status]);
GO
CREATE INDEX [IX_Orders_OrderDate] ON [dbo].[Orders] ([OrderDate] DESC);
GO

EXEC sp_addextendedproperty
    @name = N'MS_Description',
    @value = N'Orders placed or cancelled by portfolio managers on futures contracts',
    @level0type = N'Schema', @level0name = N'dbo',
    @level1type = N'Table',  @level1name = N'Orders';
GO
