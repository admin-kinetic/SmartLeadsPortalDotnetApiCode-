using SmartLeadsDataTransfer;


Console.WriteLine("Data transfer started.");

var mysqlConnectionString = "Server=kis-systems.com;Database=AutomatedLeadsDb;User ID=AutomatedLeadsDb_User;Password=\"Es8]jg[EB)D;\";";
var sqlServerConnectionString = "Server=localhost,1433;Database=SmartLeadsPortal;User ID=sa;Password=Test@123;Encrypt=True;TrustServerCertificate=True;";

var transferService = new DataTransferService(mysqlConnectionString, sqlServerConnectionString);
await transferService.TransferData(5000, 45000, 45000);

Console.WriteLine("Data transfer completed successfully.");