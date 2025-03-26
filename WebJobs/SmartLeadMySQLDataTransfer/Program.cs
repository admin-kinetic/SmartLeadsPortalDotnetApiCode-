using SmartLeadsDataTransfer;

Console.WriteLine("Data transfer started.");

var mysqlConnectionString = "Server=kis-systems.com;Database=AutomatedLeadsDb;User ID=AutomatedLeadsDb_User;Password=\"Es8]jg[EB)D;\";";
// var sqlServerConnectionString = "Server=localhost,1433;Database=SmartLeadsPortal;User ID=sa;Password=Test@123;Encrypt=True;TrustServerCertificate=True;";
var sqlServerConnectionString = "Server=tcp:kineticdb-sea-svr-test.database.windows.net,1433;Database=SmartLeadsPortalDB_Test;;Authentication=Active Directory Interactive;User Id=gjagum-adm@kistaffing.com;Encrypt=True;";

var transferService = new DataTransferService(mysqlConnectionString, sqlServerConnectionString);
// await transferService.TransferData(5000, 45000, 45000); // last working offset: 40000

await transferService.TransferData(100, 0, 4900); 

Console.WriteLine("Data transfer completed successfully.");