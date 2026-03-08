-- ============================================================
-- Test Data: Orders (~60 orders, Jan–Mar 2026)
-- Mix of Placed, Filled, PartialFill, Cancelled
-- All 8 PMs, various contracts
-- ============================================================
SET IDENTITY_INSERT [dbo].[Orders] ON;

INSERT INTO [dbo].[Orders]
    ([OrderId],[ContractId],[PmId],[Side],[Status],[Quantity],[Price],
     [OrderDate],[FillDate],[CancelDate],[CancelReason],[Notes])
VALUES
-- ===== JANUARY 2026 =====
(1,  1,  2, 'Buy',  'Filled',      50, 5850.25, '2026-01-06 09:31:00', '2026-01-06 09:31:22', NULL, NULL, 'Long S&P position initiation'),
(2,  1,  4, 'Sell', 'Filled',      25, 5862.50, '2026-01-07 10:15:00', '2026-01-07 10:15:10', NULL, NULL, NULL),
(3,  3,  1, 'Buy',  'Filled',      30, 21050.00,'2026-01-08 09:45:00', '2026-01-08 09:46:00', NULL, NULL, 'Nasdaq momentum trade'),
(4,  7,  6, 'Buy',  'Filled',     100, 117.22,  '2026-01-09 10:00:00', '2026-01-09 10:00:15', NULL, NULL, 'Duration extension — long 30Y'),
(5,  11, 2, 'Buy',  'Cancelled',   20, 76.80,   '2026-01-10 14:00:00', NULL, '2026-01-10 14:05:00', 'Price moved against us before fill', 'Crude entry attempt'),
(6,  14, 5, 'Buy',  'Filled',      10, 2650.40, '2026-01-13 11:00:00', '2026-01-13 11:00:05', NULL, NULL, 'Gold safe-haven allocation'),
(7,  17, 3, 'Sell', 'Filled',      50, 1.0412,  '2026-01-14 08:30:00', '2026-01-14 08:30:02', NULL, NULL, 'EUR/USD short on ECB outlook'),
(8,  9,  6, 'Buy',  'Filled',     200, 108.16,  '2026-01-15 13:30:00', '2026-01-15 13:30:08', NULL, NULL, '10Y note hedge'),
(9,  1,  7, 'Buy',  'Cancelled',   40, 5880.00, '2026-01-16 10:00:00', NULL, '2026-01-16 10:02:00', 'Risk limit exceeded for the day', NULL),
(10, 3,  8, 'Sell', 'Filled',      15, 21200.75,'2026-01-19 09:35:00', '2026-01-19 09:35:18', NULL, NULL, 'Partial profit take on NQ'),
(11, 11, 4, 'Buy',  'PartialFill', 30, 77.10,   '2026-01-20 15:00:00', '2026-01-20 15:01:00', NULL, NULL, '15 of 30 lots filled'),
(12, 5,  2, 'Buy',  'Filled',      20, 2195.60, '2026-01-21 09:45:00', '2026-01-21 09:45:12', NULL, NULL, 'Russell small-cap allocation'),
(13, 6,  5, 'Buy',  'Filled',      10, 44250.00,'2026-01-22 10:30:00', '2026-01-22 10:30:07', NULL, NULL, 'Dow momentum'),
(14, 19, 1, 'Buy',  'Filled',      40, 0.006521,'2026-01-23 08:00:00', '2026-01-23 08:00:03', NULL, NULL, 'JPY long — risk-off hedge'),
(15, 16, 3, 'Sell', 'Cancelled',   25, 30.50,   '2026-01-26 11:00:00', NULL, '2026-01-26 11:00:45', 'PM request — re-evaluating silver thesis', NULL),

-- ===== FEBRUARY 2026 =====
(16, 1,  2, 'Sell', 'Filled',      50, 5910.50, '2026-02-02 09:30:00', '2026-02-02 09:30:05', NULL, NULL, 'Trimming S&P long'),
(17, 3,  1, 'Buy',  'Filled',      20, 21150.25,'2026-02-03 14:00:00', '2026-02-03 14:00:22', NULL, NULL, NULL),
(18, 7,  6, 'Sell', 'Filled',      50, 118.03,  '2026-02-04 10:15:00', '2026-02-04 10:15:09', NULL, NULL, 'Rate view changed — reducing duration'),
(19, 11, 4, 'Buy',  'Filled',      40, 75.22,   '2026-02-05 13:00:00', '2026-02-05 13:00:30', NULL, NULL, 'Supply disruption trade'),
(20, 14, 7, 'Buy',  'Cancelled',   15, 2620.00, '2026-02-06 09:00:00', NULL, '2026-02-06 09:01:30', 'System error — order rejected by exchange', 'Resubmitted manually'),
(21, 17, 3, 'Buy',  'Filled',      80, 1.0380,  '2026-02-09 08:00:00', '2026-02-09 08:00:04', NULL, NULL, 'EUR reversal — covering short'),
(22, 9,  8, 'Buy',  'PartialFill', 150, 108.28, '2026-02-10 10:00:00', '2026-02-10 10:01:00', NULL, NULL, '100 of 150 lots filled at limit'),
(23, 1,  5, 'Buy',  'Filled',      60, 5875.75, '2026-02-11 09:31:00', '2026-02-11 09:31:15', NULL, NULL, 'CPI dip buy'),
(24, 16, 2, 'Buy',  'Filled',      20, 31.40,   '2026-02-12 11:30:00', '2026-02-12 11:30:02', NULL, NULL, 'Silver long on industrial demand'),
(25, 5,  4, 'Sell', 'Cancelled',   30, 2180.00, '2026-02-13 14:30:00', NULL, '2026-02-13 14:32:00', 'Price moved above limit before execution', NULL),
(26, 6,  6, 'Sell', 'Filled',      10, 44800.00,'2026-02-17 10:00:00', '2026-02-17 10:00:10', NULL, NULL, 'Profit taking on YM long'),
(27, 3,  7, 'Buy',  'Filled',      25, 21300.50,'2026-02-18 09:45:00', '2026-02-18 09:45:35', NULL, NULL, 'Tech sector rotation'),
(28, 11, 8, 'Sell', 'Filled',      20, 77.60,   '2026-02-19 13:15:00', '2026-02-19 13:15:07', NULL, NULL, 'Inventory build hedge'),
(29, 19, 3, 'Sell', 'Cancelled',   60, 0.006480,'2026-02-20 08:05:00', NULL, '2026-02-20 08:06:00', 'Risk limit exceeded — JPY position size', NULL),
(30, 14, 5, 'Buy',  'Filled',      12, 2680.20, '2026-02-23 10:30:00', '2026-02-23 10:30:03', NULL, NULL, 'Gold breakout entry'),
(31, 9,  2, 'Sell', 'PartialFill', 100, 108.50, '2026-02-24 11:00:00', '2026-02-24 11:00:45', NULL, NULL, '70 of 100 lots filled'),
(32, 1,  1, 'Buy',  'Filled',      35, 5920.25, '2026-02-25 15:00:00', '2026-02-25 15:00:09', NULL, NULL, 'End-of-month rebalance'),
(33, 17, 4, 'Sell', 'Filled',      45, 1.0445,  '2026-02-26 08:30:00', '2026-02-26 08:30:02', NULL, NULL, 'EUR month-end flow'),
(34, 7,  8, 'Buy',  'Cancelled',   75, 116.50,  '2026-02-27 10:00:00', NULL, '2026-02-27 10:01:00', 'PM request — waiting for FOMC minutes', NULL),

-- ===== MARCH 2026 (pre-roll period) =====
(35, 1,  2, 'Sell', 'Placed',      40, 5950.00, '2026-03-02 09:31:00', NULL, NULL, NULL, 'Pre-roll trim ahead of Mar expiry'),
(36, 3,  6, 'Sell', 'Placed',      30, 21400.00,'2026-03-02 09:35:00', NULL, NULL, NULL, 'Pre-roll NQ reduction'),
(37, 7,  1, 'Sell', 'Filled',     100, 117.81,  '2026-03-03 10:00:00', '2026-03-03 10:00:22', NULL, NULL, 'ZB roll initiated — selling H26'),
(38, 8,  1, 'Buy',  'Filled',     100, 118.03,  '2026-03-03 10:00:30', '2026-03-03 10:00:52', NULL, NULL, 'ZB roll — buying M26'),
(39, 9,  6, 'Sell', 'Filled',     150, 108.72,  '2026-03-03 10:05:00', '2026-03-03 10:05:10', NULL, NULL, 'ZN roll — selling H26'),
(40, 9,  6, 'Buy',  'Placed',     150, 108.94,  '2026-03-03 10:05:30', NULL, NULL, NULL, 'ZN roll — buying M26 (pending)'),
(41, 11, 4, 'Sell', 'Filled',      25, 78.10,   '2026-03-04 14:00:00', '2026-03-04 14:00:08', NULL, NULL, 'Crude profit take before roll'),
(42, 17, 3, 'Sell', 'Filled',      60, 1.0398,  '2026-03-04 08:00:00', '2026-03-04 08:00:03', NULL, NULL, '6E roll — selling H26'),
(43, 18, 3, 'Buy',  'Filled',      60, 1.0402,  '2026-03-04 08:01:00', '2026-03-04 08:01:05', NULL, NULL, '6E roll — buying M26'),
(44, 14, 5, 'Buy',  'Filled',       8, 2695.60, '2026-03-05 11:00:00', '2026-03-05 11:00:04', NULL, NULL, 'Adding to gold long'),
(45, 1,  7, 'Buy',  'Cancelled',   20, 5940.00, '2026-03-05 14:30:00', NULL, '2026-03-05 14:31:00', 'Stale price — market moved', NULL),
(46, 1,  2, 'Sell', 'Filled',      50, 5945.75, '2026-03-06 09:31:00', '2026-03-06 09:31:18', NULL, NULL, 'Roll execution — ESH26 leg'),
(47, 2,  2, 'Buy',  'Filled',      50, 5948.50, '2026-03-06 09:31:45', '2026-03-06 09:32:00', NULL, NULL, 'Roll execution — ESM26 leg'),
(48, 3,  4, 'Sell', 'Filled',      30, 21380.25,'2026-03-06 09:35:00', '2026-03-06 09:35:12', NULL, NULL, 'Roll execution — NQH26 leg'),
(49, 4,  4, 'Buy',  'Filled',      30, 21385.00,'2026-03-06 09:35:30', '2026-03-06 09:35:48', NULL, NULL, 'Roll execution — NQM26 leg'),
(50, 16, 8, 'Buy',  'Placed',      15, 31.80,   '2026-03-06 11:00:00', NULL, NULL, NULL, 'Silver entry ahead of copper correlation play'),
(51, 13, 5, 'Buy',  'Filled',      10, 4.180,   '2026-03-07 09:00:00', '2026-03-07 09:00:05', NULL, NULL, 'Nat gas winter carry unwind'),
(52, 5,  7, 'Buy',  'Placed',      25, 2190.00, '2026-03-07 10:00:00', NULL, NULL, NULL, 'Russell long — small-cap rotation'),
(53, 6,  3, 'Sell', 'Cancelled',   15, 44900.00,'2026-03-07 14:00:00', NULL, '2026-03-07 14:02:00', 'PM request — holding through Fed meeting', NULL),
(54, 19, 1, 'Buy',  'Filled',      50, 0.006498,'2026-03-07 08:00:00', '2026-03-07 08:00:02', NULL, NULL, 'JPY risk-off ahead of geopolitical news'),
(55, 11, 4, 'Buy',  'Placed',      35, 77.50,   '2026-03-07 15:30:00', NULL, NULL, NULL, 'CLJ26 last chance before roll'),
(56, 14, 6, 'Sell', 'PartialFill', 20, 2700.00, '2026-03-07 11:15:00', '2026-03-07 11:16:00', NULL, NULL, '12 of 20 lots filled at target'),
(57, 1,  8, 'Buy',  'Placed',      30, 5930.00, '2026-03-08 09:31:00', NULL, NULL, NULL, 'Dip buy — watching support'),
(58, 7,  6, 'Sell', 'Placed',      80, 118.50,  '2026-03-08 10:00:00', NULL, NULL, NULL, 'ZBH26 final roll leg'),
(59, 8,  6, 'Buy',  'Placed',      80, 118.72,  '2026-03-08 10:00:30', NULL, NULL, NULL, 'ZBM26 roll receive leg'),
(60, 17, 5, 'Sell', 'Placed',      40, 1.0420,  '2026-03-08 08:00:00', NULL, NULL, NULL, '6EH26 final close before expiry');

SET IDENTITY_INSERT [dbo].[Orders] OFF;
GO

PRINT 'Orders: 60 rows inserted (Jan-Mar 2026, mix of Placed/Filled/PartialFill/Cancelled)';
GO
