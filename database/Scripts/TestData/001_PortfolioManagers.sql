-- ============================================================
-- Test Data: PortfolioManagers
-- 5 PMs, 3 IPMs
-- ============================================================
SET IDENTITY_INSERT [dbo].[PortfolioManagers] ON;

INSERT INTO [dbo].[PortfolioManagers] ([PmId], [Name], [Type], [Email], [IsActive], [CreatedAt]) VALUES
(1, 'Alice Morgan',   'IPM', 'alice.morgan@derivativesdesk.com',   1, '2024-01-15 08:00:00'),
(2, 'Bob Chen',       'PM',  'bob.chen@derivativesdesk.com',       1, '2024-02-01 08:00:00'),
(3, 'Carol White',    'IPM', 'carol.white@derivativesdesk.com',    1, '2024-02-15 08:00:00'),
(4, 'David Kim',      'PM',  'david.kim@derivativesdesk.com',      1, '2024-03-01 08:00:00'),
(5, 'Emma Davis',     'PM',  'emma.davis@derivativesdesk.com',     1, '2024-03-15 08:00:00'),
(6, 'Frank Wilson',   'IPM', 'frank.wilson@derivativesdesk.com',   1, '2024-04-01 08:00:00'),
(7, 'Grace Lee',      'PM',  'grace.lee@derivativesdesk.com',      1, '2024-04-15 08:00:00'),
(8, 'Henry Brown',    'PM',  'henry.brown@derivativesdesk.com',    1, '2024-05-01 08:00:00');

SET IDENTITY_INSERT [dbo].[PortfolioManagers] OFF;
GO

PRINT 'PortfolioManagers: 8 rows inserted (5 PM, 3 IPM)';
GO
