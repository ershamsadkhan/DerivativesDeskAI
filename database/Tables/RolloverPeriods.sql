CREATE TABLE [dbo].[RolloverPeriods]
(
    [RolloverId]     INT IDENTITY(1,1) NOT NULL,
    [FromContractId] INT               NOT NULL,
    [ToContractId]   INT               NOT NULL,
    [RollStartDate]  DATE              NOT NULL,
    [RollDate]       DATE              NOT NULL,
    [Reason]         NVARCHAR(500)     NULL,
    [CreatedAt]      DATETIME2         NOT NULL CONSTRAINT [DF_RolloverPeriods_CreatedAt] DEFAULT GETUTCDATE(),

    CONSTRAINT [PK_RolloverPeriods] PRIMARY KEY CLUSTERED ([RolloverId] ASC),
    CONSTRAINT [FK_RolloverPeriods_FromContract] FOREIGN KEY ([FromContractId])
        REFERENCES [dbo].[FuturesContracts] ([ContractId]),
    CONSTRAINT [FK_RolloverPeriods_ToContract] FOREIGN KEY ([ToContractId])
        REFERENCES [dbo].[FuturesContracts] ([ContractId]),
    CONSTRAINT [CK_RolloverPeriods_Dates] CHECK ([RollStartDate] <= [RollDate]),
    CONSTRAINT [CK_RolloverPeriods_DiffContracts] CHECK ([FromContractId] <> [ToContractId])
);
GO

CREATE INDEX [IX_RolloverPeriods_RollDate] ON [dbo].[RolloverPeriods] ([RollDate]);
GO
CREATE INDEX [IX_RolloverPeriods_FromContractId] ON [dbo].[RolloverPeriods] ([FromContractId]);
GO

EXEC sp_addextendedproperty
    @name = N'MS_Description',
    @value = N'Rollover windows defining when positions must move from expiring to next contract',
    @level0type = N'Schema', @level0name = N'dbo',
    @level1type = N'Table',  @level1name = N'RolloverPeriods';
GO
