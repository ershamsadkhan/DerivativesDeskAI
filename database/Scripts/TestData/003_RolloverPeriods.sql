-- ============================================================
-- Test Data: RolloverPeriods
-- Active rollover windows for March/April 2026 contracts
-- ============================================================
SET IDENTITY_INSERT [dbo].[RolloverPeriods] ON;

INSERT INTO [dbo].[RolloverPeriods]
    ([RolloverId], [FromContractId], [ToContractId], [RollStartDate], [RollDate], [Reason])
VALUES
-- Equity rolls (Mar→Jun, typical 2 weeks before expiry)
(1,  1,  2,  '2026-03-06', '2026-03-18', 'Quarterly roll: ESH26 to ESM26 ahead of March expiry'),
(2,  3,  4,  '2026-03-06', '2026-03-18', 'Quarterly roll: NQH26 to NQM26 ahead of March expiry'),
(3,  5,  5,  '2026-03-06', '2026-03-18', 'Quarterly roll: RTYH26 to RTYM26 ahead of March expiry'),
(4,  6,  6,  '2026-03-06', '2026-03-18', 'Quarterly roll: YMH26 to YMM26 ahead of March expiry'),

-- Treasury rolls (roll earlier due to delivery)
(5,  7,  8,  '2026-03-03', '2026-03-19', 'Quarterly roll: ZBH26 to ZBM26, first position day Mar 3'),
(6,  9,  9,  '2026-03-03', '2026-03-19', 'Quarterly roll: ZNH26 to ZNM26, first position day Mar 3'),
(7,  10, 10, '2026-03-03', '2026-03-19', 'Quarterly roll: ZFH26 to ZFM26, first position day Mar 3'),

-- Energy rolls (monthly, roll before last trade day)
(8,  11, 12, '2026-03-17', '2026-03-20', 'Monthly roll: CLJ26 to CLK26, expires Mar 23'),
(9,  13, 13, '2026-03-20', '2026-03-25', 'Monthly roll: NGJ26 to NGK26, expires Mar 26'),

-- Metals rolls
(10, 14, 15, '2026-03-20', '2026-03-27', 'Bimonthly roll: GCJ26 to GCM26 ahead of April delivery'),
(11, 16, 16, '2026-04-20', '2026-04-27', 'Monthly roll: SIK26 to SIN26 ahead of May delivery'),

-- FX rolls
(12, 17, 18, '2026-03-09', '2026-03-16', 'Quarterly roll: 6EH26 to 6EM26, expires Mar 16'),
(13, 19, 20, '2026-03-09', '2026-03-16', 'Quarterly roll: 6JH26 to 6JM26, expires Mar 16');

SET IDENTITY_INSERT [dbo].[RolloverPeriods] OFF;
GO

PRINT 'RolloverPeriods: 13 rows inserted';
GO
