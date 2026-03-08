-- ============================================================
-- Test Data: FuturesContracts
-- 20 contracts across Equity, Rates, Commodity, FX
-- Expiry dates relative to March 2026
-- ============================================================
SET IDENTITY_INSERT [dbo].[FuturesContracts] ON;

INSERT INTO [dbo].[FuturesContracts]
    ([ContractId], [Symbol], [Description], [Exchange], [AssetClass],
     [ExpiryDate], [LotSize], [TickSize], [TickValue], [Currency], [Status])
VALUES
-- ---- Equity Index (CME) ----
(1,  'ESH26', 'E-mini S&P 500 March 2026',       'CME', 'Equity',    '2026-03-20', 1,  0.25,   12.50, 'USD', 'Active'),
(2,  'ESM26', 'E-mini S&P 500 June 2026',         'CME', 'Equity',    '2026-06-19', 1,  0.25,   12.50, 'USD', 'Active'),
(3,  'NQH26', 'E-mini Nasdaq-100 March 2026',     'CME', 'Equity',    '2026-03-20', 1,  0.25,    5.00, 'USD', 'Active'),
(4,  'NQM26', 'E-mini Nasdaq-100 June 2026',      'CME', 'Equity',    '2026-06-19', 1,  0.25,    5.00, 'USD', 'Active'),
(5,  'RTYH26','E-mini Russell 2000 March 2026',   'CME', 'Equity',    '2026-03-20', 1,  0.10,   10.00, 'USD', 'Active'),
(6,  'YMH26', 'E-mini Dow Jones March 2026',      'CBOT','Equity',    '2026-03-20', 1,  1.00,    5.00, 'USD', 'Active'),

-- ---- US Treasury Rates (CBOT) ----
(7,  'ZBH26', '30-Year US Treasury Bond March 2026', 'CBOT', 'Rates', '2026-03-19', 1,  0.0313, 31.25, 'USD', 'Active'),
(8,  'ZBM26', '30-Year US Treasury Bond June 2026',  'CBOT', 'Rates', '2026-06-18', 1,  0.0313, 31.25, 'USD', 'Active'),
(9,  'ZNH26', '10-Year US Treasury Note March 2026', 'CBOT', 'Rates', '2026-03-19', 1,  0.0156, 15.63, 'USD', 'Active'),
(10, 'ZFH26', '5-Year US Treasury Note March 2026',  'CBOT', 'Rates', '2026-03-19', 1,  0.0078,  7.81, 'USD', 'Active'),

-- ---- Energy (NYMEX) ----
(11, 'CLJ26', 'Crude Oil April 2026',             'NYMEX','Commodity', '2026-03-23', 1000, 0.01,  10.00, 'USD', 'Active'),
(12, 'CLK26', 'Crude Oil May 2026',               'NYMEX','Commodity', '2026-04-22', 1000, 0.01,  10.00, 'USD', 'Active'),
(13, 'NGJ26', 'Natural Gas April 2026',           'NYMEX','Commodity', '2026-03-26', 10000,0.001, 10.00, 'USD', 'Active'),

-- ---- Metals (COMEX) ----
(14, 'GCJ26', 'Gold April 2026',                  'COMEX','Commodity', '2026-04-29', 100,  0.10,  10.00, 'USD', 'Active'),
(15, 'GCM26', 'Gold June 2026',                   'COMEX','Commodity', '2026-06-29', 100,  0.10,  10.00, 'USD', 'Active'),
(16, 'SIK26', 'Silver May 2026',                  'COMEX','Commodity', '2026-05-27', 5000, 0.005, 25.00, 'USD', 'Active'),

-- ---- FX (CME) ----
(17, '6EH26', 'Euro FX March 2026',               'CME', 'FX',        '2026-03-16', 125000,0.0001,12.50, 'USD', 'Active'),
(18, '6EM26', 'Euro FX June 2026',                'CME', 'FX',        '2026-06-15', 125000,0.0001,12.50, 'USD', 'Active'),
(19, '6JH26', 'Japanese Yen March 2026',           'CME', 'FX',        '2026-03-16', 12500000,0.0000005,6.25,'USD','Active'),
(20, '6JM26', 'Japanese Yen June 2026',            'CME', 'FX',        '2026-06-15', 12500000,0.0000005,6.25,'USD','Active');

SET IDENTITY_INSERT [dbo].[FuturesContracts] OFF;
GO

PRINT 'FuturesContracts: 20 rows inserted';
GO
