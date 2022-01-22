CREATE OR ALTER VIEW dbo.ContactView
            WITH SCHEMABINDING
AS
SELECT r.Id
     ,  ISNULL(COUNT(*), 0) AS UsersCount
     , r.Description
     , r.Id
     , r.Created
FROM dbo.Contact
         
GO